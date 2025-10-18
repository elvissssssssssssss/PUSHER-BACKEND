
// 📁 Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using apitextil.Services;
using apitextil.Models.DTOs;

    namespace apitextil.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
  
        public class CartController : ControllerBase
        {
            private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el carrito completo de un usuario
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ActionResult<CartSummaryDto>> GetUserCart(int userId)
        {
            try
            {
                var cart = await _cartService.GetUserCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carrito del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Agrega un producto al carrito
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cartItem = await _cartService.AddToCartAsync(addToCartDto);
                return Ok(new 
                { 
                    message = "Producto agregado al carrito exitosamente",
                    data = cartItem 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto al carrito");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un item en el carrito
        /// </summary>
        [HttpPut("{cartItemId}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cartItem = await _cartService.UpdateCartItemAsync(cartItemId, updateDto);
                return Ok(new 
                { 
                    message = "Cantidad actualizada exitosamente",
                    data = cartItem 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar item del carrito {CartItemId}", cartItemId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un item del carrito
        /// </summary>
        [HttpDelete("{cartItemId}/user/{userId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId, int userId)
        {
            try
            {
                var success = await _cartService.RemoveFromCartAsync(cartItemId, userId);
                if (success)
                {
                    return Ok(new { message = "Producto eliminado del carrito exitosamente" });
                }
                else
                {
                    return NotFound(new { message = "Item del carrito no encontrado" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar item del carrito {CartItemId}", cartItemId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Limpia todo el carrito de un usuario
        /// </summary>
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            try
            {
                var success = await _cartService.ClearCartAsync(userId);
                if (success)
                {
                    return Ok(new { message = "Carrito limpiado exitosamente" });
                }
                else
                {
                    return NotFound(new { message = "Carrito no encontrado" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar carrito del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene la cantidad total de items en el carrito
        /// </summary>
        [HttpGet("count/{userId}")]
        public async Task<ActionResult<int>> GetCartItemCount(int userId)
        {
            try
            {
                var count = await _cartService.GetCartItemCountAsync(userId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cantidad de items del carrito {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
