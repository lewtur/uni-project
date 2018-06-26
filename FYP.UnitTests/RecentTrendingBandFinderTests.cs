using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FYP.Data;
using FYP.Main;
using FYP.Main.Trends;
using FYP.Models;
using Moq;
using Xunit;

namespace FYP.UnitTests
{
    public class RecentTrendingBandFinderTests
    {
        private readonly Mock<IPopularityRepository> _mockPopularityRepository;
        private readonly Mock<IPopularityFinder> _mockPopularityFinder;
        private readonly Mock<IArtistFilterer> _mockArtistFilterer;

        public RecentTrendingBandFinderTests()
        {
            _mockPopularityFinder = new Mock<IPopularityFinder>();
            _mockPopularityRepository = new Mock<IPopularityRepository>();
            _mockArtistFilterer = new Mock<IArtistFilterer>();
        }

        [Fact]
        public void IfThereIsArtistThatInATimeFrameHasAGoodPopularityFeatureThenReturnThatArtist()
        {
            // given
            _mockArtistFilterer.Setup(x => x.GetArtistsToUpdate()).Returns(new List<Artist> {new Artist {Id = 33}});
            _mockPopularityRepository.Setup(x => x.GetAllPopularityTerms())
                .ReturnsAsync(new List<SinglePopularityFeature> {new SinglePopularityFeature {Term = "swag", Score = 34}});
            _mockPopularityFinder.Setup(x => x.FindRecentFeaturesForArtist(33))
                .ReturnsAsync(
                    new List<PopularityFeature> {new PopularityFeature {KeyWords = new List<string> {"swag"}}});

            // when
            var finder = new RecentTrendingBandFinder(_mockPopularityRepository.Object, _mockPopularityFinder.Object, _mockArtistFilterer.Object);
            var result = finder.GetArtistsWithRecentFeatures().Result;

            // then
            Assert.Single(result);
        }

    }
}
