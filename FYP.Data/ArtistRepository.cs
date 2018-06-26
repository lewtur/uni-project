using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FYP.Models;
using Dapper;

namespace FYP.Data
{
    public class ArtistRepository : ConnectionBase, IArtistRepository
    {
        private const string AddArtistSp = "AddArtist";
        private const string GetArtistSp = "GetArtistByName";
        private const string GetAllArtistSp = "";

        public async Task<Artist> Get(string name)
        {
            var param = new DynamicParameters();
            param.Add("Name", name);

            using (var connection = GetConnection())
            {
                var query = await connection.QueryAsync<Artist>(GetArtistSp, param, commandType: CommandType.StoredProcedure);

                return query.Any() ? query.FirstOrDefault() : null;
            }
        }

        public async Task<IEnumerable<Artist>> GetAll()
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<Artist>("SELECT * FROM Artist");
            }
        }

        public async Task<IEnumerable<Artist>> GetAll(int page, int pageSize, string term)
        {
            using (var conn = GetConnection())
            {
                var artists = await conn.QueryAsync<Artist>("SELECT * FROM Artist");

                if (!string.IsNullOrEmpty(term))
                {
                    artists = artists.Where(x => x.Name.ToLower().Contains(term.ToLower()));
                }

                var result = artists
                    .OrderByDescending(x => x.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return result;
            }
        }

        public async Task<int> Add(Artist artist)
        {
            var param = new DynamicParameters();
            param.Add("Name", artist.Name);
            param.Add("Description", artist.Description);

            using (var conn = GetConnection())
            {
                return (await conn.QueryAsync<int>(AddArtistSp, param, commandType: CommandType.StoredProcedure)).Single();
            }
        }

        public async Task<Artist> GetNameAndSpotifyRecordId(int artistId)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("ArtistId", artistId);

                return await conn.QueryFirstOrDefaultAsync<Artist>("GetArtistNameAndSpotifyRecordId", param,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateArtistDescription(string description, int artistId)
        {
            using (var conn = GetConnection())
            {
                await conn.QueryAsync($"UPDATE Artist SET Description = '{description}' WHERE Id = {artistId}");
            }
        }

        public async Task UpdateImageUrl(string imageUrl, int artistId)
        {
            using (var conn = GetConnection())
            {
                await conn.QueryAsync($"UPDATE Artist SET ImageUrl = '{imageUrl}' WHERE Id = {artistId}");
            }
        }
    }

    public interface IArtistRepository
    {
        Task<Artist> Get(string name);
        Task<IEnumerable<Artist>> GetAll();
        Task<IEnumerable<Artist>> GetAll(int page, int pageSize, string term);
        Task<int> Add(Artist artist);
        Task<Artist> GetNameAndSpotifyRecordId(int artistId);
        Task UpdateArtistDescription(string description, int artistId);
        Task UpdateImageUrl(string imageUrl, int artistId);
    }
}
