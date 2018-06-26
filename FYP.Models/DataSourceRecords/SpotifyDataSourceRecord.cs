using System;
using System.Collections.Generic;
using FYP.Models.Abstractions;

namespace FYP.Models.DataSourceRecords
{
    public class SpotifyArtistHeader
    {
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public string SpotifyRecordId { get; set; }
        public string Type { get; set; }
        public string Genres { get; set; }
    }

    public class SpotifyArtistStats : ITrend
    {
        public int Id { get; set; }
        public int SpotifyArtistHeaderId { get; set; }
        public int Followers { get; set; }
        public int Popularity { get; set; }
        public DateTime DatePosted { get; set; }

        public DateTime GetDate()
        {
            return DatePosted;
        }

        public int GetScore()
        {
            return Popularity;
        }
    }

    public class SpotifyAlbumHeader
    {
        public int Id { get; set; }
        public int SpotifyArtistHeaderId { get; set; }
        public string SpotifyRecordId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string AlbumType { get; set; }
        public string ReleaseDate { get; set; }
        public string AlbumArtworkUrl { get; set; }
    }

    public class SpotifyAlbumStats : ITrend
    {
        public int Id { get; set; }
        public int SpotifyAlbumHeaderId { get; set; }
        public int Popularity { get; set; }
        public DateTime DatePosted { get; set; }

        public DateTime GetDate()
        {
            return DatePosted;
        }

        public int GetScore()
        {
            return Popularity;
        }
    }

    public class SpotifyArtistStatsSummary
    {
        public string ArtistName { get; set; }
        public IEnumerable<SpotifyArtistStats> Stats { get; set; }
    }
}