using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class PopularArtists
    {
        private readonly IMediator _mediator;

        public PopularArtists(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class ByDateAndLimit : IRequest<IEnumerable<DetailedArtist>>
        {
            public int DaysToLookBack { get; set; }
            public int Limit { get; set; }
        }

        public class Handler : IAsyncRequestHandler<ByDateAndLimit, IEnumerable<DetailedArtist>>
        {
            private readonly ISpotifyDataSourceRepository _spotifyRepository;

            public Handler(ISpotifyDataSourceRepository spotifyRepository)
            {
                _spotifyRepository = spotifyRepository;
            }

            public Task<IEnumerable<DetailedArtist>> Handle(ByDateAndLimit message)
            {
                if (message.DaysToLookBack > 0)
                {
                    message.DaysToLookBack *= -1;
                }

                return _spotifyRepository.GetMostPopularArtist(message.DaysToLookBack, message.Limit);
            }
        }
    }
}
