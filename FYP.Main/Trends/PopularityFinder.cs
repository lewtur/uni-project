using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models;

namespace FYP.Main.Trends
{
    public interface IPopularityFinder
    {
        Task FindAndUpdateAll();
        Task<IEnumerable<PopularityFeature>> FindRecentFeaturesForArtist(int artistId);
    }

    public class PopularityFinder : IPopularityFinder
    {
        private readonly IArtistFilterer _artistFilterer;
        private readonly IEnumerable<PopularityEvent> _events;
        private readonly IGraphHunter _graphHunter;
        private readonly ITwitterDataSourceRepository _twitterRepository;
        private readonly ISpotifyDataSourceRepository _spotifyRepository;
        private readonly IPopularityRepository _popularityRepository;

        private const int DaysToLookBack = 7;

        public PopularityFinder(IArtistFilterer artistFilterer, IGraphHunter graphHunter, ITwitterDataSourceRepository twitterRepository, ISpotifyDataSourceRepository spotifyRepository, IEnumerable<PopularityEvent> events, IPopularityRepository popularityRepository)
        {
            _artistFilterer = artistFilterer;
            _events = events;
            _popularityRepository = popularityRepository;
            _graphHunter = graphHunter;
            _twitterRepository = twitterRepository;
            _spotifyRepository = spotifyRepository;
        }

        public async Task FindAndUpdateAll()
        {
            var artists = _artistFilterer.GetArtistsToUpdate();
            var eventResults = new List<PopularityFeature>();

            foreach (var artist in artists)
            {
                var spotifyArtistHeaderId = await _spotifyRepository.GetSpotifyArtistHeaderId(artist.Id);

                var twitterData = await _twitterRepository.GetTweetSummaryForArtist(artist.Id);
                var spotifyArtistData = await _spotifyRepository.GetAllStatsForArtist(spotifyArtistHeaderId);
                var spotifyAlbumData = await GetAlbumData(spotifyArtistHeaderId, 0);

                var dateRanges = _graphHunter.GetSpikes(twitterData, spotifyArtistData, spotifyAlbumData);

                foreach (var e in _events)
                {
                    eventResults.AddRange(e.DoesEventHappenForArtistOnDateRange(artist.Id, dateRanges));
                }
            }

            eventResults = eventResults
                .Where(x => x != null && x.RecencyScore > 0)
                .OrderByDescending(x => x.Magnitude)
                .ToList();

            var keywords = new Dictionary<string, (int count, double averageMagnitude)>();

            foreach (var result in eventResults)
            {
                foreach (var word in result.KeyWords)
                {
                    var key = word.ToLower();

                    if (keywords.ContainsKey(key))
                    {
                        var (count, averageMagnitude) = keywords[key];
                        keywords[key] = (count + 1, (averageMagnitude + result.Magnitude) / count + 1);
                    }
                    else
                    {
                        keywords.Add(key, (1, result.Magnitude));
                    }
                }
            }

            var orderedKeywords = keywords.OrderByDescending(x => x.Value);

            var popularityFeatures = orderedKeywords.Select(x => new SinglePopularityFeature
            {
                Term = x.Key,
                AverageMagnitude = x.Value.averageMagnitude,
                Score = x.Value.count
            });

            await _popularityRepository.AddPopularityTerms(popularityFeatures);
        }

        public async Task<IEnumerable<PopularityFeature>> FindRecentFeaturesForArtist(int artistId)
        {
            var eventResults = new List<PopularityFeature>();            
            var spotifyArtistHeaderId = await _spotifyRepository.GetSpotifyArtistHeaderId(artistId);

            var twitterData = await _twitterRepository.GetTweetSummaryForArtist(artistId, DaysToLookBack);
            var spotifyArtistData = await _spotifyRepository.GetAllStatsForArtist(spotifyArtistHeaderId, DaysToLookBack);
            var spotifyAlbumData = await GetAlbumData(spotifyArtistHeaderId, DaysToLookBack);

            var dateRanges = _graphHunter.GetSpikes(twitterData, spotifyArtistData, spotifyAlbumData);

            foreach (var e in _events)
            {
                eventResults.AddRange(e.DoesEventHappenForArtistOnDateRange(artistId, dateRanges));
            }

            return eventResults;
        }

        private async Task<IEnumerable<AlbumStats>> GetAlbumData(int spotifyHeaderId, int daysToLookBack)
        {
            var artistAlbums = await _spotifyRepository.GetAllAlbumHeaders(spotifyHeaderId);

            var stats = artistAlbums.Select(album => new AlbumStats
                {
                    Header = album,
                    Stats = _spotifyRepository.GetAllStatsForAlbum(album.Id).Result
                })
                .GroupBy(x => x.Header.Name)
                .Select(albumGroup => albumGroup
                    .OrderByDescending(x => x.Stats.Select(y => y.Popularity).Average())
                    .Take(1)
                    .FirstOrDefault())
                .ToList();

            if (daysToLookBack <= 0) return stats;
  
            return stats.Where(x => DateTime.TryParseExact(x.Header.ReleaseDate, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date) && date > DateTime.Now.AddDays(-daysToLookBack));
        }

    }
}
