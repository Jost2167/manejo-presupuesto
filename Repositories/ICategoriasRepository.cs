using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ICategoriasRepository
{
    Task Crear(Categoria categoria);
    Task<IEnumerable<Categoria>> Obtener(int usuarioId);
    Task<Categoria> ObtenerPorId(int id, int usuarioId);
    Task Actualizar(Categoria categoria);
    Task Eliminar(int id);
    Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacion);
}