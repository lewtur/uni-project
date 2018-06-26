using System;
using System.Collections.Generic;
using System.Text;
using FYP.Data;
using FYP.Main.Trends;
using FYP.Models;
using Moq;
using Xunit;

namespace FYP.UnitTests
{
    public class GigPopularityEventTests
    {
        private readonly Mock<IEventRepository> _mockEventRepository;

        public GigPopularityEventTests()
        {
            _mockEventRepository = new Mock<IEventRepository>();
        }

        [Fact]
        public void WhenAnArtistPlayedAGigAWeekAgoAndThereWasAPopularitySpikeAfterwards_IShouldGetAPositiveScore()
        {
            // given  
            var range = new Spike {FromDate = DateTime.Now.AddDays(-3), ToDate = DateTime.Now.AddDays(-1)};
            var summary = new List<ArtistEventSummary>
            {
                new ArtistEventSummary {StartDate = DateTime.Now.AddDays(-7), VenueLocation = "swag", VenueName = "swag"}
            };

            // when
            _mockEventRepository.Setup(x => x.GetArtistEventSummary(1)).ReturnsAsync(summary);
            var gigPopularity = new GigPopularityEvent(_mockEventRepository.Object, new LinearPopularityConfig());
            var feature = gigPopularity.DoesEventHappenForArtistOnDateRange(1, range);

            // then
            Assert.True(feature.RecencyScore > 0);
        }

        [Fact]
        public void WhenAnArtistPlayedAGigAWeekAgoAndThereWasAPopularitySpikeAfterwards_IShouldGetTheVenueWhereTheyPlayed()
        {
            // given  
            var range = new Spike { FromDate = DateTime.Now.AddDays(-3), ToDate = DateTime.Now.AddDays(-1) };
            var summary = new List<ArtistEventSummary>
            {
                new ArtistEventSummary {StartDate = DateTime.Now.AddDays(-7), VenueName = "The Night Owl", VenueLocation = "Birmingham"}
            };

            // when
            _mockEventRepository.Setup(x => x.GetArtistEventSummary(1)).ReturnsAsync(summary);
            var gigPopularity = new GigPopularityEvent(_mockEventRepository.Object, new LinearPopularityConfig());
            var feature = gigPopularity.DoesEventHappenForArtistOnDateRange(1, range);

            // then
            Assert.Contains(feature.KeyWords, s => s.Equals("city-birmingham"));
            Assert.Contains(feature.KeyWords, s => s.Equals("venue-the night owl"));
        }
    }
}
