using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class CitySummary
    {
        public class AllCities : IRequest<IEnumerable<string>>
        {
        }

        public class Handler : IAsyncRequestHandler<AllCities, IEnumerable<string>>
        {
            public Task<IEnumerable<string>> Handle(AllCities message)
            {
                return Task.FromResult(new List<string>
                {
                    "Manchester",
                    "Birmingham",
                    "Leeds",
                    "Bristol",
                    "Sheffield",
                    "Liverpool",
                    "London",
                    "Glasgow",
                    "Cardiff"
                }.Select(x => x));
            }
        }
    }
}
