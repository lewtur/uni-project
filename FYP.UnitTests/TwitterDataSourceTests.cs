using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.External;
using FYP.External.DataSources;
using FYP.Main;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;
using FYP.Models.JsonModels;
using Moq;
using Xunit;

namespace FYP.UnitTests
{
    public class TwitterDataSourceTests
    {
        private readonly Mock<IHttpRequester> _httpRequester;
        private readonly Mock<ITwitterDataSourceRepository> _repository;
        private readonly Mock<ITwitterDataSourceConfig> _config;
        private readonly Mock<ICache> _cache;
        private readonly Mock<ITwitterLimitRepository> _limitRepository;

        public TwitterDataSourceTests()
        {            
            _httpRequester = new Mock<IHttpRequester>();
            _repository = new Mock<ITwitterDataSourceRepository>();
            _config = new Mock<ITwitterDataSourceConfig>();
            _cache = new Mock<ICache>();
            _limitRepository = new Mock<ITwitterLimitRepository>();
            _config.Setup(x => x.BaseApiUrl).Returns("https://www.banana.com");

            _config.Setup(x => x.MaximumNumberStoredUnderOneEntity).Returns(10);
            _config.Setup(x => x.DateFormatInitial).Returns("ddd MMM dd");
            _config.Setup(x => x.DateFormatWithYear).Returns("ddd MMM dd yyyy");
            _config.Setup(x => x.MaximumNumberStoredUnderOneEntity).Returns(4);

            _cache.Setup(x => x.Get("DateUpdateStarted")).Returns(new DateTime(2000, 1, 1).ToString("yyyy-MM-dd"));
        }
 
        [Fact]
        public void WhenATweetStartsWithARetweetItShouldNotBeSavedToTheRepository()
        {
            // given
            _httpRequester.Setup(x => x.Get<TwiterSearchResults>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new TwiterSearchResults {statuses = new List<Status> {new Status {text = "RT @ this is a retweet", created_at = "Sat Mar 03 13:47:27 +0000 2018" }, new Status {text = "Swagger McJagger", created_at = "Sat Mar 03 13:47:27 +0000 2018" } }});
            _httpRequester.Setup(x => x.Post<TwitterAuthentication>(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new TwitterAuthentication());

            // when
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.Update(new Artist {Name = "Swagger McJagger"});

            // then
            _repository.Verify(x => x.AddTweet(It.IsAny<Tweet>()), Times.Once);
        }

        [Fact]
        public void WhenTheDataSourceHasReachedTheMaximumNumberForOneEntityItShouldNotContinue()
        {
            // given   
            _httpRequester.Setup(x => x.Get<TwiterSearchResults>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new TwiterSearchResults { statuses = new List<Status> {new Status {text = "swag ", created_at = "Sat Mar 03 13:47:27 +0000 2018" } }, search_metadata = new SearchMetadata {next_results = "keep going"}});
            _httpRequester.Setup(x => x.Post<TwitterAuthentication>(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new TwitterAuthentication());

            // when
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.Update(new Artist {Name = "swag"});

            // then
            _httpRequester
                .Verify(x => x.Get<TwiterSearchResults>(It.IsAny<HttpClient>(), It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public void ItShouldMarkADateAsOutOfRangeWhenItIsADayAfterTheStartDate()
        {
            // given
            const string input = "Sun Oct 29 22:19:30 +0000 2017";

            // when
            _cache.Setup(x => x.Get("DateUpdateStarted")).Returns(DateTime.Now.ToString("yyyy-MM-dd"));
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.SetMinAndMaxDates();
            var result = twitterDataSource.DateIsInRange(input);
            
            // then
            Assert.False(result);
        }

        [Fact]
        public void ItShouldMarkADateAsNotOutOfRangeWhenItIsInTheDateRange()
        {
            // given
            var input = $"{DateTime.Now.AddDays(-1):ddd MMM dd} 22:19:30 +0000 {DateTime.Now.Year}";

            // when
            _cache.Setup(x => x.Get("DateUpdateStarted")).Returns(DateTime.Now.ToString("yyyy-MM-dd"));
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.SetMinAndMaxDates();
            var result = twitterDataSource.DateIsInRange(input);

            // then
            Assert.True(result);
        }

        [Fact]
        public void ItShouldCorrectlyAddATwitterUserToTheRepository()
        {
            // given   
            _httpRequester.Setup(x => x.Get<TwiterSearchResults>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new TwiterSearchResults { statuses = new List<Status> { new Status { text = "swag", user = new User {name = "username"}, created_at = "Sat Mar 03 13:47:27 +0000 2018" } } } );
            _httpRequester.Setup(x => x.Post<TwitterAuthentication>(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new TwitterAuthentication());

            // when
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.Update(new Artist { Name = "swag" });

            // then
            _repository.Verify(x => x.AddUpdateUser(It.Is<TwitterUser>(y => y.Name.Equals("username"))), Times.Once);
        }

        [Fact]
        public void ItShouldCorrectlyAddAUserTimestampToTheRepository()
        {
            // given   
            _httpRequester.Setup(x => x.Get<TwiterSearchResults>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new TwiterSearchResults { statuses = new List<Status> { new Status { text = "swag", user = new User { name = "username", followers_count = 23}, created_at = "Sat Mar 03 13:47:27 +0000 2018" } } });
            _httpRequester.Setup(x => x.Post<TwitterAuthentication>(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new TwitterAuthentication());

            // when
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.Update(new Artist { Name = "swag" });

            // then
            _repository.Verify(x => x.AddUserTimestamp(It.Is<TwitterUserTimestamp>(y => y.FollowersCount == 23)), Times.Once);
        }

        [Fact]
        public void ItShouldCorrectlyAddATweetToTheRepository()
        {
            // given   
            _httpRequester.Setup(x => x.Get<TwiterSearchResults>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new TwiterSearchResults { statuses = new List<Status> { new Status { text = "swag", user = new User { name = "username", followers_count = 23}, created_at = "Sat Mar 03 13:47:27 +0000 2018" } } });
            _httpRequester.Setup(x => x.Post<TwitterAuthentication>(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new TwitterAuthentication());

            // when
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.Update(new Artist { Name = "swag" });

            // then
            _repository.Verify(x => x.AddTweet(It.Is<Tweet>(y => y.Text.Equals("swag"))), Times.Once);
        }

        [Fact]
        public void ATweetShouldOnlyBeSavedInTheRepositoryIfTheTweetContainsTheNameInFull()
        {
            // given
            _httpRequester.Setup(x => x.Get<TwiterSearchResults>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new TwiterSearchResults { statuses = new List<Status>
                {
                    new Status { text = "drums are the best", created_at = "Sat Mar 03 13:47:27 +0000 2018"},
                    new Status { text = "the DRUMS are good", created_at = "Sat Mar 03 13:47:27 +0000 2018" },
                    new Status { text = "THE DRUMS", created_at = "Sat Mar 03 13:47:27 +0000 2018" },
                    new Status { text = "I like The Drums", created_at = "Sat Mar 03 13:47:27 +0000 2018" }
                } });
            _httpRequester.Setup(x => x.Post<TwitterAuthentication>(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new TwitterAuthentication());            

            // when
            var twitterDataSource = new ArtistTwitterDataSource(_repository.Object, _httpRequester.Object, _cache.Object, _config.Object, _limitRepository.Object);
            twitterDataSource.Update(new Artist { Name = "The Drums" });

            // then
            _repository.Verify(x => x.AddTweet(It.IsAny<Tweet>()), Times.Exactly(3));
        }
    }
}

