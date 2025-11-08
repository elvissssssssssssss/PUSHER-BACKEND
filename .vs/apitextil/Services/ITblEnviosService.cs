
// Services/ITblEnviosService.cs
using apitextil.Data;
using apitextil.DTOs;
using apitextil.Models;

namespace apitextil.Services
{
    public interface ITblEnviosService
    {
        Task<TblEnviosResponseDto> CreateAsync(TblEnviosCreateDto dto);
        Task<TblEnviosResponseDto?> GetByIdAsync(int id);
        Task<TblEnviosResponseDto?> GetByUserIdAsync(int userId);
        Task<IEnumerable<TblEnviosResponseDto>> GetAllAsync();
        Task<TblEnviosResponseDto?> UpdateAsync(int id, TblEnviosUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByDniAsync(string dni);
        Task<bool> ExistsByUserIdAsync(int userId);
        Task<IEnumerable<TblEnviosResponseDto>> GetByRegionAsync(string region);
        Task<IEnumerable<TblEnviosResponseDto>> GetByProvinciaAsync(string provincia);
    }
}
