using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;

namespace FYP.Data.DataSourceRepositories
{
    public class SpotifyDataSourceRepository : ConnectionBase, ISpotifyDataSourceRepository
    {
        private const string GetArtistHeaderSp = "GetSpotifyArtistHeaderBySpotifyId";
        private const string AddArtistHeaderSp = "AddSpotifyArtistHeader";
        private const string AddArtistStatsSp = "AddSpotifyArtistStats";
        private const string GetAlbumHeaderSp = "GetSpotifyAlbumHeaderBySpotifyId";
        private const string AddAlbumHeaderSp = "AddSpotifyAlbumHeader";        
        private const string AddAlbumStatsSp = "AddSpotifyAlbumStats";
        private const string GetArtistSpotifyStatsSp = "GetArtistSpotifyStats";

        public async Task<int> GetOrCreateArtistHeaderId(SpotifyArtistHeader header)
        {
            using (var conn = GetConnection())
            {
                var paramsA = new DynamicParameters();
                paramsA.Add("SpotifyId", header.SpotifyRecordId);

                var possibleHeader = await conn
                    .QueryFirstOrDefaultAsync<SpotifyArtistHeader>(GetArtistHeaderSp, paramsA,
                        commandType: CommandType.StoredProcedure);

                if (!string.IsNullOrEmpty(possibleHeader?.SpotifyRecordId)) return possibleHeader.Id;

                var paramB = new DynamicParameters();
                paramB.Add("ArtistId", header.ArtistId);
                paramB.Add("SpotifyRecordId", header.SpotifyRecordId);
                paramB.Add("Type", header.Type);
                paramB.Add("Genres", header.Genres);

                var createdId = await conn.QueryFirstOrDefaultAsync<int>(AddArtistHeaderSp, paramB,
                    commandType: CommandType.StoredProcedure);

                return createdId;
            }
        }

        public void AddArtistStats(SpotifyArtistStats stats)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("SpotifyArtistHeaderId", stats.SpotifyArtistHeaderId);
                param.Add("Followers", stats.Followers);
                param.Add("Popularity", stats.Popularity);
                param.Add("DatePosted", stats.DatePosted);

