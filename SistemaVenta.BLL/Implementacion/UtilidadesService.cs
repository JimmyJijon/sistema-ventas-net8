using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using System.Security.Cryptography;


namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {

        public string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6); //genera un GUID, lo convierte a cadena sin guiones y toma los primeros 6 caracteres
            return clave;
        }

        public string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();

            using(SHA256 hash = SHA256Managed.Create())//using para asegurar la liberacion de recursos
            {
                Encoding enc = Encoding.UTF8; //definir la codificacion a utilizar
                byte[] result = hash.ComputeHash(enc.GetBytes(texto)); //calcula el hash de la cadena de texto convertida a bytes

                foreach (byte b in result)  //recorre cada byte del resultado
                {
                    sb.Append(b.ToString("x2")); //convierte el byte a una representacion hexadecimal y lo agrega al StringBuilder
                }
            }
            return sb.ToString(); //devuelve la representacion hexadecimal completa del hash
        }

    }
}
