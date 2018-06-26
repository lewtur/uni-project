using System;

namespace FYP.Main.Trends
{
    public interface IPopularityConfig
    {
        int MaximumTimeFrameInDays { get; }
        int CalculateScore(TimeSpan timeSpanFromEvent);
    }
}