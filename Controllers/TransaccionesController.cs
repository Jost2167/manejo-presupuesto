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

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        int usuarioId = _usuarioService.ObtenerUsuarioId();

        Transaccion transaccion =  await _transaccionRepository.ObtenerPorId(id, usuarioId);

        if (transaccion is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }
        
        // Mapeo manual de Transaccion a TransaccionActulizacionViewModel  
        TransaccionActulizacionViewModel transaccionActulizacionVM = new TransaccionActulizacionViewModel()
        {
            Id = transaccion.Id,
            Fecha = transaccion.Fecha,
            Monto = transaccion.Monto,
            Nota = transaccion.Nota,
            CategoriaId = transaccion.CategoriaId,
            CuentaId = transaccion.CuentaId,
            UsuarioId = transaccion.UsuarioId,
            TipoOperacionId = transaccion.TipoOperacionId,
        };

        // Como en la BD unicamente se esta guardando el monto en valor absoluto, cuando estemos realizando
        // procesos relacionados con el monto, se debe diferenciar si es un egreso para indicar su signo (-) 
        if (transaccionActulizacionVM.TipoOperacionId == TipoOperacion.Egreso)
        {
            transaccionActulizacionVM.MontoAnterior = transaccionActulizacionVM.Monto * (-1);
        }
        
        transaccionActulizacionVM.MontoAnterior = transaccionActulizacionVM.Monto;
        
        transaccionActulizacionVM.CuentaAnteriorId = transaccion.CuentaId;
        transaccionActulizacionVM.Cuentas = await ObtenerCuentas(transaccionActulizacionVM.UsuarioId);
        transaccionActulizacionVM.Categorias = await ObtenerCategorias(transaccionActulizacionVM.UsuarioId, transaccionActulizacionVM.TipoOperacionId);

        return View(transaccionActulizacionVM);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(TransaccionActulizacionViewModel transaccionActulizacionVM)
    {
        int usuarioId = _usuarioService.ObtenerUsuarioId();

        // Validar el modelo enviado
        if (!ModelState.IsValid)
        {
            transaccionActulizacionVM.Cuentas = await ObtenerCuentas(usuarioId);
            transaccionActulizacionVM.Categorias = await ObtenerCategorias(usuarioId, transaccionActulizacionVM.TipoOperacionId);
            return View(transaccionActulizacionVM);
        }
        
        // Validar si la transaccion pertenece al usuario
        Transaccion transaccion = await _transaccionRepository.ObtenerPorId(transaccionActulizacionVM.Id, usuarioId);
        if (transaccion is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }
        
        // Validar si la categoria pertenece al usuario
        Categoria categoria = await _categoriasRepository.ObtenerPorId(transaccionActulizacionVM.CategoriaId, usuarioId);
        if (categoria is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }
        
        // Validar si la cuenta enviada pertenece al usuario
        Cuenta cuenta = await _cuentasRepository.ObtenerPorId(transaccionActulizacionVM.CuentaId, usuarioId);
        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home"); 
        }
        
        // Mapear de TransaccionActulizacionViewModel a Transaccion
        MapearTransaccionDesdeTransaccionActualizacionVM(transaccion, transaccionActulizacionVM);    

        if (transaccion.TipoOperacionId == TipoOperacion.Egreso)
        {
            transaccion.Monto = transaccion.Monto * (-1);
        }
        
        await _transaccionRepository.Actualizar(transaccion,transaccionActulizacionVM.MontoAnterior, transaccionActulizacionVM.CuentaAnteriorId);

        return RedirectToAction(nameof(Index));
    }
    
    // Dropdown dependiente
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

    private void MapearTransaccionDesdeTransaccionActualizacionVM(Transaccion transaccion, TransaccionActulizacionViewModel transaccionActulizacionVM)
    {
        transaccion.Fecha = transaccionActulizacionVM.Fecha;
        transaccion.Monto = transaccionActulizacionVM.Monto;
        transaccion.Nota = transaccionActulizacionVM.Nota;
        transaccion.CategoriaId = transaccionActulizacionVM.CategoriaId;
        transaccion.CuentaId = transaccionActulizacionVM.CuentaId;
        transaccion.TipoOperacionId = transaccionActulizacionVM.TipoOperacionId;
    }
    
}