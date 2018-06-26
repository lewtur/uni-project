using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FYP.Models;
using FYP.Models.DataSourceRecords;

namespace FYP.Data.DataSourceRepositories
{
    public class TwitterDataSourceRepository : ConnectionBase, ITwitterDataSourceRepository
    {
        private const string AddUpdateTwitterUserSp = "AddUpdateTwitterUser";
        private const string AddTwitterUserTimestampSp = "AddTwitterUserTimestamp";
        private const string AddTweetSp = "AddTweet";

        public async Task<int> AddUpdateUser(TwitterUser user)
        {
            using (var conn = GetConnection())
            {
                return await conn
                    .QuerySingleOrDefaultAsync<int>(AddUpdateTwitterUserSp, user,
                        commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddUserTimestamp(TwitterUserTimestamp timestamp)
        {
            using (var conn = GetConnection())
            {
                return await conn
                    .QuerySingleOrDefaultAsync<int>(AddTwitterUserTimestampSp, timestamp,
                        commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddTweet(Tweet tweet)
        {
            using (var conn = GetConnection())
            {
                return await conn
                    .QuerySingleOrDefaultAsync<int>(AddTweetSp, tweet, commandType: CommandType.StoredProcedure);
            }
        }
        
        public async Task<TwitterUser> GetTwitterUser(int userId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<TwitterUser>($"SELECT * FROM TwitterUser WHERE Id = ${userId}");
            }
        }

        public async Task<IEnumerable<Tweet>> GetTweetsForArtist(int artistId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<Tweet>($"SELECT * FROM Tweet WHERE ArtistId = ${artistId}");
            }
        }

        public async Task<IEnumerable<TwitterDaySummary>> GetTweetSummaryForArtist(int artistId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<TwitterDaySummary>(
                    $"SELECT * FROM TwitterDaySummary WHERE ArtistId = {artistId} ORDER BY Date");
            }
        }

        public async Task<IEnumerable<TwitterDaySummary>> GetTweetSummaryForArtist(int artistId, int daysToLookBack)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<TwitterDaySummary>("GetTweetSummaryForArtist",
                    new {ArtistId = artistId, DaysToLookBack = daysToLookBack}, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Tweet>> GetTweetsForVenue(int venueId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<Tweet>($"SELECT * FROM Tweet WHERE VenueId = ${venueId}");
            }
        }

        public async Task<TwitterUserTimestamp> GetTwitterUserTimeStamp(int twitterUserId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<TwitterUserTimestamp>($"SELECT TOP 1 * FROM TwitterUserTimestamp WHERE TwitterUserId = {twitterUserId} ORDER BY 1 DESC");
            }
        }

        public async Task<IEnumerable<string>> GetTweetTextOnDayForArtist(int artistId, DateTime date)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<string>(
                    "GetTweetsForArtistOnDate",
                    new {ArtistId = artistId, Date = date.ToString("yyyy-MM-dd") },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout:0
                );
            }
        }

        public async Task SetArtistTwitterDaySummary(int artistId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                var copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);

                try
                {
                    var summary = await conn.QueryAsync<TwitterDaySummary>("GetTweetCountPerDayForArtist",
                        new { ArtistId = artistId }, commandType: CommandType.StoredProcedure, transaction:transaction, commandTimeout:0);

                    await conn.ExecuteAsync($"DELETE FROM TwitterDaySummary WHERE ArtistId = {artistId}",
                        transaction: transaction);                    

                    copy.DestinationTableName = "TwitterDaySummary";
                    var table = new DataTable("TwitterDaySummary");
                    table.Columns.Add("ArtistId", typeof(int));
                    table.Columns.Add("Date", typeof(DateTime));
                    table.Columns.Add("TweetCount", typeof(int));
                    table.Columns.Add("Percentage", typeof(int));

                    var maxCount = summary.Max(x => x.TweetCount);

                    foreach (var day in summary)
                    {
                        table.Rows.Add(artistId, day.Date, day.TweetCount,
                            (int) (((float) day.TweetCount / (float) maxCount) * 100));
                    }

                    await copy.WriteToServerAsync(new DataTableReader(table));

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Dispose();
                    copy.Close();
                    conn.Close();
                }
            }
        }
    }    

    public interface ITwitterDataSourceRepository
    {
        Task<int> AddUpdateUser(TwitterUser user);
        Task<int> AddUserTimestamp(TwitterUserTimestamp timestamp);
        Task<int> AddTweet(Tweet tweet);
        Task<TwitterUser> GetTwitterUser(int userId);
        Task<IEnumerable<Tweet>> GetTweetsForArtist(int artistId);
        Task<IEnumerable<TwitterDaySummary>> GetTweetSummaryForArtist(int artistId);
        Task<IEnumerable<TwitterDaySummary>> GetTweetSummaryForArtist(int artistId, int daysToLookBack);
        Task<IEnumerable<Tweet>> GetTweetsForVenue(int venueId);
        Task<TwitterUserTimestamp> GetTwitterUserTimeStamp(int twitterUserId);
        Task<IEnumerable<string>> GetTweetTextOnDayForArtist(int artistId, DateTime date);
        Task SetArtistTwitterDaySummary(int artistId);
    }
}
