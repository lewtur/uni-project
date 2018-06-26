using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;

namespace FYP.Main.Trends
{
    public interface IGraphHunter
    {
        IEnumerable<Spike> GetSpikes(IEnumerable<TwitterDaySummary> twitterData,
            IEnumerable<SpotifyArtistStats> spotifyArtistData,
            IEnumerable<AlbumStats> spotifyAlbumData);
    }

    public class GraphHunter : IGraphHunter
    {
        private const int TweetPercentageSpikeThreshold = 20;
        private const int SpotifyPercentageSpikeThreshold = 1;

        public IEnumerable<Spike> GetSpikes(IEnumerable<TwitterDaySummary> twitterData, 
            IEnumerable<SpotifyArtistStats> spotifyArtistData, 
            IEnumerable<AlbumStats> spotifyAlbumData)
        {           
            var trends = new List<Spike>();

            AddTrends(trends, twitterData, TweetPercentageSpikeThreshold);
            AddTrends(trends, spotifyArtistData, SpotifyPercentageSpikeThreshold);
            AddAlbumTrends(trends, spotifyAlbumData, SpotifyPercentageSpikeThreshold);
                     
            return trends;
        }

        private static void AddAlbumTrends(ICollection<Spike> trends, IEnumerable<AlbumStats> albums, int threshold)
        {
            if (albums == null || !albums.Any()) return;

            foreach (var album in albums)
            {
                AddTrends(trends, album.Stats, threshold);
            }
        }

        private static void AddTrends(ICollection<Spike> trends, IEnumerable<ITrend> data, int threshold)
        {
            if (data == null || !data.Any()) return;

            data = data.OrderBy(x => x.GetDate());
            var first = data.First();
            var rest = data.Skip(1);

            var spikeStarted = false;
            var spikeValue = 0;
            var currentSpikeRange = new Spike();
            var lastRecord = first;

            foreach (var record in rest)
            {
                if (record.GetScore() - lastRecord.GetScore() >= threshold)
                {
                    if (spikeStarted)
                    {
                        currentSpikeRange.ToDate = record.GetDate();
                    }
                    else
                    {
                        spikeStarted = true;
                        spikeValue = record.GetScore();
                        currentSpikeRange.FromDate = record.GetDate();
                        currentSpikeRange.ToDate = record.GetDate();
                        currentSpikeRange.SpikeMagnitude = (record.GetScore() - lastRecord.GetScore()) / (double)threshold;
                    }
                }
                else if (record.GetScore() + threshold <= spikeValue + 1)
                {
                    if (spikeStarted)
                    {
                        spikeStarted = false;
                        trends.Add(currentSpikeRange);
                        currentSpikeRange = new Spike();
                    }                    
                }
                else
                {
                    if (spikeStarted)
                    {
                        currentSpikeRange.ToDate = record.GetDate();
                    }
                }

                lastRecord = record;
            }

            if (!currentSpikeRange.FromDate.Equals(default(DateTime)))
            {
                trends.Add(currentSpikeRange);
            }
        }
    }
}
