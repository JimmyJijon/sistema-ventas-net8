using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;
        private readonly IMapper _mapper;
        public UsuarioController(IUsuarioService usuarioServicio, IRolService rolServicio, IMapper mapper) 
        {
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles() //lista de roles
        {
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista() //lista de usuarios
        {
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(await _usuarioServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista }); //formato datatable
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo) //crear usuario
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>(); //respuesta generica

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo); //deserializar el modelo de usuario

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N"); //generar nombre unico para la foto
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]"; //url para enviar correo con clave

                Usuario usuario_creado = await _usuarioServicio.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo); //crear usuario

                vmUsuario = _mapper.Map<VMUsuario>(usuario_creado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;

            }
            catch (Exception ex) 
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo) //editar usuario
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>(); //respuesta generica

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuario_editado = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto); //editar usuario
                vmUsuario = _mapper.Map<VMUsuario>(usuario_editado); //mapear a VMUsuario
                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;
            }
            catch(Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse); //retornar respuesta

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar (int IdUsuario)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>(); //respuesta generica

            try
            {
                genericResponse.Estado = await _usuarioServicio.Eliminar(IdUsuario); //eliminar usuario
            }
            catch(Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

    }
}
