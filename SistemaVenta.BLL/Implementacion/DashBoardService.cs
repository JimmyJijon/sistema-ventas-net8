using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class DashBoardService : IDashBoardService //  Logica de Dashboards
    {
        private readonly IVentaRepository _repositorioVenta;
        private readonly IGenericRepository<DetalleVenta> _repositorioDetalleVenta;
        private readonly IGenericRepository<Categoria> _repositorioCategoria;
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private DateTime fechaInicio = DateTime.Now;

        public DashBoardService(IVentaRepository repositorioVenta, IGenericRepository<DetalleVenta> repositorioDetalleVenta, IGenericRepository<Categoria> repositorioCategoria, IGenericRepository<Producto> repositorioProducto)
        { 
            _repositorioVenta = repositorioVenta;
            _repositorioDetalleVenta = repositorioDetalleVenta;
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;
            fechaInicio = fechaInicio.AddDays(-7);
        }
        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch 
            {
              throw;
            }
        }

        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);
                decimal resultado = query
                    .Select(v => v.Total)
                    .Sum(v => v.Value);
                return Convert.ToString(resultado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _repositorioProducto.Consultar();
                int totalProductos = query.Count();
                return totalProductos;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalCategorias()
        {
            try
            {
                IQueryable <Categoria> query = await _repositorioCategoria.Consultar();
                int TotalCategorias = query.Count();
                return TotalCategorias;
            }
            catch
            {
                throw;
            }
        }


        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date); //Se obtiene las ventas de hace una semana en adelante

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key) //Se agrupa por fecha de registro, y se ordena de mayor a menor por fecha de registro
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() }) // Se usa un objeto anonimo y se mapea el parametro fecha y conteo de ventas de esa fecha
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total); //Se asigna la fecha como clave y el total de ventas como valor

                return resultado; //Se devuelve un diccionario de datos
            }
            catch 
            {
                throw;            
            }

        }

        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _repositorioDetalleVenta.Consultar();

                Dictionary<string, int> resultado = query
                    .Include(v => v.IdVentaNavigation) //Incluye la data de su venta, la cual servira para filtrar por la fecha de registro de la venta 
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date) // fechas mayores a una semana atras
                    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count()) //agrupar por el nombre del producto, y cuántas veces fue vendido
                    .Select(dv => new { producto = dv.Key, total = dv.Count()}).Take(4) // Crear un objeto anonimo con las propiedades producto y total
                    .ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total); //asignar clave y valor
                
                return resultado;
            }
            catch
            {
                throw;
            }
        }
    }
}
