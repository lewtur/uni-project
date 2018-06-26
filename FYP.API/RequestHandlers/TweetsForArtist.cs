using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using FYP.Models.Abstractions;
using MediatR;
using Microsoft.CodeAnalysis.CSharp;

namespace FYP.API.RequestHandlers
{
    public class TweetsForArtist
    {
        public class FullArtistTweets
        {
            public IEnumerable<TextAndLink> Tweets { get; set; }
            public IEnumerable<WordPairing> WordPairings { get; set; }
        }

        public class TextAndLink
        {
            public string Text { get; set; }
            public string Link { get; set; }
        }

        public class WordPairing
        {
            public string Text { get; set; }
            public int Size { get; set; }
        }

        public class ByArtistAndDate : IRequest<FullArtistTweets>
        {
            public string ArtistName { get; set; }
            public DateTime Date { get; set; }
        }

        public class Handler : IAsyncRequestHandler<ByArtistAndDate, FullArtistTweets>
        {
            private readonly ITwitterDataSourceRepository _twitterRepository;
            private readonly IArtistRepository _artistRepository;

            public Handler(ITwitterDataSourceRepository twitterRepository, IArtistRepository artistRepository)
            {
                _twitterRepository = twitterRepository;
                _artistRepository = artistRepository;
            }

            public async Task<FullArtistTweets> Handle(ByArtistAndDate message)
            {
                var dict = new Dictionary<string, int>();
                var artist = await _artistRepository.Get(message.ArtistName);
                var allTweets = await _twitterRepository.GetTweetTextOnDayForArtist(artist.Id, message.Date);

                var allWords = allTweets
                    .SelectMany(x => x.Split(' ', '"', '\'', ','))
                    .Select(y => y.ToLower())
                    .Where(z => !z.StartsWith("https://t.co"));

                foreach (var word in allWords)
                {
                    if (dict.TryGetValue(word, out var count))
                    {
                        dict[word] = count + 1;
                    }
                    else
                    {
                        dict.Add(word, 1);
                    }
                }

                var pairings = dict
                    .Select(x => new WordPairing {Text = x.Key, Size = x.Value})
                    .Where(x => PairingIsValid(x, artist))
                    .OrderByDescending(x => x.Size)
                    .ToList();

                var tweets = GetLinksFromTweets(allTweets);

                return new FullArtistTweets
                {
                    Tweets = tweets,
                    WordPairings = pairings
                };
            }

            private static bool PairingIsValid(WordPairing pairing, INamedEntity artist)
            {
                return pairing.Size > 1
                    && !string.IsNullOrEmpty(pairing.Text)
                    && !artist.Name.ToLower().Contains(pairing.Text)
                    && !(pairing.Text.Equals("&amp;") || pairing.Text.Equals("&"));
            }

            private static IEnumerable<TextAndLink> GetLinksFromTweets(IEnumerable<string> tweets)
            {
                var toReturn = new List<TextAndLink>();
                foreach (var tweet in tweets)
                {
                    if (tweet.Contains("https://t.co"))
                    {
                        var linkIndex = tweet.LastIndexOf("https://t.co", StringComparison.Ordinal);
                        if (linkIndex <= 0) break;

                        toReturn.Add(new TextAndLink
                        {
                            Text = tweet.Substring(0, linkIndex - 1),
                            Link = tweet.Substring(linkIndex, 23)
                        });
                    }
                    else
                    {
                        toReturn.Add(new TextAndLink { Text = tweet, Link = null });
                    }
                }

                return toReturn;
            }
        }

    }
}
