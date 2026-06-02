using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class RolService : IRolService  //clase que obtiene la lista de roles
    {

        private readonly IGenericRepository<Rol> _repositorio; //repositorio generico para acceder a los datos de la entidad Rol

        public RolService(IGenericRepository<Rol> repositorio) //inyeccion de dependencias del repositorio generico
        {
            _repositorio = repositorio;
        }

        public async Task<List<Rol>> Lista() //devuelve la lista de roles de forma asincrona
        {
            IQueryable<Rol> query = await _repositorio.Consultar();

            return query.ToList();
        }
    }
}
