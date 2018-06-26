using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FYP.Data;
using FYP.External;
using FYP.Main;
using FYP.Models;
using FYP.Models.JsonModels;
using Moq;
using Xunit;

namespace FYP.UnitTests
{
    public class EventUpdaterTests
    {
        private readonly Mock<IArtistRepository> _mockArtistRepository;
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly Mock<IVenueRepository> _mockVenueRepository;
        private readonly Mock<IEventsRetriever> _mockEventsRetriever;
        private readonly Mock<IArtistInfoRetriever> _mockArtistInfoRetriever;

        public EventUpdaterTests()
        {
            _mockArtistRepository = new Mock<IArtistRepository>();
            _mockEventRepository = new Mock<IEventRepository>();
            _mockEventsRetriever = new Mock<IEventsRetriever>();
            _mockVenueRepository = new Mock<IVenueRepository>();
            _mockArtistInfoRetriever = new Mock<IArtistInfoRetriever>();
        }

        [Fact]
        public void WhenANewArtistComesBackFromTheEventRetrieverThenItShouldWriteItToTheDatabase()
        {
            // given
            _mockEventsRetriever
                .Setup(x => x.GetEventsForDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Location>()))
                .Returns(new SkiddleEvent
                {
                    results = new List<EventResult>
                    {
                        new EventResult
                        {
                            artists = new List<SkiddleArtist>
                            {
                                new SkiddleArtist {name = "Blossoms"}
                            }
                        }
                    }
                });

            _mockArtistRepository.Setup(x => x.Get("Blossoms")).Returns(Task.FromResult(new Artist()));

        // when
            var eventUpdater = CreateEventUpdater();
            eventUpdater.RunTodaysEvents();

            // then
            _mockArtistRepository.Verify(x => x.Add(It.IsAny<Artist>()), Times.Once);
        }

        [Fact]
        public void WhenAnExistingArtistComesBackFromTheEventRetrieverThenItShouldNotWriteItToTheDatabase()
        {
            // given
            _mockEventsRetriever.Setup(x => x.GetEventsForDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Location>()))
                .Returns(new SkiddleEvent
                {
                    results = new List<EventResult>
                    {
                        new EventResult
                        {
                            artists = new List<SkiddleArtist>
                            {
                                new SkiddleArtist {name = "Blossoms"}
                            }
                        }

                    }
                });

            _mockArtistRepository.Setup(x => x.Get("Blossoms")).Returns(Task.FromResult(new Artist {Name = "Blossoms"}));

            // when
            var eventUpdater = CreateEventUpdater();
            eventUpdater.RunTodaysEvents();

            // then
            _mockArtistRepository.Verify(x => x.Add(It.IsAny<Artist>()), Times.Never);
        }

        [Fact]
        public void IfNothingComesBackFromTheEventRetrieverThenNoRepositoriesShouldBeWrittenTo()
        {
            // given
            _mockEventsRetriever.Setup(x => x.GetEventsForDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Location>())).Returns(new SkiddleEvent());

            // when
            var eventUpdater = CreateEventUpdater();
            eventUpdater.RunTodaysEvents();

            // then
            _mockArtistRepository.Verify(x => x.Add(It.IsAny<Artist>()), Times.Never);
            _mockEventRepository.Verify(x => x.Add(It.IsAny<Event>()), Times.Never);
            _mockVenueRepository.Verify(x => x.Add(It.IsAny<Venue>()), Times.Never);
        }

        [Fact]
        public void WhenAVenueIsWrittenToTheDatabaseItsIdShouldBeStoredAgainstTheEvent()
        {
            // given
            var venue = new SkiddleVenue {name = "Bojangles"};
            _mockEventsRetriever
                .Setup(x => x.GetEventsForDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Location>()))
                .Returns(new SkiddleEvent
                {
                    results = new List<EventResult> {
                        new EventResult {
                            venue = venue,
                            artists = new List<SkiddleArtist>
                            {
                                new SkiddleArtist {name = "Blossoms"}
                            }                    
                        } 
                    }
                });
            _mockVenueRepository
                .Setup(x => x.Add(It.Is<Venue>(y => y.Name == "Bojangles")))
                .Returns(Task.FromResult(99));
            _mockArtistRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(new Artist {Id = 32}));

            // when
            var eventUpdater = CreateEventUpdater();
            eventUpdater.RunTodaysEvents();
            
            // then
            _mockEventRepository.Verify(x => x.Add(It.Is<Event>(y => y.VenueId == 99)), Times.Once);
        }

        [Fact]
        public void WhenThereAreNoArtistsInTheEventTheEventNameShouldBeStoredInTheHoldingTable()
        {
            // given
            _mockEventsRetriever.Setup(x => x.GetEventsForDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Location>()))
                .Returns(new SkiddleEvent
                {
                    results = new List<EventResult>
                    {
                        new EventResult
                        {
                            eventname = "yadda yadda"
                        }
                    }
                });

            // when
            var eventUpdater = CreateEventUpdater();
            eventUpdater.RunTodaysEvents();

            // then
            _mockArtistRepository.Verify(x => x.Add(It.IsAny<Artist>()), Times.Never);
            _mockEventRepository.Verify(x => x.AddToEventHoldingTable(It.Is<EventHolding>(y => y.EventName.Equals("yadda yadda"))), Times.Once);
        }

        [Fact]
        public void IfTheEventIsAlreadyInTheDatabaseItShouldNotWriteTheEventAgain()
        {
            // given
            _mockEventsRetriever.Setup(x => x.GetEventsForDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Location>()))
                .Returns(new SkiddleEvent
                {
                    results = new List<EventResult>
                    {
                        new EventResult
                        {
                            eventname = "swagger mcjagger"
                        }
                    }
                });
            _mockEventRepository.Setup(x => x.GetEventId("swagger mcjagger")).Returns(Task.FromResult(34));

            // when
            var eventUpdater = CreateEventUpdater();
            eventUpdater.RunTodaysEvents();

            // then
            _mockEventRepository.Verify(x => x.Add(It.IsAny<Event>()), Times.Never);
        }

        private EventUpdater CreateEventUpdater()
        {
            return new EventUpdater(
                _mockArtistRepository.Object,
                _mockEventRepository.Object,
                _mockVenueRepository.Object,
                _mockEventsRetriever.Object,
                new List<Location>
                {
                    new Location {Name = "Burnley", Longitude = "-999", Latitude = "999"}
                },
                _mockArtistInfoRetriever.Object);
        }
    }
}
