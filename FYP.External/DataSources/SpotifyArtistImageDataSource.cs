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

namespace FYP.External.DataSources
{
    public class SpotifyArtistImageDataSource : IDataSource
    {
        private readonly IHttpRequester _httpRequester;
        private readonly ISpotifyDataSourceRepository _spotifyRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly ISpotifyCredentials _spotifyCredentials;
        private static readonly HttpClient Client = new HttpClient();

        private IEnumerable<SpotifyArtistHeader> _allHeaders;

        public SpotifyArtistImageDataSource(IHttpRequester httpRequester, ISpotifyDataSourceRepository spotifyRepository, IArtistRepository artistRepository, ISpotifyCredentials spotifyCredentials)
        {
            _httpRequester = httpRequester;
            _spotifyRepository = spotifyRepository;
            _artistRepository = artistRepository;
            _spotifyCredentials = spotifyCredentials;
            Client.BaseAddress = new Uri("https://api.spotify.com");
        }

        public string GetName()
        {
            return "Artist Image";
        }

        public void Update(INamedEntity source)
        {
            var artist = source as Artist;

            if (artist == null || artist.Id == 0) return;
            if (!string.IsNullOrEmpty(artist.ImageUrl)) return;

            var token = _spotifyCredentials.GetAccessToken();

            _httpRequester.ClearClient(Client);
            _httpRequester.AddHeader(Client, "Authorization", $"Bearer {token}");

            if (_allHeaders == null) _allHeaders = _spotifyRepository.GetAllArtistHeaders();

            var header = _allHeaders.FirstOrDefault(x => x.ArtistId == artist.Id);
            if (header == default(SpotifyArtistHeader)) return;

            var spotifyArtist = _httpRequester
                .Get<SpotifyArtist>(Client, $"/v1/artists/{header.SpotifyRecordId}");

            if (spotifyArtist?.images == null || !spotifyArtist.images.Any()) return;

            var imageUrl = spotifyArtist.images[0].url;

            _artistRepository.UpdateImageUrl(imageUrl, artist.Id);
        }
    }
}
