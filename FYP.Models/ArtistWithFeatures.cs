using System.Collections.Generic;

namespace FYP.Models
{
    public class ArtistWithFeatures
    {
        public Artist Artist { get; set; }
        public IList<SinglePopularityFeature> Features { get; set; }
    }

    public class UserRecommendedArtist
    {
        public Artist Artist { get; set; }
        public IList<SinglePopularityFeature> Features { get; set; }
        public IList<string> MatchedGenres { get; set; }
        public Genre Genre { get; set; }
    }
}