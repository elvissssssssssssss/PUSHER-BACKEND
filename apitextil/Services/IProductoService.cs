namespace apitextil.Services
{
    using global::apitextil.DTOs;
    // Services/IProductoService.cs
    using global::apitextil.DTOs.apitextil.DTOs;

    namespace apitextil.Services
    {
        public interface IProductoService
        {
            Task<IEnumerable<ProductoDto>> GetAllProductosAsync();
            Task<ProductoDto?> GetProductoByIdAsync(int id);
            Task<ProductoDto?> GetProductoByCodeAsync(string code);
            Task<IEnumerable<ProductoDto>> GetProductosByCategoriaAsync(string categoria);

            Task<IEnumerable<ProductoDto>> GetProductosByMarcaAsync(string marca);
            Task<IEnumerable<ProductoDto>> SearchProductosAsync(string searchTerm);
            Task<ProductoDto> CreateProductoAsync(CreateProductoDto createProductoDto);
            Task<ProductoDto?> UpdateProductoAsync(int id, UpdateProductoDto updateProductoDto);
            Task<bool> DeleteProductoAsync(int id);
            Task<bool> ProductoExistsAsync(int id);
            Task<bool> CodeExistsAsync(string code, int? excludeId = null);
           
        }
    }
}
