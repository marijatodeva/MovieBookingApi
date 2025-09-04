using Microsoft.Extensions.Options;
using MovieApi.Models;
using MovieApi.Models.System;
using MovieAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace MovieApi.Repositories
{
    public class MovieShowingRepository : IMovieShowingRepository
    {
        private readonly DBSettings _dbSettings;

        public MovieShowingRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<IEnumerable<MovieShowing>> GetShowingsForMovie(int movieId, DateTime? date)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);

            if (date.HasValue)
            {
                DateTime showDate = date.Value.Date;

                string sql = @"SELECT id, movieid, showingdate, showingtime::text AS showingtime, hall_id AS hallid
                               FROM movieshowing
                               WHERE movieid = @movieId AND showingdate = @showDate
                               ORDER BY showingtime";

                return await conn.QueryAsync<MovieShowing>(sql, new { movieId, showDate });
            }
            else
            {
                string sql = @"SELECT id, movieid, showingdate, showingtime::text AS showingtime, hall_id AS hallid
                               FROM movieshowing
                               WHERE movieid = @movieId
                               ORDER BY showingdate, showingtime";

                return await conn.QueryAsync<MovieShowing>(sql, new { movieId });
            }
        }

        public async Task<MovieShowing> GetShowingById(int id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT id, movieid, showingdate, showingtime::text AS showingtime, hall_id AS hallid
                           FROM movieshowing
                           WHERE id = @id";

            return await conn.QueryFirstOrDefaultAsync<MovieShowing>(sql, new { id });
        }

        public async Task<int> AddShowing(CreateMovieShowing showing)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            
            string sql = @"INSERT INTO movieshowing (movieid, showingdate, showingtime, hall_id)
                   VALUES (@MovieId, @ShowingDate,  @ShowingTime::time, @HallId)
                   RETURNING id;";

            int newId = await conn.ExecuteScalarAsync<int>(sql, showing);
            return newId;
        }


        public async Task<int> UpdateShowing(MovieShowing showing)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"UPDATE movieshowing
                           SET movieid = @MovieId,
                               showingdate = @ShowingDate,
                               showingtime = @ShowingTime::time,
                               hall_id = @HallId
                           WHERE id = @Id";

            return await conn.ExecuteAsync(sql, showing);
        }

        public async Task<int> DeleteShowing(int id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);

            string sql = @"DELETE FROM movieshowing WHERE id = @id";

            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<List<Seat>> GetSeatsForShowing(int showingId)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);

            string hallSql = @"SELECT hall_id FROM movieshowing WHERE id = @showingId";
            var hallId = await conn.ExecuteScalarAsync<int>(hallSql, new { showingId });

            string seatsSql = @"SELECT id, hall_id, seat_number 
                                FROM seat
                                WHERE hall_id = @hallId
                                ORDER BY seat_number";

            var seats = await conn.QueryAsync<Seat>(seatsSql, new { hallId });
            return seats.AsList();
        }

        public async Task<List<string>> GetBookedSeatsForShowing(int showingId)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);

            string sql = @"
        SELECT t.seat_number
        FROM ticket t
        WHERE t.showing_id = @showingId
        ORDER BY t.seat_number";

            var bookedSeats = await conn.QueryAsync<string>(sql, new { showingId });
            return bookedSeats.AsList();
        }

        public async Task<IEnumerable<MovieShowing>> GetAllShowingsForMovie(int movieId)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);

            string sql = @"SELECT id, movieid, showingdate, showingtime::text AS showingtime, hall_id AS hallid
                               FROM movieshowing
                               WHERE movieid = @movieId
                               ORDER BY showingtime";

            return await conn.QueryAsync<MovieShowing>(sql, new { movieId });
        }
    }
}
