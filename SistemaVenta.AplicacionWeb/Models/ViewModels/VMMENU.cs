namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMMENU
    {
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public string? Controlador { get; set; }
        public string? PaginaAccion { get; set; }
        public virtual ICollection<VMMENU>? SubMenus { get; set; }
    }
}
