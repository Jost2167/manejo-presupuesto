using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ITiposCuentasRepository
{
     Task Crear(TipoCuenta tipoCuenta);
     Task<bool> YaExiste(string nombre, int usuarioId);
     
}