using ManejoPresupuesto.Models;
using ManejoPresupuesto.Repository;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers;

public class CategoriasController: Controller
{
    private readonly ICategoriasRepository _categoriasRepository;
    private readonly ITiposCuentasService _usuarioService;
        
    public CategoriasController(ICategoriasRepository categoriasRepository, ITiposCuentasService usuarioService)
    {
        _categoriasRepository = categoriasRepository;
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int usuarioId = _usuarioService.ObtenerUsuarioId();
        IEnumerable<Categoria> categorias = await _categoriasRepository.Obtener(usuarioId); 
        return View(categorias);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        int usuarioId = _usuarioService.ObtenerUsuarioId();
        categoria.UsuarioId = usuarioId;

        await _categoriasRepository.Crear(categoria);
        return RedirectToAction(nameof(Index)); 
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        int usuarioId = _usuarioService.ObtenerUsuarioId();
        Categoria categoria = await _categoriasRepository.ObtenerPorId(id, usuarioId);

        if (categoria is null)
        {
            return RedirectToAction(nameof(Index));
        }
        
        return View(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }
        
        int usuarioId = _usuarioService.ObtenerUsuarioId();
        Categoria existeCategoria = await _categoriasRepository.ObtenerPorId(categoria.Id, usuarioId);

        if (existeCategoria is null)
        {
            return RedirectToAction("NoEncontrado","Home");
        }
        
        categoria.UsuarioId = usuarioId;
        await _categoriasRepository.Actualizar(categoria);
        return RedirectToAction(nameof(Index));
    }
        
    
    
}