using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace PreviewDay.Graph
{
    public class GraphProfileClient
    {
        private readonly ILogger<GraphProfileClient> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        public GraphProfileClient(ILogger<GraphProfileClient> logger, GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
            _logger = logger;
            
        }

        public async Task<User> GetUserName()
        {
            try
            {
                return await _graphServiceClient.Me.Request().Select(u => new 
                    {
                        u.DisplayName 
                    })
                    .GetAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"error /me - {ex.Message}");
                throw;
            }
        }
    }
}