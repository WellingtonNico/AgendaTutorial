using System.ComponentModel.DataAnnotations;
using Agenda.Models;

namespace Agenda.Data.Services;

class ContatoService
{
    private static readonly string DatabaseDir = "./Data/database.csv";
    private static readonly string DelimitadorCSV = ";";

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
        /// ler o arquivo database.csv
        var contatos = new List<Contato>();
        using (var reader = new StreamReader(DatabaseDir))
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null)
                    continue;
                var dados = line.Split(DelimitadorCSV);
                var contato = new Contato
                {
                    Id = int.Parse(dados[0]),
                    Nome = dados[1],
                    DDD = dados[2],
                    Numero = dados[3],
                    Email = dados[4],
                };
                contatos.Add(contato);
            }
        return contatos;
    }

    public static void SalvarContatos(List<Contato> contatos)
    {
        using var writer = new StreamWriter(DatabaseDir);
        foreach (var c in contatos)
            writer.WriteLine(
                $"{c.Id}{DelimitadorCSV}{c.Nome}{DelimitadorCSV}{c.DDD}{DelimitadorCSV}{c.Numero}{DelimitadorCSV}{c.Email}"
            );
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
