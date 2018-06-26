using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Models;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class TrendingArtist
    {
        public class ForUser : IRequest<IEnumerable<UserRecommendedArtist>>
        {
            public string Genres { get; set; }
        }

        public class Handler : IAsyncRequestHandler<ForUser, IEnumerable<UserRecommendedArtist>>
        {
            private readonly IPopularityRepository _popularityRepository;
            private readonly IGenreRepository _genreRepository;
            private readonly IArtistRepository _artistRepository;

            public Handler(IPopularityRepository popularityRepository, IGenreRepository genreRepository, IArtistRepository artistRepository)
            {
                _popularityRepository = popularityRepository;
                _genreRepository = genreRepository;
                _artistRepository = artistRepository;
            }

            public async Task<IEnumerable<UserRecommendedArtist>> Handle(ForUser message)
            {
                if (message.Genres == null)
                {
                    message.Genres = "";
                }

                var userGenres = message.Genres.Split(',').ToList();
                userGenres = userGenres.Where(x => !string.IsNullOrEmpty(x)).ToList();
                var trendingArtistGenres = new Dictionary<int, (IEnumerable<string> matchedGenres, Genre fullGenre)>();
                var trendingArtists = await _popularityRepository.GetAllRecentTrendingArtist();

                foreach (var artist in trendingArtists)
                {
                    var genreObject = await _genreRepository.GetGenreForArtist(artist.Artist.Id);
                    if (genreObject == null) continue;
                    
                    var genresToAdd = new List<string>();

                    if (!string.IsNullOrEmpty(genreObject.SpotifyGivenGenre))
                    {
                        genresToAdd.AddRange(genreObject.SpotifyGivenGenre.Split(',').Where(x => !string.IsNullOrEmpty(x)));
                    }

                    trendingArtistGenres.Add(artist.Artist.Id, (genresToAdd, genreObject));
                }

                var matchedSet = new List<UserRecommendedArtist>();
                foreach (var genre in userGenres)
                {
                    foreach (var artist in trendingArtistGenres)
                    {
                        if (!artist.Value.matchedGenres.Contains(genre)) continue;

                        var a = matchedSet.FirstOrDefault(x => x.Artist.Id == artist.Key);

                        if (a?.MatchedGenres != null)
                        {
                            a.MatchedGenres.Add(genre);
                        }
                        else
                        {
                            var match = trendingArtists.FirstOrDefault(x => x.Artist.Id == artist.Key);
                            if (match == null) continue;

                            matchedSet.Add(new UserRecommendedArtist
                            {
                                Artist = match.Artist,
                                Features = match.Features,
                                MatchedGenres = new List<string> { genre },
                                Genre = artist.Value.fullGenre
                            });
                        }
                    }
                }

                if (!matchedSet.Any())
                {
                    matchedSet = trendingArtists
                        .Take(5)
                        .Select(x => new UserRecommendedArtist
                        {
                            Artist = x.Artist,
                            Features = x.Features,
                            MatchedGenres = new List<string>(),
                            Genre = _genreRepository.GetGenreForArtist(x.Artist.Id).Result
                        })
                        .ToList();
                }

                foreach (var artist in matchedSet)
                {
                    artist.Artist = await _artistRepository.GetNameAndSpotifyRecordId(artist.Artist.Id);
                }

                matchedSet = matchedSet.OrderByDescending(x => x.MatchedGenres.Count).ToList();

                return matchedSet;
            }
        }
    }
}
