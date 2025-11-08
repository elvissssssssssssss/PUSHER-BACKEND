using apitextil.DTOs;
using apitextil.Services;
using apitextil.Services.apitextil.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apitextil.Data; // tu DbContext

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class EnviosController : ControllerBase
    {
        private readonly IEnvioService _envioService;
        private readonly EcommerceContext _context;
       

        public EnviosController(IEnvioService envioService, EcommerceContext context)
        {
            _envioService = envioService;
            _context = context;
        }


     
        [HttpGet("estados")]
        public async Task<IActionResult> GetEstadosEnvio()
        {
            try
            {
                var estados = await _envioService.GetEstadosEnvioAsync();
                return Ok(estados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener estados: {ex.Message}" });
            }
        }

        [HttpGet("seguimiento/{ventaId}")]
        public async Task<IActionResult> GetSeguimientoByVentaId(int ventaId)
        {
            try
            {
                var seguimiento = await _envioService.GetSeguimientoByVentaIdAsync(ventaId);
                return Ok(seguimiento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener seguimiento: {ex.Message}" });
            }
        }

        [HttpPost("seguimiento")]
      //  [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AddSeguimiento([FromBody] CreateSeguimientoEnvioDto seguimientoDto)
        {
            try
            {
                var result = await _envioService.AddSeguimientoAsync(seguimientoDto);
                if (result)
                    return Ok(new { message = "Seguimiento agregado correctamente" });
                else
                    return BadRequest(new { message = "Error al agregar seguimiento" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al agregar seguimiento: {ex.Message}" });
            }
        }

        [HttpGet("documentos/{ventaId}")]
        public async Task<IActionResult> GetDocumentosByVentaId(int ventaId)
        {
            try
            {
                var documentos = await _envioService.GetDocumentosByVentaIdAsync(ventaId);
                return Ok(documentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener documentos: {ex.Message}" });
            }
        }

        [HttpPost("documentos")]
       // [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AddDocumento([FromBody] CreateDocumentoEnvioDto documentoDto)
        {
            try
            {
                var result = await _envioService.AddDocumentoAsync(documentoDto);
                if (result)
                    return Ok(new { message = "Documento agregado correctamente" });
                else
                    return BadRequest(new { message = "Error al agregar documento" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al agregar documento: {ex.Message}" });
            }
        }

        [HttpPost("upload-documento")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UploadDocumento([FromForm] UploadDocumentoDto uploadDto)
        {
            try
            {
                var rutaArchivo = await _envioService.UploadDocumentoAsync(uploadDto.archivo, uploadDto.tipo_documento);

                var documentoDto = new CreateDocumentoEnvioDto
                {
                    venta_id = uploadDto.venta_id,
                    tipo_documento = uploadDto.tipo_documento,
                    nombre_archivo = uploadDto.archivo.FileName,
                    ruta_archivo = rutaArchivo
                };

                var result = await _envioService.AddDocumentoAsync(documentoDto);

                if (result)
                    return Ok(new { message = "Documento subido correctamente", ruta = rutaArchivo });
                else
                    return BadRequest(new { message = "Error al guardar documento en base de datos" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al subir documento: {ex.Message}" });
            }
        }

        
        [HttpPost("confirmar-entrega/{ventaId}")]
        public async Task<IActionResult> ConfirmarEntrega(int ventaId)
        {
            try
            {
                var result = await _envioService.ConfirmarEntregaAsync(ventaId);

                return result
                    ? Ok(new { message = "Entrega confirmada correctamente" })
                    : BadRequest(new { message = "No se pudo confirmar la entrega" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al confirmar entrega: {ex.Message}" });
            }
        }


        [HttpGet("mis-seguimientos")]
       
        public async Task<IActionResult> GetMisSeguimientos()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var envios = await _context.TblEnvios
                .Where(e => e.UserId == userId)
                .ToListAsync();

            return Ok(envios);
        }




    }
}