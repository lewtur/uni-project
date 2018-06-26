using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models
{
    public class DetailedArtist
    {
        public string Name { get; set; }
        public int ArtistId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyRecordId { get; set; }
        public int Delta { get; set; }
        public string SpotifyGivenGenre { get; set; }
        public string OtherGenresGivenInRelatedArtists { get; set; }
        public string MostPopularGenreOfRelatedArtists { get; set; }
    }
}
