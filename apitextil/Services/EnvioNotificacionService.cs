using PusherServer;
using Microsoft.Extensions.Configuration;
using apitextil.Data;
using Microsoft.EntityFrameworkCore;

namespace apitextil.Services
{
    public interface IEnvioNotificacionService
    {
        Task NotificarActualizacionEnvio(int ventaId, int estadoId, string ubicacionActual, string observaciones);
    }

    public class EnvioNotificacionService : IEnvioNotificacionService
    {
        private readonly Pusher _pusher;
        private readonly EcommerceContext _context;

        public EnvioNotificacionService(IConfiguration configuration, EcommerceContext context)
        {
            _context = context;

            var pusherOptions = new PusherOptions
            {
                Cluster = configuration["Pusher:Cluster"],
                Encrypted = true
            };

            _pusher = new Pusher(
                configuration["Pusher:AppId"],
                configuration["Pusher:Key"],
                configuration["Pusher:Secret"],
                pusherOptions
            );
        }

        public async Task NotificarActualizacionEnvio(
            int ventaId,
            int estadoId,
            string ubicacionActual,
            string observaciones)
        {
            try
            {
                // ✅ CORREGIDO: Usar el nombre correcto del DbSet
                var venta = await _context.Ventas  // ← Usa "Ventas" (con "s")
                    .FirstOrDefaultAsync(v => v.Id == ventaId);

                if (venta == null)
                {
                    Console.WriteLine($"⚠️ Venta {ventaId} no encontrada");
                    return;
                }

                var userId = venta.UserId;

                // ✅ CORREGIDO: Usar el nombre correcto del DbSet
                var estado = await _context.EstadoEnvios  // ← Usa "EstadoEnvios" (con "s")
                    .FirstOrDefaultAsync(e => e.id == estadoId);

                if (estado == null)
                {
                    Console.WriteLine($"⚠️ Estado {estadoId} no encontrado");
                    return;
                }

                // Preparar datos de la notificación
                var notificationData = new
                {
                    venta_id = ventaId,
                    estado_id = estadoId,
                    estado_nombre = estado.nombre,
                    estado_descripcion = estado.descripcion,
                    ubicacion_actual = ubicacionActual,
                    observaciones = observaciones,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tipo = "actualizacion_envio"
                };

                // Enviar notificación al canal privado del usuario
                await _pusher.TriggerAsync(
                    $"private-user-{userId}",
                    "envio-actualizado",
                    notificationData
                );

                Console.WriteLine($"✅ Notificación enviada al usuario {userId} para venta {ventaId} - Estado: {estado.nombre}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al enviar notificación: {ex.Message}");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
            }
        }
    }
}
