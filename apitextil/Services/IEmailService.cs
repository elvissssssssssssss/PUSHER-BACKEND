namespace apitextil.Services
{
    public interface IEmailService
    {
        Task EnviarEmailVentaExitosaAsync(string emailDestino, string nombreCliente,
            decimal montoTotal, string numeroVenta);
    }
}
