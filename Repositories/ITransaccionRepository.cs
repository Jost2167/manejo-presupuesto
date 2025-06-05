using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ITransaccionRepository
{
    Task Crear(Transaccion transaccion);
    Task Actualizar(Transaccion transaccion, decimal montoAnterior, int CuentaAnterior);
    Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    Task Eliminar(int id);
}