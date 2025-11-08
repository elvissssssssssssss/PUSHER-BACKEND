using apitextil.Models;
using apitextil.DTOs;



namespace apitextil.Services
{
    public interface IEnvioService
    {
        Task<List<EstadoEnvio>> GetEstadosEnvioAsync();
        Task<List<SeguimientoEnvioDto>> GetSeguimientoByVentaIdAsync(int ventaId);
        Task<bool> AddSeguimientoAsync(CreateSeguimientoEnvioDto seguimientoDto);
        Task<List<DocumentoEnvioDto>> GetDocumentosByVentaIdAsync(int ventaId);
        Task<bool> AddDocumentoAsync(CreateDocumentoEnvioDto documentoDto);
        Task<bool> ConfirmarEntregaAsync(int ventaId);

        Task<List<SeguimientoEnvioDto>> GetSeguimientosByUsuarioIdAsync(int usuarioId);


        Task<string> UploadDocumentoAsync(IFormFile archivo, string tipoDocumento);
    }
}
