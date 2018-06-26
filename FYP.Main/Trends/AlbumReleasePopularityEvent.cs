using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using FYP.Models.DataSourceRecords;

namespace FYP.Main.Trends
{
    public class AlbumReleasePopularityEvent : PopularityEvent
    {
        private readonly ISpotifyDataSourceRepository _spotifyRepository;
        private readonly IPopularityConfig _popularityConfig;

        public AlbumReleasePopularityEvent(ISpotifyDataSourceRepository spotifyRepository, IPopularityConfig popularityConfig)
        {
            _spotifyRepository = spotifyRepository;
            _popularityConfig = popularityConfig;
        }

        public override string GetName => "Album";

        public override PopularityFeature DoesEventHappenForArtistOnDateRange(int artistId, Spike spike)
        {
            var spotifyArtistHeaderId = _spotifyRepository.GetSpotifyArtistHeaderId(artistId).Result;

            var albums = _spotifyRepository
                .GetAllAlbumHeaders(spotifyArtistHeaderId)
                .Result
                .GroupBy(x => x.Name)
                .Select(albumGroup => albumGroup
                    .Take(1)
                    .FirstOrDefault())
                .ToList();

            var candidates = new List<PopularityFeature>(); 

            foreach (var album in albums)
            {                
                if (!DateTime.TryParseExact(album.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var releaseDate)) continue;

                var result = _popularityConfig.CalculateScore(releaseDate - spike.FromDate);

                if (result <= 0) continue;

                candidates.Add(new PopularityFeature
                {
                    RecencyScore = result,
                    KeyWords = new List<string> {$"label-{album.Label.ToLower()}", $"album-{releaseDate.DayOfWeek.ToString().ToLower()}"},
                    Magnitude = spike.SpikeMagnitude
                });
            }

            return candidates.FirstOrDefault(x => x.RecencyScore == candidates.Max(y => y.RecencyScore));
        }
    }
}
