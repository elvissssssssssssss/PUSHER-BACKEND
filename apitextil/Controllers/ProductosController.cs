
using apitextil.DTOs;
using apitextil.DTOs.apitextil.DTOs;
using apitextil.Services;
using apitextil.Services.apitextil.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase // Corregido el nombre
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: api/Productos
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<ProductoDto>>))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProductos()
        {
            try
            {
                var productos = await _productoService.GetAllProductosAsync();
                return Ok(new ApiResponse<IEnumerable<ProductoDto>>(true, "Productos obtenidos exitosamente", productos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Error interno del servidor", null, ex.Message));
            }
        }







        // GET: api/Productos/categoria/mujer_uniformes_deportivas
        [HttpGet("categoria/{categoria}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<ProductoDto>>))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProductosByCategoria(string categoria)
        {
            try
            {
                var productos = await _productoService.GetProductosByCategoriaAsync(categoria);
                return Ok(new ApiResponse<IEnumerable<ProductoDto>>(true, "Productos filtrados", productos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Error interno del servidor", null, ex.Message));
            }
        }




        // GET: api/Productos/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<ProductoDto>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProductoById(int id)
        {
            try
            {
                var producto = await _productoService.GetProductoByIdAsync(id);
                return producto == null
                    ? NotFound(new ApiResponse(false, "Producto no encontrado"))
                    : Ok(new ApiResponse<ProductoDto>(true, "Producto obtenido exitosamente", producto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Error interno del servidor", null, ex.Message));
            }
        }
        //Controllers/ProductosController.cs
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ApiResponse<ProductoDto>))]
        public async Task<IActionResult> CreateProducto([FromForm] CreateProductoDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Datos inválidos", ModelState));

            try
            {
                var createdProducto = await _productoService.CreateProductoAsync(createDto);
                return CreatedAtAction(
                    nameof(GetProductoById),
                    new { id = createdProducto.Id },
                    new ApiResponse<ProductoDto>(true, "Producto creado exitosamente", createdProducto)
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Error interno del servidor", null, ex.Message));
            }
        }
        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                var deleted = await _productoService.DeleteProductoAsync(id);
                return deleted
                    ? Ok(new ApiResponse(true, "Producto eliminado correctamente"))
                    : NotFound(new ApiResponse(false, "Producto no encontrado"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Error interno al eliminar el producto", null, ex.Message));
            }
        }


        private void DeleteOldimage(string imagen)
        {
            throw new NotImplementedException();
        }


        // PUT: apitextil/controllers/ProductosController.cs



        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProducto(int id, [FromForm] UpdateProductoDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Datos inválidos", ModelState));

            try
            {
                // Procesar imágenes si se enviaron
                if (updateDto.imagen != null)
                {
                    updateDto.imagenPath = await SaveFileAsync(updateDto.imagen);
                }
                if (updateDto.imagen2 != null)
                {
                    updateDto.imagen2Path = await SaveFileAsync(updateDto.imagen2);
                }
                if (updateDto.imagen3 != null)
                {
                    updateDto.imagen3Path = await SaveFileAsync(updateDto.imagen3);
                }

                var updatedProducto = await _productoService.UpdateProductoAsync(id, updateDto);

                return updatedProducto == null
                    ? NotFound(new ApiResponse(false, "Producto no encontrado"))
                    : Ok(new ApiResponse<ProductoDto>(true, "Producto actualizado exitosamente", updatedProducto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Error al actualizar el producto", null, ex.Message));
            }
        }


        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
        }
        // Resto de tus endpoints...
    }

    // Clase para estandarizar las respuestas
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public ApiResponse(bool success, string message, T data, string error = null)
            : base(success, message, error)
        {
            Data = data;
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public object Errors { get; set; }

        public ApiResponse(bool success, string message, object? errors = null, string? error = null)
        {
            Success = success;
            Message = message;
            Error = error;
            Errors = errors;
        }
    }
}