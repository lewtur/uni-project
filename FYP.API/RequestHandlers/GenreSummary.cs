using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class GenreSummary
    {
        public class AllGenres : IRequest<IEnumerable<string>>
        {
        }

        public class Handler : IAsyncRequestHandler<AllGenres, IEnumerable<string>>
        {
            public Task<IEnumerable<string>> Handle(AllGenres message)
            {
                return Task.FromResult(new List<string>
                {
                    "Rock",
                    "Indie",
                    "Dance",
                    "Jazz",
                    "Blues",
                    "Steely Dan",
                    "Classical",
                    "Funk"
                }.Select(x => x));
            }
        }
    }
}
