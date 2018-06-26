using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FYP.Models.Abstractions;
using FYP.Models.JsonModels;

namespace FYP.External
{
    public interface ISpotifyCredentials
    {
        string GetAccessToken();
    }

    public class SpotifyCredentials : ISpotifyCredentials
    {
        private readonly ICache _cache;
        private readonly IHttpRequester _httpRequester;

        private const string CacheKey = "SpotifyAuthToken";
        private const string ClientId = "***REMOVED***";
        private const string ClientSecret = "***REMOVED***";

        public SpotifyCredentials(ICache cache, IHttpRequester httpRequester)
        {
            _cache = cache;
            _httpRequester = httpRequester;
        }

        public string GetAccessToken()
        {            
            var token = _cache.Get(CacheKey);
            if (token != null) return token;

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}"));

            var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("grant_type", "client_credentials") });

            SpotifyAuthentication authentication;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");
                authentication = _httpRequester.Post<SpotifyAuthentication>(client, "https://accounts.spotify.com/api/token", formContent);
            }

            token = authentication.access_token;
            _cache.Add(token, CacheKey, DateTime.Now.AddSeconds(authentication.expires_in - 60));
            return authentication.access_token;            
        }
    }
}
