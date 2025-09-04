using MovieApi.Models;
using MovieAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApi.Repositories
{
    public interface IMovieShowingRepository
    {
        Task<IEnumerable<MovieShowing>> GetShowingsForMovie(int movieId, DateTime? date);
        Task<IEnumerable<MovieShowing>> GetAllShowingsForMovie(int movieId);
        Task<MovieShowing> GetShowingById(int id);
        Task<int> AddShowing(CreateMovieShowing showing);
        Task<int> UpdateShowing(MovieShowing showing);
        Task<int> DeleteShowing(int id);
        Task<List<Seat>> GetSeatsForShowing(int showingId); 
        Task<List<string>> GetBookedSeatsForShowing(int showingId); 
    }
}
