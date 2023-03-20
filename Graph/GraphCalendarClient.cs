using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace PreviewDay.Graph
{
    public class GraphCalendarClient
    {
        private readonly ILogger<GraphCalendarClient> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        public GraphCalendarClient(ILogger<GraphCalendarClient> logger, GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
            _logger = logger; 
        }

        public async Task<IUserCalendarViewCollectionPage> GetCalendarEventsForToday(string timeZone){

            var startOfDay = DateTime.Now.Date;
            var EndOfDay = DateTime.Now.AddDays(1).Date.AddSeconds(-1);
            
            var viewOptions = new List<QueryOption>
            {
                new QueryOption("startDateTime", startOfDay.ToString("o")),
                new QueryOption("endDateTime", EndOfDay.ToString("o"))
            };
            
            try
            {
                return await _graphServiceClient
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
            }
            catch(Exception ex)
            {
                _logger.LogError($"error at me/calendarView - Query options: startDateTime{viewOptions[0]} " +
                $"endDateTime {viewOptions[1]} - Header: timeZone {timeZone} - Error: {ex.Message}");
                throw;
            }
        }
    }
}