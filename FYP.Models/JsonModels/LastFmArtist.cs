using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace FYP.Models.JsonModels
{
    public class Stats
    {
        public string listeners { get; set; }
        public string playcount { get; set; }
    }
    
    public class Image2
    {
        public string text { get; set; }
        public string size { get; set; }
    }

    public class LastFmArtist2
    {
        public string name { get; set; }
        public string url { get; set; }
        public List<Image2> image { get; set; }
    }

    public class Similar
    {
        public List<Artist2> artist { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Tags
    {
        public List<Tag> tag { get; set; }
    }

    public class Link
    {
        public string text { get; set; }
        public string rel { get; set; }
        public string href { get; set; }
    }

    public class Links
    {
        public Link link { get; set; }
    }

    public class Bio
    {
        public Links links { get; set; }
        public string published { get; set; }
        public string summary { get; set; }
        public string content { get; set; }
    }

    public class LastFmArtistBase
    {
        public string name { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
        public List<Image2> image { get; set; }
        public string streamable { get; set; }
        public string ontour { get; set; }
        public Stats stats { get; set; }
        public Similar similar { get; set; }
        public Tags tags { get; set; }
        public Bio bio { get; set; }
    }

    public class LastFmArtist
    {
        public LastFmArtistBase artist { get; set; }
    }
}
