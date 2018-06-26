using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using MediatR;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace FYP.API.RequestHandlers
{
    public class AlbumSearch
    {
        public class ByReleaseDate : IRequest<IEnumerable<Album>>
        {
            public string Date { get; set; }
        }

        //public class AlbumByReleaseDateValidator : AbstractValidator<ByReleaseDate>
        //{
        //    public AlbumByReleaseDateValidator()
        //    {
        //        RuleFor(x => x.Date)
        //            .Must(y => DateTime.TryParseExact(y, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result));
        //    }
        //}

        public class Handler : IAsyncRequestHandler<ByReleaseDate, IEnumerable<Album>>
        {
            private readonly ISpotifyDataSourceRepository _spotifyRepository;

            public Handler(ISpotifyDataSourceRepository spotifyRepository)
            {
                _spotifyRepository = spotifyRepository;
            }

            public async Task<IEnumerable<Album>> Handle(ByReleaseDate message)
            {
                var allAlbums = await _spotifyRepository.GetAlbumsByDate(message.Date);

                return allAlbums
                    .Where(x =>
                        allAlbums.Count(y => y.AlbumName.Equals(x.AlbumName)) == 1 &&
                        allAlbums.Count(z => z.ArtistName.Equals(x.ArtistName)) == 1);
            }
        }
    }
}
