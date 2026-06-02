using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;



namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService  //implementa IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio; 

        public FireBaseService(IGenericRepository<Configuracion> repositorio) //inyeccion de dependencia 
        {
            _repositorio = repositorio;
        }


        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage")); //busca las configuraciones en la tabla Configuracion donde el Recurso sea FireBase_Storage

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor); //convierte el resultado de la consulta en un diccionario para acceder facilmente a las propiedades y valores

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"])); //crea una instancia de autenticacion de Firebase con la api_key obtenida del diccionario
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]); //realiza la autenticacion con el email y clave obtenidos del diccionario 

                var cancellation = new CancellationTokenSource(); //permite cancelar la operacion si es necesario

                var task = new FirebaseStorage( //crea una instancia de FirebaseStorage para interactuar con el almacenamiento de Firebase
                    Config["ruta"], //ruta del almacenamiento obtenida del diccionario
                    new FirebaseStorageOptions //opciones de configuracion para FirebaseStorage
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken), //proporciona el token de autenticacion necesario para acceder al almacenamiento
                        ThrowOnCancel = true //indica que se debe lanzar una excepcion si la operacion es cancelada
                    })
                    .Child(Config[CarpetaDestino]) //navega a la carpeta destino dentro del almacenamiento
                    .Child(NombreArchivo) //navega al archivo especifico dentro de la carpeta
                    .PutAsync(StreamArchivo, cancellation.Token);       //sube el archivo utilizando el stream proporcionado y el token de cancelacion
                UrlImagen = await task; //espera a que la tarea de subida se complete y obtiene la URL del archivo subido

            } 
            catch (Exception ex) 
            {
                UrlImagen = ""; //en caso de error, retorna una cadena vacia
            }

            return UrlImagen;  //retorna la URL del archivo subido o una cadena vacia en caso de error
        }

        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .DeleteAsync(); //elimina el archivo especificado en la ruta dada
                await task;     //espera a que la tarea de eliminacion se complete

                return true;//retorna true si la eliminacion fue exitosa
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
