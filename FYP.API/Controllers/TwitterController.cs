using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.API.RequestHandlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FYP.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class TwitterController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public TwitterController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TweetsForArtist.FullArtistTweets), 200)]
        public async Task<IActionResult> Get([FromQuery] TweetsForArtist.ByArtistAndDate query)
        {
            var cacheKey = $"FullArtistTweets-{query.ArtistName}-{query.Date}";

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
    }
}
