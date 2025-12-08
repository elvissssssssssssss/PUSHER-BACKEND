// Services/EmailService.cs
using apitextil.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace apitextil.Services
{
    public interface IEmailService
    {
        Task EnviarCorreoPagoConfirmado(int ventaId);
        Task EnviarEmailVentaExitosaAsync(string emailDestino, string nombreCliente,
            decimal montoTotal, string numeroVenta, string enlacePdf = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EcommerceContext _context; // ⬅️ nuevo

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger,
            IHttpClientFactory httpClientFactory,
        EcommerceContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        public async Task EnviarCorreoPagoConfirmado(int ventaId)
        {
            var venta = await _context.Ventas
                .Include(v => v.User)               // FK a tblusuarios
                .FirstOrDefaultAsync(v => v.Id == ventaId);

            if (venta == null)
                return;

            var email = venta.User?.email ?? string.Empty;
            var nombre = $"{venta.User?.nombre} {venta.User?.apellido}".Trim();

            if (string.IsNullOrWhiteSpace(email))
                return;

            await EnviarEmailVentaExitosaAsync(
                email,
                string.IsNullOrWhiteSpace(nombre) ? "Cliente" : nombre,
                venta.Total,
                venta.Id.ToString(),
                enlacePdf: null
            );
        }



        public async Task EnviarEmailVentaExitosaAsync(
            string emailDestino,
            string nombreCliente,
            decimal montoTotal,
            string numeroVenta,
            string enlacePdf = null)
        {
            try
            {
                Console.WriteLine($"📧 [RESEND] Enviando a: {emailDestino}");

                var apiKey = _configuration["RESEND_API_KEY"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception("RESEND_API_KEY no configurada");
                }

                Console.WriteLine($"🔑 API Key configurada");

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var emailData = new
                {
                    from = "NTX-SAC <onboarding@resend.dev>",
                    to = new[] { emailDestino },
                    subject = $"✅ Pago Exitoso - Pedido #{numeroVenta}",
                    html = $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Confirmación de Pedido</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Arial, sans-serif;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f4f4; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.08); overflow: hidden;'>
                    
                    <!-- Header con gradiente -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0 0 10px 0; font-size: 32px; font-weight: 700; letter-spacing: -0.5px;'>
                                ✓ ¡Pago Exitoso!
                            </h1>
                            <p style='color: rgba(255,255,255,0.95); margin: 0; font-size: 16px;'>
                                Tu pedido #{numeroVenta} ha sido confirmado
                            </p>
                        </td>
                    </tr>

                    <!-- Contenido principal -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #1f2937; margin: 0 0 20px 0; font-size: 24px; font-weight: 600;'>
                                Hola {nombreCliente},
                            </h2>
                            <p style='color: #4b5563; line-height: 1.7; margin: 0 0 30px 0; font-size: 16px;'>
                                ¡Gracias por tu compra en <strong style='color: #dc2626;'>NTX-SAC</strong>! Tu pago ha sido procesado exitosamente y tu pedido está siendo preparado para envío.
                            </p>

                            <!-- Card de información del pedido -->
                            <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background: linear-gradient(to bottom right, #fef2f2, #fef3f2); border-radius: 10px; border: 1px solid #fee2e2; margin-bottom: 30px; overflow: hidden;'>
                                <tr>
                                    <td style='padding: 25px;'>
                                        <table width='100%' cellpadding='12' cellspacing='0' border='0'>
                                            <tr>
                                                <td style='color: #6b7280; font-size: 15px; padding: 8px 0; border-bottom: 1px solid #fee2e2;'>
                                                    <strong>📦 Número de Pedido</strong>
                                                </td>
                                                <td style='color: #1f2937; font-size: 16px; font-weight: 600; text-align: right; padding: 8px 0; border-bottom: 1px solid #fee2e2;'>
                                                    #{numeroVenta}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='color: #6b7280; font-size: 15px; padding: 8px 0; border-bottom: 1px solid #fee2e2;'>
                                                    <strong>📅 Fecha de Compra</strong>
                                                </td>
                                                <td style='color: #1f2937; font-size: 15px; text-align: right; padding: 8px 0; border-bottom: 1px solid #fee2e2;'>
                                                    {DateTime.Now:dd/MM/yyyy HH:mm}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='color: #6b7280; font-size: 15px; padding: 12px 0 0 0;'>
                                                    <strong>💰 Monto Total</strong>
                                                </td>
                                                <td style='color: #dc2626; font-size: 28px; font-weight: 700; text-align: right; padding: 12px 0 0 0;'>
                                                    S/ {montoTotal:F2}
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                            {(!string.IsNullOrEmpty(enlacePdf) ? $@"
                            <!-- BOTÓN DE DESCARGA DE BOLETA -->
                            <table width='100%' cellpadding='0' cellspacing='0' border='0' style='margin-bottom: 30px;'>
                                <tr>
                                    <td align='center'>
                                        <a href='{enlacePdf}' target='_blank' style='display: inline-block; background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%); color: #ffffff; text-decoration: none; padding: 16px 40px; border-radius: 8px; font-size: 16px; font-weight: 600; box-shadow: 0 4px 6px rgba(220, 38, 38, 0.3);'>
                                            📄 Descargar Boleta (PDF)
                                        </a>
                                    </td>
                                </tr>
                                <tr>
                                    <td align='center' style='padding-top: 12px;'>
                                        <p style='color: #6b7280; font-size: 13px; margin: 0;'>
                                            También puedes copiar este enlace: <br/>
                                            <a href='{enlacePdf}' style='color: #dc2626; word-break: break-all;'>{enlacePdf}</a>
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            " : "")}

                            <!-- Sección de qué sigue -->
                            <div style='background: #f9fafb; border-left: 4px solid #dc2626; padding: 20px; border-radius: 6px; margin-bottom: 25px;'>
                                <h3 style='color: #1f2937; margin: 0 0 10px 0; font-size: 18px;'>¿Qué sigue?</h3>
                                <ul style='color: #4b5563; margin: 0; padding-left: 20px; line-height: 1.8;'>
                                    <li>Recibirás un correo cuando tu pedido sea enviado</li>
                                    <li>El tiempo de entrega estimado es de 3-5 días hábiles</li>
                                    <li>Puedes rastrear tu pedido en nuestra web</li>
                                </ul>
                            </div>

                            <p style='color: #4b5563; line-height: 1.7; margin: 0; font-size: 15px;'>
                                Si tienes alguna pregunta, no dudes en contactarnos. Estamos aquí para ayudarte.
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background: linear-gradient(to right, #1f2937, #111827); padding: 30px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='color: #ffffff; font-size: 18px; font-weight: 600; margin: 0 0 15px 0;'>
                                <span style='color: #dc2626;'>NTX-SAC</span> Tienda Textil
                            </p>
                            <p style='color: #9ca3af; font-size: 13px; margin: 0 0 10px 0;'>
                                Este es un correo automático, por favor no respondas directamente.
                            </p>
                            <p style='color: #6b7280; font-size: 12px; margin: 0;'>
                                © 2025 NTX-SAC. Todos los derechos reservados.
                            </p>
                        </td>
                    </tr>

                </table>
                
                <!-- Texto adicional para evitar spam -->
                <p style='color: #9ca3af; font-size: 11px; text-align: center; margin: 20px 0 0 0; max-width: 500px;'>
                    Este correo fue enviado a {emailDestino} porque realizaste una compra en nuestra tienda.
                </p>
            </td>
        </tr>
    </table>
</body>
</html>"

                };

                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📨 Enviando email a Resend API...");

                var response = await client.PostAsync("https://api.resend.com/emails", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅✅✅ EMAIL ENVIADO EXITOSAMENTE!");
                    _logger.LogInformation($"Email enviado a {emailDestino}");
                }
                else
                {
                    Console.WriteLine($"❌ Error de Resend API: {responseBody}");
                    throw new Exception($"Error al enviar email: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                Console.WriteLine($"❌ Error completo: {ex.Message}");
                throw;
            }
        }
    }
}