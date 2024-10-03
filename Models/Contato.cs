using System.ComponentModel.DataAnnotations;
using Agenda.Models.Validators;

namespace Agenda.Models;

class Contato
{
    public int Id { get; set; }

    [
        Required(ErrorMessage = "O nome é obrigatório."),
        MaxLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres."),
        NaoPermitePontoVirgula(ErrorMessage = "O nome não pode conter ponto virgula.")
    ]
    public string Nome { get; set; } = "";

    [
        Required(ErrorMessage = "O DDD é obrigatório."),
        Length(2, 2, ErrorMessage = "O DDD deve ter 2 dígitos."),
        ApenasNumeros(ErrorMessage = "O DDD deve conter apenas números."),
    ]
    public string DDD { get; set; } = "";

    [
        Display(Name = "Número"),
        Required(ErrorMessage = "O número é obrigatório."),
        MinLength(8, ErrorMessage = "O número ter no mínimo 8 dígitos."),
        MaxLength(9, ErrorMessage = "O número ter no máximo 9 dígitos."),
        ApenasNumeros(ErrorMessage = "O número deve conter apenas números."),
    ]
    public string Numero { get; set; } = "";
}
