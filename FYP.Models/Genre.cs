using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models
{
    public class Genre
    {
        public int ArtistId { get; set; }
        public string SpotifyGivenGenre { get; set; }
        public string MostPopularGenreOfRelatedArtists { get; set; }
        public string OtherGenresGivenInRelatedArtists { get; set; }
    }
}
