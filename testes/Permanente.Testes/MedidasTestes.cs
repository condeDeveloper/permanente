using Conde.Permanente;
using Xunit;

namespace Conde.Permanente.Testes;

/// <summary>
/// Os testes das TABELAS que o medidor imprime.
///
/// Toda frase do texto que e um numero esta aqui como afirmacao, porque texto nao
/// quebra quando deixa de ser verdade e teste quebra.
/// </summary>
public class MedidasTestes
{
    [Fact]
    public void RyserBateEmTodaLinhaECustaMenos()
    {
        var linhas = Medidas.RyserContraADefinicao();

        Assert.Equal(13, linhas.Count);
        Assert.All(linhas, l => Assert.True(l.Batem, l.Matriz));
        Assert.All(linhas, l => Assert.True(l.ContasDeRyser < l.ContasDaDefinicao, l.Matriz));

        // na de ordem sete a diferenca ja passa de trinta e cinco vezes
        var sete = linhas.Single(l => l.Matriz == "todos uns 7");

        Assert.Equal(5_040, sete.PorRyser);
        Assert.True(sete.ContasDaDefinicao > 35 * sete.ContasDeRyser);
    }

    /// <summary>
    /// A frase do texto: trocar duas linhas inverte o determinante e nao mexe na
    /// permanente, e em todos uns as duas contas estao o mais longe possivel.
    /// </summary>
    [Fact]
    public void OSinalInverteODeterminanteENaoAPermanente()
    {
        var linhas = Medidas.OQueOSinalFaz();

        Assert.Equal(9, linhas.Count);
        Assert.All(linhas, l => Assert.Equal(-l.Determinante, l.DeterminanteTrocado));
        Assert.All(linhas, l => Assert.Equal(l.Permanente, l.PermanenteTrocada));

        var oito = linhas.Single(l => l.Matriz == "todos uns 8");

        Assert.Equal(0, oito.Determinante);
        Assert.Equal(40_320, oito.Permanente);
    }

    /// <summary>
    /// A frase do texto: os dois determinantes polinomiais batem em toda linha, e a
    /// razao entre Ryser e Gauss cresce sem parar.
    /// </summary>
    [Fact]
    public void ARazaoEntreRyserEGaussCresce()
    {
        var linhas = Medidas.OCustoDasDuasContas();

        Assert.Equal(13, linhas.Count);
        Assert.All(linhas, l => Assert.True(l.OsDoisDeterminantesBatem, $"ordem {l.Ordem}"));

        var primeira = linhas.First(l => l.Ordem == 4);
        var ultima = linhas.Single(l => l.Ordem == 15);

        Assert.True(ultima.RyserSobreGauss > 100 * primeira.RyserSobreGauss);
        Assert.True(ultima.ContasDaDefinicao > 1_000_000L * ultima.ContasDeRyser);
    }

    /// <summary>
    /// O ACHADO QUE ME CORRIGIU: o custo de Gauss NAO cresce de forma monotona com a
    /// ordem, e o de Bareiss cresce.
    ///
    /// Eu escrevi a tabela esperando duas colunas crescentes. Gauss pula a linha
    /// inteira quando a entrada da coluna ja e zero, e desiste assim que acha uma
    /// coluna sem pivo, entao numa matriz de zeros e uns singular ele cobra menos na
    /// ordem doze do que na onze. Bareiss nao pula nada e cresce sempre.
    /// </summary>
    [Fact]
    public void OCustoDeGaussNaoEhMonotonoEODeBareissEh()
    {
        var linhas = Medidas.OCustoDasDuasContas();

        var caiu = 0;

        for (var i = 1; i < linhas.Count; i++)
        {
            if (linhas[i].ContasDeGauss < linhas[i - 1].ContasDeGauss) caiu++;

            Assert.True(linhas[i].ContasDeBareiss > linhas[i - 1].ContasDeBareiss, $"ordem {linhas[i].Ordem}");
        }

        Assert.True(caiu > 0, "nenhuma queda no custo de Gauss");

        // e Bareiss segue a conta do cubo: um terco de n ao cubo, mais ou menos
        var quinze = linhas.Single(l => l.Ordem == 15);

        Assert.InRange(quinze.ContasDeBareiss, 900, 1_200);
    }

    /// <summary>
    /// A frase do texto: os tres caminhos de contagem batem, e perguntar SE existe e
    /// muito mais barato que perguntar QUANTAS.
    /// </summary>
    [Fact]
    public void ContarCustaMuitoMaisQueSaberSeExiste()
    {
        var linhas = Medidas.ContarEhOutraCoisa();

        Assert.Equal(16, linhas.Count);
        Assert.All(linhas, l => Assert.True(l.Batem, l.Matriz));
        Assert.All(linhas, l => Assert.Equal(l.Permanente > 0, l.Existe));

        var oito = linhas.Single(l => l.Matriz == "todos uns 8");

        Assert.Equal(40_320, oito.Permanente);
        Assert.True(oito.PassosDaArvore > 100_000);
        Assert.True(oito.PassosDoExiste < 200);
    }

    /// <summary>
    /// A frase do texto: a transicao da densidade e abrupta.
    ///
    /// Com um decimo de uns, as duzentas matrizes dao permanente zero; com sete
    /// decimos, nenhuma da.
    /// </summary>
    [Fact]
    public void ATransicaoDaDensidadeEhAbrupta()
    {
        var linhas = Medidas.ADensidadeDosUns();

        Assert.Equal(10, linhas.Count);

        var um = linhas.Single(l => l.Chance == 0.1);
        var sete = linhas.Single(l => l.Chance == 0.7);
        var cheia = linhas.Single(l => l.Chance == 1.0);

        Assert.Equal(200, um.QuantasTemZero);
        Assert.Equal(0, sete.QuantasTemZero);
        Assert.Equal(0, cheia.QuantasTemZero);
        Assert.Equal(40_320, cheia.MaiorPermanente);
        Assert.Equal(40_320, cheia.PermanenteMedia);

        // e tanto a permanente media quanto o custo da arvore so crescem
        for (var i = 1; i < linhas.Count; i++)
        {
            Assert.True(linhas[i].PermanenteMedia >= linhas[i - 1].PermanenteMedia, $"{linhas[i].Chance}");
            Assert.True(linhas[i].PassosMedios > linhas[i - 1].PassosMedios, $"{linhas[i].Chance}");
        }
    }

    /// <summary>
    /// A frase do texto: com um orcamento de um bilhao de contas, Gauss chega a ordem
    /// mil, Ryser para perto de vinte e cinco e a definicao para em doze.
    /// </summary>
    [Fact]
    public void OOrcamentoDeUmBilhaoSeparaOsTres()
    {
        var linhas = Medidas.AteOndeCadaUmChega();

        Assert.Equal(10, linhas.Count);

        Assert.Equal("os tres", linhas.Single(l => l.Ordem == 10).QuemCabe);
        Assert.Equal("Gauss e Ryser", linhas.Single(l => l.Ordem == 12).QuemCabe);
        Assert.Equal("Gauss e Ryser", linhas.Single(l => l.Ordem == 25).QuemCabe);
        Assert.Equal("so Gauss", linhas.Single(l => l.Ordem == 30).QuemCabe);
        Assert.Equal("so Gauss", linhas.Single(l => l.Ordem == 1_000).QuemCabe);

        // em ordem mil, Gauss cobra exatamente um bilhao
        Assert.Equal(1e9, linhas.Single(l => l.Ordem == 1_000).Gauss, 0);
    }
}
