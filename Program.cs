using Agenda.Data.Services;
using Agenda.Models;

namespace Agenda;

class Program
{
    public static ContatoService contatoService = new();
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
        var contatos = contatoService.ListarContatos();
        ImprimirContatos(contatos);
        return contatos;
    }

    private static void ImprimirContatos(List<Contato> contatos)
    {
        var formato = "| {0,-3} | {1,-30} | {2,-3} | {3,-9} | {4,-40} |";
        Console.WriteLine($"\nListando {contatos.Count} contatos...");
        Console.WriteLine(formato, "ID", "Nome", "DDD", "Número", "Email");
        Console.WriteLine(new string('-', 101));
        foreach (var contato in contatos)
        {
            Console.WriteLine(
                formato,
                contato.Id,
                contato.Nome,
                contato.DDD,
                contato.Numero,
                contato.Email
            );
        }
    }

    private static void AdicionarContato()
    {
        var contato = ObterContatoDoInput(null);
        contatoService.SalvarContato(contato);
        ImprimirTextoVerde($"\nContato criado com sucesso: {contato.Nome}");
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
        contatoService.SalvarContato(contato);
        ImprimirTextoVerde($"\nContato editado com sucesso!: {contato.Nome}");
    }

    private static Contato SelecionarContato()
    {
        var contatos = ListarContatos();
        Console.WriteLine("\nInforme o ID do contato:");
        var identificador = Console.ReadLine() ?? "";
        while (!IsIdentificadorValido(identificador, contatos))
        {
            Console.WriteLine("\nID inválido, informe novamente:");
            identificador = Console.ReadLine() ?? "";
        }
        var contato = contatos.First(c => c.Id == int.Parse(identificador));
        Console.WriteLine("\nContato selecionado:");
        ImprimirTextoAmarelo($"ID: {contato.Id}");
        ImprimirTextoAmarelo($"Nome: {contato.Nome}");
        ImprimirTextoAmarelo($"DDD: {contato.DDD}");
        ImprimirTextoAmarelo($"Número: {contato.Numero}");
        ImprimirTextoAmarelo($"Email: {contato.Email}");
        return contato;
    }

    private static void RemoverContato()
    {
        var contato = SelecionarContato();
        contatoService.RemoverContato(contato);
        ImprimirTextoVerde($"\nContato removido com sucesso: {contato.Nome}");
    }

    private static void FiltrarContatos()
    {
        Console.WriteLine("\nInforme o filtro:");
        var filtro = Console.ReadLine() ?? "";
        var contatos = contatoService.FiltrarContatos(filtro);
        ImprimirTextoVerde($"\nResultados da busca por {filtro}:");
        ImprimirContatos(contatos);
    }

    private static string ObterInputComValorPadrao(string mensagem, string valorPadrao)
    {
        Console.WriteLine(mensagem);
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            return valorPadrao;
        }
        return input;
    }

    private static Contato ObterContatoDoInput(Contato? contato)
    {
        var sufixo = "";
        if (contato == null)
        {
            contato = new Contato();
        }
        else
        {
            sufixo = "(Deixe vazio para manter os dados atuais)";
        }
        do
        {
            contato.Nome = ObterInputComValorPadrao(
                $"\nInforme o novo nome{sufixo}:",
                contato.Nome
            );
            contato.DDD = ObterInputComValorPadrao($"\nInforme o novo DDD{sufixo}:", contato.DDD);
            contato.Numero = ObterInputComValorPadrao(
                $"\nInforme o novo número{sufixo}:",
                contato.Numero
            );
            contato.Email = ObterInputComValorPadrao(
                $"\nInforme o novo email{sufixo}:",
                contato.Email
            );

            var erros = contatoService.ValidarContato(contato);

            if (erros.Count > 0)
            {
                Console.WriteLine("\nOs dados não são válidos:");
                foreach (var erro in erros)
                {
                    foreach (var campo in erro.MemberNames)
                    {
                        ImprimirTextoVermelho($"{campo} - {erro.ErrorMessage}");
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

    public static void ImprimirTextoVerde(string texto)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(texto);
        Console.ResetColor();
    }

    public static void ImprimirTextoVermelho(string texto)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(texto);
        Console.ResetColor();
    }

    public static void ImprimirTextoAmarelo(string texto)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(texto);
        Console.ResetColor();
    }
}
