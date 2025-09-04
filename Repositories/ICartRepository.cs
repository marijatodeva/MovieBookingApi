using MovieApi.Models;
using System;
using System.Collections.Generic;

public interface ICartRepository
{
    List<CartItem> GetCart();
    void AddToCart(CartItem item);
    void UpdateQuantity(int movieId, DateTime date, string time, int amount);
    void RemoveFromCart(int movieId, DateTime date, string time);
    void ClearCart();
}
