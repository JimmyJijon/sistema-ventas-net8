
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;




namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuService menuService, IMapper mapper)
        {
            _menuService = menuService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
         {
            ClaimsPrincipal claimUser = HttpContext.User;

            List<VMMENU> listaMenus;

            if (claimUser.Identity.IsAuthenticated)
            {
                string idUsuario = claimUser.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault();

                listaMenus = _mapper.Map<List<VMMENU>>(await _menuService.ObtenerMenus(int.Parse(idUsuario)));
            }
            else
            {
                listaMenus = new List<VMMENU> { };
            }

            return View(listaMenus); 
        }
    }
}
