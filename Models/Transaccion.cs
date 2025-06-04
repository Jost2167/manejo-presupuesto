using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models;

public class Transaccion
{
    public int Id { get; set; }
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;
    public decimal Monto { get; set; }
    [StringLength(1000, ErrorMessage = "La nota debe ser menor a 1000 caracteres")]
    public string Nota { get; set; }                
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una Categoria")]
    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una Cuenta")]
    [Display(Name = "Cuenta")]
    public int CuentaId { get; set; }
    public int UsuarioId { get; set; }
}