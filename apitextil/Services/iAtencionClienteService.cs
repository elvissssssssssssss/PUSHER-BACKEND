
// ============================================
// 3. Services/iAtencionClienteService.cs
// ============================================
using apitextil.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace apitextil.Services
{
    public interface IAtencionClienteService
    {
        Task<AtencionClienteResponseDto> CrearMensajeContacto(AtencionClienteDto dto);
        Task<List<AtencionClienteResponseDto>> ObtenerTodosMensajes();
        Task<AtencionClienteResponseDto> ObtenerMensajePorId(int id);
        Task<bool> ActualizarEstado(int id, string nuevoEstado);
        Task<List<AtencionClienteResponseDto>> ObtenerMensajesPorEstado(string estado);
    }
}
