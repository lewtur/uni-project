using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FYP.Models.JsonModels;

namespace FYP.External
{
    public interface IArtistInfoRetriever
    {
        string GetArtistDescription(string artistName);
    }

    public class ArtistInfoRetriever : IArtistInfoRetriever
    {
        private readonly IHttpRequester _httpRequester;
        private const string ApiKey = "2acdf55e3153556159ec5377676e3c53";
        private static readonly HttpClient Client = new HttpClient();

        public ArtistInfoRetriever(IHttpRequester httpRequester)
        {
            _httpRequester = httpRequester;
            Client.BaseAddress = new Uri("http://ws.audioscrobbler.com");
        }


        public string GetArtistDescription(string artistName)
        {
            _httpRequester.ClearClient(Client);
            var artist = _httpRequester.Get<LastFmArtist>(Client, $"/2.0/?method=artist.getinfo&artist={artistName}&api_key={ApiKey}&format=json");

            return artist.artist?.bio?.content;
        }
    }   
}
