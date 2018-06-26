using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models
{
    public class EventLinkup
    {
        public int Id { get; set; }
        public IEnumerable<int> ArtistIds { get; set; }
        public int EventId { get; set; }
    }
}
