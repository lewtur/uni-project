using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using FluentValidation;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class EventByDate
    {
        public class FilterByDate : IRequest<IEnumerable<FullEvent>>
        {
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string Cities { get; set; }
            public string Genres { get; set; }
        }

        public class EventByDateValidator : AbstractValidator<FilterByDate>
        {
            public EventByDateValidator()
            {
                RuleFor(x => x.EndDate)
                    .Must(y => DateTime.TryParseExact(y, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result));
                RuleFor(x => x.StartDate)
                    .Must(y => DateTime.TryParseExact(y, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result));
            }
        }


        public class Handler : IAsyncRequestHandler<FilterByDate, IEnumerable<FullEvent>>
        {
            private readonly IVenueRepository _venueRepository;
            private readonly IEventRepository _eventRepository;
            private readonly IArtistRepository _artistRepository;
            private readonly IGenreRepository _genreRepository;

            public Handler(IVenueRepository venueRepository, IEventRepository eventRepository, IArtistRepository artistRepository, IGenreRepository genreRepository)
            {
                _venueRepository = venueRepository;
                _eventRepository = eventRepository;
                _artistRepository = artistRepository;
                _genreRepository = genreRepository;
            }

            public async Task<IEnumerable<FullEvent>> Handle(FilterByDate message)
            {               
                var fullEvents = new List<FullEvent>();

                var currentDate = DateTime.ParseExact(message.StartDate, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                var endDate = DateTime.ParseExact(message.EndDate, "yyyy-MM-dd", CultureInfo.CurrentCulture);

                var expression = currentDate.Day == endDate.Day
                    ? new Func<bool>(() => currentDate.Day == endDate.Day) 
                    : new Func<bool>(() => currentDate.Day != endDate.Day || currentDate.Month != endDate.Month);

                var events = new List<Event>();
                while (expression.Invoke())
                {
                    events.AddRange(_eventRepository.GetEventsOnDate(currentDate.ToString("yyyy-MM-dd")).Result);
                    currentDate = currentDate.AddDays(1);
                }

                foreach (var gig in events)
                {
                    var venue = await _venueRepository.GetVenue(gig.VenueId);
                    var linkups = await _eventRepository.GetArtistIdsFromEventIdLinkups(gig.Id);
                    var artistList = linkups.Select(artistId => _artistRepository.GetNameAndSpotifyRecordId(artistId).Result).ToList();

                    var detailedArtistList = (from artist in artistList
                        where artist != null
                        let genres = _genreRepository.GetGenreForArtist(artist.Id).Result
                        select new DetailedArtist
                        {
                            Name = artist.Name,
                            ArtistId = artist.Id,
                            Description = artist.Description,
                            ImageUrl = artist.ImageUrl,
                            SpotifyRecordId = artist.SpotifyRecordId,
                            Delta = 0,
                            SpotifyGivenGenre = genres?.SpotifyGivenGenre,
                            OtherGenresGivenInRelatedArtists = genres?.OtherGenresGivenInRelatedArtists,
                            MostPopularGenreOfRelatedArtists = genres?.MostPopularGenreOfRelatedArtists
                        }).ToList();

                    fullEvents.Add(new FullEvent
                    {
                        Event = gig,
                        Venue = venue,
                        Artist = detailedArtistList
                    });
                }

                return fullEvents;
            }
        }
    }
}
