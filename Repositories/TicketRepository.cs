using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieApi.Models;
using MovieApi.Models.System;
using MovieApi.Repositories;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MovieApi.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DBSettings _dbSettings;

        public TicketRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTickets()
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"
                SELECT 
                    t.id, 
                    m.name AS MovieName,
                    u.name AS UserName, 
                    t.watch_movie, 
                    t.amount,
                    t.payment_method,
                    t.seat_number,
                    t.price
                FROM ticket t
                INNER JOIN movie m ON t.movie_id = m.id
                INNER JOIN ""AppUser"" u ON t.user_id = u.id;
            ";

            var tickets = await conn.QueryAsync<TicketResponse>(sql);
            return tickets;
        }

        public async Task<TicketResponse> GetTicketById(long id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"
                SELECT 
                    t.id, 
                    m.name AS MovieName,
                    u.name AS UserName, 
                    t.watch_movie, 
                    t.amount,
                    t.payment_method,
                    t.seat_number,
                    t.price
                FROM ticket t
                INNER JOIN movie m ON t.movie_id = m.id
                INNER JOIN ""AppUser"" u ON t.user_id = u.id
                WHERE t.id = @id;
            ";

            var ticket = await conn.QueryFirstOrDefaultAsync<TicketResponse>(sql, new { id });
            return ticket;
        }


public async Task<Ticket> CreateTicket(CreateTicket ticket)
    {
        using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
        await conn.OpenAsync();

        using var transaction = await conn.BeginTransactionAsync();

        string sqlGetShowing = @"
        SELECT showingdate, showingtime, hall_id
        FROM movieshowing
        WHERE id = @ShowingId;
    ";

        var showing = await conn.QuerySingleAsync<(DateTime showingdate, TimeSpan showingtime, int hall_id)>(
            sqlGetShowing, new { ticket.ShowingId }, transaction);


        var watchMovie = showing.showingdate.Date + showing.showingtime;

        string sqlTicket = @"
        INSERT INTO ticket (movie_id, user_id, watch_movie, amount, payment_method, seat_number, price, showing_id)
        VALUES (@MovieId, @UserId, @WatchMovie, @Amount, @PaymentMethod, @SeatNumber, @Price, @ShowingId)
        RETURNING id;
    ";

        var newTicketId = await conn.ExecuteScalarAsync<int>(sqlTicket, new
        {
            ticket.MovieId,
            ticket.UserId,
            WatchMovie = watchMovie,
            ticket.Amount,
            ticket.PaymentMethod,
            ticket.SeatNumber,
            ticket.Price,
            ticket.ShowingId
        }, transaction);


        string sqlGetSeatId = @"
        SELECT s.id
        FROM seat s
        LEFT JOIN booking b ON s.id = b.seat_id AND b.showing_id = @ShowingId
        WHERE s.seat_number = @SeatNumber AND s.hall_id = @HallId AND b.id IS NULL
        LIMIT 1;
    ";

        var seatId = await conn.ExecuteScalarAsync<int>(sqlGetSeatId, new { ticket.SeatNumber, HallId = showing.hall_id, ShowingId = ticket.ShowingId }, transaction);

        if (seatId == 0)
        {
            throw new Exception($"Seat {ticket.SeatNumber} is already booked for this showing.");
        }

        string sqlBooking = @"
        INSERT INTO booking (showing_id, seat_id, user_id, ticket_id)
        VALUES (@ShowingId, @SeatId, @UserId, @TicketId);
    ";

        await conn.ExecuteAsync(sqlBooking, new
        {
            ShowingId = ticket.ShowingId,
            SeatId = seatId,
            UserId = ticket.UserId,
            TicketId = newTicketId
        }, transaction);


        await transaction.CommitAsync();

        return new Ticket
        {
            Id = newTicketId,
            MovieId = ticket.MovieId,
            UserId = ticket.UserId,
            WatchMovie = watchMovie,
            Amount = ticket.Amount,
            PaymentMethod = ticket.PaymentMethod,
            SeatNumber = ticket.SeatNumber,
            Price = ticket.Price,
            ShowingId = ticket.ShowingId
        };
    }



    public async Task<int> UpdateTicket(long id, UpdateTicket ticket)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"
                UPDATE ticket
                SET watch_movie = @WatchMovie,
                    amount = @Amount,
q                   price=@Price
                WHERE id = @Id;
            ";

            return await conn.ExecuteAsync(sql, new
            {
                Id = id,
                ticket.WatchMovie,
                ticket.Amount
            });
        }

        public async Task<int> DeleteTicket(long id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = "DELETE FROM ticket WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new { id });
        }
        public async Task<IEnumerable<string>> GetBookedSeats(int showingId)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            await conn.OpenAsync();

            string sql = @"
        SELECT s.seat_number
        FROM booking b
        INNER JOIN seat s ON b.seat_id = s.id
        WHERE b.showing_id = @ShowingId;
    ";

            var bookedSeats = await conn.QueryAsync<string>(sql, new { ShowingId = showingId });
            return bookedSeats;
        }

        public async Task<IEnumerable<TicketResponse>> GetTicketsByUser(int userId)
        {

            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"
SELECT 
    t.id,
    m.name AS MovieName,
    u.name AS UserName,
    (s.showingdate + s.showingtime)::timestamp AS WatchMovie,
    t.amount,
    t.payment_method AS PaymentMethod,
    t.seat_number AS SeatNumber,
    t.price
FROM ticket t
INNER JOIN movie m ON t.movie_id = m.id
INNER JOIN ""AppUser"" u ON t.user_id = u.id
INNER JOIN movieshowing s ON t.showing_id = s.id
WHERE t.user_id = @userId;
";

            var tickets = await conn.QueryAsync<TicketResponse>(sql, new { userId });


            return tickets;
        }
    }
}
