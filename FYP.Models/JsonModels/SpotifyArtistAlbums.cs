using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models.JsonModels
{
    public class AlbumExternalUrls
    {
        public string spotify { get; set; }
    }

    public class AlbumArtist
    {
        public AlbumExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ExternalUrls2
    {
        public string spotify { get; set; }
    }

    public class AlbumImage
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class AlbumItem
    {
        public string album_type { get; set; }
        public List<AlbumArtist> artists { get; set; }
        public List<string> available_markets { get; set; }
        public ExternalUrls2 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<AlbumImage> images { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class SpotifyArtistAlbums
    {
        public string href { get; set; }
        public List<AlbumItem> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }
}
