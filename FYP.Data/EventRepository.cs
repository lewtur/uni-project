using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FYP.Models;

namespace FYP.Data
{
    public class EventRepository : ConnectionBase, IEventRepository
    {
        private const string AddEventSp = "AddEvent";
        private const string AddEventLinkupSp = "AddEventLinkup";
        private const string GetEventIdSp = "GetEventId";
        private const string AddToEventHoldingTableSp = "AddToEventHoldingTable";
        private const string GetArtistEventSummarySp = "GetEventSummaryForArtist";

        public async Task<int> Add(Event @event)
        {
            using (var conn = GetConnection())
            {
               return await conn.QueryFirstOrDefaultAsync<int>(AddEventSp, @event, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<int>> AddLinkup(EventLinkup linkup)
        {
            if (linkup.EventId <= 0 || !linkup.ArtistIds.Any()) return null;

            using (var conn = GetConnection())
            {
                var returnList = new List<int>();

                foreach (var link in linkup.ArtistIds)
                {
                    var param = new DynamicParameters();
                    param.Add("ArtistId", link);
                    param.Add("EventId", linkup.EventId);

                    var query = await conn.QueryFirstOrDefaultAsync<int>(AddEventLinkupSp, param, commandType: CommandType.StoredProcedure);

                    returnList.Add(query);
                }

                return returnList;
            }
        }

        public async Task<int> GetEventId(string name)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("Name", name);

                return await conn.QuerySingleOrDefaultAsync<int>(GetEventIdSp, param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddToEventHoldingTable(EventHolding eventHolding)
        {
            using (var conn = GetConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<int>(AddToEventHoldingTableSp, eventHolding, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<ArtistEventSummary>> GetArtistEventSummary(int artistId)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("ArtistId", artistId);

                return await conn.QueryAsync<ArtistEventSummary>(GetArtistEventSummarySp, param,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Event>> GetEventsOnDate(string date)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<Event>($"SELECT * FROM Event WHERE StartDate = '{date}'");
            }
        }

        public async Task<IEnumerable<int>> GetArtistIdsFromEventIdLinkups(int eventId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<int>($"SELECT ArtistId FROM EventLinkup WHERE EventId = {eventId}");
            }
        }
    }

    public interface IEventRepository
    {
        Task<int> Add(Event @event);
        Task<IEnumerable<int>> AddLinkup(EventLinkup linkup);
        Task<int> GetEventId(string name);
        Task<int> AddToEventHoldingTable(EventHolding eventHolding);
        Task<IEnumerable<ArtistEventSummary>> GetArtistEventSummary(int artistId);
        Task<IEnumerable<Event>> GetEventsOnDate(string date);
        Task<IEnumerable<int>> GetArtistIdsFromEventIdLinkups(int eventId);
    }
}
