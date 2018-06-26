using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Models;

namespace FYP.Main.Trends
{
    public interface IRecentTrendingBandFinder
    {
        Task<IEnumerable<ArtistWithFeatures>> GetArtistsWithRecentFeatures();
        Task AddTrendingArtists();
    }

    public class RecentTrendingBandFinder : IRecentTrendingBandFinder
    {
        private readonly IPopularityRepository _popularityRepository;
        private readonly IPopularityFinder _popularityFinder;
        private readonly IArtistFilterer _artistFilterer;

        public RecentTrendingBandFinder(IPopularityRepository popularityRepository, IPopularityFinder popularityFinder, IArtistFilterer artistFilterer)
        {
            _popularityRepository = popularityRepository;
            _popularityFinder = popularityFinder;
            _artistFilterer = artistFilterer;
        }

        public async Task<IEnumerable<ArtistWithFeatures>> GetArtistsWithRecentFeatures()
        {
            var artists = _artistFilterer.GetArtistsToUpdate();
            var mostPopularFeatures = (await _popularityRepository.GetAllPopularityTerms()).Where(x => x.Score > 1);
            var recentArtists = new List<ArtistWithFeatures>();

            var i = 0;
            var max = artists.Count();

            foreach (var artist in artists)
            {
                Console.WriteLine($"{++i}/{max}");
                var artistWithFeatures = new ArtistWithFeatures {Artist = artist, Features = new List<SinglePopularityFeature>()};
                var recentArtistFeatures = await _popularityFinder.FindRecentFeaturesForArtist(artist.Id);

                foreach (var artistFeature in recentArtistFeatures)
                {
                    if (artistFeature == null) continue;

                    var artistKeyWords = artistFeature.KeyWords;

                    foreach (var popularFeature in mostPopularFeatures)
                    {
                        if (artistKeyWords.Contains(popularFeature.Term) && !artistWithFeatures.Features.Select(x => x.Term).Contains(popularFeature.Term))
                        {
                            artistWithFeatures.Features.Add(popularFeature);
                        }
                    }

                }

                if (artistWithFeatures.Features.Any())
                {
                    recentArtists.Add(artistWithFeatures);
                }
            }

            return recentArtists.OrderByDescending(x => x.Features.Sum(y => y.Score)).ToList();
        }

        public async Task AddTrendingArtists()
        {
            var artists = await GetArtistsWithRecentFeatures();
            await _popularityRepository.SetRecentTrendingArtists(artists);
        }
    }
}