                conn.Query(AddArtistStatsSp, param, commandType: CommandType.StoredProcedure);
            }
        }

        public int GetOrCreateAlbumHeaderId(SpotifyAlbumHeader header)
        {
            using (var conn = GetConnection())
            {
                var paramsA = new DynamicParameters();
                paramsA.Add("SpotifyId", header.SpotifyRecordId);

                var possibleHeader = conn
                    .Query<SpotifyAlbumHeader>(GetAlbumHeaderSp, paramsA, commandType: CommandType.StoredProcedure)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(possibleHeader?.SpotifyRecordId)) return possibleHeader.Id;

                var paramB = new DynamicParameters();
                paramB.Add("SpotifyArtistHeaderId", header.SpotifyArtistHeaderId);
                paramB.Add("SpotifyRecordId", header.SpotifyRecordId);
                paramB.Add("Label", header.Label);
                paramB.Add("AlbumType", header.AlbumType);
                paramB.Add("ReleaseDate", header.ReleaseDate);
                paramB.Add("Name", header.Name);
                paramB.Add("AlbumArtworkUrl", header.AlbumArtworkUrl);

                var createdId = conn.Query<int>(AddAlbumHeaderSp, paramB,
                        commandType: CommandType.StoredProcedure)
                    .FirstOrDefault();

                return createdId;
            }
        }

        public void AddAlbumStats(SpotifyAlbumStats stats)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("SpotifyAlbumHeaderId", stats.SpotifyAlbumHeaderId);
                param.Add("Popularity", stats.Popularity);
                param.Add("DatePosted", stats.DatePosted);

                conn.Query(AddAlbumStatsSp, param, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<SpotifyArtistHeader> GetAllArtistHeaders()
        {
            using (var conn = GetConnection())
            {
                return conn.Query<SpotifyArtistHeader>("SELECT * FROM SpotifyArtistHeader");
            }
        }

        public async Task<int> GetArtistHeaderIdByArtistId(int artistId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<int>(
                    $"SELECT TOP 1 Id FROM SpotifyArtistHeader WHERE ArtistId = {artistId}");
            }
        }

        public async Task<IEnumerable<SpotifyAlbumHeader>> GetAllAlbumHeaders()
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<SpotifyAlbumHeader>("SELECT * FROM SpotifyAlbumHeader");
            }
        }

        public async Task<IEnumerable<SpotifyAlbumHeader>> GetAllAlbumHeaders(int spotifyArtistHeaderId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<SpotifyAlbumHeader>(
                    $"SELECT * FROM SpotifyAlbumHeader WHERE SpotifyArtistHeaderId = {spotifyArtistHeaderId}");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByDate(string date)
        {
            using (var conn = GetConnection())
            {           
                return await conn.QueryAsync<Album>("GetAlbumsReleasedOnDate", new {Date = date},
                    commandType: CommandType.StoredProcedure);
            }
        }

        public virtual async Task<IEnumerable<DetailedArtist>> GetMostPopularArtist(int daysToLookBack, int limit)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<DetailedArtist>("GetChangeInSpotifyPopularity",
                    new {NumberOfDaysToLookBack = daysToLookBack, Limit = limit}, commandType: CommandType.StoredProcedure, commandTimeout:60);
            }
        }

        public async Task<IEnumerable<SpotifyArtistStats>> GetAllStatsForArtist(int spotifyArtistHeaderId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<SpotifyArtistStats>(
                    $"SELECT * FROM SpotifyArtistStats WHERE SpotifyArtistHeaderId = {spotifyArtistHeaderId}");
            }
        }

        public async Task<IEnumerable<SpotifyArtistStats>> GetAllStatsForArtist(int spotifyArtistHeaderId, int daysToLookBack)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<SpotifyArtistStats>("GetAllSpotifyStatsForArtist",
                    new {SpotifyArtistHeaderId = spotifyArtistHeaderId, DaysToLookBack = daysToLookBack},
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> GetSpotifyArtistHeaderId(int artistId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<int>($"SELECT Id From SpotifyArtistHeader WHERE ArtistId = {artistId}");
            }
        }

        public async Task<IEnumerable<SpotifyAlbumStats>> GetAllStatsForAlbum(int spotifyAlbumHeaderId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<SpotifyAlbumStats>(
                    $"SELECT * FROM SpotifyAlbumStats WHERE SpotifyAlbumHeaderId = {spotifyAlbumHeaderId}");
            }
        }
    }    

    public interface ISpotifyDataSourceRepository
    {
        Task<int> GetOrCreateArtistHeaderId(SpotifyArtistHeader header);
        void AddArtistStats(SpotifyArtistStats stats);
        int GetOrCreateAlbumHeaderId(SpotifyAlbumHeader header);
        void AddAlbumStats(SpotifyAlbumStats stats);
        IEnumerable<SpotifyArtistHeader> GetAllArtistHeaders();
        Task<int> GetArtistHeaderIdByArtistId(int artistId);
        Task<IEnumerable<SpotifyAlbumHeader>> GetAllAlbumHeaders();
        Task<IEnumerable<SpotifyAlbumHeader>> GetAllAlbumHeaders(int spotifyArtistHeaderId);
        Task<IEnumerable<Album>> GetAlbumsByDate(string date);
        Task<IEnumerable<DetailedArtist>> GetMostPopularArtist(int daysToLookBack, int limit);
        Task<IEnumerable<SpotifyArtistStats>> GetAllStatsForArtist(int spotifyArtistHeaderId);
        Task<IEnumerable<SpotifyArtistStats>> GetAllStatsForArtist(int spotifyArtistHeaderId, int daysToLookBack);
        Task<int> GetSpotifyArtistHeaderId(int artistId);
        Task<IEnumerable<SpotifyAlbumStats>> GetAllStatsForAlbum(int spotifyAlbumHeaderId);
    }
}
