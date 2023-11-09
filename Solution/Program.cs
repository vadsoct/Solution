using System.Text.Json;

public class Program
{
    private static void Main(string[] args)
    {
        var estacionamento = new Estacionamento();
        estacionamento.IniciarEstacionamento();
        InterfaceUsuario(estacionamento);
    }

    private static void InterfaceUsuario(Estacionamento estacionamento)
    {
        bool sairAplicacao = false;
        do
        {
            Console.Clear();
            estacionamento.ImprimirMapa();

            Console.WriteLine();
            Console.WriteLine("Digite o numero de qual operação deseja realizar \n");
            Console.WriteLine("1 - Estacionar \n");
            Console.WriteLine("2 - Retirar \n");
            Console.WriteLine("3 - Consultar \n");
            Console.WriteLine("4 - Status \n");
            Console.WriteLine("5 - Sair \n");
            Console.WriteLine();


            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":

                    Console.WriteLine();
                    Console.WriteLine("Qual o ID da vaga? \n");
                    string id = Console.ReadLine();
                    int idVaga = int.Parse(id);

                    Console.WriteLine("Qual tipo de veiculo deseja estacionar \n");
                    Console.WriteLine("1 - Moto \n");
                    Console.WriteLine("2 - Carro \n");
                    Console.WriteLine("3 - Van \n");

                    string tipoVeiculo = Console.ReadLine();

                    if (tipoVeiculo != "1" || tipoVeiculo != "2" || tipoVeiculo != "3")
                    {
                        TipoVeiculo tipo = (TipoVeiculo)Enum.ToObject(typeof(TipoVeiculo), int.Parse(tipoVeiculo));

                        Console.WriteLine("Qual placa do veiculo? \n");

                        string placaVeiculo = Console.ReadLine();

                        Console.WriteLine("Qual modelo do veiculo? \n");

                        string modeloVeiculo = Console.ReadLine();

                        Console.WriteLine("Qual fabricante do veiculo? \n");

                        string fabricanteVeiculo = Console.ReadLine();

                        var resultado = estacionamento.Estacionar(idVaga, tipo, placaVeiculo, modeloVeiculo, fabricanteVeiculo);
                        Console.WriteLine(resultado);
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Veiculo não cadastrado!");
                        Console.ReadLine();
                    }
                    break;

                case "2":

                    Console.WriteLine("Qual o ID da vaga? \n");
                    string idRetirada = Console.ReadLine();
                    int idVagaRetirada = int.Parse(idRetirada);
                    estacionamento.Retirada(idVagaRetirada);
                    break;

                case "3":
                    Console.WriteLine("Qual o ID da vaga? \n");
                    string idConsulta = Console.ReadLine();
                    int idConsultaVaga = int.Parse(idConsulta);
                    var detalhesVaga = estacionamento.ConsultarPorIdVaga(idConsultaVaga);
                    Console.WriteLine(detalhesVaga);
                    Console.ReadLine();
                    break;

                case "4":
                    estacionamento.ApresentarStatus();
                    break;

                case "5":
                    sairAplicacao = true;
                    break;

                default:
                    Console.WriteLine("Opção invalida");
                    break;
            }
        } while (!sairAplicacao);
    }
}

public enum TipoVeiculo
{
    Moto = 1,
    Carro = 2,
    Van = 3,
}
public enum TipoVaga
{
    Pequena = 1,
    Media = 2,
    Grande = 3,
}
public class Estacionamento
{
    public Estacionamento()
    {
        VagasPequenas = new VagaPequena[2, 16];
        VagasMedias = new VagaMedia[2, 10];
        VagasGrandes = new VagaGrande[1, 4];
    }
    public Vaga[,] VagasPequenas { get; set; }
    public Vaga[,] VagasMedias { get; set; }
    public Vaga[,] VagasGrandes { get; set; }

