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

                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(alias, correoOrigen));
                mensaje.To.Add(new MailboxAddress(string.Empty, CorreoDestino));
                mensaje.Subject = Asunto;
                mensaje.Body = new TextPart("html") { Text = Mensaje };

                using var cliente = new SmtpClient();
                cliente.LocalDomain = "[127.0.0.1]";

                // Auto detecta SSL/STARTTLS según el puerto (465=SSL, 587=STARTTLS, 1025=plain)
                await cliente.ConnectAsync(host, puerto, SecureSocketOptions.Auto);

                if (!string.IsNullOrWhiteSpace(clave) && cliente.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                {
                    // Eliminar espacios. Google muestra la contraseña con espacios (ej: "abcd efgh ijkl mnop")
                    // pero MailKit requiere que se envíe sin espacios para que Gmail la acepte.
                    string claveLimpia = clave.Replace(" ", "");
                    await cliente.AuthenticateAsync(correoOrigen, claveLimpia);
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
