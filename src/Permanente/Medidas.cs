namespace Conde.Permanente;

/// <summary>
/// As MEDIDAS, que e onde o repositorio deixa de ser uma curiosidade de algebra
/// linear e vira um numero.
///
/// A pergunta de todas as tabelas e a mesma: quanto custa um sinal? As duas contas
/// tem a mesma formula; uma delas tem um mais ou menos na frente de cada parcela.
/// E e so isso que separa um algoritmo de primeiro semestre de um problema que
/// ninguem sabe resolver.
/// </summary>
public static class Medidas
{
    public record LinhaDefinicao(
        string Matriz,
        int Ordem,
        long PelaDefinicao,
        long PorRyser,
        bool Batem,
        long ContasDaDefinicao,
        long ContasDeRyser);

    /// <summary>
    /// RYSER contra a DEFINICAO, valor e custo, em toda matriz pequena.
    ///
    /// A coluna que importa nao e a do valor, que sempre bate: e a das contas. Com
    /// ordem seis a definicao faz milhares de multiplicacoes e Ryser faz centenas.
    /// </summary>
    public static List<LinhaDefinicao> RyserContraADefinicao()
    {
        var linhas = new List<LinhaDefinicao>();

        foreach (var (nome, m) in Matrizes.Pequenas(4))
        {
            var definicao = Metodos.PermanentePelaDefinicao(m);
            var ryser = Metodos.Ryser(m);

            linhas.Add(new LinhaDefinicao(
                nome, m.Ordem, definicao.Valor, ryser.Valor, definicao.Valor == ryser.Valor,
                definicao.Contas, ryser.Contas));
        }

        return linhas;
    }

    public record LinhaSinal(
        string Matriz,
        int Ordem,
        long Determinante,
        long Permanente,
        long DeterminanteTrocado,
        long PermanenteTrocada);

    /// <summary>
    /// O SINAL, visto de perto: as duas contas na mesma matriz, e nela com duas
    /// linhas trocadas.
    ///
    /// Trocar duas linhas inverte o sinal do determinante e nao mexe na permanente.
    /// E na matriz de todos uns o determinante e ZERO e a permanente e n FATORIAL,
    /// que e a maior distancia possivel entre duas contas que tem a mesma formula.
    /// </summary>
    public static List<LinhaSinal> OQueOSinalFaz()
    {
        var linhas = new List<LinhaSinal>();

        foreach (var (nome, m) in Matrizes.ParaOSinal())
        {
            var trocada = m.ComLinhasTrocadas(0, 1);

            linhas.Add(new LinhaSinal(
                nome,
                m.Ordem,
                Metodos.DeterminantePelaDefinicao(m).Valor,
                Metodos.PermanentePelaDefinicao(m).Valor,
                Metodos.DeterminantePelaDefinicao(trocada).Valor,
                Metodos.PermanentePelaDefinicao(trocada).Valor));
        }

        return linhas;
    }

    public record LinhaCusto(
        int Ordem,
        long Determinante,
        long Permanente,
        bool OsDoisDeterminantesBatem,
        long ContasDeGauss,
        long ContasDeBareiss,
        long ContasDeRyser,
        long ContasDaDefinicao,
        double RyserSobreGauss);

    /// <summary>
    /// O ABISMO: o custo das duas contas na MESMA matriz, ordem por ordem.
    ///
    /// Gauss cresce como um cubo e Ryser cresce como uma potencia de dois. A razao
    /// entre as duas colunas comeca pequena e nao para de subir.
    ///
    /// E a definicao, que serve de juiz, para de caber: ordem onze ja custa centenas
    /// de milhoes de multiplicacoes.
    /// </summary>
    public static List<LinhaCusto> OCustoDasDuasContas()
    {
        var linhas = new List<LinhaCusto>();

        for (var ordem = 3; ordem <= 15; ordem++)
        {
            var m = Matrizes.ZerosEUns(ordem, 0.6, (ulong)(ordem * 7 + 1));

            var gauss = Metodos.DeterminantePorGauss(m);
            var bareiss = Metodos.DeterminantePorBareiss(m);
            var ryser = Metodos.Ryser(m);
            var daDefinicao = ordem <= 10 ? Metodos.PermanentePelaDefinicao(m).Contas : Metodos.Fatorial(ordem) * ordem;

            linhas.Add(new LinhaCusto(
                ordem,
                bareiss.Valor,
                ryser.Valor,
                gauss.Valor == bareiss.Valor,
                gauss.Contas,
                bareiss.Contas,
                ryser.Contas,
                daDefinicao,
                gauss.Contas == 0 ? 0 : (double)ryser.Contas / gauss.Contas));
        }

        return linhas;
    }

