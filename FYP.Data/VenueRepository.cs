using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FYP.Models;

namespace FYP.Data
{
    public class VenueRepository : ConnectionBase, IVenueRepository
    {
        private const string AddVenueSp = "AddVenue";
        private const string GetVenueByNameSp = "GetVenueByName";

        public async Task<int> Add(Venue venue)
        {
            using (var conn = GetConnection())
            {
                var possibleVenue = await GetVenue(venue.Name);

                if (possibleVenue?.Id != null)
                {
                    return possibleVenue.Id;
                }

                return await conn.QueryFirstOrDefaultAsync<int>(AddVenueSp, venue.GetSqlFriendlyVenue(),
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Venue> GetVenue(string name)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("Name", name);

                var venues = await conn.QueryAsync<Venue>(GetVenueByNameSp, param, commandType: CommandType.StoredProcedure);

                var venue = venues.FirstOrDefault(x => x.Name == name);

                return !string.IsNullOrEmpty(venue?.Name) ? venue : null;
            }
        }

        public async Task<Venue> GetVenue(int venueId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<Venue>($"SELECT TOP 1 * FROM Venue WHERE Id = {venueId}");
            }
        }

        public async Task<IEnumerable<Venue>> GetAll()
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<Venue>("SELECT * FROM Venue");
            }
        }
    }

    public interface IVenueRepository
    {
        Task<int> Add(Venue venue);
        Task<Venue> GetVenue(string name);
        Task<Venue> GetVenue(int venueId);
        Task<IEnumerable<Venue>> GetAll();
    }
}
