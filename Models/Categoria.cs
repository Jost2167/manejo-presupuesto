using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models;

public class Categoria
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El {0} es obligatorio")]
    [StringLength(maximumLength:50, MinimumLength = 2,ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "El {0} es obligatorio")]
    [Display(Name = "Tipo operación")]
    public TipoOperacion TipoOperacionId { get; set; }
    public int UsuarioId { get; set; }
}