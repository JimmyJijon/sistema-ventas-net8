using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Logging;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;
        private readonly ILogger<CorreoService> _logger;

        public CorreoService(IGenericRepository<Configuracion> repositorio, ILogger<CorreoService> logger)
        {
            _repositorio = repositorio;
            _logger = logger;
        }

        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                // Obtener configuración SMTP desde la base de datos
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
                Dictionary<string, string> Config = query.ToDictionary(
                    keySelector: c => c.Propiedad,
                    elementSelector: c => c.Valor
                );

                string correoOrigen = Config["correo"];
                string clave        = Config["clave"];
                string alias        = Config["alias"];
                string host         = Config["host"];
                int    puerto       = int.Parse(Config["puerto"]);
                bool   usarSsl      = Config.ContainsKey("ssl") && Config["ssl"].Equals("true", StringComparison.OrdinalIgnoreCase);

                // Construir el mensaje con MimeKit
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(alias, correoOrigen));
                mensaje.To.Add(new MailboxAddress(string.Empty, CorreoDestino));
                mensaje.Subject = Asunto;
                mensaje.Body = new TextPart("html") { Text = Mensaje };

                // Enviar con MailKit (reemplazo moderno de SmtpClient, con soporte SSL real en Linux)
                using var cliente = new SmtpClient();

                // Puerto 587 -> STARTTLS (negociación TLS después de conectar)
                // Puerto 465 -> SSL/TLS directo
                // Puerto 1025 (maildev) -> sin SSL
                SecureSocketOptions socketOption = usarSsl
                    ? (puerto == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                    : SecureSocketOptions.None;

                await cliente.ConnectAsync(host, puerto, socketOption);

                // Solo autenticar si hay credenciales (Maildev no requiere autenticación)
                if (!string.IsNullOrWhiteSpace(clave))
                {
                    await cliente.AuthenticateAsync(correoOrigen, clave);
                }

                await cliente.SendAsync(mensaje);
                await cliente.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {Destino}: {Mensaje}", CorreoDestino, ex.Message);
                return false;
            }
        }
    }
}