    public Estacionamento IniciarEstacionamento()
    {
        int idVagas = 0;
        var estacionamento = new Estacionamento();

        for (int i = 0; i < VagasPequenas.GetLength(0); i++)
        {
            for (int j = 0; j < VagasPequenas.GetLength(1); j++)
            {
                idVagas++;
                VagasPequenas[i, j] = new VagaPequena(idVagas, i, j);
            }
        }
        for (int i = 0; i < VagasMedias.GetLength(0); i++)
        {
            for (int j = 0; j < VagasMedias.GetLength(1); j++)
            {
                idVagas++;
                VagasMedias[i, j] = new VagaMedia(idVagas, i, j);
            }
        }

        for (int i = 0; i < VagasGrandes.GetLength(0); i++)
        {
            for (int j = 0; j < VagasGrandes.GetLength(1); j++)
            {
                idVagas++;
                VagasGrandes[i, j] = new VagaGrande(idVagas, i, j);
            }
        }
        return estacionamento;
    }
    public void ApresentarStatus()
    {
        Status status = new Status();

        var listaVagas = BuscarListaVagas().ToList();
        status.VagasTotais = listaVagas.Count();
        status.VagasUsadas = listaVagas.Count(x => x.Ocupada);

        status.VagasMotosTotal = listaVagas.Count(x => x.TipoVaga == TipoVaga.Pequena);

        status.VagasUsadasMoto = listaVagas.Count(x => x.Ocupada && x.Veiculo.Tipo == TipoVeiculo.Moto);

        var listaCarros = VerificaVagaMedia().ToList();
        status.VagasCarrosTotal = listaVagas.Count(x => x.TipoVaga == TipoVaga.Media);

        status.VagasUsadasCarros = listaVagas.Count(x => x.Ocupada && x.TipoVaga == TipoVaga.Media);

        var listaVans = VerificaVagaGrande().ToList();
        status.VagasVansTotal = listaVagas.Count(x => x.TipoVaga == TipoVaga.Grande);

        status.VagasUsadasVans = listaVagas.Count(x => x.Ocupada && x.TipoVaga == TipoVaga.Grande);

        var VagasVansSetorCarros = listaVagas.Where(x => x.TipoVaga == TipoVaga.Media).OrderBy(y => y.Id).ToList();
        int vagasVanCarroDisponiveis = CalcularVagasGrandesEntreVagasMedias(VagasVansSetorCarros);

        Console.WriteLine($"Quantidade de vagas restantes: {status.VagasRestantes}");
        Console.WriteLine($"Quantidade de vagas no estacionamento: {status.VagasTotais}");

        string estacionamentoCheio = status.EstacionamentoCheio ? "Não" : "Sim";
        Console.WriteLine($"O estacionamento está cheio: {estacionamentoCheio}");

        string estacionamentoVazio = status.EstacionamentoVazio ? "Sim" : "Não";
        Console.WriteLine($"O estacionamento está vazio: {estacionamentoVazio}");

        Console.WriteLine();

        Console.WriteLine($"Quantidade de vagas restantes no setor de motos : {status.VagasDisponiveisMoto}");
        Console.WriteLine($"Quantidade de vagas no setor de moto do estacionamento: {status.VagasMotosTotal}");

        string estacionamentoMotoCheio = status.EstacionamentoMotoCheio ? "Não" : "Sim";
        Console.WriteLine($"O setor do estacionamento de motos está cheio: {estacionamentoCheio}");

        string estacionamentoMotoVazio = status.EstacionamentoMotoVazio ? "Sim" : "Não";
        Console.WriteLine($"O estacionamento de moto está vazio: {estacionamentoVazio}");

        Console.WriteLine();

        Console.WriteLine($"Quantidade de vagas carro restantes: {status.VagasDisponiveisCarro}");
        Console.WriteLine($"Quantidade de vagas de carro no estacionamento: {status.VagasCarrosTotal}");

        string estacionamentoCarroCheio = status.EstacionamentoCarroCheio ? "Não" : "Sim";
        Console.WriteLine($"O estacionamento de carros está cheio: {estacionamentoCarroCheio}");

        string estacionamentoCarroVazio = status.EstacionamentoCarroVazio ? "Sim" : "Não";
        Console.WriteLine($"O estacionamento de carros está vazio: {estacionamentoCarroVazio}");

        Console.WriteLine();

        Console.WriteLine($"Quantidade de vagas vans restantes no setor de Vans: {status.VagasDisponiveisVans}");
        Console.WriteLine($"Quantidade de vagas de vans no estacionamento: {status.VagasVansTotal}");

        string estacionamentoVansCheio = status.EstacionamentoVansCheio ? "Não" : "Sim";
        Console.WriteLine($"O estacionamento de vans está cheio: {estacionamentoVansCheio}");

        string estacionamentoVansVazio = status.EstacionamentoVansVazio ? "Sim" : "Não";
        Console.WriteLine($"O estacionamento de vans está vazio: {estacionamentoVansVazio}");

        Console.WriteLine($"Quantidade de vans que podem ser estacionadas no setor de carro: {vagasVanCarroDisponiveis}");

        Console.ReadLine();
    }
    public void ImprimirMapa()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Os numeros em vermelho estão ocupados");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Os numeros em azul estão livres");
        Console.ResetColor();

        Console.WriteLine();
        for (int i = 0; i < VagasPequenas.GetLength(0); i++)
        {
            for (int j = 0; j < VagasPequenas.GetLength(1); j++)
            {
                Console.Write(" __ ");
                if (j == VagasPequenas.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasPequenas.GetLength(1); j++)
            {
                Console.Write("|  |");
                if (j == VagasPequenas.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasPequenas.GetLength(1); j++)
            {
                string valorA = VagasPequenas[i, j].Id <= 9 ? "0" : "";
                Console.Write("|");
                if (VagasPequenas[i, j].Ocupada)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.Write(valorA + VagasPequenas[i, j].Id);

                Console.ResetColor();
                Console.Write("|");
                if (j == VagasPequenas.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasPequenas.GetLength(1); j++)
            {
                Console.Write("|__|");
                if (j == VagasPequenas.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
        }

        for (int i = 0; i < VagasMedias.GetLength(0); i++)
        {
            for (int j = 0; j < VagasMedias.GetLength(1); j++)
            {
                Console.Write(" ____ ");
                if (j == VagasMedias.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasMedias.GetLength(1); j++)
            {
                Console.Write("|    |");
                if (j == VagasMedias.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasMedias.GetLength(1); j++)
            {
                string valorB = VagasMedias[i, j].Id <= 9 ? "0" : "";
                Console.Write("| ");
                if (VagasMedias[i, j].Ocupada)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.Write(valorB + VagasMedias[i, j].Id);
                Console.ResetColor();
                Console.Write(" |");

                if (j == VagasMedias.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasMedias.GetLength(1); j++)
            {
                Console.Write("|____|");
                if (j == VagasMedias.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
        }

        for (int i = 0; i < VagasGrandes.GetLength(0); i++)
        {
            for (int j = 0; j < VagasGrandes.GetLength(1); j++)
            {
                Console.Write(" ______________ ");
                if (j == VagasGrandes.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasGrandes.GetLength(1); j++)
            {
                Console.Write("|              |");
                if (j == VagasGrandes.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasGrandes.GetLength(1); j++)
            {
                string valorC = VagasGrandes[i, j].Id <= 9 ? "0" : "";
                Console.Write("|      ");


                if (VagasGrandes[i, j].Ocupada)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.Write(valorC + VagasGrandes[i, j].Id);
                Console.ResetColor();

                Console.Write("      |");
                if (j == VagasGrandes.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            for (int j = 0; j < VagasGrandes.GetLength(1); j++)
            {
                Console.Write("|______________|");
                if (j == VagasGrandes.GetLength(1) - 1)
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine();
        }
    }
    public string ConsultarPorIdVaga(int id)
    {
        var vagasDisponiveis = BuscarListaVagas();
        var detalhes = vagasDisponiveis.FirstOrDefault(x => x.Id.Equals(id));
        string json = JsonSerializer.Serialize(detalhes);
        return json;
    }
    public string Retirada(int id)
    {
        var vagas = BuscarListaVagas();
        var vagaSelecionada = vagas.FirstOrDefault(x => x.Id.Equals(id));

        if (vagaSelecionada == null || vagaSelecionada.Veiculo == null)
        {
            return "Vaga não localizada.";
        }

        if (vagaSelecionada.Veiculo.Tipo == TipoVeiculo.Van)
        {
            vagas.Where(x => x.Veiculo != null && x.Veiculo.Placa == vagaSelecionada.Veiculo.Placa)
                .ToList()
                .ForEach(x => x.RetirarVeiculo());

            return "Van removida com sucesso";
        }

        if (vagaSelecionada.Ocupada)
        {
            return vagaSelecionada.RetirarVeiculo();
        }

        return "Não foi possível retirar seu veículo";
    }
    public string Estacionar(int id, TipoVeiculo tipo, string placa, string modelo, string? fabricante)
    {
        var veiculo = new Veiculo(placa, modelo, fabricante, tipo);
        var listaVagasDisponiveis = VerificarVagaPorTipo(tipo);
        var vagaEscolhida = listaVagasDisponiveis.FirstOrDefault(x => x.Id.Equals(id));

        if (vagaEscolhida == null)
        {
            return "Vaga escolhida não disponivel.";
        }

        if (vagaEscolhida.Ocupada)
        {
            return "Vaga escolhida já está ocupada.";
        }

        if (vagaEscolhida.TipoVaga == TipoVaga.Pequena)
        {
            return VagasPequenas[vagaEscolhida.PosicaoX, vagaEscolhida.PosicaoY].EstacionarVeiculo(veiculo);
        }

        if (vagaEscolhida.TipoVaga == TipoVaga.Media)
        {
            if (tipo == TipoVeiculo.Van)
            {
                VagasMedias[vagaEscolhida.PosicaoX, vagaEscolhida.PosicaoY - 1].EstacionarVeiculo(veiculo);
                VagasMedias[vagaEscolhida.PosicaoX, vagaEscolhida.PosicaoY + 1].EstacionarVeiculo(veiculo);
            }

            return VagasMedias[vagaEscolhida.PosicaoX, vagaEscolhida.PosicaoY].EstacionarVeiculo(veiculo);
        }

        if (vagaEscolhida.TipoVaga == TipoVaga.Grande)
        {
            return VagasGrandes[vagaEscolhida.PosicaoX, vagaEscolhida.PosicaoY].EstacionarVeiculo(veiculo);
        }

        return "Erro ao estacionar na vaga.";
    }
    public List<Vaga> VerificarVagaPorTipo(TipoVeiculo tipo)
    {
        switch (tipo)
        {
            case TipoVeiculo.Moto:
                return VerificaVagasPequenas();

            case TipoVeiculo.Carro:
                return VerificaVagaMedia();

            case TipoVeiculo.Van:
                return VerificaVagaGrande();

            default:
                return new List<Vaga>();
        }
    }
    public List<Vaga> BuscarListaVagas()
    {
        var vagasDisponiveis = new List<Vaga>();

        for (int i = 0; i < VagasPequenas.GetLength(0); i++)
        {
            for (int j = 0; j < VagasPequenas.GetLength(1); j++)
            {
                vagasDisponiveis.Add(VagasPequenas[i, j]);
            }
        }
        for (int i = 0; i < VagasMedias.GetLength(0); i++)
        {
            for (int j = 0; j < VagasMedias.GetLength(1); j++)
            {
                vagasDisponiveis.Add(VagasMedias[i, j]);
            }
        }

        for (int i = 0; i < VagasGrandes.GetLength(0); i++)
        {
            for (int j = 0; j < VagasGrandes.GetLength(1); j++)
            {
                vagasDisponiveis.Add(VagasGrandes[i, j]);
            }
        }
        return vagasDisponiveis;
    }
    private List<Vaga> VerificaVagasPequenas()
    {
        var vagasDisponiveis = BuscarListaVagas()
            .Where(x => x.Ocupada == false)
            .ToList();
        return vagasDisponiveis;
    }
    private List<Vaga> VerificaVagaMedia()
    {
        List<Vaga> vagasDisponiveis = BuscarListaVagas()
           .Where(x => x.Ocupada == false && (x.TipoVaga == TipoVaga.Media || x.TipoVaga == TipoVaga.Grande))
           .ToList();
        return vagasDisponiveis;
    }
    private List<Vaga> VerificaVagaGrande()
    {
        var vagasDisponiveis = new List<Vaga>();

        for (int i = 0; i < VagasMedias.GetLength(0); i++)
        {
            for (int j = 0; j < VagasMedias.GetLength(1); j++)
            {
                var numeroVagasCarro = VagasMedias.GetLength(1);

                if (decimal.Parse(j.ToString()) > 0 && decimal.Parse(j.ToString()) + 1 < numeroVagasCarro)
                {
                    var vagaSelecionada = VagasMedias[i, j];
                    var vagaEsquerda = VagasMedias[i, j - 1];
                    var vagaDireita = VagasMedias[i, j + 1];

                    if (!VagasMedias[i, j].Ocupada && !VagasMedias[i, j - 1].Ocupada && !VagasMedias[i, j + 1].Ocupada)
                    {
                        var a = VagasMedias[i, j];
                        vagasDisponiveis.Add(VagasMedias[i, j]);
                    }
                }

            }
        }
        foreach (var vaga in VagasGrandes)
        {
            if (!vaga.Ocupada)
            {
                vagasDisponiveis.Add(vaga);
            }
        }
        return vagasDisponiveis;
    }
    private static int CalcularVagasGrandesEntreVagasMedias(List<Vaga> VagasVansSetorCarros)
    {
        int vagasVanCarroDisponiveis = 0;
        decimal quantidadeVansCarro = 0;
        int index = 0;
        int linha = 0;
        foreach (var vagaSetorCarro in VagasVansSetorCarros)
        {
            if (index == 0)
            {
                linha = vagaSetorCarro.PosicaoX;
            }
            index++;
            quantidadeVansCarro++;

            if (vagaSetorCarro.Ocupada)
            {
                quantidadeVansCarro = 0;
            }
            if (linha != vagaSetorCarro.PosicaoX)
            {
                quantidadeVansCarro = 0;
                linha = vagaSetorCarro.PosicaoX;
            }
            if (quantidadeVansCarro % 3 == 0 && quantidadeVansCarro != 0)
            {
                vagasVanCarroDisponiveis++;
                quantidadeVansCarro = 0;
            }

        }

        return vagasVanCarroDisponiveis;
    }
}
public class Status
{
    public int VagasTotais { get; set; }
    public int VagasUsadas { get; set; }
    public int VagasRestantes => VagasTotais - VagasUsadas;
    public bool EstacionamentoCheio => VagasRestantes == 0;
    public bool EstacionamentoVazio => VagasRestantes == VagasTotais;
    public int VagasDisponiveisMoto => VagasMotosTotal - VagasUsadasMoto;
    public int VagasUsadasMoto { get; set; }
    public int VagasMotosTotal { get; set; }
    public bool EstacionamentoMotoCheio => VagasMotosTotal == VagasUsadasMoto;
    public bool EstacionamentoMotoVazio => VagasMotosTotal == VagasDisponiveisMoto;
    public int VagasDisponiveisCarro => VagasCarrosTotal - VagasUsadasCarros;
    public int VagasUsadasCarros { get; set; }
    public int VagasCarrosTotal { get; set; }
    public bool EstacionamentoCarroCheio => VagasCarrosTotal == VagasUsadasCarros;
    public bool EstacionamentoCarroVazio => VagasCarrosTotal == VagasDisponiveisCarro ;
    public int VagasDisponiveisVans => VagasVansTotal - VagasUsadasVans;
    public int VagasUsadasVans { get; set; }
    public int VagasVansTotal { get; set; }
    public bool EstacionamentoVansCheio => VagasVansTotal == VagasUsadasVans;
    public bool EstacionamentoVansVazio => VagasVansTotal == VagasDisponiveisVans;
}
public abstract class Vaga
{
    public int Id { get; internal set; }
    public TipoVaga TipoVaga { get; set; }
    public int PosicaoX { get; set; }
    public int PosicaoY { get; set; }
    public bool Ocupada { get; internal set; }
    public decimal DimencaoX { get; internal set; }
    public decimal Dimencaoy { get; internal set; }
    public Veiculo? Veiculo { get; set; }

    public virtual string EstacionarVeiculo(Veiculo veiculo)
    {
        Veiculo = veiculo;
        Ocupada = true;
        return "Veiculo cadastrado com sucesso!";
    }

    public virtual string RetirarVeiculo()
    {
        Veiculo = null;
        Ocupada = false;
        return "Veiculo retirado com sucesso!";
    }
}
public class Veiculo
{
    public Veiculo(string? placa, string? modelo, string? fabricante, TipoVeiculo tipo)
    {
        Placa = placa;
        Modelo = modelo;
        Fabricante = fabricante;
        Tipo = tipo;
    }
    public string? Placa { get; set; }
    public string? Modelo { get; set; }
    public string? Fabricante { get; set; }
    public TipoVeiculo Tipo { get; set; }

}
public class VagaPequena : Vaga
{
    public VagaPequena(int id, int x, int y)
    {
        Id = id;
        TipoVaga = TipoVaga.Pequena;
        PosicaoX = x;
        PosicaoY = y;
        DimencaoX = 1;
        Dimencaoy = 2;
        Ocupada = false;
    }
}
public class VagaMedia : Vaga
{
    public VagaMedia(int id, int x, int y)
    {
        Id = id;
        TipoVaga = TipoVaga.Media;
        PosicaoX = x;
        PosicaoY = y;
        DimencaoX = 2;
        Dimencaoy = 2;
        Ocupada = false;
    }
}
public class VagaGrande : Vaga
{
    public VagaGrande(int id, int x, int y)
    {
        Id = id;
        TipoVaga = TipoVaga.Grande;
        PosicaoX = x;
        PosicaoY = y;
        DimencaoX = 6;
        Dimencaoy = 2;
        Ocupada = false;
    }
}