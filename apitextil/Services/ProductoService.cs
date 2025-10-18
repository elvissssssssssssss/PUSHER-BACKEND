using apitextil.Data;
using apitextil.DTOs;
using apitextil.DTOs.apitextil.DTOs;
using apitextil.Models;
using apitextil.Services.apitextil.Services;
using Microsoft.EntityFrameworkCore;
//Services/ProductoService.cs
namespace apitextil.Services
{
    public class ProductoService : IProductoService
    {
        private readonly EcommerceContext _context;

        public ProductoService(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllProductosAsync()
        {
            var productos = await _context.Productos
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return productos.Select(MapToDto);
        }

        public async Task<ProductoDto?> GetProductoByIdAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            return producto != null ? MapToDto(producto) : null;
        }

        public async Task<ProductoDto?> GetProductoByCodeAsync(string code)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Code == code);
            return producto != null ? MapToDto(producto) : null;
        }

        public async Task<IEnumerable<ProductoDto>> GetProductosByCategoriaAsync(string categoria)
        {
            var productos = await _context.Productos
                .Where(p => p.Categoria == categoria)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return productos.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductoDto>> GetProductosByMarcaAsync(string marca)
        {
            var productos = await _context.Productos
                .Where(p => p.Marca == marca)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return productos.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductoDto>> SearchProductosAsync(string searchTerm)
        {
            var productos = await _context.Productos
                .Where(p => p.Nombre.Contains(searchTerm) ||
                           p.Description!.Contains(searchTerm) ||
                           p.Marca.Contains(searchTerm) ||
                           p.Categoria!.Contains(searchTerm) ||
                           p.Code.Contains(searchTerm))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return productos.Select(MapToDto);
        }

        public async Task<ProductoDto> CreateProductoAsync(CreateProductoDto createProductoDto)
        {
            // Verificar si el código ya existe
            if (await CodeExistsAsync(createProductoDto.Code))
            {
                throw new InvalidOperationException("Ya existe un producto con este código");
            }

            // Procesar las imágenes y convertirlas a cadenas de texto (por ejemplo, rutas de archivo)
            string? imagenPath = await SaveFileAsync(createProductoDto.imagen);
            string? imagen2Path = await SaveFileAsync(createProductoDto.imagen2);
            string? imagen3Path = await SaveFileAsync(createProductoDto.imagen3);

            var producto = new Producto
            {
                Nombre = createProductoDto.Nombre,
                Description = createProductoDto.Description,
                Marca = createProductoDto.Marca,
                Color = createProductoDto.Color,
                Precio = createProductoDto.Precio,
                PrecioDescuento = createProductoDto.PrecioDescuento,
                Stock = createProductoDto.Stock,
                Talla = createProductoDto.Talla,
                Code = createProductoDto.Code,
                imagen = imagenPath,
                imagen2 = imagen2Path,
                imagen3 = imagen3Path,
                Categoria = createProductoDto.Categoria,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return MapToDto(producto);
        }

        // Método auxiliar para guardar archivos y devolver la ruta
        private async Task<string?> SaveFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var filePath = Path.Combine("wwwroot/uploads", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
        //Service/ProductoService.cs
        public async Task<ProductoDto?> UpdateProductoAsync(int id, UpdateProductoDto updateProductoDto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return null;

            // Verificar si el código ya existe (excluyendo el producto actual)
            if (!string.IsNullOrEmpty(updateProductoDto.Code) &&
                await CodeExistsAsync(updateProductoDto.Code, id))
            {
                throw new InvalidOperationException("Ya existe un producto con este código");
            }

            // Actualizar solo los campos que no son nulos
            if (!string.IsNullOrEmpty(updateProductoDto.Nombre))
                producto.Nombre = updateProductoDto.Nombre;

            if (updateProductoDto.Description != null)
                producto.Description = updateProductoDto.Description;

            if (!string.IsNullOrEmpty(updateProductoDto.Marca))
                producto.Marca = updateProductoDto.Marca;

            if (!string.IsNullOrEmpty(updateProductoDto.Color))
                producto.Color = updateProductoDto.Color;

            if (updateProductoDto.Precio.HasValue)
                producto.Precio = updateProductoDto.Precio.Value;

            if (updateProductoDto.PrecioDescuento.HasValue)
                producto.PrecioDescuento = updateProductoDto.PrecioDescuento;

            if (updateProductoDto.Stock.HasValue)
                producto.Stock = updateProductoDto.Stock.Value;

            if (!string.IsNullOrEmpty(updateProductoDto.Talla))
                producto.Talla = updateProductoDto.Talla;

            if (!string.IsNullOrEmpty(updateProductoDto.Code))
                producto.Code = updateProductoDto.Code;

            if (updateProductoDto.Categoria != null)
                producto.Categoria = updateProductoDto.Categoria;

            // Actualizar rutas de imágenes si se proporcionaron
            if (!string.IsNullOrEmpty(updateProductoDto.imagenPath))
            {
                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(producto.imagen))
                {
                    DeleteOldImage(producto.imagen);
                }
                producto.imagen = updateProductoDto.imagenPath;
            }

            if (!string.IsNullOrEmpty(updateProductoDto.imagen2Path))
            {
                if (!string.IsNullOrEmpty(producto.imagen2))
                {
                    DeleteOldImage(producto.imagen2);
                }
                producto.imagen2 = updateProductoDto.imagen2Path;
            }

            if (!string.IsNullOrEmpty(updateProductoDto.imagen3Path))
            {
                if (!string.IsNullOrEmpty(producto.imagen3))
                {
                    DeleteOldImage(producto.imagen3);
                }
                producto.imagen3 = updateProductoDto.imagen3Path;
            }

            producto.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return MapToDto(producto);
        }

        private void DeleteOldImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            try
            {
                // Normaliza la ruta, reemplazando separadores si vienen de la web
                var relativePath = imagePath.Replace('/', Path.DirectorySeparatorChar)
                                            .Replace('\\', Path.DirectorySeparatorChar);

                // Si ya incluye 'uploads/', no lo repitas
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Console.WriteLine($"Imagen eliminada: {fullPath}");
                }
                else
                {
                    Console.WriteLine($"No se encontró la imagen para eliminar: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
            }
        }


        public async Task<bool> DeleteProductoAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return false;

            // Opcional: eliminar imágenes asociadas si existen
            DeleteOldImage(producto.imagen);
            DeleteOldImage(producto.imagen2);
            DeleteOldImage(producto.imagen3);

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }




        public async Task<bool> ProductoExistsAsync(int id)
        {
            return await _context.Productos.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Productos.Where(p => p.Code == code);

            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        private static ProductoDto MapToDto(Producto producto)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Description = producto.Description,
                Marca = producto.Marca,
                Color = producto.Color,
                Precio = producto.Precio,
                PrecioDescuento = producto.PrecioDescuento,
                Stock = producto.Stock,
                Talla = producto.Talla,
                Code = producto.Code,
                imagen = producto.imagen,
                imagen2 = producto.imagen2,
                imagen3 = producto.imagen3,
                Categoria = producto.Categoria,
                CreatedAt = producto.CreatedAt,
                UpdatedAt = producto.UpdatedAt
            };
        }
    }
}