using System;
using System.Collections.Generic;
using System.Text;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;

namespace FYP.Models
{
    public class FullArtistStats
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public IEnumerable<ArtistEventSummary> ArtistGigs { get; set; }
        public IEnumerable<SpotifyArtistStats> SpotifyArtistStats { get; set; }
        public IEnumerable<SlimAlbumHeader> Albums { get; set; }
        public IEnumerable<TwitterDaySummary> TweetSummary { get; set; }
        public Genre Genre { get; set; }
    }

    public class TwitterDaySummary : ITrend
    {
        public DateTime Date { get; set; }
        public int TweetCount { get; set; }
        public int Percentage { get; set; }

        public DateTime GetDate()
        {
            return Date;
        }

        public int GetScore()
        {
            return Percentage;
        }
    }

    public class SlimAlbumHeader
    {
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class AlbumStats
    {
        public SpotifyAlbumHeader Header { get; set; }
        public IEnumerable<SpotifyAlbumStats> Stats { get; set; }
    }
}
