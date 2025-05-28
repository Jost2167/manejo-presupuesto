using System.ComponentModel.DataAnnotations;
using ManejoPresupuesto.Validations;

namespace ManejoPresupuesto.Models;

public class Cuenta
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido.")]
    [StringLength(maximumLength: 50, MinimumLength = 3)]
    [PrimeraLetraMayuscula]
    public string Nombre { get; set; }
    public decimal Balance { get; set; }
    [StringLength(maximumLength:1000)]
    public string Descripcion { get; set; }
    [Display(Name = "Tipo cuenta")]
    public int TipoCuentaId { get; set; }
    public string TipoCuenta { get; set; }
}