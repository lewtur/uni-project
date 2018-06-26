using System.Collections.Generic;
using System.Linq;
using System.Text;
using FYP.Data;
using FYP.Models;

namespace FYP.Main.Trends
{
    public class GigPopularityEvent : PopularityEvent
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPopularityConfig _popularityConfig;

        public GigPopularityEvent(IEventRepository eventRepository, IPopularityConfig popularityConfig)
        {
            _eventRepository = eventRepository;
            _popularityConfig = popularityConfig;
        }

        public override string GetName => "Gig";

        public override PopularityFeature DoesEventHappenForArtistOnDateRange(int artistId, Spike spike)
        {
            var events = _eventRepository.GetArtistEventSummary(artistId).Result;

            var candidates = new List<PopularityFeature>();

            foreach (var e in events)
            {
                var result = _popularityConfig.CalculateScore(e.StartDate - spike.FromDate);
                if (result <= 0) continue;

                candidates.Add(new PopularityFeature
                {
                    RecencyScore = result,
                    KeyWords = new List<string> {$"venue-{e.VenueName.ToLower()}", $"city-{e.VenueLocation.ToLower()}", $"gig-{e.StartDate.DayOfWeek.ToString().ToLower()}"},
                    Magnitude = spike.SpikeMagnitude
                });
            }

            return candidates.FirstOrDefault(x => x.RecencyScore == candidates.Max(y => y.RecencyScore));
        }
    }
}
