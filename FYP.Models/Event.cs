using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Cancelled { get; set; }
        public int VenueId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string DoorsOpen { get; set; }
        public string DoorsClose { get; set; }
        public string LastEntry { get; set; }
        public int MinAge { get; set; }
        public string EntryPrice { get; set; }
    }

    public class FullEvent
    {
        public Event Event { get; set; }
        public Venue Venue { get; set; }
        public IEnumerable<DetailedArtist> Artist { get; set; }
    }

    public class ArtistEventSummaryHeader
    {
        public string ArtistName { get; set; }
        public IEnumerable<ArtistEventSummary> Summary { get; set; }
    }

    public class ArtistEventSummary
    {
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
    }
}
