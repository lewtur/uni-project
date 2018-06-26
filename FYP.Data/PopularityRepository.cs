using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FYP.Models;

namespace FYP.Data
{
    public class PopularityRepository : ConnectionBase, IPopularityRepository
    {
        public async Task AddPopularityTerms(IEnumerable<SinglePopularityFeature> terms)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                var copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);

                try
                {
                    await conn.ExecuteAsync("DELETE FROM MostPopularThings", transaction: transaction);

                    var table = new DataTable("MostPopularThings");
                    copy.DestinationTableName = "MostPopularThings";
                    table.Columns.Add("Term", typeof(string));
                    table.Columns.Add("Score", typeof(int));
                    table.Columns.Add("AverageMagnitude", typeof(float));
                    table.Columns.Add("Rank", typeof(int));

                    var i = 0;
                    foreach (var term in terms)
                    {
                        table.Rows.Add(term.Term, term.Score, term.AverageMagnitude, ++i);
                    }

                    await copy.WriteToServerAsync(new DataTableReader(table));

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    transaction.Dispose();
                    copy.Close();
                    conn.Close();
                }
            }
        }

        public async Task<IEnumerable<SinglePopularityFeature>> GetAllPopularityTerms()
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<SinglePopularityFeature>("SELECT * FROM MostPopularThings ORDER BY Rank");
            }
        }

        public async Task SetRecentTrendingArtists(IEnumerable<ArtistWithFeatures> trendingArtists)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                var mainCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
                var featuresCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);

                try
                {
                    await conn.ExecuteAsync("DELETE FROM RecentTrendingArtistLinkup", transaction: transaction);
                    await conn.ExecuteAsync("DELETE FROM RecentTrendingArtists", transaction: transaction);

                    var mainTable = new DataTable();
                    var featuresTable = new DataTable();

                    mainCopy.DestinationTableName = "RecentTrendingArtists";
                    featuresCopy.DestinationTableName = "RecentTrendingArtistLinkup";

                    mainTable.Columns.Add("Rank", typeof(int));
                    mainTable.Columns.Add("ArtistId", typeof(int));

                    featuresTable.Columns.Add("RecentTrendingArtistRank", typeof(int));
                    featuresTable.Columns.Add("PopularityFeatureRank", typeof(int));

                    var artistRank = 1;
                    foreach (var artist in trendingArtists)
                    {
                        mainTable.Rows.Add(artistRank, artist.Artist.Id);
                        foreach (var feature in artist.Features)
                        {
                            featuresTable.Rows.Add(artistRank, feature.Rank);
                        }
                        ++artistRank;
                    }

                    await mainCopy.WriteToServerAsync(new DataTableReader(mainTable));
                    await featuresCopy.WriteToServerAsync(new DataTableReader(featuresTable));

                    await conn.ExecuteAsync("DELETE FROM RecentTrendingArtistsHeader", transaction: transaction);
                    await conn.ExecuteAsync(
                        "INSERT INTO RecentTrendingArtistsHeader (DateLastUpdated) VALUES (GETDATE())",
                        transaction: transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Dispose();
                    mainCopy.Close();
                    featuresCopy.Close();
                    conn.Close();
                }
            }
        }

        public async Task<IEnumerable<ArtistWithFeatures>> GetAllRecentTrendingArtist()
        {
            var artistsToReturn = new List<ArtistWithFeatures>();
            using (var conn = GetConnection())
            {
                var artists = await conn.QueryAsync<RecentTrendingArtist>("SELECT * FROM RecentTrendingArtists");
                foreach (var artist in artists)
                {
                    var features =
                        await conn.QueryAsync<SinglePopularityFeature>("GetTrendingArtistFeatures",
                            new {ArtistRank = artist.Rank}, commandType: CommandType.StoredProcedure);

                    artistsToReturn.Add(new ArtistWithFeatures
                    {
                        Artist = new Artist { Id = artist.ArtistId },
                        Features = features.ToList()
                    });
                }
            }

            return artistsToReturn;
        }

        private class RecentTrendingArtist
        {
            public int Rank { get; set; }
            public int ArtistId { get; set; }
        }
    }

    public interface IPopularityRepository
    {
        Task AddPopularityTerms(IEnumerable<SinglePopularityFeature> terms);
        Task<IEnumerable<SinglePopularityFeature>> GetAllPopularityTerms();
        Task SetRecentTrendingArtists(IEnumerable<ArtistWithFeatures> trendingArtists);
        Task<IEnumerable<ArtistWithFeatures>> GetAllRecentTrendingArtist();
    }
}
