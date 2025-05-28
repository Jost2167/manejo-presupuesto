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

    public async Task<IActionResult> Index()
    {
        int usuarioId = _tiposCuentasService.ObtenerUsuarioId();

        // Obtiene todas las cuentas asociadas al usuario actual
        IEnumerable<Cuenta> cuentasConTipoCuenta = await _cuentasRepository.Buscar(usuarioId);

        var modelo = cuentasConTipoCuenta
            // Agrupa las cuentas por el nombre del tipo de cuenta
            // Cada grupo generado tendrá una propiedad 'Key' (el nombre del tipo de cuenta)
            // y una colección de cuentas que pertenecen a ese tipo
            .GroupBy(x => x.TipoCuenta)
            .Select(grupo => new IndiceCuentasViewModel
            {
                TipoCuenta = grupo.Key,
                Cuentas = grupo
            }).ToList();

        return View(modelo);
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

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        int usuarioId = _tiposCuentasService.ObtenerUsuarioId();
        Cuenta cuenta = await _cuentasRepository.ObtenerPorId(id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }
        
        // Conversion de Cuenta a CuentaCreanViewModel
        CuentaCreacionViewModel cuentaVM = new CuentaCreacionViewModel()
        {
            Id = cuenta.Id,
            Nombre = cuenta.Nombre,
            Balance = cuenta.Balance,
            Descripcion = cuenta.Descripcion,
            TipoCuentaId = cuenta.TipoCuentaId
        };
        
        // Obtener SelectListItem de TiposCuentas
        IEnumerable<SelectListItem> tiposCuentas = await ObtenerTiposCuentas(usuarioId);
        cuentaVM.TiposCuentas = tiposCuentas;
        
        return View(cuentaVM);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaVM)
    {
        int usuarioId = _tiposCuentasService.ObtenerUsuarioId();
        Cuenta cuenta = await _cuentasRepository.ObtenerPorId(cuentaVM.Id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        TipoCuenta tipoCuenta = await _tiposCuentasRepository.ObtenerPorId(cuentaVM.TipoCuentaId, usuarioId);

        if (tipoCuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await _cuentasRepository.Actualizar(cuentaVM);
        
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