using System.Collections.Generic;
using System.Linq;
using Autofac;
using FYP.Data;
using FYP.Models;

namespace FYP.Main
{
    public interface IArtistFilterer
    {
        IEnumerable<Artist> GetArtistsToUpdate();
    }

    public class ArtistFilterer : IArtistFilterer
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ITwitterLimitRepository _twitterLimitRepository;
        private readonly IInvalidSpotifyRepository _invalidSpotifyRepository;

        public ArtistFilterer(IArtistRepository artistRepository, ITwitterLimitRepository twitterLimitRepository, IInvalidSpotifyRepository invalidSpotifyRepository)
        {
            _artistRepository = artistRepository;
            _twitterLimitRepository = twitterLimitRepository;
            _invalidSpotifyRepository = invalidSpotifyRepository;
        }

        public IEnumerable<Artist> GetArtistsToUpdate()
        {
            var allArtists = _artistRepository.GetAll().Result;
            var validTwitterArtists = allArtists
                .Where(x => !_twitterLimitRepository.ArtistHasExceededLimit(x.Id).Result).ToList();

            return validTwitterArtists
                .Where(x => !_invalidSpotifyRepository.ArtistIsInInvalidList(x.Id).Result).ToList();
        }
    }
}