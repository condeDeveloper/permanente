using Conde.Permanente;
using Xunit;

namespace Conde.Permanente.Testes;

public class MatrizTestes
{
    /// <summary>
    /// O SORTEIO da mesma semente da a mesma sequencia, e e por isso que ele esta
    /// escrito aqui em vez de vir da biblioteca.
    /// </summary>
    [Fact]
    public void OSorteioEhSempreOMesmo()
    {
        var um = new Sorteio(7);
        var outro = new Sorteio(7);

        for (var i = 0; i < 1_000; i++) Assert.Equal(um.Proximo(), outro.Proximo());
    }

    /// <summary>
    /// O DESLOCAMENTO de trinta e dois bits nao e enfeite: sem ele, o bit de baixo do
    /// gerador linear alterna par e impar a cada passo, e uma matriz de zeros e uns
    /// sorteada com esse bit vira um tabuleiro de xadrez.
    /// </summary>
    [Fact]
    public void OsBitsDeBaixoNaoServiriam()
    {
        var sorteio = new Sorteio(1);
        var trocas = 0;
        var anterior = -1;

        for (var i = 0; i < 1_000; i++)
        {
            var atual = sorteio.Ate(2);

            if (anterior >= 0 && atual != anterior) trocas++;

            anterior = atual;
        }

        // com os bits de baixo, isso daria 999 trocas, uma por passo
        Assert.InRange(trocas, 400, 600);
    }

    [Fact]
    public void AMatrizPrecisaSerQuadrada()
    {
        Assert.Throws<ArgumentException>(() => new Matriz(new long[2, 3]));
    }

    /// <summary>
    /// A matriz guarda uma COPIA dos valores, e nao o vetor que recebeu.
    /// </summary>
    [Fact]
    public void AMatrizNaoCompartilhaOVetor()
    {
        var valores = new long[2, 2];
        var m = new Matriz(valores);

        valores[0, 0] = 99;

        Assert.Equal(0, m[0, 0]);
    }

    [Fact]
    public void ATranspostaEhUmaInvolucao()
    {
        foreach (var (nome, m) in Matrizes.Pequenas(5))
        {
            var volta = m.Transposta().Transposta();

            for (var i = 0; i < m.Ordem; i++)
                for (var j = 0; j < m.Ordem; j++)
                    Assert.Equal(m[i, j], volta[i, j]);
        }
    }

    [Fact]
    public void AsMatrizesDeZerosEUnsSaoDeZerosEUns()
    {
        foreach (var (nome, m) in Matrizes.DeZerosEUns())
            Assert.True(m.EhDeZerosEUns(), nome);

        Assert.True(Matrizes.Identidade(5).EhDeZerosEUns());
        Assert.True(Matrizes.TodosUns(5).EhDeZerosEUns());
        Assert.False(Matrizes.Sorteada(5, 9, 1).EhDeZerosEUns());
    }

    /// <summary>
    /// A matriz de TODOS UNS: a permanente dela e n fatorial, porque TODA permutacao
    /// tem produto um. E o determinante dela e zero.
    /// </summary>
    [Fact]
    public void TodosUnsDaFatorialNaPermanenteEZeroNoDeterminante()
    {
        for (var ordem = 1; ordem <= 8; ordem++)
        {
            var m = Matrizes.TodosUns(ordem);

            Assert.Equal(Metodos.Fatorial(ordem), Metodos.Ryser(m).Valor);
            Assert.Equal(ordem == 1 ? 1 : 0, Metodos.DeterminantePorBareiss(m).Valor);
        }
    }

    /// <summary>
    /// A TRIANGULAR com uns na diagonal e o unico feitio em que as duas contas
    /// coincidem por motivo estrutural: so a identidade tem produto nao nulo.
    /// </summary>
    [Fact]
    public void NaTriangularAsDuasContasCoincidem()
    {
        for (ulong semente = 1; semente <= 20; semente++)
        {
            var m = Matrizes.Triangular(6, semente);

            Assert.Equal(1, Metodos.Ryser(m).Valor);
            Assert.Equal(1, Metodos.DeterminantePorBareiss(m).Valor);
        }
    }

    [Fact]
    public void ALinhaDeZerosZeraAsDuasContas()
    {
        for (var ordem = 2; ordem <= 8; ordem++)
        {
            var m = Matrizes.ComLinhaDeZeros(ordem);

            Assert.Equal(0, Metodos.Ryser(m).Valor);
            Assert.Equal(0, Metodos.DeterminantePorBareiss(m).Valor);
        }
    }
}
