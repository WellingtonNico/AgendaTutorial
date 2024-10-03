using System.ComponentModel.DataAnnotations;

namespace Agenda.Models.Validators;

class ApenasNumeros : ValidationAttribute
{
    public new required string ErrorMessage { get; set; }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string s && s.All(char.IsDigit))
        {
            return ValidationResult.Success!;
        }
        return new ValidationResult(ErrorMessage, [validationContext.DisplayName!]);
    }
}
