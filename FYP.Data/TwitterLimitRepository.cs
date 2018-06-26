using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FYP.Models;

namespace FYP.Data
{
    public class TwitterLimitRepository : ConnectionBase, ITwitterLimitRepository
    {
        private const int Limit = 10;

        public async Task AddUpdateArtist(int artistId, int capacityUsed)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("ArtistId", artistId);
                param.Add("MaximumCapacityReached", capacityUsed);

                await conn.QueryAsync("AddUpdateArtistTwitterCapacity", param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AddUpdateVenue(int venueId, int capacityUsed)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("VenueId", venueId);
                param.Add("MaximumCapacityReached", capacityUsed);

                await conn.QueryAsync("AddUpdateVenueTwitterCapacity", param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> ArtistHasExceededLimit(int artistId)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("ArtistId", artistId);
                var count = await conn.QueryFirstOrDefaultAsync<int>("GetTimesReachedTwitterLimitForArtist", param, commandType: CommandType.StoredProcedure);

                return count > Limit;
            }
        }

        public async Task<bool> VenueHasExceededLimit(int venueId)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("VenueId", venueId);
                var count = await conn.QueryFirstOrDefaultAsync<int>("GetTimesReachedTwitterLimitForVenue", param, commandType: CommandType.StoredProcedure);

                return count > Limit;
            }
        }
    }

    public interface ITwitterLimitRepository
    {
        Task AddUpdateArtist(int artistId, int capacityUsed);
        Task AddUpdateVenue(int venueId, int capacityUsed);
        Task<bool> ArtistHasExceededLimit(int artistId);
        Task<bool> VenueHasExceededLimit(int venueId);
    }
}
