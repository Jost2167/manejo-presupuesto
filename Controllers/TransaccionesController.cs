using ManejoPresupuesto.Models;
using ManejoPresupuesto.Repository;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers;

public class TransaccionesController: Controller
{
    private readonly ITiposCuentasService _usuarioService;
    private readonly ICuentasRepository _cuentasRepository;
    
    public TransaccionesController(
        ITiposCuentasService usuarioService, 
        ICuentasRepository cuentasRepository)
    {
        _usuarioService = usuarioService;
        _cuentasRepository = cuentasRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        TransaccionCreacionViewModel transaccionVM = new TransaccionCreacionViewModel();
        transaccionVM.Cuentas = await ObtenerCuentas();
        return View(transaccionVM);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(TransaccionCreacionViewModel transaccionVM)
    {
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IEnumerable<SelectListItem>> ObtenerCuentas()
    {
        int usuarioId = _usuarioService.ObtenerUsuarioId();
        IEnumerable<Cuenta> cuentas = await _cuentasRepository.Buscar(usuarioId);

        return cuentas.Select(c => new SelectListItem()
        {
            Value = c.Id.ToString(),
            Text = c.Nombre
        });
    }
    
}