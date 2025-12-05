using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace apitextil.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        public async Task EnviarEmailVentaExitosaAsync(string emailDestino, string nombreCliente,
            decimal montoTotal, string numeroVenta)
        {
            try
            {
                // ✨ Convertir logo a Base64 para mayor confiabilidad
                string logoBase64 = "";
                try
                {
                    var logoPath = Path.Combine(_environment.WebRootPath, "uploads", "logo.png");
                    if (File.Exists(logoPath))
                    {
                        var logoBytes = await File.ReadAllBytesAsync(logoPath);
                        logoBase64 = Convert.ToBase64String(logoBytes);
                    }
                }
                catch (Exception logoEx)
                {
                    _logger.LogWarning($"No se pudo cargar el logo: {logoEx.Message}");
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Smtp:FromName"],
                    _configuration["Smtp:FromEmail"]
                ));
                message.To.Add(new MailboxAddress(nombreCliente, emailDestino));
                message.Subject = $"✅ Pago Exitoso - Pedido #{numeroVenta}";

                // ✨ Usar logo Base64 si está disponible, sino usar URL de fallback
                string logoSrc = !string.IsNullOrEmpty(logoBase64)
                    ? $"data:image/png;base64,{logoBase64}"
                    : "https://pusher-backend-elvis.onrender.com/uploads/logo.png";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
                    
                    <!-- Logo y Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%); padding: 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <img src='https://pusher-backend-elvis.onrender.com/logo.png'
 alt='NTX Logo' style='height: 60px; margin-bottom: 15px;' />
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>
                                ✓ ¡Pago Exitoso!
                            </h1>
                            <p style='color: #fee; margin: 10px 0 0 0; font-size: 14px;'>
                                Tu pedido ha sido confirmado
                            </p>
                        </td>
                    </tr>

                    <!-- Contenido Principal -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #1f2937; margin: 0 0 20px 0; font-size: 22px;'>
                                Hola {nombreCliente},
                            </h2>
                            <p style='color: #4b5563; line-height: 1.6; margin: 0 0 25px 0; font-size: 16px;'>
                                ¡Gracias por tu compra! Tu pago ha sido procesado exitosamente y tu pedido está siendo preparado.
                            </p>

                            <!-- Información del Pedido -->
                            <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f9fafb; border-radius: 6px; border: 1px solid #e5e7eb; margin-bottom: 25px;'>
                                <tr>
                                    <td style='padding: 20px;'>
                                        <table width='100%' cellpadding='8' cellspacing='0' border='0'>
                                            <tr>
                                                <td style='color: #6b7280; font-size: 14px; border-bottom: 1px solid #e5e7eb;'>
                                                    <strong>Número de Pedido:</strong>
                                                </td>
                                                <td style='color: #1f2937; font-size: 16px; font-weight: bold; text-align: right; border-bottom: 1px solid #e5e7eb;'>
                                                    #{numeroVenta}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='color: #6b7280; font-size: 14px; border-bottom: 1px solid #e5e7eb; padding-top: 12px;'>
                                                    <strong>Fecha:</strong>
                                                </td>
                                                <td style='color: #1f2937; font-size: 14px; text-align: right; border-bottom: 1px solid #e5e7eb; padding-top: 12px;'>
                                                    {DateTime.Now:dd/MM/yyyy HH:mm}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='color: #6b7280; font-size: 14px; padding-top: 12px;'>
                                                    <strong>Monto Total:</strong>
                                                </td>
                                                <td style='color: #dc2626; font-size: 22px; font-weight: bold; text-align: right; padding-top: 12px;'>
                                                    S/ {montoTotal:F2}
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                            <!-- Mensaje adicional con icono -->
                         
                            <p style='color: #4b5563; line-height: 1.6; margin: 0; font-size: 15px;'>
                                Si tienes alguna pregunta, no dudes en contactarnos.
                            </p>
                            <p style='color: #4b5563; line-height: 1.6; margin: 15px 0 0 0; font-size: 15px;'>
                                Gracias por confiar en <strong style='color: #dc2626;'>NTX-SAC</strong>.
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f9fafb; padding: 25px 30px; border-top: 1px solid #e5e7eb; border-radius: 0 0 8px 8px;'>
                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td align='center'>
                                        <p style='color: #9ca3af; font-size: 12px; margin: 0 0 10px 0;'>
                                            Este es un correo automático, por favor no respondas a este mensaje.
                                        </p>
                                        <p style='color: #6b7280; font-size: 13px; margin: 0;'>
                                            © 2025 <strong style='color: #dc2626;'>NTX-SAC</strong> - Tienda Textil Premium
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        _configuration["Smtp:Host"],
                        int.Parse(_configuration["Smtp:Port"]),
                        SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(
                        _configuration["Smtp:User"],
                        _configuration["Smtp:Password"]
                    );

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email enviado exitosamente a {emailDestino}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
                throw;
            }
        }
    }
}
