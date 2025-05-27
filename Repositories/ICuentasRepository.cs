using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ICuentasRepository
{
    Task Crear(Cuenta cuenta);
}