using PusherServer;
using Microsoft.Extensions.Configuration;

namespace apitextil.Services
{
    public interface INotificationService
    {
        Task EnviarNotificacionEstadoEnvio(int userId, int ventaId, string estadoNombre, string ubicacionActual, string observaciones);
    }

    public class NotificationService : INotificationService
    {
        private readonly Pusher _pusher;

        public NotificationService(IConfiguration configuration)
        {
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

        public async Task EnviarNotificacionEstadoEnvio(
            int userId,
            int ventaId,
            string estadoNombre,
            string ubicacionActual,
            string observaciones)
        {
            var data = new
            {
                venta_id = ventaId,
                estado = estadoNombre,
                ubicacion = ubicacionActual,
                observaciones = observaciones,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                tipo = "actualizacion_envio"
            };

            // Canal privado por usuario
            await _pusher.TriggerAsync(
                $"private-user-{userId}",
                "envio-actualizado",
                data
            );
        }
    }
}
