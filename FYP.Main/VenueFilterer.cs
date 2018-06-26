using System.Collections.Generic;
using System.Linq;
using Autofac;
using FYP.Data;
using FYP.Models;

namespace FYP.Main
{
    public interface IVenueFilterer
    {
        IEnumerable<Venue> GetVenuesToUpdate();
    }

    public class VenueFilterer : IVenueFilterer
    {
        private readonly IVenueRepository _venueRepository;
        private readonly ITwitterLimitRepository _twitterLimitRepository;

        public VenueFilterer(IVenueRepository venueRepository, ITwitterLimitRepository twitterLimitRepository)
        {
            _venueRepository = venueRepository;
            _twitterLimitRepository = twitterLimitRepository;
        }

        public IEnumerable<Venue> GetVenuesToUpdate()
        {
            var venues = _venueRepository.GetAll().Result;

            return venues
                .Where(x => !_twitterLimitRepository.VenueHasExceededLimit(x.Id).Result).ToList();
        }
    }
}