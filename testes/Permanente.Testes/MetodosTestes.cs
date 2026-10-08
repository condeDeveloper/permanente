using Conde.Permanente;
using Xunit;

namespace Conde.Permanente.Testes;

public class MetodosTestes
{
    /// <summary>
    /// As PERMUTACOES: sao n fatorial, todas diferentes, e cada uma e uma bijecao.
    ///
    /// O gerador de Heap devolve o MESMO vetor a cada passo, so trocado, e por isso
    /// qualquer comparacao entre permutacoes precisa copiar antes. Essa armadilha
    /// esta aqui como teste justamente porque ela e silenciosa.
    /// </summary>
    [Fact]
    public void AsPermutacoesSaoTodasEDistintas()
    {
        for (var n = 1; n <= 7; n++)
        {
            var vistas = new HashSet<string>();
            var quantas = 0;

            foreach (var p in Metodos.Permutacoes(n))
            {
                quantas++;

                Assert.Equal(n, p.Length);
                Assert.Equal(n, p.Distinct().Count());
                Assert.All(p, x => Assert.InRange(x, 0, n - 1));

                vistas.Add(string.Join(",", p));
            }

            Assert.Equal(Metodos.Fatorial(n), quantas);
            Assert.Equal(Metodos.Fatorial(n), vistas.Count);
        }
    }

    /// <summary>
    /// O SINAL da permutacao: metade delas e par e metade e impar, a partir de dois.
    /// </summary>
    [Fact]
    public void MetadeDasPermutacoesEhPar()
    {
        for (var n = 2; n <= 7; n++)
        {
            var pares = Metodos.Permutacoes(n).Count(p => Metodos.Sinal(p) == 1);

            Assert.Equal(Metodos.Fatorial(n) / 2, pares);
        }
    }

    /// <summary>
    /// RYSER bate com a DEFINICAO em toda matriz do repositorio.
    ///
    /// E a conferencia central: a formula de 1963 nao se parece em nada com a soma
    /// sobre permutacoes, e ela e a razao de a permanente ser calculavel ate ordem
    /// vinte e poucos em vez de doze.
    /// </summary>
    [Fact]
    public void RyserBateComADefinicao()
    {
        foreach (var (nome, m) in Matrizes.Pequenas(20))
            Assert.Equal(Metodos.PermanentePelaDefinicao(m).Valor, Metodos.Ryser(m).Valor);

        for (var ordem = 1; ordem <= 8; ordem++)
            for (ulong semente = 1; semente <= 30; semente++)
            {
                var m = Matrizes.ZerosEUns(ordem, 0.5, semente);

                Assert.Equal(Metodos.PermanentePelaDefinicao(m).Valor, Metodos.Ryser(m).Valor);
            }
    }

    /// <summary>
    /// E ela bate tambem com inteiros que nao sao zero nem um, que e onde a formula
    /// de Ryser poderia escorregar por causa das somas parciais.
    /// </summary>
    [Fact]
    public void RyserBateComInteirosQuaisquer()
    {
        for (var ordem = 1; ordem <= 7; ordem++)
            for (ulong semente = 1; semente <= 30; semente++)
            {
                var m = Matrizes.Sorteada(ordem, 6, semente * 13 + 1);

                Assert.Equal(Metodos.PermanentePelaDefinicao(m).Valor, Metodos.Ryser(m).Valor);
            }
    }

    /// <summary>
    /// RYSER custa MUITO menos que a definicao, e e isso que a tabela mede.
    ///
    /// Dois elevado a n vezes n contra n fatorial vezes n: em ordem oito sao mil e
    /// seiscentas contas contra trezentas e vinte mil.
    /// </summary>
    [Fact]
    public void RyserCustaMuitoMenosQueADefinicao()
    {
        var m = Matrizes.ZerosEUns(8, 0.8, 5);

        var definicao = Metodos.PermanentePelaDefinicao(m);
        var ryser = Metodos.Ryser(m);

        Assert.Equal(definicao.Valor, ryser.Valor);
        Assert.True(definicao.Contas > 100 * ryser.Contas, $"{definicao.Contas} contra {ryser.Contas}");

        // e o teto do custo de Ryser e dois elevado a n vezes n
        Assert.True(ryser.Contas <= (1L << 8) * 8);
    }

