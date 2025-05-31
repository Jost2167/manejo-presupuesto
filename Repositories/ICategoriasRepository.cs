using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ICategoriasRepository
{
    Task Crear(Categoria categoria);
    Task<IEnumerable<Categoria>> Obtener(int usuarioId);
}