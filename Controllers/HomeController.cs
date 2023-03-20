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
        private readonly GraphCalendarClient _graphCalendarClient;

        public HomeController(ILogger<HomeController> logger, GraphProfileClient graphProfileClient, 
        GraphServiceClient graphServiceClient, GraphToDoClient graphToDoClient, GraphCalendarClient graphCalendarClient)
        {
            _graphCalendarClient = graphCalendarClient;
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

            // Begin - ToDo section
            var lists = await _graphToDoClient.GetToDoLists();

            // Pass lists to ViewComponent 
            ViewData["ToDoLists"] = lists;

            // End of ToDo Section

            // Begin Calendar Section

            var userMailBoxSettings = await _graphProfileClient.GetUserMailBoxSettings();

            // Get timezone from Mailbox settings. Default to Eastern if none detected
            // Pass to ViewData to pass to CalendarEvents ViewComponent
            var timeZone = (String.IsNullOrEmpty(userMailBoxSettings.MailboxSettings.TimeZone)) 
                ? "Eastern Standard Time" : userMailBoxSettings.MailboxSettings.TimeZone;

            var calendarEvents = await _graphCalendarClient.GetCalendarEventsForToday(timeZone);

            // End Calendar section

            ViewData["calendarEvents"] = calendarEvents;

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