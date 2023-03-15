using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using PreviewDay.Graph;
using PreviewDay.Models;
using System.Diagnostics;
using System.Text.Json;

namespace PreviewDay.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GraphProfileClient _graphProfileClient;
        private readonly GraphServiceClient _graphServiceClient;
        private readonly GraphToDoClient _graphToDoClient;

        public HomeController(ILogger<HomeController> logger, GraphProfileClient graphProfileClient, 
        GraphServiceClient graphServiceClient, GraphToDoClient graphToDoClient)
        {
            _graphToDoClient = graphToDoClient;
            _graphServiceClient = graphServiceClient;
            _graphProfileClient = graphProfileClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // TODO - organize/refactor some of this out of controller

            // Get User Name from Profile 
            
            var user = await _graphProfileClient.GetUserName();

            var firstName = user.DisplayName.Split(' ')[0];

            ViewData["userFirstName"] = char.ToUpper(firstName[0]) + firstName.Substring(1);

            //Begin - ToDo section
            var lists = await _graphToDoClient.GetToDoLists();

            var completeToDoLists = new List<ToDoList>();

            // Loop through Lists to get Tasks from Graph
            foreach(var list in lists)
            {
                var todoList = new ToDoList 
                {
                    ListName = list.DisplayName
                };

                var tasks = await _graphServiceClient.Me.Todo.Lists[list.Id].Tasks
                    .Request()
                    .Filter("status ne 'completed'")
                    .OrderBy("importance desc")
                    .GetAsync();

                foreach(var task in tasks)
                {
                    var todoTask = new ToDoTask 
                    {
                        Title = task.Title,
                        Content = task.Body.Content,
                        Importance = task.Importance
                    };

                    // Check for SubTasks
                    var addiDatacheckList = task.AdditionalData.ContainsKey("checklistItems") ? 
                        task.AdditionalData["checklistItems"] : null;

                    if(addiDatacheckList is not null){
                        var checkList = addiDatacheckList.ToString();

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        todoTask.SubTasks = JsonSerializer.Deserialize<List<SubTask>>(checkList!, options)!;
                    }

                    todoList.Tasks.Add(todoTask);

                }


                completeToDoLists.Add(todoList);    
            }

            ViewData["GraphApiResult"] = completeToDoLists;

            // End of ToDo Section

            // Begin Calendar Section

            var startOfDay = DateTime.Now.Date;
            var EndOfDay = DateTime.Now.AddDays(1).Date.AddSeconds(-1);

            var userMailBoxSettings = await _graphServiceClient
                .Me
                .Request()
                .Select(u => new
                {
                    u.MailboxSettings
                })
                .GetAsync();

            // Get timezone from Mailbox settings. Default to Eastern if none detected
            var timeZone = (String.IsNullOrEmpty(userMailBoxSettings.MailboxSettings.TimeZone)) 
                ? "Eastern Standard Time" : userMailBoxSettings.MailboxSettings.TimeZone;

            var viewOptions = new List<QueryOption>
            {
                new QueryOption("startDateTime", startOfDay.ToString("o")),
                new QueryOption("endDateTime", EndOfDay.ToString("o"))
            };
            
            var calendarEvents = await _graphServiceClient
                .Me
                .CalendarView
                .Request(viewOptions)
                .Header("Prefer", $"outlook.timezone=\"{timeZone}\"")
                .Select(evt => new
                {
                evt.Subject,
                evt.Organizer,
                evt.Start,
                evt.End
                })
                .OrderBy("start/DateTime")
                .GetAsync();

            // Convert Start and End from ISO 8061 strings to DateTime and then to strings
            List<formattedTime> timeStuff = new List<formattedTime>();
            foreach(var evt in calendarEvents)
            {
                string start = "";
                string end = "";
                var timeInfo = new formattedTime 
                {
                    Subject = evt.Subject
                };

                if(DateTime.TryParse(evt.Start.DateTime, out DateTime startDateTime))
                {
                    start = $"{startDateTime.ToShortDateString()} {startDateTime.ToShortTimeString()}";
                }
                else
                {
                    start = evt.Start.DateTime;
                }

                if(DateTime.TryParse(evt.End.DateTime, out DateTime endDateTime))
                {
                    end = $"{endDateTime.ToShortDateString()} {endDateTime.ToShortTimeString()}";
                }
                else
                {
                    end = evt.End.DateTime;
                }

                timeInfo.StartTime = start;
                timeInfo.EndTime = end;

                timeStuff.Add(timeInfo);

            }

            ViewData["calendarEvents"] = calendarEvents;
            ViewData["formattedTime"] = timeStuff;

            // End Calendar section



            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}