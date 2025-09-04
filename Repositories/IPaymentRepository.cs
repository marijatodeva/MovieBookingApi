using MovieApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApi.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment> GetPaymentByIdAsync(int id);
        Task<int> AddPaymentAsync(Payment payment);
        Task<int> UpdatePaymentAsync(int id, Payment payment);
        Task<int> DeletePaymentAsync(int id);
    }
}
