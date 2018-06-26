using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models
{
    public class TwitterLimitBookmark
    {
        public int Id { get; set; }
        public int? ArtistId { get; set; }
        public int? VenueId { get; set; }
        public int TimesReached { get; set; }
        public int MaximumCapacityReached { get; set; }
        public DateTime DatePosted { get; set; }
    }    
}
