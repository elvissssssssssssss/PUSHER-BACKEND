using apitextil.DTOs;
using apitextil.Models;

namespace apitextil.Services
{
    public interface iRoleService
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(int id);
        Task<Rol> CreateAsync(CreateRolDto dto, int creadoPor);
        Task<bool> UpdateAsync(int id, UpdateRolDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
