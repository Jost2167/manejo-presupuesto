using System.ComponentModel.DataAnnotations;
using ManejoPresupuesto.Validations;

namespace ManejoPresupuesto.Models;

public class TipoCuenta
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El {0} es requerido.")]
    [PrimeraLetraMayuscula]
    public string Nombre { get; set; }
    public int Orden { get; set; }
    public int UsuarioId { get; set; }
}