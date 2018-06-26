using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models
{
    public class PopularityFeature
    {
        public int RecencyScore { get; set; }
        public IEnumerable<string> KeyWords { get; set; }
        public double Magnitude { get; set; }
    }

    public class SinglePopularityFeature
    {
        public int Rank { get; set; }
        public int Score { get; set; }
        public string Term { get; set; }
        public double AverageMagnitude { get; set; }
    }
}
