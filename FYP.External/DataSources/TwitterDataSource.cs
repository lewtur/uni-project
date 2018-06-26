using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;
using FYP.Models.JsonModels;
using Newtonsoft.Json;

namespace FYP.External.DataSources
{    
    public abstract class TwitterDataSource : IDataSource
    {
        public abstract bool IsArtist { get; }        

        private readonly ITwitterDataSourceRepository _repository;
        private readonly IHttpRequester _httpRequester;
        private readonly ICache _cache;
        private readonly ITwitterDataSourceConfig _config;
        private readonly ITwitterLimitRepository _limitRepository;

        private DateTime _requestEndTime;
        private DateTime _minDate;
        private DateTime _maxDate;

        private static readonly HttpClient Client = new HttpClient();

        protected TwitterDataSource(
            ITwitterDataSourceRepository repository,
            IHttpRequester httpRequester,
            ICache cache,
            ITwitterDataSourceConfig config,
            ITwitterLimitRepository limitRepository)
        {
            _repository = repository;
            _httpRequester = httpRequester;
            _cache = cache;
            _config = config;
            _limitRepository = limitRepository;
            Client.BaseAddress = new Uri(_config.BaseApiUrl);
        }

        public string GetName()
        {
            return "Twitter";
        }

        public void Update(INamedEntity source)
        {
            if (_minDate == default(DateTime) || _maxDate == default(DateTime))
            {
                SetMinAndMaxDates();
            }

            var numberSent = 0;           
            string url = null;

            while (true)
            {                
                if (numberSent >= _config.MaximumNumberStoredUnderOneEntity)
                {
                    AddToLimitRepository(source);
                    return;
                }

                var accessToken = GetAccessToken();
                var tweetResultSet = GetTweetResultSet(accessToken, url, source);

                if (tweetResultSet?.statuses == null) return;

                foreach (var tweetResult in tweetResultSet.statuses)
                {
                    if (tweetResult.text.StartsWith("RT @")) continue;
                    if (!tweetResult.text.ToLower().Contains(source.Name.ToLower())) continue;

                    if (!DateIsInRange(tweetResult.created_at)) return;
                    if (++numberSent >= _config.MaximumNumberStoredUnderOneEntity)
                    {
                        AddToLimitRepository(source);
                        return;
                    }

                    try
                    {
                        SaveTweetDetails(tweetResult, source);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error {e.Message} when saving data, skipping to next tweet. ");
                    }                    
                }

                var nextPage = tweetResultSet.search_metadata?.next_results;
                if (!string.IsNullOrEmpty(nextPage))
                {
                    url = nextPage;
                    continue;
                }
                break;
            }
        }

        public bool DateIsInRange(string twitterDate)
        {
            if (string.IsNullOrEmpty(twitterDate)) return false;

            var inputSubstring = twitterDate.Substring(0, _config.DateFormatInitial.Length);
            inputSubstring += $" {twitterDate.Substring(twitterDate.Length - 4)}";

            try
            {
                var dateOfTweet = DateTime.ParseExact(inputSubstring, _config.DateFormatWithYear, CultureInfo.InvariantCulture);

                return dateOfTweet >= _minDate;
            }
            catch (Exception e)
            {
                return true;
            }           
        }

        public void SetMinAndMaxDates()
        {
            var updateStartedString = _cache.Get("DateUpdateStarted");

            if (!string.IsNullOrEmpty(updateStartedString))
            {
                var updateStarted = DateTime.ParseExact(updateStartedString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                _maxDate = updateStarted.AddDays(0);
                _minDate = updateStarted.AddDays(-1);
            }
            else
            {
                throw new Exception("Start date not set in the cache");
            }

        }

        private void SaveTweetDetails(Status tweetResult, INamedEntity source)
        {
            var userResult = tweetResult.user;
            var userId = 0;

            if (userResult != null)
            {
                userId = AddUserToRepository(userResult);

                AddUserTimestampToRepository(userId, userResult);
            }

            var tweet = CreateTweetObject(userId, tweetResult);

            if (IsArtist) tweet.ArtistId = source.Id;
            else tweet.VenueId = source.Id;

            _repository.AddTweet(tweet);
        }

        private void AddToLimitRepository(INamedEntity source)
        {
            if (IsArtist)
            {
                _limitRepository.AddUpdateArtist(source.Id, _config.MaximumNumberStoredUnderOneEntity);
            }
            else
            {
                _limitRepository.AddUpdateVenue(source.Id, _config.MaximumNumberStoredUnderOneEntity);
            }
        }

        private string GetAccessToken()
        {
            var token = _cache.Get(_config.CacheKey);
            if (token != null) return token;

            _httpRequester.ClearClient(Client);

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config.ClientId}:{_config.ClientSecret}"));

            _httpRequester.AddHeader(Client, "Authorization", $"Basic {authToken}");

            var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("grant_type", "client_credentials") });

            var authentication = _httpRequester
                .Post<TwitterAuthentication>(Client, "/oauth2/token", formContent);

            token = authentication.access_token;
            _requestEndTime = DateTime.Now.AddMinutes(_config.RateLimitWindowInMinutes);
            _cache.Add(token, _config.CacheKey, _requestEndTime);

            return authentication.access_token;
        }

        private TwiterSearchResults GetTweetResultSet(string accessToken, string url, INamedEntity source)
        {
            _httpRequester.ClearClient(Client);

            _httpRequester.AddHeader(Client, "Authorization", $"Bearer {accessToken}");

            var endpointUrl = string.IsNullOrEmpty(url)
                ? $"/1.1/search/tweets.json?&q={source.Name}&count=100&include_entities=0&result_type=recent&lang=en&until={_maxDate:yyyy-MM-dd}"
                : $"/1.1/search/tweets.json{url}";

            return _httpRequester.Get<TwiterSearchResults>(Client, endpointUrl);
        }        

        private int AddUserToRepository(User userResult)
        {
            return _repository.AddUpdateUser(new TwitterUser
            {
                Id = 0,
                DateSignedUp = userResult.created_at,
                Location = userResult.location,
                Name = userResult.name,
                ScreenName = userResult.screen_name,
                TwitterId = userResult.id_str,
                UserDescription = userResult.description
            }).Result;
        }

        private void AddUserTimestampToRepository(int userId, User userResult)
        {
            _repository.AddUserTimestamp(new TwitterUserTimestamp
            {
                Id = 0,
                DateCreated = DateTime.Now,
                FavouritesCount = userResult.favourites_count,
                FollowersCount = userResult.followers_count,
                FriendsCount = userResult.friends_count,
                ListedCount = userResult.listed_count,
                StatusesCount = userResult.statuses_count,
                TwitterUserId = userId,
                Verified = userResult.verified
            });
        }

        private static Tweet CreateTweetObject(int userId, Status tweetResult)
        {
            return new Tweet
            {
                ArtistId = null,
                VenueId = null,
                TwitterUserId = userId,
                Id = 0,
                DateCreated = tweetResult.created_at,
                DateSavedInDb = DateTime.Now,
                FavouriteCount = tweetResult.favorite_count,
                Language = tweetResult.lang,
                RetweetCount = tweetResult.retweet_count,
                Text = tweetResult.text,
                TwitterId = tweetResult.id_str
            };
        }
    }
}
