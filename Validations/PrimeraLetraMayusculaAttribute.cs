using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Validations;

public class PrimeraLetraMayusculaAttribute: ValidationAttribute
{
    override protected ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // value es el valor del campo
        // Verificar si el valor es null
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success;
        }

        string primeraLetra = value.ToString()[0].ToString();

        if (primeraLetra != primeraLetra.ToUpper())
        {
            return new ValidationResult("La primera letra debe ser mayúcula.");
        }

        return ValidationResult.Success;
    }
}