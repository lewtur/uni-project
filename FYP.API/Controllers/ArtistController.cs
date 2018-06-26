using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FYP.API.RequestHandlers;
using FYP.Models;
using FYP.Models.DataSourceRecords;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYP.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class ArtistController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public ArtistController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ArtistSearch.Result), 200)]
        public async Task<IActionResult> Get([FromQuery] ArtistSearch.ArtistSearchQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("MostPopular")]
        [ProducesResponseType(typeof(IEnumerable<DetailedArtist>), 200)]
        public async Task<IActionResult> Get([FromQuery] PopularArtists.ByDateAndLimit query)
        {
            var cacheKey = $"PopularArtists-{query.DaysToLookBack}-{query.Limit}";

            // ReSharper disable once InvertIf
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<DetailedArtist> result))
            {
                result = await _mediator.Send(query);

                var cacheEntryOptions = new MemoryCacheEntryOptions()                    
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, result, cacheEntryOptions);
            }

            return Ok(result);
        }

        [HttpGet("Stats")]
        [ProducesResponseType(typeof(FullArtistStats), 200)]
        public async Task<IActionResult> Get([FromQuery] ArtistStats.ByArtistId query)
        {
            var cacheKey = $"ArtistStats-{query.ArtistName}";

            // ReSharper disable once InvertIf
            if (!_cache.TryGetValue(cacheKey, out var result))
            {
                result = await _mediator.Send(query);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60));

                _cache.Set(cacheKey, result, cacheEntryOptions);
            }

            return Ok(result);
        }

        [HttpGet("GetUsersTrendingArtist")]
        [ProducesResponseType(typeof(IEnumerable<UserRecommendedArtist>), 200)]
        public async Task<IActionResult> Get([FromQuery] TrendingArtist.ForUser query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
