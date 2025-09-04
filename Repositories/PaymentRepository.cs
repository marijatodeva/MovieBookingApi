using Dapper;
using Microsoft.Extensions.Options;
using MovieApi.Models;
using MovieApi.Models.System;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApi.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DBSettings _dbSettings;

        public PaymentRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT id, ticketid, seats, cardnumber, cardholder, expiry, cvv, paidon, paymentmethod, amount
                           FROM payment
                           ORDER BY paidon DESC";

            return await conn.QueryAsync<Payment>(sql);
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT id, ticketid, seats, cardnumber, cardholder, expiry, cvv, paidon, paymentmethod, amount
                           FROM payment
                           WHERE id = @id";

            return await conn.QueryFirstOrDefaultAsync<Payment>(sql, new { id });
        }

        public async Task<int> AddPaymentAsync(Payment payment)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"INSERT INTO payment (ticketid, seats, cardnumber, cardholder, expiry, cvv, paidon, paymentmethod, amount) 
                           VALUES (@TicketId, @Seats, @CardNumber, @CardHolder, @Expiry, @CVV, @PaidOn, @PaymentMethod, @Amount)
                           RETURNING id";

            return await conn.ExecuteScalarAsync<int>(sql, payment);
        }

        public async Task<int> UpdatePaymentAsync(int id, Payment payment)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"UPDATE payment
                           SET ticketid = @TicketId,
                               seats = @Seats,
                               cardnumber = @CardNumber,
                               cardholder = @CardHolder,
                               expiry = @Expiry,
                               cvv = @CVV,
                               paidon = @PaidOn,
                               paymentmethod = @PaymentMethod,
                               amount = @Amount
                           WHERE id = @Id";

            return await conn.ExecuteAsync(sql, new
            {
                Id = id,
                payment.TicketId,
                payment.Seats,
                payment.CardNumber,
                payment.CardHolder,
                payment.Expiry,
                payment.CVV,
                payment.PaidOn,
                payment.PaymentMethod,
                payment.Amount
            });
        }

        public async Task<int> DeletePaymentAsync(int id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"DELETE FROM payment WHERE id = @id";

            return await conn.ExecuteAsync(sql, new { id });
        }
    }
}
