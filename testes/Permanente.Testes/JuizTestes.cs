using Conde.Permanente;
using Xunit;

namespace Conde.Permanente.Testes;

/// <summary>
/// O JUIZ INDEPENDENTE, que nao sabe o que e permanente.
///
/// Ele percorre a arvore de distribuicoes e conta folhas. Se as duas contas batessem
/// porque uma chama a outra, isso nao seria conferencia nenhuma; elas batem porque
/// contar distribuicoes E a permanente de uma matriz de zeros e uns, e esse e um
/// teorema, nao uma escolha de implementacao.
/// </summary>
public class JuizTestes
{
    /// <summary>
    /// A permanente de uma matriz de ZEROS E UNS e o numero de EMPARELHAMENTOS
    /// PERFEITOS, pelos tres caminhos.
    /// </summary>
    [Fact]
    public void APermanenteDeZerosEUnsContaEmparelhamentos()
    {
        foreach (var (nome, m) in Matrizes.DeZerosEUns())
        {
            var permanente = Metodos.Ryser(m).Valor;

            Assert.Equal(permanente, Juiz.Emparelhamentos(m).Quantos);
            Assert.Equal(permanente, Juiz.EmparelhamentosPorTabela(m));
        }

        for (var ordem = 1; ordem <= 8; ordem++)
            foreach (var chance in new[] { 0.2, 0.5, 0.8 })
                for (ulong semente = 1; semente <= 20; semente++)
                {
                    var m = Matrizes.ZerosEUns(ordem, chance, semente * 101 + (ulong)ordem);
                    var permanente = Metodos.Ryser(m).Valor;

                    Assert.Equal(permanente, Juiz.Emparelhamentos(m).Quantos);
                    Assert.Equal(permanente, Juiz.EmparelhamentosPorTabela(m));
                }
    }

    /// <summary>
    /// E a DEFINICAO, que e o primeiro juiz, confirma o segundo.
    /// </summary>
    [Fact]
    public void ADefinicaoConfirmaOJuiz()
    {
        for (var ordem = 1; ordem <= 7; ordem++)
            for (ulong semente = 1; semente <= 25; semente++)
            {
                var m = Matrizes.ZerosEUns(ordem, 0.6, semente * 7 + 2);

                Assert.Equal(Metodos.PermanentePelaDefinicao(m).Valor, Juiz.Emparelhamentos(m).Quantos);
            }
    }

    /// <summary>
    /// O juiz se recusa a julgar matriz que nao e de zeros e uns, porque fora dai a
    /// permanente nao conta coisa nenhuma.
    /// </summary>
    [Fact]
    public void OJuizSoJulgaZerosEUns()
    {
        var m = Matrizes.Sorteada(5, 9, 1);

        Assert.Throws<ArgumentException>(() => Juiz.Emparelhamentos(m));
        Assert.Throws<ArgumentException>(() => Juiz.EmparelhamentosPorTabela(m));
    }

    /// <summary>
    /// O ACHADO PRINCIPAL: perguntar SE existe uma distribuicao e barato, e perguntar
    /// QUANTAS existem e caro.
    ///
    /// Na matriz de todos uns de ordem oito, saber que existe custa cento e vinte
    /// passos; saber que sao quarenta mil trezentas e vinte custa mais de cem mil.
    /// A mesma matriz, duas perguntas que soam parecidas, dois mundos de custo.
    ///
    /// E nao e um detalhe de implementacao: a primeira pergunta esta em tempo
    /// polinomial desde sempre, e a segunda e completa para a classe de contagem.
    /// </summary>
    [Fact]
    public void SaberSeExisteEhMuitoMaisBaratoQueContar()
    {
        var m = Matrizes.TodosUns(8);

        var existe = Juiz.ExisteEmparelhamento(m);
        var contar = Juiz.Emparelhamentos(m);

        Assert.True(existe.Existe);
        Assert.Equal(40_320, contar.Quantos);

        Assert.True(existe.Passos < 200, $"existe custou {existe.Passos}");
        Assert.True(contar.Passos > 100_000, $"contar custou {contar.Passos}");
        Assert.True(contar.Passos > 500 * existe.Passos);
    }

    /// <summary>
    /// E as duas perguntas concordam no essencial: existe se e so se a permanente nao
    /// e zero.
    /// </summary>
    [Fact]
    public void ExisteSeESomenteSeAPermanenteNaoEhZero()
    {
        var quantas = 0;
        var comZero = 0;

        foreach (var chance in new[] { 0.2, 0.3, 0.4, 0.5, 0.7 })
            for (ulong semente = 1; semente <= 60; semente++)
            {
                var m = Matrizes.ZerosEUns(7, chance, semente * 29 + 1);
                var permanente = Metodos.Ryser(m).Valor;

                Assert.Equal(permanente > 0, Juiz.ExisteEmparelhamento(m).Existe);

                quantas++;
                if (permanente == 0) comZero++;
            }

        // e os dois lados aparecem de verdade na amostra
        Assert.True(comZero > 20, $"so {comZero} com permanente zero");
        Assert.True(comZero < quantas - 20, $"{comZero} de {quantas} com permanente zero");
    }

    /// <summary>
    /// A matriz de TODOS UNS da n fatorial pela arvore tambem: toda distribuicao
    /// serve.
    /// </summary>
    [Fact]
    public void TodosUnsDaFatorialPelaArvore()
    {
        for (var ordem = 1; ordem <= 8; ordem++)
            Assert.Equal(Metodos.Fatorial(ordem), Juiz.Emparelhamentos(Matrizes.TodosUns(ordem)).Quantos);
    }

    /// <summary>
    /// A IDENTIDADE da UM: existe uma unica maneira de dar uma tarefa a cada um
    /// quando cada um sabe fazer so a sua.
    ///
    /// E a arvore gasta n mais um passos nisso, que e o caso mais barato possivel.
    /// </summary>
    [Fact]
    public void AIdentidadeDaUmEmUmCaminhoSo()
    {
        for (var ordem = 1; ordem <= 10; ordem++)
        {
            var conta = Juiz.Emparelhamentos(Matrizes.Identidade(ordem));

            Assert.Equal(1, conta.Quantos);
            Assert.Equal(ordem + 1, conta.Passos);
        }
    }
}
