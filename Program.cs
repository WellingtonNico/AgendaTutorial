﻿using Agenda.Data.Services;
using Agenda.Models;

namespace Agenda;

class Program
{
    public static Dictionary<string, string> opcoes =
        new()
        {
            { "1", "Listar Contatos" },
            { "2", "Adicionar Contato" },
            { "3", "Editar Contato" },
            { "4", "Remover Contato" },
            { "5", "Filtrar Contatos" },
            { "6", "Sair" },
        };

    static void Main(string[] args)
    {
        var continuar = Menu();
        while (continuar)
        {
            Console.WriteLine("\n\nPressione ENTER para continuar...");
            Console.ReadLine();
            continuar = Menu();
        }
    }

    private static bool Menu()
    {
        Console.WriteLine("\n\nMenu:");
        foreach (var opcao in opcoes)
        {
            Console.WriteLine($"{opcao.Key} - {opcao.Value}");
        }
        Console.WriteLine("\nInforme sua intenção:");
        var input = Console.ReadLine() ?? "";
        while (!opcoes.ContainsKey(input!))
        {
            Console.WriteLine("Opção inválida, informe novamente:");
            input = Console.ReadLine() ?? "";
        }
        switch (input)
        {
            case "1":
                ListarContatos();
                break;
            case "2":
                AdicionarContato();
                break;
            case "3":
                EditarContato();
                break;
            case "4":
                RemoverContato();
                break;
            case "5":
                FiltrarContatos();
                break;
            case "6":
                Console.WriteLine("Saindo...");
                return false;
        }
        return true;
    }

    private static List<Contato> ListarContatos()
    {
        var contatos = ContatoService.ListarContatos();
        ImprimirContatos(contatos);
        return contatos;
    }

    private static void ImprimirContatos(List<Contato> contatos)
    {
        Console.WriteLine($"Listando {contatos.Count} contatos...");
        Console.WriteLine("ID | Nome | DDD | Número");
        foreach (var contato in contatos)
        {
            Console.WriteLine($"{contato.Id} | {contato.Nome}  | {contato.DDD} | {contato.Numero}");
        }
    }

    private static void AdicionarContato()
    {
        var contato = ObterContatoDoInput(null);
        ContatoService.SalvarContato(contato);
        Console.WriteLine($"Contato criado com sucesso: {contato.Nome}");
    }

    private static bool IsIdentificadorValido(string identificador, List<Contato> contatos)
    {
        if (int.TryParse(identificador, out var id))
        {
            return contatos.Any(c => c.Id == id);
        }
        return false;
    }

    private static void EditarContato()
    {
        var contato = SelecionarContato();

        contato = ObterContatoDoInput(contato);

        ContatoService.SalvarContato(contato);
        Console.WriteLine($"Contato editado com sucesso!: {contato.Nome}");
    }

    private static Contato SelecionarContato()
    {
        var contatos = ListarContatos();
        Console.WriteLine("\nInforme o ID do contato:");
        var identificador = Console.ReadLine() ?? "";
        while (!IsIdentificadorValido(identificador, contatos))
        {
            Console.WriteLine("ID inválido, informe novamente:");
            identificador = Console.ReadLine() ?? "";
        }
        var contato = contatos.First(c => c.Id == int.Parse(identificador));
        Console.WriteLine("Contato selecionado:");
        Console.WriteLine($"ID: {contato.Id}");
        Console.WriteLine($"Nome: {contato.Nome}");
        Console.WriteLine($"DDD: {contato.DDD}");
        Console.WriteLine($"Numero: {contato.Numero}");
        return contato;
    }

    private static void RemoverContato()
    {
        var contato = SelecionarContato();
        ContatoService.RemoverContato(contato);
        Console.WriteLine($"Contato removido com sucesso: {contato.Nome}");
    }

    private static void FiltrarContatos()
    {
        Console.WriteLine("\nInforme o filtro:");
        var filtro = Console.ReadLine() ?? "";
        var contatos = ContatoService.FiltrarContatos(filtro);
        Console.WriteLine($"Resultados da busca por {filtro}:");
        ImprimirContatos(contatos);
    }

    private static Contato ObterContatoDoInput(Contato? contato)
    {
        if (contato == null)
        {
            contato = new Contato();
        }
        do
        {
            Console.WriteLine("\nInforme o novo nome:");
            contato.Nome = Console.ReadLine() ?? "";
            Console.WriteLine("Informe o novo DDD:");
            contato.DDD = Console.ReadLine() ?? "";
            Console.WriteLine("Informe o novo número:");
            contato.Numero = Console.ReadLine() ?? "";

            var erros = ContatoService.ValidarContato(contato);

            if (erros.Count > 0)
            {
                Console.WriteLine("Os dados não são válidos:");
                foreach (var erro in erros)
                {
                    foreach (var campo in erro.MemberNames)
                    {
                        Console.WriteLine($"{campo} - {erro.ErrorMessage}");
                    }
                }
                Console.WriteLine("\nRe-informe os dados:");
            }
            else
            {
                break;
            }
        } while (true);
        return contato;
    }
}
