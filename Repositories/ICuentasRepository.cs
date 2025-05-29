using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ICuentasRepository
{
    Task Crear(Cuenta cuenta);
    Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
    Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    Task Actualizar(CuentaCreacionViewModel cuentaVM);
    Task Eliminar(int id);

}