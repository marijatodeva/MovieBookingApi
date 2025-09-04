using Dapper;
using Microsoft.Extensions.Options;
using MovieApi.Models;
using MovieApi.Models.System;
using Npgsql;

namespace MovieApi.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DBSettings _dbSettings;

        public MovieRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT 
                               id, 
                               name, 
                               duration, 
                               release_date, 
                               amount, 
                               imageurl, 
                               description, 
                               genre, 
                               price,
                               director,
                               stars,
                               trailer_url,
                               ""Rating""
 
                           FROM movie";

            var result = await conn.QueryAsync<Movie>(sql);
            return result;
        }

        public async Task<Movie> GetMovieById(long id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT 
                               id, 
                               name, 
                               duration, 
                               release_date, 
                               amount, 
                               imageurl, 
                               description, 
                               genre, 
                               price,
                               director,
                               stars,
                               trailer_url,
                               ""Rating""

                           FROM movie 
                           WHERE id = @id";

            var movie = await conn.QueryFirstOrDefaultAsync<Movie>(sql, new { id });
            return movie;
        }

        public async Task<int> CreateMovie(Movie movie)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"INSERT INTO movie 
                           (id, name, duration, release_date, amount, imageurl, description, genre, price, director, stars, trailer_url, ""Rating"")
                           VALUES 
                           (@Id, @Name, @Duration, @ReleaseDate, @Amount, @ImageUrl, @Description, @Genre, @Price, @Director, @Stars, @TrailerUrl, @Rating)";

            return await conn.ExecuteAsync(sql, movie);
        }

        public async Task<int> UpdateMovie(long id, Movie movie)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"UPDATE movie
                           SET name = @Name,
                               duration = @Duration,
                               release_date = @ReleaseDate,
                               amount = @Amount,
                               imageurl = @ImageUrl,
                               description = @Description, 
                               genre = @Genre,
                               price = @Price,
                               director = @Director,
                               stars = @Stars,
                               trailer_url = @TrailerUrl,
                               ""Rating"" = @Rating
                           WHERE id = @Id";

            return await conn.ExecuteAsync(sql, new
            {
                Id = id,
                movie.Name,
                movie.Duration,
                movie.ReleaseDate,
                movie.Amount,
                movie.ImageUrl,
                movie.Description,
                movie.Genre,
                movie.Price,
                movie.Director,
                movie.Stars,
                movie.TrailerUrl,
                movie.Rating

            });
        }

        public async Task<int> DeleteMovie(long id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = "DELETE FROM movie WHERE id = @id";

            return await conn.ExecuteAsync(sql, new { id });
        }
    }
}
