using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ITransaccionRepository
{
    Task Crear(Transaccion transaccion);
}