    /// <summary>
    /// Os DOIS determinantes polinomiais batem entre si e com a DEFINICAO.
    ///
    /// Gauss usa fracoes exatas e Bareiss nao usa fracao nenhuma; os dois chegam ao
    /// mesmo inteiro, e a definicao, que nao sabe o que e eliminacao, confirma.
    /// </summary>
    [Fact]
    public void OsDoisDeterminantesBatemComADefinicao()
    {
        foreach (var (nome, m) in Matrizes.Pequenas(20))
        {
            var pelaDefinicao = Metodos.DeterminantePelaDefinicao(m).Valor;

            Assert.Equal(pelaDefinicao, Metodos.DeterminantePorGauss(m).Valor);
            Assert.Equal(pelaDefinicao, Metodos.DeterminantePorBareiss(m).Valor);
        }

        for (var ordem = 1; ordem <= 7; ordem++)
            for (ulong semente = 1; semente <= 30; semente++)
            {
                var m = Matrizes.Sorteada(ordem, 5, semente * 17 + 3);
                var pelaDefinicao = Metodos.DeterminantePelaDefinicao(m).Valor;

                Assert.Equal(pelaDefinicao, Metodos.DeterminantePorGauss(m).Valor);
                Assert.Equal(pelaDefinicao, Metodos.DeterminantePorBareiss(m).Valor);
            }
    }

    /// <summary>
    /// TROCAR DUAS LINHAS inverte o determinante e NAO MEXE na permanente.
    ///
    /// E a diferenca inteira entre as duas contas, numa linha de teste: trocar duas
    /// linhas troca a paridade de toda permutacao, e a permanente nao olha para
    /// paridade.
    /// </summary>
    [Fact]
    public void TrocarLinhasInverteODeterminanteENaoMexeNaPermanente()
    {
        for (ulong semente = 1; semente <= 40; semente++)
        {
            var m = Matrizes.Sorteada(6, 5, semente);
            var trocada = m.ComLinhasTrocadas(1, 4);

            Assert.Equal(-Metodos.DeterminantePorBareiss(m).Valor, Metodos.DeterminantePorBareiss(trocada).Valor);
            Assert.Equal(Metodos.Ryser(m).Valor, Metodos.Ryser(trocada).Valor);
        }
    }

    /// <summary>
    /// As duas contas sobrevivem a TRANSPOSICAO, pelo mesmo motivo: somar sobre as
    /// permutacoes e somar sobre as inversas delas.
    /// </summary>
    [Fact]
    public void AsDuasContasNaoMudamComATransposta()
    {
        foreach (var (nome, m) in Matrizes.Pequenas(20))
        {
            Assert.Equal(Metodos.Ryser(m).Valor, Metodos.Ryser(m.Transposta()).Valor);
            Assert.Equal(Metodos.DeterminantePorBareiss(m).Valor, Metodos.DeterminantePorBareiss(m.Transposta()).Valor);
        }
    }

    /// <summary>
    /// A PERMANENTE de uma matriz nao negativa nunca e negativa, e o determinante e.
    ///
    /// A permanente e uma soma de produtos de numeros nao negativos; o determinante
    /// alterna. Parece obvio escrito assim, e nao e: as duas contas tem a mesma
    /// formula.
    /// </summary>
    [Fact]
    public void APermanenteNuncaEhNegativaEODeterminanteEh()
    {
        var negativos = 0;

        for (ulong semente = 1; semente <= 200; semente++)
        {
            var m = Matrizes.ZerosEUns(6, 0.5, semente);

            Assert.True(Metodos.Ryser(m).Valor >= 0);

            if (Metodos.DeterminantePorBareiss(m).Valor < 0) negativos++;
        }

        Assert.True(negativos > 30, $"so {negativos} determinantes negativos em 200");
    }

    /// <summary>
    /// A DEFINICAO se recusa a rodar acima de ordem onze, e isso e proposital.
    ///
    /// Doze de ordem sao quase seis bilhoes de multiplicacoes, e um juiz que trava a
    /// integracao continua deixa de ser juiz.
    /// </summary>
    [Fact]
    public void ADefinicaoSeRecusaAcimaDeOnze()
    {
        Assert.Throws<ArgumentException>(() => Metodos.PermanentePelaDefinicao(Matrizes.TodosUns(12)));
        Assert.Throws<ArgumentException>(() => Metodos.DeterminantePelaDefinicao(Matrizes.TodosUns(12)));
        Assert.Throws<ArgumentException>(() => Metodos.Ryser(Matrizes.TodosUns(25)));
    }

    [Fact]
    public void AMatrizDeOrdemZeroDaUm()
    {
        var vazia = new Matriz(0);

        Assert.Equal(1, Metodos.PermanentePelaDefinicao(vazia).Valor);
        Assert.Equal(1, Metodos.Ryser(vazia).Valor);
        Assert.Equal(1, Metodos.DeterminantePorGauss(vazia).Valor);
        Assert.Equal(1, Metodos.DeterminantePorBareiss(vazia).Valor);
    }
}
