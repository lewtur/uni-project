using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.External;
using FYP.External.DataSources;
using FYP.Main;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;
using Moq;
using Xunit;

namespace FYP.IntegrationTests
{
    public class UpdateIntegrationTests
    {
        private readonly Mock<IArtistRepository> _mockArtistRepository;
        private readonly Mock<IVenueRepository> _mockVenueRepository;
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly Mock<IGenreRepository> _mockGenreRepository;
        private readonly Mock<ISpotifyDataSourceRepository> _mockSpotifyDataSourceRepository;
        private readonly Mock<IInvalidSpotifyRepository> _mockInvalidSpotifyRepository;
        private readonly Mock<ITwitterDataSourceRepository> _mockTwitterDataSourceRepository;
        private readonly Mock<ITwitterLimitRepository> _mockTwitterLimitRepository;

        public UpdateIntegrationTests()
        {
            _mockArtistRepository = new Mock<IArtistRepository>();
            _mockVenueRepository = new Mock<IVenueRepository>();
            _mockEventRepository = new Mock<IEventRepository>();
            _mockGenreRepository = new Mock<IGenreRepository>();
            _mockSpotifyDataSourceRepository = new Mock<ISpotifyDataSourceRepository>();
            _mockInvalidSpotifyRepository = new Mock<IInvalidSpotifyRepository>();
            _mockTwitterDataSourceRepository = new Mock<ITwitterDataSourceRepository>();
            _mockTwitterLimitRepository = new Mock<ITwitterLimitRepository>();
        }

