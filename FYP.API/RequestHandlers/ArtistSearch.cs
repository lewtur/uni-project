using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore.Update;

namespace FYP.API.RequestHandlers
{
    public class ArtistSearch
    {
        public class ArtistSearchQuery : IRequest<Result>
        {
            public string Term { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

        public class Result
        {
            public string Term { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public IEnumerable<Artist> Artists { get; set; }
        }

        public class Handler : IAsyncRequestHandler<ArtistSearchQuery, Result>
        {
            private readonly IArtistRepository _artistRepository;

            public Handler(IArtistRepository artistRepository)
            {
                _artistRepository = artistRepository;
            }

            public async Task<Result> Handle(ArtistSearchQuery message)
            {
                var result = new Result
                {
                    Page = message.Page,
                    Term = message.Term,
                    PageSize = message.PageSize
                };

                //var mockArtists = new List<Artist> {new Artist {Id = 1, Name = "Blossoms"}, new Artist { Id = 2, Name = "Egg"}};

                var artists = await _artistRepository.GetAll(message.Page, message.PageSize, message.Term);

                result.Artists = artists;

                return result;
            }
        }
    }
}
