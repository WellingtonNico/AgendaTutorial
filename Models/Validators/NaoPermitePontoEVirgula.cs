using System.ComponentModel.DataAnnotations;

namespace Agenda.Models.Validators;

class NaoPermitePontoVirgula : ValidationAttribute
{
    public new required string ErrorMessage { get; set; }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string s && s.Contains(';'))
        {
            return new ValidationResult(ErrorMessage, [validationContext.DisplayName!]);
        }
        return ValidationResult.Success!;
    }
}
