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
    private readonly ITransaccionRepository _transaccionRepository;
    
    public TransaccionesController(
        ITiposCuentasService usuarioService, 
        ICuentasRepository cuentasRepository,
        ICategoriasRepository categoriasRepository,
        ITransaccionRepository TransaccionRepository)
    {
        _usuarioService = usuarioService;
        _cuentasRepository = cuentasRepository;
        _categoriasRepository = categoriasRepository;
        _transaccionRepository = TransaccionRepository;
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
        int usuarioId = _usuarioService.ObtenerUsuarioId();

        if (!ModelState.IsValid)
        {
            TransaccionCreacionViewModel transaccionVMTemp = new TransaccionCreacionViewModel();
            transaccionVMTemp.Cuentas = await ObtenerCuentas(usuarioId);
            transaccionVMTemp.Categorias = await ObtenerCategorias(usuarioId, transaccionVM.TipoOperacionId);
            return View(transaccionVMTemp);
        }

        Categoria categoria = await _categoriasRepository.ObtenerPorId(transaccionVM.CategoriaId, usuarioId);
        Cuenta cuenta = await _cuentasRepository.ObtenerPorId(transaccionVM.CuentaId, usuarioId);

        if (categoria is null || cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        transaccionVM.UsuarioId = usuarioId;
        
        // Como en el formulario de transaccion, se tiene un campo de tipo de operacion y monto,
        // el monto que ingresa no debe de llevar el (-) indicando que es un egreso, sino, que esto
        // se representa a traves del tipo de operacion
        if (transaccionVM.TipoOperacionId == TipoOperacion.Egreso)
        {
            transaccionVM.Monto = transaccionVM.Monto * (-1);
        }
        
        await _transaccionRepository.Crear(transaccionVM); 
        
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