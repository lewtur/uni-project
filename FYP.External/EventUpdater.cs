using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FYP.Data;
using FYP.Models;
using FYP.Models.JsonModels;
using Newtonsoft.Json;

namespace FYP.External
{
    public class EventUpdater : IEventUpdater
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IEventsRetriever _eventsRetriever;
        private readonly IArtistInfoRetriever _artistInfoRetriever;
        private readonly List<Location> _locations;

        public EventUpdater(IArtistRepository artistRepository, IEventRepository eventRepository, IVenueRepository venueRepository, IEventsRetriever eventsRetriever, List<Location> locations, IArtistInfoRetriever artistInfoRetriever)
        {
            _artistRepository = artistRepository;
            _eventRepository = eventRepository;
            _venueRepository = venueRepository;
            _eventsRetriever = eventsRetriever;
            _locations = locations;
            _artistInfoRetriever = artistInfoRetriever;
        }

        public void RunTodaysEvents()
        {
            RunEventsForDateRange(DateTime.Now, DateTime.Now);
        }

        public void RunEventsForDateRange(DateTime startDate, DateTime endDate)
        {
            foreach (var location in _locations)
            { 
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Running events for {location.Name}");
                var eventOverview = _eventsRetriever.GetEventsForDateRange(startDate, endDate, location);

                if (eventOverview?.results == null) continue;

                foreach (var eventResult in eventOverview.results)
                {
                    if (_eventRepository.GetEventId(eventResult.eventname).Result > 0) continue;

                    var eventContainsArtists = false;
                    var artistIds = new List<int>();

                    if (eventResult.artists != null)
                    {
                        foreach (var artist in eventResult.artists)
                        {
                            var artistId = SaveOrGetArtist(artist);
                            if (artistId <= 0) continue;

                            eventContainsArtists = true;
                            artistIds.Add(artistId);
                        }
                    }

                    var venueId = 0;
                    if (!string.IsNullOrEmpty(eventResult.venue?.name))
                    {                   
                        venueId = AddVenue(eventResult.venue);
                    }

                    var eventId = AddEvent(eventResult, venueId);

                    if (eventContainsArtists)
                    {
                        _eventRepository.AddLinkup(new EventLinkup { ArtistIds = artistIds, EventId = eventId });
                    }                  
                    else
                    {
                        _eventRepository.AddToEventHoldingTable(
                            new EventHolding {EventId = eventId, EventName = eventResult.eventname});
                    }
                }
            }
        }

        private int SaveOrGetArtist(SkiddleArtist artist)
        {
            if (string.IsNullOrEmpty(artist.name)) return 0;

            var call = _artistRepository.Get(artist.name);
            call.Wait();
            var savedArtist = call.Result;

            if (!string.IsNullOrEmpty(savedArtist?.Name)) return savedArtist.Id;

            var artistDescription = _artistInfoRetriever.GetArtistDescription(artist.name);

            return _artistRepository.Add(new Artist {Name = artist.name, Description = artistDescription}).Result;            
        }

        private int AddVenue(SkiddleVenue venue)
        {
            return _venueRepository.Add(new Venue
            {
                Name = venue?.name,
                Address = venue?.address,
                Town = venue?.town,
                PostCode = venue?.postcode,
                Latitude = venue?.latitude.ToString(CultureInfo.CurrentCulture),
                Longitude = venue?.longitude.ToString(CultureInfo.CurrentCulture)
            }).Result;
        }

        private int AddEvent(EventResult eventResult, int venueId)
        {
            return _eventRepository.Add(new Event
            {
                Name = eventResult.eventname,
                Cancelled = eventResult.cancelled == "0",
                VenueId = venueId,
                StartDate = eventResult.startdate,
                EndDate = eventResult.enddate,
                Description = eventResult.description,
                DoorsOpen = eventResult.openingtimes?.doorsclose,
                DoorsClose = eventResult.openingtimes?.doorsopen,
                LastEntry = eventResult.openingtimes?.lastentry,
                MinAge = string.IsNullOrEmpty(eventResult.minage) ? 0 : int.Parse(eventResult.minage)
            }).Result;
        }
    }

    public interface IEventUpdater
    {
        void RunTodaysEvents();
        void RunEventsForDateRange(DateTime startDate, DateTime endDate);
    }
}