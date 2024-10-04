using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Agenda.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace Agenda.Data.Services;

class ContatoService
{
    private static readonly string DatabaseDir = "./Data/database.csv";
    private static readonly CsvConfiguration CsvConfig =
        new() { Delimiter = ";", CultureInfo = CultureInfo.InvariantCulture };

    public ContatoService()
    {
        ConfigurarBanco();
    }

    public void ConfigurarBanco()
    {
        var bancoDeDados = new FileInfo(DatabaseDir);
        if (!bancoDeDados.Exists)
        {
            bancoDeDados.Create().Close();
            using (var writer = new StreamWriter(DatabaseDir))
            using (var csv = new CsvWriter(writer, CsvConfig))
            {
                csv.WriteHeader<Contato>();
            }
        }
    }

    public Contato SalvarContato(Contato contato)
    {
        /// ler o arquivo database.csv, verificar se o contato já existe comparando o DDD e o número
        /// se não existir, adicionar o contato
        /// se existir, atualizar o contato
        /// salvar o arquivo database.csv
        /// retornar o contato salvo
        var contatos = ListarContatos();
        if (contato.Id != 0)
        {
            contatos = contatos.Select((c, _) => c.Id == contato.Id ? contato : c).ToList();
        }
        else
        {
            var lastId = contatos.Any() ? contatos.Max(c => c.Id) : 0;
            contato.Id = lastId + 1;
            contatos.Add(contato);
        }
        SalvarContatos(contatos);
        return contato;
    }

    public List<Contato> ListarContatos()
    {
        using var reader = new StreamReader(DatabaseDir);
        using var csv = new CsvReader(reader, CsvConfig);
        return csv.GetRecords<Contato>().ToList();
    }

    public void SalvarContatos(List<Contato> contatos)
    {
        using var writer = new StreamWriter(DatabaseDir);
        using var csv = new CsvWriter(writer, CsvConfig);
        csv.WriteRecords(contatos);
    }

    public List<Contato> FiltrarContatos(string filtro)
    {
        return ListarContatos()
            .Where(c =>
                c.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase)
                || c.Numero.Contains(filtro)
            )
            .ToList();
    }

    public void RemoverContato(Contato contato)
    {
        var contatos = ListarContatos();
        contatos.RemoveAll(c => c.Id == contato.Id);
        SalvarContatos(contatos);
    }

    public List<ValidationResult> ValidarContato(Contato contato)
    {
        var state = new ValidationContext(contato);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(contato, state, results, true);
        return results;
    }
}
