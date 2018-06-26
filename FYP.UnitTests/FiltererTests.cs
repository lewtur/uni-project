using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Main;
using FYP.Models;
using Moq;
using Xunit;

namespace FYP.UnitTests
{
    public class FiltererTests
    {
        private readonly Mock<IArtistRepository> _mockArtistRepository;
        private readonly Mock<IVenueRepository> _mockVenueRepository;
        private readonly Mock<ITwitterLimitRepository> _mockTwitterLimitRepository;
        private readonly Mock<IInvalidSpotifyRepository> _mockInvalidSpotifyRepository;

        public FiltererTests()
        {
            _mockArtistRepository = new Mock<IArtistRepository>();
            _mockVenueRepository = new Mock<IVenueRepository>();
            _mockTwitterLimitRepository = new Mock<ITwitterLimitRepository>();
            _mockInvalidSpotifyRepository = new Mock<IInvalidSpotifyRepository>();
        }

        [Fact]
        public void IfAnArtistIsExcludedByTwitterAndNotSpotifyItShouldNotBeUpdated()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll()).ReturnsAsync(new List<Artist>
            {
                new Artist {Id = 1, Name = "banana"},
                new Artist {Id = 2, Name = "bananas"}
            });

            // when
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(1)).ReturnsAsync(false);
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(2)).ReturnsAsync(true);
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(It.IsAny<int>())).ReturnsAsync(false);
            var artistFilterer = new ArtistFilterer(_mockArtistRepository.Object, _mockTwitterLimitRepository.Object,
                _mockInvalidSpotifyRepository.Object);
            
            // then
            var artists = artistFilterer.GetArtistsToUpdate();
            Assert.True(artists.Count() == 1);
            Assert.False(artists.First().Id == 2);
        }

        [Fact]
        public void IfAnArtistIsExcludedBySpotiftAndNotTwitterItShouldNotBeUpdated()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll()).ReturnsAsync(new List<Artist>
            {
                new Artist {Id = 1, Name = "banana"},
                new Artist {Id = 2, Name = "bananas"}
            });

            // when
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(It.IsAny<int>())).ReturnsAsync(false);
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(1)).ReturnsAsync(true);
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(2)).ReturnsAsync(false);
            var artistFilterer = new ArtistFilterer(_mockArtistRepository.Object, _mockTwitterLimitRepository.Object,
                _mockInvalidSpotifyRepository.Object);

            // then
            var artists = artistFilterer.GetArtistsToUpdate();
            Assert.True(artists.Count() == 1);
            Assert.False(artists.First().Id == 1);
        }

        [Fact]
        public void IfAnArtistIsExcludedBySpotiftAndTwitterItShouldNotBeUpdated()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll()).ReturnsAsync(new List<Artist>
            {
                new Artist {Id = 1, Name = "banana"},
                new Artist {Id = 2, Name = "bananas"}
            });

            // when
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(It.IsAny<int>())).ReturnsAsync(true);
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(It.IsAny<int>())).ReturnsAsync(true);
            var artistFilterer = new ArtistFilterer(_mockArtistRepository.Object, _mockTwitterLimitRepository.Object,
                _mockInvalidSpotifyRepository.Object);

            // then
            var artists = artistFilterer.GetArtistsToUpdate();
            Assert.False(artists.Any());
        }

        [Fact]
        public void IfAnArtistIsNotExcludedBySpotifyOrTwitterThenItShouldBeUpdated()
        {
            // given
            _mockArtistRepository.Setup(x => x.GetAll()).ReturnsAsync(new List<Artist>
            {
                new Artist {Id = 1, Name = "banana"},
                new Artist {Id = 2, Name = "bananas"}
            });

            // when
            _mockTwitterLimitRepository.Setup(x => x.ArtistHasExceededLimit(It.IsAny<int>())).ReturnsAsync(false);
            _mockInvalidSpotifyRepository.Setup(x => x.ArtistIsInInvalidList(It.IsAny<int>())).ReturnsAsync(false);
            var artistFilterer = new ArtistFilterer(_mockArtistRepository.Object, _mockTwitterLimitRepository.Object,
                _mockInvalidSpotifyRepository.Object);

            // then
            var artists = artistFilterer.GetArtistsToUpdate();
            Assert.True(artists.Count(x => x.Id == 1 || x.Id == 2) == 2);
        }

        [Fact]
        public void IfAVenueIsExcludedByTwitterItShouldNotBeUpdated()
        {
            // given
            _mockVenueRepository.Setup(x => x.GetAll()).ReturnsAsync(new List<Venue>
            {
                new Venue {Id = 1, Name = "banana lounge"},
                new Venue {Id = 2, Name = "the bananas"}
            });

            // when
            _mockTwitterLimitRepository.Setup(x => x.VenueHasExceededLimit(1)).ReturnsAsync(false);
            _mockTwitterLimitRepository.Setup(x => x.VenueHasExceededLimit(2)).ReturnsAsync(true);
            var venueFilterer = new VenueFilterer(_mockVenueRepository.Object, _mockTwitterLimitRepository.Object);

            // then
            var venues = venueFilterer.GetVenuesToUpdate();
            Assert.True(venues.Count() == 1);
            Assert.True(venues.First().Id == 1);
        }
    }
}
