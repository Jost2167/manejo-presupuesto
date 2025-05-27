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
    private readonly ICuentasRepository _cuentasRepository; 
        
    public CuentasController(
        ITiposCuentasService tiposCuentasService,
        ITiposCuentasRepository tiposCuentasRepository,
        ICuentasRepository cuentasRepository)
    {
        _tiposCuentasService = tiposCuentasService;
        _tiposCuentasRepository = tiposCuentasRepository;
        _cuentasRepository = cuentasRepository; 
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        int usuarioId = _tiposCuentasService.ObtenerUsuarioId();
        CuentaCreacionViewModel cuentaVM = new CuentaCreacionViewModel();
        cuentaVM.TiposCuentas = await ObtenerTiposCuentas(usuarioId); 
        return View(cuentaVM); 
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CuentaCreacionViewModel cuentaVM)
    {
        int usuarioId = _tiposCuentasService.ObtenerUsuarioId();
        
        // Se valida si el tipo cuenta seleccionado pertenece a ese usuario
        TipoCuenta tipoCuenta = await _tiposCuentasRepository.ObtenerPorId(cuentaVM.TipoCuentaId, usuarioId);

        if (tipoCuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        if (!ModelState.IsValid)
        {
            // Cargar los TiposCuentas convertidos a SelectListItem para presentar de nuevo a la vista
            cuentaVM.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View();
        }
        
        await _cuentasRepository.Crear(cuentaVM);

        return RedirectToAction(nameof(Index));
    }
    
    // Metodo para obtener los SelectListItem de TipoCuenta
    public async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
    {
       IEnumerable<TipoCuenta> tiposCuentas = await _tiposCuentasRepository.Obtener(usuarioId);
       return tiposCuentas.Select(tc => new SelectListItem()
       {
            Value = tc.Id.ToString(),
            Text = tc.Nombre
       });
    }
    
    
    
    
}