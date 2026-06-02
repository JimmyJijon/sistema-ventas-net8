using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Implementacion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class //clase generica que servirá para hacer operaciones en la bdd con cualquier entidad (clase) sin necesidad de crear un repository para cada una
    {

        private readonly DbventaContext _dbContext; 

        public GenericRepository(DbventaContext dbContext)
        {
            _dbContext = dbContext; //inyección de dependencia
        }

        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro) //Metodo que retorna una entidad (clase que representa una tabla de la base de datos), y que funciona con un filtro
        {                                                                          //Expression indica que le pasaré un script de consulta ej: u => u.Nombre == "Jimmy" y EF lo interpretara internamente a sql para consulta la bdd
            try
            {
                TEntity entidad = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filtro); //accede a la tabla con .Set<TEntity> y con .FirstOrDefaultAsync consigue el primer registro que coincida con el filtro
                return entidad; //retorna el resultado en caso de obtener un registro (True)
            }
            catch
            {
                throw; //en caso de ser null, envia la excepcion al usuario que llamó al método 
            }
        }

        public async Task<TEntity> Crear(TEntity entidad) //le paso el objeto que deseo guardar
        {
            try
            {
                _dbContext.Set<TEntity>().Add(entidad); //.Set obtengo la tabla en cuestion, y con .Add marca y prepara el objeto que voy a guardar en la base de datos
                await _dbContext.SaveChangesAsync(); //realiza la operacion guardar en la base de datos
                return entidad; //retorno el objeto guardado
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _dbContext.Update(entidad);  //utilizamos el metodo Update() y le pasamos el parametro entidad
                await _dbContext.SaveChangesAsync(); //guardamos cambios en la bdd
                return true; //retornamos true si la operacion tuvo éxito
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _dbContext.Remove(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            try
            {
                IQueryable<TEntity> queryEntidad = filtro == null ? _dbContext.Set<TEntity>() : _dbContext.Set<TEntity>().Where(filtro);
                return queryEntidad;
            } catch
            {
                throw;
            }

        }

    }
}
