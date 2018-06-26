using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Models;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class EventSummary
    {
        public class ByArtistName : IRequest<ArtistEventSummaryHeader>
        {
            public string ArtistName { get; set; }
        }

        public class Handler : IAsyncRequestHandler<ByArtistName, ArtistEventSummaryHeader>
        {
            private readonly IArtistRepository _artistRepository;
            private readonly IEventRepository _eventRepository;

            public Handler(IArtistRepository artistRepository, IEventRepository eventRepository)
            {
                _artistRepository = artistRepository;
                _eventRepository = eventRepository;
            }

            public async Task<ArtistEventSummaryHeader> Handle(ByArtistName message)
            {
                var artist = await _artistRepository.Get(message.ArtistName);
                var summary = await _eventRepository.GetArtistEventSummary(artist.Id);

                return new ArtistEventSummaryHeader
                {
                    ArtistName = artist.Name,
                    Summary = summary
                };
            }
        }
    }

   
}
