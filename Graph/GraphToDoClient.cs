using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace PreviewDay.Graph
{
    public class GraphToDoClient
    {
        private readonly ILogger<GraphToDoClient> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        public GraphToDoClient(ILogger<GraphToDoClient> logger, GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
            _logger = logger;
            
        }

        public async Task<ITodoListsCollectionPage> GetToDoLists()
        {
            try
            {
                return await _graphServiceClient.Me.Todo.Lists
                .Request()
                .GetAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error at /me/todo/lists - {ex.Message}");
                throw;
            }
            
        }

        public async Task<ITodoTaskListTasksCollectionPage> GetTasksForList(string listId)
        {
            try
            {
                return await _graphServiceClient.Me.Todo.Lists[listId].Tasks
                    .Request()
                    .Filter("status ne 'completed'")
                    .OrderBy("importance desc")
                    .GetAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error at /me/todo/lists/listId/tasks - {ex.Message}");
                throw;
            }
            
        }

        
    }
}