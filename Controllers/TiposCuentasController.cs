using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers;

public class TiposCuentasController: Controller
{
    private readonly ITiposCuentasRepository _tiposCuentasRepository;
    public TiposCuentasController(ITiposCuentasRepository tiposCuentasRepository)
    {
        _tiposCuentasRepository = tiposCuentasRepository;
    }
    
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

        tipoCuenta.UsuarioId = 1;
        
        bool yaExiste = await _tiposCuentasRepository.YaExiste(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
        
        if (yaExiste)
        {
            // Agrega un mensaje de error a la clave del campo que contiene el error
            ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El {nameof(tipoCuenta.Nombre)} ya existe en la base de datos.");
            // Devuelve la vista con el mensaje de error 
            return View(tipoCuenta);
        }
        
        await _tiposCuentasRepository.Crear(tipoCuenta);
        
        return View();
    }
    
}