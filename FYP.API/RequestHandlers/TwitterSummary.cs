using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class TwitterSummary
    {
        public class TwitterSummaryHeader
        {
            public int AverageTweetsPerDay { get; set; }
            public int FollowersOfMostPopularTweet { get; set; }
            public IEnumerable<TwitterPeak> Peaks { get; set; }
        }

        public class TwitterPeak
        {
            public DateTime Date { get; set; }
            public int NumberOfTweets { get; set; }
        }

        public class ByArtistName : IRequest<TwitterSummaryHeader>
        {
            public string ArtistName { get; set; }
        }

        public class Handler : IAsyncRequestHandler<ByArtistName, TwitterSummaryHeader>
        {
            private readonly ITwitterDataSourceRepository _twitterRepository;
            private readonly IArtistRepository _artistRepository;

            public Handler(ITwitterDataSourceRepository twitterRepository, IArtistRepository artistRepository)
            {
                _twitterRepository = twitterRepository;
                _artistRepository = artistRepository;
            }

            public async Task<TwitterSummaryHeader> Handle(ByArtistName message)
            {
                var artist = await _artistRepository.Get(message.ArtistName);

                var tweets = await _twitterRepository.GetTweetsForArtist(artist.Id);

                //var dateSubstring = twitterDate.Substring(0, DateFormat.Length);

                //var dateOfTweet = DateTime.ParseExact(inputSubstring, DateFormat, CultureInfo.InvariantCulture);

                //var tweetsGroupedByDays = tweets.GroupBy(x => new { a = x.DateCreated.Substring(0, 3) }).Select(x => x);
                var tweetsGroupedByDay = tweets.GroupBy(x => new
                {
                    a = x.DateCreated.Substring(0, 3),
                    b = x.DateCreated.Substring(4, 3),
                    c = x.DateCreated.Substring(8, 2)
                }).Select(x => x);

                var averagePerDay = tweets.Count() / tweetsGroupedByDay.Count();

                var maxFollowers = 0;
                foreach (var tweet in tweets)
                {
                    var timestamp = await _twitterRepository.GetTwitterUserTimeStamp(tweet.TwitterUserId);
                    maxFollowers = timestamp.FollowersCount > maxFollowers ? timestamp.FollowersCount : maxFollowers;
                }

                tweetsGroupedByDay = tweetsGroupedByDay.OrderByDescending(x => x.Count()).Take(4);
                var many = tweetsGroupedByDay.Select(
                    x => new TwitterPeak {Date = x.First().DateSavedInDb, NumberOfTweets = x.Count()});

                return new TwitterSummaryHeader
                {
                    AverageTweetsPerDay = averagePerDay,
                    FollowersOfMostPopularTweet = maxFollowers,
                    Peaks = many
                };
            }
        }
    }
}
