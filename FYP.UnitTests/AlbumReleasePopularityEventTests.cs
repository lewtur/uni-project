using System;
using System.Collections.Generic;
using System.Text;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Main.Trends;
using FYP.Models;
using FYP.Models.DataSourceRecords;
using Moq;
using Xunit;

namespace FYP.UnitTests
{
    public class AlbumReleasePopularityEventTests
    {
        private readonly Mock<ISpotifyDataSourceRepository> _mockSpotifyRepository;

        public AlbumReleasePopularityEventTests()
        {
            _mockSpotifyRepository = new Mock<ISpotifyDataSourceRepository>();
        }

        [Fact]
        public void WhenAnArtistReleasedAnAlbumAWeekAgoAndThereWasAPopularitySpikeAfterwards_IShouldGetAPositiveScore()
        {          
            // given
            var range = new Spike { FromDate = DateTime.Now.AddDays(-3), ToDate = DateTime.Now.AddDays(-1) };
            var summary = new List<SpotifyAlbumHeader>
            {
                new SpotifyAlbumHeader {Label = "Sick Label", ReleaseDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd")}
            };

            // when
            _mockSpotifyRepository.Setup(x => x.GetAllAlbumHeaders(It.IsAny<int>())).ReturnsAsync(summary);
            var albumPopularity = new AlbumReleasePopularityEvent(_mockSpotifyRepository.Object, new LinearPopularityConfig());
            var feature = albumPopularity.DoesEventHappenForArtistOnDateRange(1, range);

            // then
            Assert.True(feature.RecencyScore > 0);
        }

        [Fact]
        public void WhenAnArtistReleasedTwoAlbumsOnTheSameDayTheScoreShouldOnlyConsiderOneOfThem()
        {
            // given
            var range = new Spike { FromDate = DateTime.Now.AddDays(-2), ToDate = DateTime.Now.AddDays(-1) };
            var summary = new List<SpotifyAlbumHeader>
            {
                new SpotifyAlbumHeader {Label = "Sick Label", ReleaseDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")},
                new SpotifyAlbumHeader {Label = "Sick Label", ReleaseDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")}
            };

            // when
            _mockSpotifyRepository.Setup(x => x.GetAllAlbumHeaders(It.IsAny<int>())).ReturnsAsync(summary);
            var albumPopularity = new AlbumReleasePopularityEvent(_mockSpotifyRepository.Object, new LinearPopularityConfig());
            var feature = albumPopularity.DoesEventHappenForArtistOnDateRange(1, range);

            // then
            Assert.True(feature.RecencyScore < 100);
        }

        [Fact]
        public void WhenThereAreTwoPossibleAlbumsToPickFromItShouldReturnTheOneClosestToTheDateRange()
        {
            // given
            var range = new Spike { FromDate = DateTime.Now.AddDays(-2), ToDate = DateTime.Now.AddDays(-1) };
            var summary = new List<SpotifyAlbumHeader>
            {
                new SpotifyAlbumHeader {Label = "Pick me", ReleaseDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")},
                new SpotifyAlbumHeader {Label = "Not me", ReleaseDate = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd")}
            };

            _mockSpotifyRepository.Setup(x => x.GetAllAlbumHeaders(It.IsAny<int>())).ReturnsAsync(summary);
            var albumPopularity = new AlbumReleasePopularityEvent(_mockSpotifyRepository.Object, new LinearPopularityConfig());
            var feature = albumPopularity.DoesEventHappenForArtistOnDateRange(1, range);

            // then
            Assert.Contains(feature.KeyWords, x => x.Equals("label-pick me"));
        }
    }
}
