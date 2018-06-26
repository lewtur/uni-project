using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.API.RequestHandlers;
using FYP.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace FYP.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class EventController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public EventController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpGet("ByArtistName")]
        [ProducesResponseType(typeof(ArtistEventSummaryHeader), 200)]
        public async Task<IActionResult> Get([FromQuery] EventSummary.ByArtistName query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Cities")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> Get(CitySummary.AllCities cities)
        {
            var result = await _mediator.Send(cities);
            return Ok(result);
        }

        [HttpGet("Genres")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> Get(GenreSummary.AllGenres genres)
        {
            var result = await _mediator.Send(genres);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FullEvent>), 200)]
        public async Task<IActionResult> Get([FromQuery] EventByDate.FilterByDate dates)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var cacheKey = $"EventsByDate-{dates.StartDate}-{dates.EndDate}";

            // ReSharper disable once InvertIf
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<FullEvent> result))
            {
                result = await _mediator.Send(dates);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, result, cacheEntryOptions);
            }

            return Ok(result);
        }
    }
}
