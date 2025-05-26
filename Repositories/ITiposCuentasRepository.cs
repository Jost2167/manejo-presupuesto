using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Repository;

public interface ITiposCuentasRepository
{
     Task Crear(TipoCuenta tipoCuenta);
     Task<bool> YaExiste(string nombre, int usuarioId);
     Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
     Task Actualizar(TipoCuenta tipoCuenta);
     Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
     Task Eliminar(int id);
     
     

}