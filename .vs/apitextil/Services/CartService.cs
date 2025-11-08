
// 📁 Services/CartService.cs - Actualizado para tu estructura
using Microsoft.EntityFrameworkCore;
using apitextil.Data;
using apitextil.Models;
using apitextil.Models.DTOs;


namespace apitextil.Services
{
    public class CartService : ICartService
    {
        private readonly EcommerceContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(EcommerceContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartSummaryDto> GetUserCartAsync(int userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var cartItemsDto = cartItems.Select(item => new CartItemDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    Talla = item.Talla,
                    Quantity = item.Quantity,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    ProductName = item.Product?.Nombre ?? "Producto no encontrado",
                    ProductImage = item.Product?.imagen ?? "",
                    ProductDescription = item.Product?.Description ?? "",
                    ProductPrice = item.Product?.Precio ?? 0,
                    ProductStock = item.Product?.Stock ?? 0,
                    TotalPrice = (item.Product?.Precio ?? 0) * item.Quantity,
                     // Nuevos campos agregados
                    Color = item.Product?.Color ?? "",
                    PrecioDescuento = item.Product?.PrecioDescuento
                }).ToList();

                var subTotal = cartItemsDto.Sum(item => item.TotalPrice);
                var tax = subTotal * 0.18m; // IGV 18%
                var total = subTotal + tax;

                return new CartSummaryDto
                {
                    TotalItems = cartItemsDto.Sum(item => item.Quantity),
                    SubTotal = subTotal,
                    Tax = tax,
                    Total = total,
                    Items = cartItemsDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carrito del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<CartItemDto> AddToCartAsync(AddToCartDto addToCartDto)
        {
            try
            {
                // Validar que el producto existe
                var product = await _context.Productos.FindAsync(addToCartDto.ProductId);
                if (product == null)
                {
                    throw new ArgumentException("Producto no encontrado");
                }

                // Validar stock disponible
                if (product.Stock < addToCartDto.Quantity)
                {
                    throw new ArgumentException("Stock insuficiente");
                }

                // Verificar si el item ya existe en el carrito
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == addToCartDto.UserId &&
                                            c.ProductId == addToCartDto.ProductId &&
                                            c.Talla == addToCartDto.Talla);

                if (existingItem != null)
                {
                    // Actualizar cantidad
                    var newQuantity = existingItem.Quantity + addToCartDto.Quantity;
                    if (newQuantity > 10)
                    {
                        throw new ArgumentException("Cantidad máxima excedida (10 unidades por producto)");
                    }

                    if (newQuantity > product.Stock)
                    {
                        throw new ArgumentException("Stock insuficiente");
                    }

                    existingItem.Quantity = newQuantity;
                    existingItem.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return await GetCartItemDtoAsync(existingItem.Id);
                }
                else
                {
                    // Crear nuevo item
                    var cartItem = new CartItem
                    {
                        UserId = addToCartDto.UserId,
                        ProductId = addToCartDto.ProductId,
                        Talla = addToCartDto.Talla,
                        Quantity = addToCartDto.Quantity
                    };

                    _context.CartItems.Add(cartItem);
                    await _context.SaveChangesAsync();

                    return await GetCartItemDtoAsync(cartItem.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto al carrito");
                throw;
            }
        }

        public async Task<CartItemDto> UpdateCartItemAsync(long cartItemId, UpdateCartItemDto updateDto)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == cartItemId);

                if (cartItem == null)
                {
                    throw new ArgumentException("Item del carrito no encontrado");
                }

                // Validar stock
                if (cartItem.Product.Stock < updateDto.Quantity)
                {
                    throw new ArgumentException("Stock insuficiente");
                }

                cartItem.Quantity = updateDto.Quantity;
                cartItem.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return await GetCartItemDtoAsync(cartItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar item del carrito {CartItemId}", cartItemId);
                throw;
            }
        }

        public async Task<bool> RemoveFromCartAsync(long cartItemId, int userId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

                if (cartItem == null)
                {
                    return false;
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar item del carrito {CartItemId}", cartItemId);
                throw;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar carrito del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            try
            {
                return await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cantidad de items del carrito {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ValidateCartItemAsync(long cartItemId, int userId)
        {
            return await _context.CartItems
                .AnyAsync(c => c.Id == cartItemId && c.UserId == userId);
        }

        private async Task<CartItemDto> GetCartItemDtoAsync(long cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartItemId);

            if (cartItem == null)
                throw new ArgumentException("Item del carrito no encontrado");

            return new CartItemDto
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                ProductId = cartItem.ProductId,
                Talla = cartItem.Talla,
                Quantity = cartItem.Quantity,
                CreatedAt = cartItem.CreatedAt,
                UpdatedAt = cartItem.UpdatedAt,
                ProductName = cartItem.Product?.Nombre ?? "Producto no encontrado",
                ProductImage = cartItem.Product?.imagen ?? "",
                ProductDescription = cartItem.Product?.Description ?? "",
                ProductPrice = cartItem.Product?.Precio ?? 0,
                ProductStock = cartItem.Product?.Stock ?? 0,
                TotalPrice = (cartItem.Product?.Precio ?? 0) * cartItem.Quantity,
                 //  También aquí
                  Color = cartItem.Product?.Color ?? "",
                PrecioDescuento = cartItem.Product?.PrecioDescuento
            };
        }
    }
}
