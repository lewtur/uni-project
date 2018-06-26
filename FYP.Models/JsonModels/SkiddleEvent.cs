using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models.JsonModels
{
    public class SkiddleVenue
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string town { get; set; }
        public string postcode_lookup { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string type { get; set; }
        public int rating { get; set; }
    }

    public class Openingtimes
    {
        public string doorsopen { get; set; }
        public string doorsclose { get; set; }
        public string lastentry { get; set; }
    }

    public class Rep
    {
        public object enabled { get; set; }
    }

    public class EventResult
    {
        public string id { get; set; }
        public string EventCode { get; set; }
        public string eventname { get; set; }
        public string cancelled { get; set; }
        public SkiddleVenue venue { get; set; }
        public string imageurl { get; set; }
        public string largeimageurl { get; set; }
        public string link { get; set; }
        public string date { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string description { get; set; }
        public Openingtimes openingtimes { get; set; }
        public string minage { get; set; }
        public object imgoing { get; set; }
        public string goingtocount { get; set; }
        public bool tickets { get; set; }
        public string entryprice { get; set; }
        public Rep rep { get; set; }
        public List<SkiddleArtist> artists { get; set; }
        public List<SkiddleArtistGenre> genres { get; set; }
    }

    public class SkiddleArtist
    {
        public string artistid { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public object spotifymp3url { get; set; }
        public object spotifyartisturl { get; set; }
    }

    public class SkiddleArtistGenre
    {
        public string genreid { get; set; }
        public string name { get; set; }
    }

    public class SkiddleEvent
    {
        public int error { get; set; }
        public string totalcount { get; set; }
        public int pagecount { get; set; }
        public List<EventResult> results { get; set; }
    }
}
