using System;

namespace FYP.Main
{
    public interface IMainUpdater
    {
        void UpdateEventsAndDataSources();
        void UpdateDataSources();
        void UpdateEventsInDateRange(DateTime startDate, DateTime endDate);
    }
}