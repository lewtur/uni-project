using System;
using System.Runtime.CompilerServices;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.External;
using FYP.External.DataSources;
using FYP.Main;
using FYP.Models;
using FYP.Models.DataSourceRecords;
using Moq;
using Xunit;

namespace FYP.IntegrationTests
{
    public class API_TwitterDataSourceTests
    {
        private readonly Mock<ITwitterDataSourceRepository> _mockTwitterRepository;
        private readonly Mock<ITwitterLimitRepository> _mockLimitRepository;
             
        public API_TwitterDataSourceTests()
        {            
            _mockTwitterRepository = new Mock<ITwitterDataSourceRepository>();
            _mockLimitRepository = new Mock<ITwitterLimitRepository>();
        }

        [Fact]
        public void ShouldBeAbleToMakeARequestToTheTwitterApiForAnArtistThroughTheDataSource()
        {
            // given
            MemoryCache.Instance.Add(DateTime.Now.ToString("yyyy-MM-dd"), "DateUpdateStarted", DateTime.Now.AddHours(6));            
            var twitterDataSource = new ArtistTwitterDataSource(_mockTwitterRepository.Object, new HttpRequester(),
                MemoryCache.Instance, new TwitterDataSourceConfigForIntegrationTests(), _mockLimitRepository.Object);            

            // when
            twitterDataSource.Update(new Artist {Name = "Blossoms"});

            // then
            _mockTwitterRepository.Verify(x => x.AddTweet(It.IsAny<Tweet>()), Times.Once);
        }
    }

    public class TwitterDataSourceConfigForIntegrationTests : TwitterDataSourceConfig
    {
        public override int MaximumNumberStoredUnderOneEntity => 2;
    }
}
