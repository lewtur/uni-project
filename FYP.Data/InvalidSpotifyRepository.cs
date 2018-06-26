using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace FYP.Data
{
    public interface IInvalidSpotifyRepository
    {
        Task AddArtistToInvalidList(int artistId);
        Task<bool> ArtistIsInInvalidList(int artistId);
    }

    public class InvalidSpotifyRepository : ConnectionBase, IInvalidSpotifyRepository
    {
        public async Task AddArtistToInvalidList(int artistId)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("ArtistId", artistId);

                await conn.ExecuteAsync("AddInvalidSpotifyArtistName", param,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> ArtistIsInInvalidList(int artistId)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("ArtistId", artistId);

                var result = await conn.QueryAsync<int>("GetInvalidSpotifyArtistName", param,
                    commandType: CommandType.StoredProcedure);

                return result.Any();
            }
        }
    }
}
