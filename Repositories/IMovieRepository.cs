using MovieApi.Models;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAllMovies();
    Task<Movie> GetMovieById(long id);
    Task<int> CreateMovie(Movie movie);
    Task<int> UpdateMovie(long id, Movie movie);
    Task<int> DeleteMovie(long id);
}
