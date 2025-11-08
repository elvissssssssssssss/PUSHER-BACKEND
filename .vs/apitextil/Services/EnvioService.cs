using apitextil.Data;
using apitextil.DTOs;
using apitextil.Models;
using Microsoft.EntityFrameworkCore;


namespace apitextil.Services
{
    public class EnvioService : IEnvioService
    {
        private readonly EcommerceContext _context;
        private readonly IWebHostEnvironment _environment;

        public EnvioService(EcommerceContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<EstadoEnvio>> GetEstadosEnvioAsync()
        {
            return await _context.EstadoEnvios.ToListAsync();
        }

        public async Task<List<SeguimientoEnvioDto>> GetSeguimientoByVentaIdAsync(int ventaId)
        {
            return await _context.SeguimientoEnvios
                .Where(se => se.venta_id == ventaId)
                .Include(se => se.EstadoEnvio)
                .OrderByDescending(se => se.fecha_cambio)
                .Select(se => new SeguimientoEnvioDto
                {
                    id = se.id,
                    venta_id = se.venta_id,
                    estado_id = se.estado_id,
                    ubicacion_actual = se.ubicacion_actual,
                    observaciones = se.observaciones,
                    fecha_cambio = se.fecha_cambio,
                    confirmado_por_cliente = se.confirmado_por_cliente,
                    fecha_confirmacion = se.fecha_confirmacion,
                    estado_nombre = se.EstadoEnvio.nombre,
                    estado_descripcion = se.EstadoEnvio.descripcion
                })
                .ToListAsync();
        }

        public async Task<List<SeguimientoEnvioDto>> GetSeguimientosByUsuarioIdAsync(int usuarioId)
        {
            // Traer IDs de las ventas del usuario
            var ventasUsuario = await _context.Ventas
                .Where(v => v.UserId == usuarioId)
                .Select(v => v.Id)
                .ToListAsync();

            if (!ventasUsuario.Any())
                return new List<SeguimientoEnvioDto>();

            // Traer seguimientos de esas ventas
            return await _context.SeguimientoEnvios
                .Where(se => ventasUsuario.Contains(se.venta_id))
                .Include(se => se.EstadoEnvio)
                .OrderByDescending(se => se.fecha_cambio)
                .Select(se => new SeguimientoEnvioDto
                {
                    id = se.id,
                    venta_id = se.venta_id,
                    estado_id = se.estado_id,
                    ubicacion_actual = se.ubicacion_actual,
                    observaciones = se.observaciones,
                    fecha_cambio = se.fecha_cambio,
                    confirmado_por_cliente = se.confirmado_por_cliente,
                    fecha_confirmacion = se.fecha_confirmacion,
                    estado_nombre = se.EstadoEnvio.nombre,
                    estado_descripcion = se.EstadoEnvio.descripcion
                })
                .ToListAsync();
        }

        public async Task<bool> AddSeguimientoAsync(CreateSeguimientoEnvioDto seguimientoDto)
        {
            try
            {
                var seguimiento = new SeguimientoEnvio
                {
                    venta_id = seguimientoDto.venta_id,
                    estado_id = seguimientoDto.estado_id,
                    ubicacion_actual = seguimientoDto.ubicacion_actual,
                    observaciones = seguimientoDto.observaciones,
                    fecha_cambio = DateTime.Now,
                    confirmado_por_cliente = false,
                    creado_en = DateTime.Now,
                    actualizado_en = DateTime.Now
                };

                _context.SeguimientoEnvios.Add(seguimiento);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<DocumentoEnvioDto>> GetDocumentosByVentaIdAsync(int ventaId)
        {
            return await _context.DocumentoEnvios
                .Where(de => de.venta_id == ventaId)
                .OrderByDescending(de => de.fecha_subida)
                .Select(de => new DocumentoEnvioDto
                {
                    id = de.id,
                    venta_id = de.venta_id,
                    tipo_documento = de.tipo_documento,
                    nombre_archivo = de.nombre_archivo,
                    ruta_archivo = de.ruta_archivo,
                    fecha_subida = de.fecha_subida
                })
                .ToListAsync();
        }

        public async Task<bool> AddDocumentoAsync(CreateDocumentoEnvioDto documentoDto)
        {
            try
            {
                var documento = new DocumentoEnvio
                {
                    venta_id = documentoDto.venta_id,
                    tipo_documento = documentoDto.tipo_documento,
                    nombre_archivo = documentoDto.nombre_archivo,
                    ruta_archivo = documentoDto.ruta_archivo,
                    fecha_subida = DateTime.Now,
                    creado_en = DateTime.Now,
                    actualizado_en = DateTime.Now
                };

                _context.DocumentoEnvios.Add(documento);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ConfirmarEntregaAsync(int ventaId)
        {
            try
            {
                var ultimoSeguimiento = await _context.SeguimientoEnvios
                    .Where(se => se.venta_id == ventaId)
                    .OrderByDescending(se => se.fecha_cambio)
                    .FirstOrDefaultAsync();

                if (ultimoSeguimiento != null)
                {
                    ultimoSeguimiento.confirmado_por_cliente = true;
                    ultimoSeguimiento.fecha_confirmacion = DateTime.Now;
                    ultimoSeguimiento.actualizado_en = DateTime.Now;

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<string> UploadDocumentoAsync(IFormFile archivo, string tipoDocumento)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("Archivo no válido");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "envios");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{archivo.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/uploads/envios/{uniqueFileName}";
        }
    }
}