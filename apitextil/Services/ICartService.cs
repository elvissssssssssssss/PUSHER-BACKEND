
// 📁 Services/ICartService.cs
using apitextil.Models.DTOs;

namespace apitextil.Services
{
    public interface ICartService
    {
        Task<CartSummaryDto> GetUserCartAsync(int userId);
        Task<CartItemDto> AddToCartAsync(AddToCartDto addToCartDto);
        Task<CartItemDto> UpdateCartItemAsync(long cartItemId, UpdateCartItemDto updateDto);
        Task<bool> RemoveFromCartAsync(long cartItemId, int userId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<bool> ValidateCartItemAsync(long cartItemId, int userId);
    }
}
