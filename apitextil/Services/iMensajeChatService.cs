using apitextil.Data;
using apitextil.DTOs;
using apitextil.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace apitextil.Services
{
    public interface IMensajeChatService
    {
        Task<MensajeChatResponseDto> GuardarMensaje(MensajeChatDto dto);
        Task<List<MensajeChatResponseDto>> ObtenerHistorialPorAtencion(int atencionId);
        Task<HistorialChatDto> ObtenerHistorialCompleto(int atencionId);
        Task<bool> MarcarMensajesComoLeidos(int atencionId, string emisorTipo);
        Task<HistorialChatDto?> ObtenerHistorialPorUsuarioAsync(int userId);
    
    }
}
