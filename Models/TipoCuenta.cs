using System.ComponentModel.DataAnnotations;
using ManejoPresupuesto.Validations;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Models;

public class TipoCuenta //: IValidatableObject
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El {0} es requerido.")]
    [PrimeraLetraMayuscula]
    // Remote indica que controlador y método se va a encargar de recibir
    // la peticion AJAX del frontend para verificar la validacion de algun campo,
    // y ese método debe de devolver un Json como respuesta.
    [Remote("VerificarSiExiste","TiposCuentas")] 
    public string Nombre { get; set; }
    public int Orden { get; set; }
    public int UsuarioId { get; set; }
    
    // Validacion desde el modelo
    /*
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Nombre))
        {
            string primeraLetra = Nombre[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                // yield permite devolver elementos uno a uno desde metodos que implementen IEnumerable
                yield return new ValidationResult("La primera letra debe ser mayúscula.", 
                    new []{ nameof(Nombre)});
            }
        }
        yield break;
    }
    */
}