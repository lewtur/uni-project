using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.External;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;
using FYP.Models.JsonModels;

namespace FYP.Main
{
    internal class UnusedHelperClasses
    {
        internal class AlbumArtworkHelper : ConnectionBase
        {
            private readonly ICache _cache = MemoryCache.Instance;
            private static readonly HttpClient Client = new HttpClient();
            private const string CacheKey = "SpotifyAuthToken";
            private const string ClientId = "***REMOVED***";
            private const string ClientSecret = "***REMOVED***";
            private readonly IHttpRequester _httpRequester = new HttpRequester();

            public void UpdateAlbums()
            {
                var token = GetAccessToken();
                var spotifyRepo = new SpotifyDataSourceRepository();
                var albums = spotifyRepo.GetAllAlbumHeaders().Result;

                _httpRequester.ClearClient(Client);
                _httpRequester.AddHeader(Client, "Authorization", $"Bearer {token}");
                Client.BaseAddress = new Uri("https://api.spotify.com");
                var i = 0;
                var errors = "";

                foreach (var album in albums.Where(x => string.IsNullOrEmpty(x.AlbumArtworkUrl)))
                {
                    Console.WriteLine($"starting {i++}");
                    try
                    {
                        var spotifyAlbum = _httpRequester
                            .Get<SpotifyAlbumDetail>(Client, $"/v1/albums/?ids={album.SpotifyRecordId}")
                            .albums?.First();

                        if (spotifyAlbum?.images == null || !spotifyAlbum.images.Any()) continue;

                        using (var conn = GetConnection())
                        {
                            conn.Query(
                                $"UPDATE SpotifyAlbumHeader SET AlbumArtworkUrl = '{spotifyAlbum.images[1].url}' WHERE Id = {album.Id}");
                        }
                    }
                    catch (Exception e)
                    {
                        errors += $"{album.Name}\n";
                    }
                }

                Console.WriteLine(errors);

            }

            private string GetAccessToken()
            {
                var token = _cache.Get(CacheKey);
                if (token != null) return token;

                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}"));

                var formContent = new FormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    });

                SpotifyAuthentication authentication;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");
                    authentication =
                        _httpRequester.Post<SpotifyAuthentication>(client, "https://accounts.spotify.com/api/token",
                            formContent);
                }

                token = authentication.access_token;
                _cache.Add(token, CacheKey, DateTime.Now.AddSeconds(authentication.expires_in - 60));
                return authentication.access_token;
            }
        }

        internal class ArtistInfoHelper : ConnectionBase
        {
            public void SetDescriptions()
            {
                var artistRepo = new ArtistRepository();
                var infoRetriever = new ArtistInfoRetriever(new HttpRequester());
                var artists = artistRepo.GetAll().Result;

                var i = 0;
                foreach (var artist in artists.Where(x => string.IsNullOrEmpty(x.Description)))
                {

                    var bio = infoRetriever.GetArtistDescription(artist.Name);
                    if (string.IsNullOrEmpty(bio)) continue;

                    using (var conn = GetConnection())
                    {
                        var escaped = System.Security.SecurityElement.Escape(bio);
                        conn.Query($"UPDATE Artist SET Description = '{escaped}' WHERE Id = {artist.Id}");
                    }
                    Console.WriteLine($"Done {++i}");
                }
            }
        }

        internal class SpotifyImageHelper : ConnectionBase
        {
            private readonly ICache _cache = MemoryCache.Instance;
            private static readonly HttpClient Client = new HttpClient();
            private const string CacheKey = "SpotifyAuthToken";
            private const string ClientId = "32a91ca9eb674c03838bb11ce5413225";
            private const string ClientSecret = "aa00500c8ec4406eb0f97fb199b0dee5";
            private readonly IHttpRequester _httpRequester = new HttpRequester();

            public void UpdatePics()
            {
                var token = GetAccessToken();
                var spotifyRepo = new SpotifyDataSourceRepository();
                var artists = spotifyRepo.GetAllArtistHeaders();

                _httpRequester.ClearClient(Client);
                _httpRequester.AddHeader(Client, "Authorization", $"Bearer {token}");
                Client.BaseAddress = new Uri("https://api.spotify.com");
                var i = 0;
                var errors = "";

                foreach (var artist in artists)
                {
                    Console.WriteLine($"starting {i++}");
                    try
                    {
                        var spotifyArtist = _httpRequester
                            .Get<SpotifyArtist>(Client, $"/v1/artists/{artist.SpotifyRecordId}");

                        if (spotifyArtist?.images == null || !spotifyArtist.images.Any()) continue;

                        using (var conn = GetConnection())
                        {
                            conn.Query(
                                $"UPDATE Artist SET ImageUrl = '{spotifyArtist.images[0].url}' WHERE Id = {artist.ArtistId}");
                        }
                    }
                    catch (Exception e)
                    {
                        errors += $"{artist.ArtistId}\n";
                    }
                }

                Console.WriteLine(errors);

            }

            private string GetAccessToken()
            {
                var token = _cache.Get(CacheKey);
                if (token != null) return token;

                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}"));

                var formContent = new FormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    });

                SpotifyAuthentication authentication;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");
                    authentication =
                        _httpRequester.Post<SpotifyAuthentication>(client, "https://accounts.spotify.com/api/token",
                            formContent);
                }

                token = authentication.access_token;
                _cache.Add(token, CacheKey, DateTime.Now.AddSeconds(authentication.expires_in - 60));
                return authentication.access_token;
            }
        }

        internal class TweetChomper : ConnectionBase
        {
            // before cull: 6722694           

            public async void Chomp()
            {
                IEnumerable<Tweet> tweets;

                using (var conn = GetConnection())
                {
                    tweets = conn.Query<Tweet>("SELECT * FROM RefinedTweet", commandTimeout: 0);
                }

                Console.WriteLine($"{tweets.Count()} to get through. This may be a while...");
                var increment = tweets.Count() / 100;
                var i = 0;
                var percentage = 0;
                var countLock = new object();

                var options = new ParallelOptions {MaxDegreeOfParallelism = 4};      
                

                foreach (var tweet in tweets)
                {
                    if (++i % increment == 0) Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {percentage++}% done.");

                    var nameSql = tweet.ArtistId.HasValue
                        ? $"SELECT Name FROM Artist WHERE Id = {tweet.ArtistId}"
                        : $"SELECT Name FROM Venue WHERE Id = {tweet.VenueId}";

                    using (var conn = GetConnection())
                    {
                        try
                        {
                            var entityName = await conn.QueryFirstOrDefaultAsync<string>(nameSql);

                            if (string.IsNullOrEmpty(entityName)) continue;
                            if (tweet.Text.ToLower().Contains(entityName.ToLower())) continue;

                            await conn.ExecuteAsync($"DELETE FROM RefinedTweet WHERE Id = {tweet.Id}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }                                                                                       
                }                
            }

            public void DeleteRecord(string sql, int attemptNumber)
            {
                if (attemptNumber >= 3) return;

                try
                {
                    using (var conn = GetConnection())
                    {
                        conn.Query(sql);
                    }
                }
                catch (TimeoutException)
                {
                    DeleteRecord(sql, attemptNumber + 1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }   
}
