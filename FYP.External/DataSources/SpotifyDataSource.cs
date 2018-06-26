using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;
using FYP.Models.JsonModels;
using Newtonsoft.Json;

namespace FYP.External.DataSources
{
    public class SpotifyDataSource : IDataSource
    {
        private const string CacheKey = "SpotifyAuthToken";
        private const string ClientId = "***REMOVED***";
        private const string ClientSecret = "***REMOVED***";
        private const string BaseApiUrl = "https://api.spotify.com";

        private readonly ISpotifyDataSourceRepository _repository;
        private readonly IHttpRequester _httpRequester;
        private readonly IInvalidSpotifyRepository _invalidSpotifyRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ISpotifyCredentials _spotifyCredentials;

        private static readonly HttpClient Client = new HttpClient();

        public SpotifyDataSource(ISpotifyDataSourceRepository repository, IHttpRequester httpRequester, IInvalidSpotifyRepository invalidSpotifyRepository, IGenreRepository genreRepository, ISpotifyCredentials spotifyCredentials)
        {
            _repository = repository;
            _httpRequester = httpRequester;
            _invalidSpotifyRepository = invalidSpotifyRepository;
            _genreRepository = genreRepository;
            _spotifyCredentials = spotifyCredentials;
            Client.BaseAddress = new Uri(BaseApiUrl);
                       
        }        

        public void Update(INamedEntity source)
        {
            var accessToken = _spotifyCredentials.GetAccessToken();
            var spotifyId = GetSpotifyArtistId(source.Name, accessToken);

            if (string.IsNullOrEmpty(spotifyId))
            {
                _invalidSpotifyRepository.AddArtistToInvalidList(source.Id);
                return;
            }

            _httpRequester.ClearClient(Client);
            _httpRequester.AddHeader(Client, "Authorization", $"Bearer {accessToken}");

            var spotifyArtist = _httpRequester.Get<SpotifyArtist>(Client, $"/v1/artists/{spotifyId}/");

            if (spotifyArtist == null) return;

            var artistHeaderId = _repository.GetOrCreateArtistHeaderId(new SpotifyArtistHeader
            {
                SpotifyRecordId = spotifyArtist.id,
                Type = spotifyArtist.type,
                Genres = spotifyArtist.genres?.Aggregate("", (c, n) => c + $"{n},"),
                ArtistId = source.Id
            }).Result;

            _repository.AddArtistStats(new SpotifyArtistStats
            {
                DatePosted = DateTime.UtcNow,
                Followers = spotifyArtist.followers.total,
                Popularity = spotifyArtist.popularity,
                SpotifyArtistHeaderId = artistHeaderId
            });

            UpdateGenresForArtist(spotifyArtist, source);

            var artistAlbumCalls = _httpRequester.Get<SpotifyArtistAlbums>(Client, $"/v1/artists/{spotifyId}/albums");

            if (artistAlbumCalls == null) return;

            var albumIds = artistAlbumCalls.items
                .Select(album => album.id)
                .ToList();

            if (albumIds.Count <= 0) return;

            var idParam = albumIds
                .Aggregate("", (current, next) => current + $"{next},");

            idParam = idParam.Remove(idParam.Length - 1);

            var albumDetail = _httpRequester.Get<SpotifyAlbumDetail>(Client, $"/v1/albums/?ids={idParam}");

            if (albumDetail == null) return;

            foreach (var album in albumDetail.albums)
            {
                var albumArtworkUrl = "";

                if (album?.images != null && album.images.Any())
                {
                    albumArtworkUrl = album.images?[1]?.url;
                }

                var albumHeaderId = _repository.GetOrCreateAlbumHeaderId(new SpotifyAlbumHeader
                {
                    AlbumType = album.type,
                    Label = album.label,
                    ReleaseDate = album.release_date,
                    SpotifyArtistHeaderId = artistHeaderId,
                    SpotifyRecordId = album.id,
                    Name = album.name,
                    AlbumArtworkUrl = albumArtworkUrl
                });

                _repository.AddAlbumStats(new SpotifyAlbumStats
                {
                    DatePosted = DateTime.UtcNow,
                    Popularity = album.popularity,
                    SpotifyAlbumHeaderId = albumHeaderId
                });
            }            
        }

        public string GetName()
        {
            return "Spotify";
        }

        private string GetSpotifyArtistId(string artistName, string accessToken)
        {
            _httpRequester.ClearClient(Client);
            _httpRequester.AddHeader(Client, "Authorization", $"Bearer {accessToken}");       

            var searchResults =
                _httpRequester.Get<SpotifyArtistSearch>(Client, $"v1/search?q={artistName}&type=artist");

            return searchResults?.artists?.items
                .Where(x => x.name.Equals(artistName, StringComparison.CurrentCultureIgnoreCase))
                .Select(y => y.id)
                .FirstOrDefault();            
        }

        private void UpdateGenresForArtist(SpotifyArtist spotifyArtist, INamedEntity source)
        {
            if (_genreRepository.GetGenreForArtist(source.Id) != null) return;

            var relatedArtists = _httpRequester.Get<RelatedArtists>(Client, $"/v1/artists/{spotifyArtist.id}/related-artists");

            if (relatedArtists == null) return;

            var dict = new Dictionary<string, int>();
            foreach (var relatedArtist in relatedArtists.artists)
            {
                if (relatedArtist.genres == null) continue;

                foreach (var relatedGenre in relatedArtist.genres)
                {
                    if (dict.ContainsKey(relatedGenre))
                    {
                        dict[relatedGenre] += 1;
                    }
                    else
                    {
                        dict.Add(relatedGenre, 1);
                    }
                }
            }
            if (!dict.Any()) return;

            var orderedGenres = dict.OrderByDescending(x => x.Value);
            var otherGenres = orderedGenres
                .Skip(1)
                .Take(orderedGenres.Count() / 2)
                .Select(x => x.Key)
                .Aggregate("", (c, n) => c + $"{n},");

            var genre = new Genre
            {
                ArtistId = source.Id,
                SpotifyGivenGenre = spotifyArtist.genres?.Aggregate("", (c, n) => c + $"{n},"),
                MostPopularGenreOfRelatedArtists = orderedGenres.First().Key,
                OtherGenresGivenInRelatedArtists = otherGenres
            };

            _genreRepository.AddGenreForArtist(genre);
        }

        internal class RelatedArtists
        {
            public IEnumerable<SpotifyArtist> artists { get; set; }
        }
    }
}