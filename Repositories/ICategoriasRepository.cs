using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ICategoriasRepository
{
    Task Crear(Categoria categoria);
}