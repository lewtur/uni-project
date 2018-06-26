using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    public class API_SpotifyDataSourceTests
    {
        private readonly Mock<ISpotifyDataSourceRepository> _mockSpotifyRepository;
        private readonly Mock<IInvalidSpotifyRepository> _mockInvalidRepository;
        private readonly Mock<IGenreRepository> _mockGenreRepository;

        public API_SpotifyDataSourceTests()
        {
            _mockSpotifyRepository = new Mock<ISpotifyDataSourceRepository>();
            _mockInvalidRepository = new Mock<IInvalidSpotifyRepository>();
            _mockGenreRepository = new Mock<IGenreRepository>();
        }

        [Fact]
        public void ShouldBeAbleToMakeARequestToTheSpotifyApiForAnArtistThroughTheDataSource()
        {
            // given
            _mockSpotifyRepository.Setup(x => x.GetOrCreateArtistHeaderId(It.IsAny<SpotifyArtistHeader>())).Returns(Task.FromResult(1));
            var spotifyDataSource = new SpotifyDataSource(_mockSpotifyRepository.Object, new HttpRequester(),
                _mockInvalidRepository.Object, _mockGenreRepository.Object, new SpotifyCredentials(MemoryCache.Instance, new HttpRequester()));

            // when
            spotifyDataSource.Update(new Artist { Name = "Blossoms" });

            // then
            _mockSpotifyRepository.Verify(x => x.AddArtistStats(It.IsAny<SpotifyArtistStats>()), Times.Once);
        }
    }
}
