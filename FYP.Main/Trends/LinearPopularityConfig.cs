using System;

namespace FYP.Main.Trends
{
    public class LinearPopularityConfig : IPopularityConfig
    {
        public int MaximumTimeFrameInDays => 5;

        public int CalculateScore(TimeSpan timeSpanFromEvent)
        {
            var days = timeSpanFromEvent.TotalDays;

            if (days >= MaximumTimeFrameInDays) return 0;

            var val = MaximumTimeFrameInDays - Math.Abs(timeSpanFromEvent.TotalDays);

            return (int)(val / MaximumTimeFrameInDays * 100);
        }
    }
}