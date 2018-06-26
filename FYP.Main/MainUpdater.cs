using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Autofac;
using FYP.External;
using FYP.Models;
using FYP.Models.Abstractions;

namespace FYP.Main
{
    public class MainUpdater : IMainUpdater
    {
        private readonly IEventUpdater _eventUpdater;
        private readonly IEnumerable<IDataSource> _artistDataSources;
        private readonly IEnumerable<IDataSource> _venueDataSources;
        private readonly IEnumerable<IPostUpdateAction> _postUpdateActions;
        private readonly IArtistFilterer _artistFilterer;
        private readonly IVenueFilterer _venueFilterer;
        private readonly ILogger _logger;

        public MainUpdater(IEnumerable<IDataSource> artistDataSources, IEnumerable<IDataSource> venueDataSources, IEnumerable<IPostUpdateAction> postUpdateActions, IEventUpdater eventUpdater, IArtistFilterer artistFilterer, IVenueFilterer venueFilterer, ILogger logger)
        {
            _artistDataSources = artistDataSources;
            _venueDataSources = venueDataSources;
            _postUpdateActions = postUpdateActions;
            _eventUpdater = eventUpdater;
            _artistFilterer = artistFilterer;
            _venueFilterer = venueFilterer;
            _logger = logger;
        }

        public void UpdateEventsAndDataSources()
        {
            _eventUpdater.RunEventsForDateRange(DateTime.Now.AddDays(29), DateTime.Now.AddDays(31));
            UpdateDataSources();
        }

        public void UpdateDataSources()
        {
            MemoryCache.Instance.Add(DateTime.Now.ToString("yyyy-MM-dd"), "DateUpdateStarted", DateTime.Now.AddHours(6));

            var artists = _artistFilterer.GetArtistsToUpdate();
            var artistCount = artists.Count();            

            _logger.Write($"Retrieved {artistCount} artists. Starting artist update");

            foreach (var dataSource in _artistDataSources)
            {
                var i = 0;
                _logger.Write($"Starting {dataSource.GetName()} update");
                foreach (var artist in artists)
                {
                    dataSource.Update(artist);
                    if (++i % 100 == 0) _logger.Write($"Processed {i}/{artistCount} artists for {dataSource.GetName()} data source");
                }
                _logger.Write($"Finished {dataSource.GetName()} update");
            }

            _logger.Write("Finished artist update.");

            var venues = _venueFilterer.GetVenuesToUpdate();
            var venueCount = venues.Count();

            _logger.Write($"Retrieved {venueCount} venues. Starting venue update");

            foreach (var dataSource in _venueDataSources)
            {
                var i = 0;
                foreach (var venue in venues)
                {
                    dataSource.Update(venue);
                    if (++i % 100 == 0) _logger.Write($"Processed {i}/{venueCount} venues for {dataSource.GetName()} data source");
                }
            }

            _logger.Write("Finished venue update. Starting post update actions");

            foreach (var action in _postUpdateActions)
            {
                foreach (var artist in artists)
                {
                    var call = action.Act(artist.Id);
                    call.Wait();
                }
            }
        }

        public void UpdateEventsInDateRange(DateTime startDate, DateTime endDate)
        {
            var currentDate = startDate;
            while (currentDate.Day != endDate.Day || currentDate.Month != endDate.Month)
            {
                _eventUpdater.RunEventsForDateRange(currentDate, currentDate);
                currentDate = currentDate.AddDays(1);
            }
        }
    }
}