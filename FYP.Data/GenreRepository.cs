using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FYP.Models;

namespace FYP.Data
{
    public class GenreRepository : ConnectionBase, IGenreRepository
    {
        public async Task AddGenreForArtist(Genre genre)
        {
            using (var conn = GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("ArtistId", genre.ArtistId);
                param.Add("SpotifyGivenGenre", genre.SpotifyGivenGenre);
                param.Add("MostPopularGenreOfRelatedArtists", genre.MostPopularGenreOfRelatedArtists);
                param.Add("OtherGenresGivenInRelatedArtists", genre.OtherGenresGivenInRelatedArtists);

                await conn.QueryAsync("AddGenreForArtist", param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Genre> GetGenreForArtist(int artistId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<Genre>("GetGenreForArtist", new {ArtistId = artistId}, commandType: CommandType.StoredProcedure);
            }
        }
    }

    public interface IGenreRepository
    {
        Task AddGenreForArtist(Genre genre);
        Task<Genre> GetGenreForArtist(int artistId);
    }    
}
