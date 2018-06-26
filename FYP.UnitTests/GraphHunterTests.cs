using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FYP.Main.Trends;
using FYP.Models;
using FYP.Models.DataSourceRecords;
using Xunit;

namespace FYP.UnitTests
{
    public class GraphHunterTests
    {
        [Fact]
        public void WhenIHaveAnObviousSpikeInTwitterDataItShouldReturnADateRangeWithTheDateOfTheSpike()
        {
            // given
            var tweets = new List<TwitterDaySummary>
            {
                new TwitterDaySummary {Date = DateTime.Now, Percentage = 7},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-1), Percentage = 85},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-2), Percentage = 4}
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(tweets, null, null);

            // then
            Assert.Equal(DateTime.Now.AddDays(-1).Day, dates.First().FromDate.Day);
        }

        [Fact]
        public void WhenThereIsATwoDaySpikeInTheTwitterDataItShouldReturnADateRangeThatIncludesBothDates()
        {
            // given
            var tweets = new List<TwitterDaySummary>
            {
                new TwitterDaySummary {Date = DateTime.Now, Percentage = 7},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-1), Percentage = 85},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-2), Percentage = 82},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-3), Percentage = 4}
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(tweets, null, null);

            // then
            Assert.Equal(dates.First().FromDate.Day, DateTime.Now.AddDays(-2).Day);
            Assert.Equal(dates.First().ToDate.Day, DateTime.Now.AddDays(-1).Day);
        }

        [Fact]
        public void WhenThereAreMultipleOneDaySpikesInTheTwitterDataItShouldReturnMultipleDateRangesWithThoseDate()
        {
            // given
            var tweets = new List<TwitterDaySummary>
            {
                new TwitterDaySummary {Date = DateTime.Now, Percentage = 85},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-1), Percentage = 4},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-2), Percentage = 44},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-3), Percentage = 4}
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(tweets, null, null);

            // then
            Assert.Equal(dates.Last().FromDate.Day, DateTime.Now.Day);
            Assert.Equal(dates.Last().ToDate.Day, DateTime.Now.Day);
            Assert.Equal(dates.First().FromDate.Day, DateTime.Now.AddDays(-2).Day);
            Assert.Equal(dates.First().ToDate.Day, DateTime.Now.AddDays(-2).Day);
        }

        [Fact]
        public void WhenThereAreMultipletwoDaySpikesInTheTwitterDataItShouldReturnMultipleDateRangesWithThoseDate()
        {
            // given
            var tweets = new List<TwitterDaySummary>
            {
                new TwitterDaySummary {Date = DateTime.Now, Percentage = 85},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-1), Percentage = 84},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-2), Percentage = 4},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-3), Percentage = 44},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-4), Percentage = 48},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-5), Percentage = 4}
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(tweets, null, null);

            // then
            Assert.Equal(dates.Last().FromDate.Day, DateTime.Now.AddDays(-1).Day);
            Assert.Equal(dates.Last().ToDate.Day, DateTime.Now.Day);
            Assert.Equal(dates.First().FromDate.Day, DateTime.Now.AddDays(-4).Day);
            Assert.Equal(dates.First().ToDate.Day, DateTime.Now.AddDays(-3).Day);
        }

        [Fact]
        public void ItShouldNotMarkASteadilyDecreasingTrendInTwitterDataAsOneBigSpike()
        {
            // given
            var tweets = new List<TwitterDaySummary>
            {
                new TwitterDaySummary {Date = DateTime.Now, Percentage = 20},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-1), Percentage = 30},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-2), Percentage = 40},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-3), Percentage = 50},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-4).AddHours(-1), Percentage = 60},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-5), Percentage = 1}
            };

            // when
            var hunter = new GraphHunter();
            var date = hunter.GetSpikes(tweets, null, null).First();
            var range = (date.ToDate - date.FromDate).Days;

            // then
            Assert.NotEqual(range + 1, tweets.Count - 1);
        }

        [Fact]
        public void ItShouldBeAbleToDetectAndUpwardsSpotifyArtistTrend()
        {
            // given
            var spotifyData = new List<SpotifyArtistStats>
            {
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-4), Popularity = 1},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-3), Popularity = 4},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-2), Popularity = 5},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-1), Popularity = 6},
                new SpotifyArtistStats {DatePosted = DateTime.Now, Popularity = 4}                               
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(null, spotifyData, null);

            // then
            Assert.Equal(dates.First().FromDate.Day, DateTime.Now.AddDays(-3).Day);
            Assert.Equal(dates.First().ToDate.Day, DateTime.Now.AddDays(-1).Day);
        }

        [Fact]
        public void ItShouldNotMarkAConstantSpotifyPopularityAsATrend()
        {
            // given
            var spotifyData = new List<SpotifyArtistStats>
            {
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-4), Popularity = 2},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-3), Popularity = 2},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-2), Popularity = 2}
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(null, spotifyData, null);

            // then
            Assert.Empty(dates);
        }

        [Fact]
        public void ItShouldMarkTwoUpwardsMovementsWithALevelMiddleAsTwoSeperateTrends()
        {
            // given
            var spotifyData = new List<SpotifyArtistStats>
            {
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-4), Popularity = 2},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-3), Popularity = 4},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-2), Popularity = 4},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-1), Popularity = 4},
                new SpotifyArtistStats {DatePosted = DateTime.Now, Popularity = 5}
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(null, spotifyData, null);

            // then
            Assert.Equal(2, dates.Count());
        }

        [Fact]
        public void ItShouldDetectTheTrendsInAnAlbum()
        {
            // given
            var albumStats = new List<AlbumStats>
            {
                new AlbumStats
                {
                    Stats = new List<SpotifyAlbumStats>
                    {
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(-3), Popularity = 10},
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(-2), Popularity = 12},
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(-1), Popularity = 13},
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(0), Popularity = 10}
                    }
                }
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(null, null, albumStats);

            // then
            Assert.Single(dates);
        }

        [Fact]
        public void ItShouldReturnTheTrendsForSpotifyArtistData_TweetData_AndAlbumData()
        {
            // given
            var artistData = new List<SpotifyArtistStats>
            {
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-14), Popularity = 1},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-13), Popularity = 4},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-12), Popularity = 5},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-11), Popularity = 6},
                new SpotifyArtistStats {DatePosted = DateTime.Now.AddDays(-10), Popularity = 4}
            };

            var albumData = new List<AlbumStats>
            {
                new AlbumStats
                {
                    Stats = new List<SpotifyAlbumStats>
                    {
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(-8), Popularity = 10},
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(-7), Popularity = 12},
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(-6), Popularity = 13},
                        new SpotifyAlbumStats { DatePosted = DateTime.Now.AddDays(-5), Popularity = 10}
                    }
                }
            };

            var tweets = new List<TwitterDaySummary>
            {
                new TwitterDaySummary {Date = DateTime.Now, Percentage = 7},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-1), Percentage = 85},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-2), Percentage = 4}
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(tweets, artistData, albumData);

            // then
            Assert.Equal(3, dates.Count());
        }

        [Fact]
        public void ItShouldCorrectlyIdentifyTheSpikeMagnitudeForTwitterData()
        {
            // given
            var tweets = new List<TwitterDaySummary>
            {
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-4), Percentage = 10},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-3), Percentage = 90},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-2), Percentage = 10},
                new TwitterDaySummary {Date = DateTime.Now.AddDays(-1), Percentage = 10},
                new TwitterDaySummary {Date = DateTime.Now, Percentage = 30}                
            };

            // when
            var hunter = new GraphHunter();
            var dates = hunter.GetSpikes(tweets, null, null);

            // then
            Assert.Equal(1, dates.Last().SpikeMagnitude);
            Assert.Equal(4, dates.First().SpikeMagnitude);
        }
    }
}
