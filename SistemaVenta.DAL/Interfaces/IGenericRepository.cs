using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class //interfaz que permitirá que otras clases puedan utilizar los metodos Obtener, Crear, Editar, Eliminar y Consultar
                                                                       //mas escalable, reutilizacion de codigo, mantenimiento a largo plazo
    {
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro);
        Task<TEntity> Crear(TEntity entidad);
        Task<bool> Editar(TEntity entidad);
        Task<bool> Eliminar(TEntity entidad);
        Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null); // por defecto es nulo 
    }
}
