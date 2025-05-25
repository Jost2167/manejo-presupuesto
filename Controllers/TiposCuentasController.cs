using System.Collections;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Repository;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers;

public class TiposCuentasController: Controller
{
    private readonly ITiposCuentasRepository _tiposCuentasRepository;
    private readonly ITiposCuentasService _tiposCuentasService;
    public TiposCuentasController(ITiposCuentasRepository tiposCuentasRepository, ITiposCuentasService tiposCuentasService)
    {
        _tiposCuentasRepository = tiposCuentasRepository;
        _tiposCuentasService = tiposCuentasService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int usuarioId = _tiposCuentasService.ObtenerUsuarioId();
        IEnumerable<TipoCuenta> lista = await _tiposCuentasRepository.Obtener(usuarioId);
        return View(lista);
    }
    
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
    {
        if (!ModelState.IsValid)
        {
            return View(tipoCuenta);
        }

        tipoCuenta.UsuarioId = _tiposCuentasService.ObtenerUsuarioId();
        
        bool yaExiste = await _tiposCuentasRepository.YaExiste(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
        
        if (yaExiste)
        {
            // Agrega un mensaje de error a la clave del campo que contiene el error
            ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El {nameof(tipoCuenta.Nombre)} ya existe en la base de datos.");
            // Devuelve la vista con el mensaje de error 
            return View(tipoCuenta);
        }
        
        await _tiposCuentasRepository.Crear(tipoCuenta);

        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    // Esto es lo que ejecuta la peticion AJAX del frontend
    public async Task<IActionResult> VerificarSiExiste(string nombre, int usuarioId)
    {
        usuarioId = _tiposCuentasService.ObtenerUsuarioId();
        bool siExiste = await _tiposCuentasRepository.YaExiste(nombre, usuarioId);

        if (siExiste)
        {
            return Json($"El nombre {nombre} ya existe.");  // Si existe, se envía Json indicando el mensaje de error    
        }
        
        return Json(true);  // Si no existe, se envía un true en Json indicando que no hubo ningún error
    } 
}