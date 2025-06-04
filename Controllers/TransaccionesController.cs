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
    private readonly ICategoriasRepository _categoriasRepository;
    
    public TransaccionesController(
        ITiposCuentasService usuarioService, 
        ICuentasRepository cuentasRepository,
        ICategoriasRepository categoriasRepository)
    {
        _usuarioService = usuarioService;
        _cuentasRepository = cuentasRepository;
        _categoriasRepository = categoriasRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        int usuarioId = _usuarioService.ObtenerUsuarioId();
        TransaccionCreacionViewModel transaccionVM = new TransaccionCreacionViewModel();
        transaccionVM.Cuentas = await ObtenerCuentas(usuarioId);
        transaccionVM.Categorias = await ObtenerCategorias(usuarioId, transaccionVM.TipoOperacionId);
        return View(transaccionVM);
    }

    [HttpPost]
    
    public async Task<IActionResult> Crear(TransaccionCreacionViewModel transaccionVM)
    {
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
    {
        int usuarioId = _usuarioService.ObtenerUsuarioId();
        IEnumerable<SelectListItem> categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
        return Ok(categorias);
    }

    public async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
    {
        IEnumerable<Categoria> categorias = await _categoriasRepository.Obtener(usuarioId, tipoOperacion);

        return categorias.Select(ct => new SelectListItem()
        {
            Value = ct.Id.ToString(),
            Text = ct.Nombre
        });
    }
        
    public async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
    {
        IEnumerable<Cuenta> cuentas = await _cuentasRepository.Buscar(usuarioId);

        return cuentas.Select(c => new SelectListItem()
        {
            Value = c.Id.ToString(),
            Text = c.Nombre
        });
    }
    
}