using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using System;

[Route("api/[controller]")]
[ApiController]
public class CartApiController : ControllerBase
{
    private readonly ICartRepository _cartRepo;

    public CartApiController(ICartRepository cartRepo)
    {
        _cartRepo = cartRepo;
    }


    [HttpGet]
    public IActionResult GetCart()
    {
        var cart = _cartRepo.GetCart();
        return Ok(cart);
    }


    [HttpPost("Add")]
    public IActionResult AddToCart([FromBody] CartItem item)
    {
        if (item == null)
            return BadRequest(new { message = "Invalid cart item" });

        _cartRepo.AddToCart(item);
        return Ok(new { message = "Added to cart" });
    }


    [HttpPost("UpdateQuantity")]
    public IActionResult UpdateQuantity(int movieId, DateTime date, string time, int amount)
    {
        _cartRepo.UpdateQuantity(movieId, date, time, amount);
        return Ok(new { message = "Quantity updated" });
    }


    [HttpPost("Remove")]
    public IActionResult RemoveFromCart(int movieId, DateTime date, string time)
    {
        _cartRepo.RemoveFromCart(movieId, date, time);
        return Ok(new { message = "Removed from cart" });
    }


    [HttpPost("Clear")]
    public IActionResult ClearCart()
    {
        _cartRepo.ClearCart();
        return Ok(new { message = "Cart cleared" });
    }
}
