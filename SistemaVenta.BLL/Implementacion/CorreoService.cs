using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService //heredamos e implementamos la interface ICorreoService
    {

        private readonly IGenericRepository<Configuracion> _repositorio;

        public CorreoService(IGenericRepository<Configuracion> repositorio) //Inyeccion de dependencia
        {
            _repositorio = repositorio;
        }

        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo")); //busca registros de la tabla Configuracion donde Recurso sea igual a "Servicio_Correo"
                

                //convierte la lista de configuraciones en un diccionario clave-valor para acceder más facilmente
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                //crear un objeto credenciales para autenticarse en el servisor SMTP
                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                //Crear el mensaje del correo
                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                //agrega el destinatario (quien recibe el correo)
                correo.To.Add(new MailAddress(CorreoDestino));

                //leer si se debe usar SSL desde la BD (true para Gmail real, false para Maildev local)
                bool usarSsl = Config.ContainsKey("ssl") && Config["ssl"].Equals("true", StringComparison.OrdinalIgnoreCase);

                //configurar el cliente SMTP (Simple Mail Transfer Protocol)
                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = usarSsl  //controlado desde la base de datos
                };

                //envia correo
                clienteServidor.Send(correo);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
