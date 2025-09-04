using MovieApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieApi.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly List<CartItem> _cart = new List<CartItem>();

        public List<CartItem> GetCart()
        {
            return _cart;
        }

        public void AddToCart(CartItem item)
        {
            var existing = _cart.FirstOrDefault(c =>
                c.MovieId == item.MovieId &&
                c.SelectedDate.Date == item.SelectedDate.Date &&
                c.SelectedTime == item.SelectedTime);

            if (existing != null)
            {
                foreach (var seat in item.Seats)
                {
                    if (!existing.Seats.Contains(seat))
                        existing.Seats.Add(seat);
                }
                existing.Amount = existing.Seats.Count;
            }
            else
            {
                _cart.Add(item);
            }
        }

        public void UpdateQuantity(int movieId, DateTime date, string time, int amount)
        {
            var item = _cart.FirstOrDefault(c =>
                c.MovieId == movieId &&
                c.SelectedDate.Date == date.Date &&
                c.SelectedTime == time);

            if (item != null)
            {
                item.Amount = amount;
                if (item.Seats != null && item.Seats.Count > amount)
                {
                    item.Seats = item.Seats.Take(amount).ToList();
                }

                if (item.Amount <= 0)
                    _cart.Remove(item);
            }
        }

        public void RemoveFromCart(int movieId, DateTime date, string time)
        {
            var item = _cart.FirstOrDefault(c =>
                c.MovieId == movieId &&
                c.SelectedDate.Date == date.Date &&
                c.SelectedTime == time);

            if (item != null)
                _cart.Remove(item);
        }

        public void ClearCart()
        {
            _cart.Clear();
        }
    }
}
