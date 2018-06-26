using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.External;
using FYP.External.DataSources;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;
using FYP.Models.JsonModels;
using Moq;
using Xunit;

namespace FYP.UnitTests
{
    public class SpotifyDataSourceTests
    {
        private readonly Mock<IHttpRequester> _mockHttpRequester;
        private readonly Mock<ISpotifyDataSourceRepository> _mockSpotifyRepository;        
        private readonly Mock<IInvalidSpotifyRepository> _mockInvalidRepository;
        private readonly Mock<IGenreRepository> _mockGenreRepository;
        private readonly Mock<ISpotifyCredentials> _mockSpotifyCredentials;
        private readonly Mock<ICache> _cache;

        public SpotifyDataSourceTests()
        {
            _mockHttpRequester = new Mock<IHttpRequester>();
            _mockSpotifyRepository = new Mock<ISpotifyDataSourceRepository>();
            _mockInvalidRepository = new Mock<IInvalidSpotifyRepository>();
            _mockGenreRepository = new Mock<IGenreRepository>();
            _mockSpotifyCredentials = new Mock<ISpotifyCredentials>();
            _cache = new Mock<ICache>();
        }

        [Fact]
        public void WhenAnArtistIsPassedToTheDataSourceItShouldUpdateItsStatsAndItsAlbums()
        {
            // given
            SetupHttpRequesterWithDefaults();

            // when
            var spotifyDataSource = CreateDataSourceWithMocks();
            spotifyDataSource.Update(new Artist {Name = "banana"});

            // then
            _mockSpotifyRepository.Verify(x => x.AddArtistStats(It.IsAny<SpotifyArtistStats>()), Times.Once);
            _mockSpotifyRepository.Verify(x => x.AddAlbumStats(It.IsAny<SpotifyAlbumStats>()), Times.Once);
        }

        [Fact]
        public void WhenNoDataIsReturnedFromTheSearchItShouldAddTheIdToTheInvalidSpotifyRepository()
        {
            // given
            SetupHttpRequesterWithDefaults();
            _mockHttpRequester.Setup(x => x.Get<SpotifyArtistSearch>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new SpotifyArtistSearch());

            // when
            var spotifyDataSource = CreateDataSourceWithMocks();
            spotifyDataSource.Update(new Artist {Name = "The Unknowns", Id = 99});

            // then
            _mockHttpRequester.Verify(x => x.Get<SpotifyArtistSearch>(It.IsAny<HttpClient>(), It.IsAny<string>()), Times.Once);
            _mockInvalidRepository.Verify(x => x.AddArtistToInvalidList(99), Times.Once);
            _mockSpotifyRepository.Verify(x => x.AddArtistStats(It.IsAny<SpotifyArtistStats>()), Times.Never);
        }

        private void SetupHttpRequesterWithDefaults()
        {
            _mockHttpRequester.Setup(x => x.Post<SpotifyAuthentication>(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new SpotifyAuthentication());
            _mockHttpRequester.Setup(x => x.Get<SpotifyArtistSearch>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new SpotifyArtistSearch { artists = new Artists { items = new List<Item> { new Item { id = "1", name = "banana"} } } });
            _mockHttpRequester.Setup(x => x.Get<SpotifyArtist>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new SpotifyArtist { id = "1", followers = new Followers { total = 100 }, popularity = 200 });
            _mockHttpRequester.Setup(x => x.Get<SpotifyArtistAlbums>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new SpotifyArtistAlbums { items = new List<AlbumItem> { new AlbumItem { id = "11" } } });
            _mockHttpRequester.Setup(x => x.Get<SpotifyAlbumDetail>(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .Returns(new SpotifyAlbumDetail { albums = new List<AlbumDetail> { new AlbumDetail { id = "12", label = "swag" } } });
        }

        private SpotifyDataSource CreateDataSourceWithMocks()
        {
            return new SpotifyDataSource(_mockSpotifyRepository.Object, _mockHttpRequester.Object, _mockInvalidRepository.Object,
                _mockGenreRepository.Object, _mockSpotifyCredentials.Object);
        }
    }
}
