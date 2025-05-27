using ManejoPresupuesto.Models;
using ManejoPresupuesto.Repository;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers;

public class CuentasController : Controller
{
    private readonly ITiposCuentasService _tiposCuentasService;
    private readonly ITiposCuentasRepository _tiposCuentasRepository;
        
    public CuentasController(ITiposCuentasService tiposCuentasService, ITiposCuentasRepository tiposCuentasRepository)
    {
        _tiposCuentasService = tiposCuentasService;
        _tiposCuentasRepository = tiposCuentasRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        int usuarioId = _tiposCuentasService.ObtenerUsuarioId();
        IEnumerable<TipoCuenta> tiposCuentas = await _tiposCuentasRepository.Obtener(usuarioId);
        CuentaCreacionViewModel cuentaViewModel = new CuentaCreacionViewModel();
        cuentaViewModel.TiposCuentas = tiposCuentas.Select(tc => new SelectListItem
        {
            Value = tc.Id.ToString(),
            Text  = tc.Nombre
        });
        return View(cuentaViewModel); 
    }
    
}