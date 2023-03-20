using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using PreviewDay.Graph;
using PreviewDay.Models;

namespace PreviewDay.ViewComponents
{
    public class TaskLists : ViewComponent
    {
        private readonly GraphToDoClient _graphToDoClient;
        public TaskLists(GraphToDoClient graphToDoClient)
        {
            _graphToDoClient = graphToDoClient;
            
        }

        public async Task<IViewComponentResult> InvokeAsync(ITodoListsCollectionPage lists)
        {
            
            var ToDoListWithTasks = await GetToDoListWithTasks(lists);
            
            return View(ToDoListWithTasks);

        }

        private async Task<List<ToDoList>> GetToDoListWithTasks(ITodoListsCollectionPage lists)
        {
            var completeToDoLists = new List<ToDoList>();

            // Loop through Lists to get Tasks from Graph
            foreach(var list in lists)
            {
                var todoList = new ToDoList 
                {
                    ListName = list.DisplayName
                };

                var tasks = await _graphToDoClient.GetTasksForList(list.Id);

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

            return completeToDoLists;

        }

    }
}