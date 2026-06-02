using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.Entity;
using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace SistemaVenta.AplicacionWeb.Utilidades.Automapper
{
    public class AutoMapperProfile : Profile // Clase que hereda de Profile de AutoMapper
    {
        public AutoMapperProfile() // Constructor de la clase AutoMapperProfile
        {   
            
            #region Rol
            CreateMap<Rol, VMRol>().ReverseMap(); //Sirve para mapear de Rol a VMRol y viceversa
            #endregion Rol

            #region Usuario
            CreateMap<Usuario, VMUsuario>() //Sirve para mapear de Usuario a VMUsuario
              .ForMember(destino =>
                 destino.EsActivo,
                 opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0) // Mapear el valor booleano a entero (1 o 0
              )
              .ForMember(destino =>
              destino.NombreRol,
              opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion) // Mapear el nombre del rol desde la entidad relacionada
              );

            CreateMap<VMUsuario, Usuario>()
                .ForMember(destino =>
                   destino.EsActivo,
                   opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(destino =>
                   destino.IdRolNavigation,
                   opt => opt.Ignore() // Ignorar la propiedad de navegación para evitar problemas al mapear
                );
            #endregion

            #region Negocio
            //CreateMap<Negocio, VMNegocio>() //Sirve para mapear de Negocio a VMNegocio
            //    .ForMember(destino =>
            //       destino.PorcentajeImpuesto,
            //       opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-EC"))) // Convertir decimal a string con formato de cultura es-EC
            //    );

            //CreateMap<VMNegocio, Negocio>()
            //    .ForMember(destino =>
            //    destino.PorcentajeImpuesto,
            //    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-EC")))
            //    );
            CreateMap<Negocio, VMNegocio>();
            CreateMap<VMNegocio, Negocio>();
            #endregion

            #region Categoria
            CreateMap<Categoria, VMCategoria>()
                .ForMember(destino=>
                   destino.esActivo,
                   opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)    // Mapear el valor booleano a entero (1 o 0
                );

            CreateMap<VMCategoria, Categoria>()
                .ForMember(destino =>
                   destino.EsActivo,
                   opt => opt.MapFrom(origen => origen.esActivo == 1 ? true : false) // Mapear el valor entero a booleano (true o false
                );
            #endregion

            #region Producto
            CreateMap<Producto, VMProducto>() //Sirve para mapear de Producto a VMProducto
                .ForMember(destino =>
                   destino.EsActivo,
                   opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0) // Mapear el valor booleano a entero (1 o 0
                )
                .ForMember(destino =>
                   destino.NombreCategoria,
                   opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion)
                );
            //.ForMember(destino =>
            //   destino.Precio,
            //   opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-EC"))) // Convertir decimal a string con formato de cultura es-EC
            //);

            CreateMap<VMProducto, Producto>()
                .ForMember(destino =>
                   destino.EsActivo,
                   opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false) // Mapear el valor entero a booleano (true o false
                )
                .ForMember(destino =>
                destino.IdCategoriaNavigation,
                opt => opt.Ignore()
                );
                //.ForMember(destino =>
                //destino.Precio,
                //opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-EC")))    // Convertir string a decimal con formato de cultura es-EC
                //);
            #endregion

            #region TipoDocumentoVenta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap(); //Sirve para mapear de TipoDocumentoVenta a VMTipoDocumentoVenta y viceversa
            #endregion

            #region Venta
            CreateMap<Venta, VMVenta>() //Sirve para mapear de Venta a VMVenta
               .ForMember(destino =>
                  destino.TipoDocumentoVenta,
                  opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion) // Mapear el nombre del tipo de documento desde la entidad relacionada
               )
               .ForMember(destino =>
                  destino.Usuario,
                  opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre)
               )
               //.ForMember(destino =>
               //   destino.SubTotal,
               //   opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-EC")))
               //)
               //.ForMember(destino =>
               //   destino.ImpuestoTotal,
               //   opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-EC")))
               //)
               //.ForMember(destino =>
               //    destino.Total,
               //    opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-EC")))
               //)
               .ForMember(destino =>
                   destino.FechaRegistro,
                   opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy"))
               );

            CreateMap<VMVenta, Venta>(); //Sirve para mapear de VMVenta a Venta
                                         //.ForMember(destino =>
                                         //    destino.SubTotal,
                                         //    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-EC")))
                                         //)
                                         //.ForMember(destino =>
                                         //destino.ImpuestoTotal,
                                         //opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-EC")))
                                         //)
                                         //.ForMember(destino =>
                                         //destino.Total,
                                         //opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-EC")))
                                         //);
            #endregion

            #region DetalleVenta
            CreateMap<DetalleVenta, VMDetalleVenta>(); //Sirve para mapear de DetalleVenta a VMDetalleVenta
                //.ForMember(destino =>
                //  destino.Precio,
                //  opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-EC")))
                //)
                //.ForMember(destino =>
                //   destino.Total,
                //   opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-EC")))
                //);
            CreateMap<VMDetalleVenta, DetalleVenta>();  
            //.ForMember(destino =>
            //   destino.Precio,
            //   opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-EC")))
            //)
            //.ForMember(destino =>
            //   destino.Total,
            //   opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-EC")))
            //);                                          
            CreateMap<DetalleVenta, VMReporteVenta>()
                .ForMember(destino =>
                    destino.FechaRegistro,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                )
                .ForMember(destino =>
                    destino.NumeroVenta,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta)
                )
                .ForMember(destino =>
                    destino.TipoDocumento,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion)
                )
                .ForMember(destino =>
                    destino.DocumentoCliente,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente)
                )
                .ForMember(destino =>
                    destino.NombreCliente,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation.NombreCliente)
                )
                // InvariantCulture: separador decimal estable (punto), alineado con JSON/JS; evita el lío coma/punto de es-EC.
                // (Sin ?. ni pattern matching: MapFrom se traduce a expression tree y no los admite.)
                .ForMember(destino =>
                    destino.SubTotalVenta,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation != null && origen.IdVentaNavigation.SubTotal.HasValue
                        ? Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, CultureInfo.InvariantCulture)
                        : null))
                .ForMember(destino =>
                    destino.ImpuestoTotalVenta,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation != null && origen.IdVentaNavigation.ImpuestoTotal.HasValue
                        ? Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, CultureInfo.InvariantCulture)
                        : null))
                .ForMember(destino =>
                    destino.TotalVenta,
                    opt => opt.MapFrom(origen => origen.IdVentaNavigation != null && origen.IdVentaNavigation.Total.HasValue
                        ? Convert.ToString(origen.IdVentaNavigation.Total.Value, CultureInfo.InvariantCulture)
                        : null))
                .ForMember(destino =>
                    destino.Producto,
                    opt => opt.MapFrom(origen => origen.DescripcionProducto)
                );
                //.ForMember(destino =>
                //    destino.Precio,
                //    opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-EC")))
                //)
                //.ForMember(destino =>
                //    destino.Total,
                //    opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-EC")))
                //);
            #endregion

            #region Menu

            CreateMap<Menu, VMMENU>() //Sirve para mapear de Menu a VMMENU
                .ForMember(destino =>
                   destino.SubMenus,
                   opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation) // Mapear los submenús desde la propiedad de navegación inversa
                );

            #endregion
        }
    }
}
