using MovieApi.Models;

namespace MovieApi.Repositories
{
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketResponse>> GetAllTickets();
        Task<TicketResponse> GetTicketById(long id);
        Task<Ticket> CreateTicket(CreateTicket ticket);
        Task<int> UpdateTicket(long id, UpdateTicket ticket);
        Task<int> DeleteTicket(long id);
        Task<IEnumerable<TicketResponse>> GetTicketsByUser(int userId);
        // Ново (со showingId)
        Task<IEnumerable<string>> GetBookedSeats(int showingId);
    }
}