        [Fact]
        public void ShouldBeAbleToRunADailyUpdateUsingTheSpotifyDataSource()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll())
                .Returns(Task.FromResult(new List<Artist> {new Artist {Name = "Blossoms", Id = 99}}.Select(x => x)));
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(99))
                .Returns(Task.FromResult(false));
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(99))
                .Returns(Task.FromResult(false));
            _mockSpotifyDataSourceRepository.Setup(x => x.GetOrCreateArtistHeaderId(It.IsAny<SpotifyArtistHeader>()))
                .Returns(Task.FromResult(909));
            _mockSpotifyDataSourceRepository.Setup(x => x.GetOrCreateAlbumHeaderId(It.IsAny<SpotifyAlbumHeader>()))
                .Returns(9009);
            _mockGenreRepository.Setup(x => x.GetGenreForArtist(99))
                .Returns(Task.FromResult(new Genre()));


           var updater = new MainUpdater(
                new List<IDataSource>
                {
                    new SpotifyDataSource(
                        _mockSpotifyDataSourceRepository.Object,
                        new HttpRequester(),
                        _mockInvalidSpotifyRepository.Object,
                        _mockGenreRepository.Object,
                        new SpotifyCredentials(MemoryCache.Instance, new HttpRequester())
                    )
                },
                new List<IDataSource>(),
                new List<IPostUpdateAction>(),
                new EventUpdater(
                    _mockArtistRepository.Object,
                    _mockEventRepository.Object,
                    _mockVenueRepository.Object,
                    new EventsRetriever(new HttpRequester()),
                    new List<Location>
                    {
                        new Location {Name = "Manchester", Longitude = "-2.2446", Latitude = "53.4839", Radius = "6"}
                    },
                    new ArtistInfoRetriever(new HttpRequester())
                ),
                new ArtistFilterer(
                    _mockArtistRepository.Object,
                    _mockTwitterLimitRepository.Object,
                    _mockInvalidSpotifyRepository.Object
                ),
                new VenueFilterer(
                    _mockVenueRepository.Object,
                    _mockTwitterLimitRepository.Object
                ),
                new ConsoleLogger()
            );

            // when
            updater.UpdateDataSources();

            // then
            _mockInvalidSpotifyRepository.Verify(x => x.AddArtistToInvalidList(99), Times.Never);
            _mockSpotifyDataSourceRepository.Verify(x => x.AddArtistStats(It.IsAny<SpotifyArtistStats>()), Times.Once);
            _mockGenreRepository.Verify(x => x.GetGenreForArtist(99), Times.Once);
            _mockSpotifyDataSourceRepository.Verify(x => x.AddAlbumStats(It.IsAny<SpotifyAlbumStats>()), Times.AtLeastOnce);
        }

        [Fact]
        public void ShouldBeAbleToRunADailyUpdateUsingTheTwitterDataSource()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll())
                .Returns(Task.FromResult(new List<Artist> { new Artist { Name = "Blossoms", Id = 99 } }.Select(x => x)));
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(99))
                .Returns(Task.FromResult(false));
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(99))
                .Returns(Task.FromResult(false));

            var updater = new MainUpdater(
                new List<IDataSource>
                {
                    new ArtistTwitterDataSource(
                        _mockTwitterDataSourceRepository.Object,
                        new HttpRequester(), 
                        MemoryCache.Instance,
                        new TwitterDataSourceConfigForIntegrationTests(),
                        _mockTwitterLimitRepository.Object
                    )
                },
                new List<IDataSource>(),
                new List<IPostUpdateAction>(),
                new EventUpdater(
                    _mockArtistRepository.Object,
                    _mockEventRepository.Object,
                    _mockVenueRepository.Object,
                    new EventsRetriever(new HttpRequester()),
                    new List<Location>
                    {
                        new Location {Name = "Manchester", Longitude = "-2.2446", Latitude = "53.4839", Radius = "6"}
                    },
                    new ArtistInfoRetriever(new HttpRequester())
                ),
                new ArtistFilterer(
                    _mockArtistRepository.Object,
                    _mockTwitterLimitRepository.Object,
                    _mockInvalidSpotifyRepository.Object
                ),
                new VenueFilterer(
                    _mockVenueRepository.Object,
                    _mockTwitterLimitRepository.Object
                ),
                new ConsoleLogger()
            );

            // when
            updater.UpdateDataSources();

            // then
            _mockTwitterDataSourceRepository.Verify(x => x.AddUpdateUser(It.IsAny<TwitterUser>()), Times.AtLeastOnce);
            _mockTwitterDataSourceRepository.Verify(x => x.AddUserTimestamp(It.IsAny<TwitterUserTimestamp>()), Times.AtLeastOnce);
            _mockTwitterDataSourceRepository.Verify(x => x.AddTweet(It.IsAny<Tweet>()), Times.AtLeastOnce);
        }

        [Fact]
        public void ShouldBeAbleToRunADailyUpdateUsingTheArtistDescriptionDataSource()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll())
                .Returns(Task.FromResult(new List<Artist> { new Artist { Name = "Blossoms", Id = 99 } }.Select(x => x)));
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(99))
                .Returns(Task.FromResult(false));
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(99))
                .Returns(Task.FromResult(false));

            var updater = new MainUpdater(
                new List<IDataSource>
                {
                    new ArtistDescriptionDataSource(
                        _mockArtistRepository.Object,
                        new ArtistInfoRetriever(new HttpRequester())
                    )
                },
                new List<IDataSource>(),
                new List<IPostUpdateAction>(),
                new EventUpdater(
                    _mockArtistRepository.Object,
                    _mockEventRepository.Object,
                    _mockVenueRepository.Object,
                    new EventsRetriever(new HttpRequester()),
                    new List<Location>
                    {
                        new Location {Name = "Manchester", Longitude = "-2.2446", Latitude = "53.4839", Radius = "6"}
                    },
                    new ArtistInfoRetriever(new HttpRequester())
                ),
                new ArtistFilterer(
                    _mockArtistRepository.Object,
                    _mockTwitterLimitRepository.Object,
                    _mockInvalidSpotifyRepository.Object
                ),
                new VenueFilterer(
                    _mockVenueRepository.Object,
                    _mockTwitterLimitRepository.Object
                ),
                new ConsoleLogger()
            );

            // when
            updater.UpdateDataSources();

            // then
            _mockArtistRepository.Verify(x => x.UpdateArtistDescription(It.IsAny<string>(), 99), Times.Once);
        }

        [Fact]
        public void ShouldBeAbleToRunADailyUpdateUsingTheSpotifyImageDataSource()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll())
                .Returns(Task.FromResult(new List<Artist> { new Artist { Name = "Blossoms", Id = 99 } }.Select(x => x)));
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(99))
                .Returns(Task.FromResult(false));
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(99))
                .Returns(Task.FromResult(false));
            _mockSpotifyDataSourceRepository.Setup(x => x.GetAllArtistHeaders())
                .Returns(new List<SpotifyArtistHeader> {new SpotifyArtistHeader {ArtistId = 99, SpotifyRecordId = "22RISwgVJyZu9lpqAcv1F5" } });

            var updater = new MainUpdater(
                new List<IDataSource>
                {
                    new SpotifyArtistImageDataSource(
                        new HttpRequester(),
                        _mockSpotifyDataSourceRepository.Object,
                        _mockArtistRepository.Object,
                        new SpotifyCredentials(MemoryCache.Instance, new HttpRequester())
                    )
                },
                new List<IDataSource>(),
                new List<IPostUpdateAction>(),
                new EventUpdater(
                    _mockArtistRepository.Object,
                    _mockEventRepository.Object,
                    _mockVenueRepository.Object,
                    new EventsRetriever(new HttpRequester()),
                    new List<Location>
                    {
                        new Location {Name = "Manchester", Longitude = "-2.2446", Latitude = "53.4839", Radius = "6"}
                    },
                    new ArtistInfoRetriever(new HttpRequester())
                ),
                new ArtistFilterer(
                    _mockArtistRepository.Object,
                    _mockTwitterLimitRepository.Object,
                    _mockInvalidSpotifyRepository.Object
                ),
                new VenueFilterer(
                    _mockVenueRepository.Object,
                    _mockTwitterLimitRepository.Object
                ),
                new ConsoleLogger()
            );

            // when
            updater.UpdateDataSources();

            // then
            _mockArtistRepository.Verify(x => x.UpdateImageUrl(It.IsAny<string>(), 99), Times.Once);
        }
    }    
}
