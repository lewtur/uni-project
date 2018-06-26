using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.API.RequestHandlers;
using FYP.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FYP.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class AlbumController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public AlbumController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpGet("ByReleaseDate")]
        [ProducesResponseType(typeof(IEnumerable<Album>), 200)]
        public async Task<IActionResult> Get([FromQuery] AlbumSearch.ByReleaseDate query)
        {
            var cacheKey = $"AlbumsByDate-{query.Date}";

            // ReSharper disable once InvertIf
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Album> result))
            {
                result = await _mediator.Send(query);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, result, cacheEntryOptions);
            }
            
            return Ok(result);
        }

        //[HttpGet("ByArtistName")]
        //[ProducesResponseType(typeof(ArtistEventSummaryHeader), 200)]
        //public async Task<IActionResult> Get([FromQuery] EventSummary.ByArtistName query)
        //{
        //    var result = await _mediator.Send(query);
        //    return Ok(result);
        //}
    }
}
