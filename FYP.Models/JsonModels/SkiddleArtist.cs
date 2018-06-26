using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models.JsonModels
{
    public class Result
    {
        public string id { get; set; }
        public string name { get; set; }
        public string imageurl { get; set; }
        public object nextevent { get; set; }
        public int favourite { get; set; }
        public string spotifymp3url { get; set; }
        public string spotifyartisturl { get; set; }
    }

    public class SkiddleArtistOverview
    {
        public int error { get; set; }
        public string totalcount { get; set; }
        public int pagecount { get; set; }
        public List<Result> results { get; set; }
    }
}
