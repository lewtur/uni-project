using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FYP.Data;
using FYP.Models;
using FYP.Models.JsonModels;
using Newtonsoft.Json;

namespace FYP.External
{
    public class EventsRetriever : IEventsRetriever
    {
        private const string ApiUrl = "http://www.skiddle.com/api/v1/events/search/" + 
            "?api_key=6783027b3d3ca0f3563d90d846625f3a&eventcode=LIVE&description=1" + 
            "&orderBy=date&limit=100";

        private readonly IHttpRequester _httpRequester;

        public EventsRetriever(IHttpRequester httpRequester)
        {
            _httpRequester = httpRequester;
        }

        public SkiddleEvent GetEventsForDay(DateTime date, Location location)
        {
            return GetEventsForDateRange(date, date, location);
        }

        public SkiddleEvent GetEventsForDateRange(DateTime minDate, DateTime maxDate, Location location)
        {
            using (var client = new HttpClient())
            {
                var minDateString = GetFormattedDate(minDate);
                var maxDateString = GetFormattedDate(maxDate);

                var url = ApiUrl + $"&latitude={location.Latitude}&longitude={location.Longitude}" +
                          $"&radius={location.Radius}&minDate={minDateString}&maxDate={maxDateString}";

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return _httpRequester.Get<SkiddleEvent>(client, url);
            }
        }

        private string GetFormattedDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }

    public interface IEventsRetriever
    {
        SkiddleEvent GetEventsForDateRange(DateTime minDate, DateTime maxDate, Location location);
    }
}