    public record LinhaContagem(
        string Matriz,
        int Ordem,
        long Permanente,
        long Emparelhamentos,
        long PorTabela,
        bool Batem,
        long PassosDaArvore,
        long PassosDoExiste,
        bool Existe);

    /// <summary>
    /// A permanente de ZEROS E UNS CONTA, e por isso o juiz independente existe.
    ///
    /// Tres caminhos: a formula de Ryser, a arvore de distribuicoes e a tabela sobre
    /// subconjuntos. Nenhum dos dois ultimos sabe o que e permanente, e os tres dao o
    /// mesmo numero.
    ///
    /// A ultima coluna e a que incomoda: perguntar SE existe uma distribuicao custa
    /// algumas dezenas de passos, e perguntar QUANTAS custa milhares. A mesma matriz,
    /// a mesma pergunta quase com as mesmas palavras.
    /// </summary>
    public static List<LinhaContagem> ContarEhOutraCoisa()
    {
        var linhas = new List<LinhaContagem>();

        foreach (var (nome, m) in Matrizes.DeZerosEUns())
        {
            var ryser = Metodos.Ryser(m);
            var arvore = Juiz.Emparelhamentos(m);
            var tabela = Juiz.EmparelhamentosPorTabela(m);
            var existe = Juiz.ExisteEmparelhamento(m);

            linhas.Add(new LinhaContagem(
                nome, m.Ordem, ryser.Valor, arvore.Quantos, tabela,
                ryser.Valor == arvore.Quantos && tabela == arvore.Quantos,
                arvore.Passos, existe.Passos, existe.Existe));
        }

        return linhas;
    }

    public record LinhaAlcance(int Ordem, double Definicao, double Ryser, double Gauss, string QuemCabe);

    /// <summary>
    /// ATE ONDE cada metodo chega, com um orcamento de um bilhao de contas.
    ///
    /// Gauss chega a ordem mil e nao sua. Ryser para perto de vinte e cinco. A
    /// definicao para em doze. E nenhuma dessas tres linhas vai mudar com um
    /// computador mais rapido: dobrar a velocidade move Ryser em UMA ordem.
    /// </summary>
    public static List<LinhaAlcance> AteOndeCadaUmChega()
    {
        var linhas = new List<LinhaAlcance>();
        const double orcamento = 1e9;

        foreach (var ordem in new[] { 5, 10, 12, 15, 20, 25, 30, 50, 100, 1_000 })
        {
            var definicao = FatorialDouble(ordem) * ordem;
            var ryser = Math.Pow(2, ordem) * ordem;
            var gauss = (double)ordem * ordem * ordem;

            var quem =
                gauss > orcamento ? "nenhum" :
                ryser > orcamento ? "so Gauss" :
                definicao > orcamento ? "Gauss e Ryser" : "os tres";

            linhas.Add(new LinhaAlcance(ordem, definicao, ryser, gauss, quem));
        }

        return linhas;
    }

    private static double FatorialDouble(int n)
    {
        var f = 1.0;

        for (var i = 2; i <= n; i++) f *= i;

        return f;
    }

    public record LinhaDensidade(double Chance, int Ordem, long PermanenteMedia, long MaiorPermanente, int QuantasTemZero, double PassosMedios);

    /// <summary>
    /// Quantos uns sao precisos para que a resposta deixe de ser zero?
    ///
    /// A pergunta parece boba e a resposta e a parte interessante do problema: a
    /// transicao e abrupta. Com poucos uns quase toda matriz tem permanente zero, e
    /// um pouco acima disso a permanente media ja esta na casa dos milhares.
    /// </summary>
    public static List<LinhaDensidade> ADensidadeDosUns()
    {
        var linhas = new List<LinhaDensidade>();
        const int ordem = 8;
        const int quantas = 200;

        foreach (var chance in new[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 })
        {
            var soma = 0L;
            var maior = 0L;
            var comZero = 0;
            var passos = 0L;

            for (ulong semente = 1; semente <= quantas; semente++)
            {
                var m = Matrizes.ZerosEUns(ordem, chance, semente * 1_000 + (ulong)(chance * 10));
                var valor = Metodos.Ryser(m).Valor;

                soma += valor;
                maior = Math.Max(maior, valor);
                if (valor == 0) comZero++;

                passos += Juiz.Emparelhamentos(m).Passos;
            }

            linhas.Add(new LinhaDensidade(chance, ordem, soma / quantas, maior, comZero, (double)passos / quantas));
        }

        return linhas;
    }
}
