using System.Collections.Generic;
using System.Linq;
using FYP.Models;

namespace FYP.Main.Trends
{
    public abstract class PopularityEvent
    {
        public abstract string GetName { get; }

        /// <summary>
        /// Used to calculate in an event happened for an artist on a date range.
        /// </summary>
        /// <param name="artistId">The artist to check.</param>
        /// <param name="spike">The date range to look in.</param>
        /// <returns>A tuple containing the score referring to the recency of the event, and a collection of string to note any keywords.</returns>
        public abstract PopularityFeature DoesEventHappenForArtistOnDateRange(int artistId, Spike spike);

        public IEnumerable<PopularityFeature> DoesEventHappenForArtistOnDateRange(int artistId,
            IEnumerable<Spike> dateRanges)
        {
            return dateRanges.Select(dateRange => DoesEventHappenForArtistOnDateRange(artistId, dateRange)).ToList();
        }
    }
}