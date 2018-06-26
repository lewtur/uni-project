using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models;
using FYP.Models.DataSourceRecords;
using MediatR;

namespace FYP.API.RequestHandlers
{
    public class ArtistStats
    {
        public class ByArtistId : IRequest<FullArtistStats>
        {
            public string ArtistName { get; set; }
        }

        public class Handler : IAsyncRequestHandler<ByArtistId, FullArtistStats>
        {
            private readonly ISpotifyDataSourceRepository _spotifyRepository;
            private readonly ITwitterDataSourceRepository _twitterRepository;
            private readonly IArtistRepository _artistRepository;
            private readonly IEventRepository _eventRepository;
            private readonly IGenreRepository _genreRepository;

            public Handler(ISpotifyDataSourceRepository spotifyRepository, IArtistRepository artistRepository, IEventRepository eventRepository, ITwitterDataSourceRepository twitterRepository, IGenreRepository genreRepository)
            {
                _spotifyRepository = spotifyRepository;
                _artistRepository = artistRepository;
                _eventRepository = eventRepository;
                _twitterRepository = twitterRepository;
                _genreRepository = genreRepository;
            }

            public async Task<FullArtistStats> Handle(ByArtistId message)
            {                
                var artist = await _artistRepository.Get(message.ArtistName);
                var spotifyHeaderId = await
                    _spotifyRepository.GetArtistHeaderIdByArtistId(artist.Id);
                          
                var fullStats = new FullArtistStats
                {
                    ArtistId = artist.Id,
                    ArtistName = artist.Name,
                    Albums = new List<SlimAlbumHeader>(),
                };

                var minDate = await AssignArtistStats(fullStats, spotifyHeaderId);
                await AssignAlbums(fullStats, spotifyHeaderId, minDate);
                await AssignArtistGigs(fullStats, artist.Id);
                await AssignArtistTweets(fullStats, artist.Id);
                await AssignGenres(fullStats, artist.Id);

                return fullStats;
            }

            private async Task<DateTime> AssignArtistStats(FullArtistStats stats, int spotifyHeaderId)
            {
                var artistStats = await _spotifyRepository.GetAllStatsForArtist(spotifyHeaderId);
                var max = artistStats.Max(x => x.Popularity);
                var factor = 100.0 / max;

                foreach (var stat in artistStats)
                {
                    stat.Popularity = (int) (stat.Popularity * factor);
                }

                stats.SpotifyArtistStats = artistStats;
                return artistStats.Min(x => x.DatePosted);
            }

            private async Task AssignAlbums(FullArtistStats stats, int spotifyHeaderId, DateTime minDate)
            {
                var albumStats = new List<SlimAlbumHeader>();
                var artistAlbums = (await _spotifyRepository.GetAllAlbumHeaders(spotifyHeaderId))
                    .GroupBy(x => x.Name)
                    .Select(albumGroup => albumGroup
                        .Take(1)
                        .FirstOrDefault())
                    .ToList();

                foreach (var album in artistAlbums)
                {
                    if (!DateTime.TryParseExact(album.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var releaseDate)) continue;

                    if (releaseDate > minDate)
                    {
                        albumStats.Add(new SlimAlbumHeader
                        {
                            Name = album.Name,
                            ReleaseDate = releaseDate
                        });
                    }
                }
                stats.Albums = albumStats;
            }

            private async Task AssignArtistGigs(FullArtistStats stats, int artistId)
            {
                var events = await _eventRepository.GetArtistEventSummary(artistId);
                stats.ArtistGigs = events;
            }

            private async Task AssignArtistTweets(FullArtistStats stats, int artistId)
            {
                var tweets = await _twitterRepository.GetTweetSummaryForArtist(artistId);
                stats.TweetSummary = tweets;
            }

            private async Task AssignGenres(FullArtistStats stats, int artistId)
            {
                var genres = await _genreRepository.GetGenreForArtist(artistId);
                stats.Genre = genres;
            }
        }
    }
}
