namespace Conde.Permanente;

/// <summary>
/// OS METODOS, e o assunto todo esta na diferenca entre eles.
///
/// - A DEFINICAO soma, sobre todas as n! permutacoes, o produto de uma entrada por
///   linha. E o juiz, e ela e a mesma para as duas contas a menos do sinal.
///
/// - RYSER, 1963: a permanente por inclusao e exclusao sobre os SUBCONJUNTOS de
///   colunas. Dois elevado a n vezes n, contra n fatorial vezes n. Com doze, isso e
///   a diferenca entre cinquenta mil e quase seis bilhoes.
///
/// - GAUSS: o determinante por eliminacao, em n ao cubo. Para a permanente nao
///   existe nada parecido, e nao e por falta de tentativa: Valiant provou em 1979
///   que ela e completa para a classe de contagem.
///
/// Tirar os sinais de uma formula transforma um exercicio de primeiro semestre num
/// problema aberto, e as tabelas deste repositorio sao sobre esse abismo.
/// </summary>
public static class Metodos
{
    /// <param name="Valor">o resultado da conta</param>
    /// <param name="Contas">quantas multiplicacoes foram feitas</param>
    public record Resultado(long Valor, long Contas);

    /// <summary>
    /// A PERMANENTE pela definicao: a soma sobre todas as permutacoes.
    ///
    /// Custa n fatorial vezes n multiplicacoes, e e por isso que ela e juiz e nao
    /// algoritmo: com doze de ordem sao quase seis bilhoes.
    /// </summary>
    public static Resultado PermanentePelaDefinicao(Matriz m)
    {
        if (m.Ordem > 11) throw new ArgumentException("a definicao so vai ate ordem 11", nameof(m));

        var total = 0L;
        var contas = 0L;

        foreach (var p in Permutacoes(m.Ordem))
        {
            var produto = 1L;

            for (var i = 0; i < m.Ordem; i++)
            {
                contas++;
                produto *= m[i, p[i]];
            }

            total += produto;
        }

        return new Resultado(total, contas);
    }

    /// <summary>
    /// O DETERMINANTE pela definicao: a mesma soma, com o SINAL de cada permutacao.
    ///
    /// Ele esta aqui para a comparacao ser honesta: as duas contas, pelo mesmo
    /// caminho, custam exatamente a mesma coisa. A diferenca de custo entre elas
    /// aparece so quando se usa o metodo bom de cada uma, e para a permanente nao
    /// existe metodo bom.
    /// </summary>
    public static Resultado DeterminantePelaDefinicao(Matriz m)
    {
        if (m.Ordem > 11) throw new ArgumentException("a definicao so vai ate ordem 11", nameof(m));

        var total = 0L;
        var contas = 0L;

        foreach (var p in Permutacoes(m.Ordem))
        {
            var produto = (long)Sinal(p);

            for (var i = 0; i < m.Ordem; i++)
            {
                contas++;
                produto *= m[i, p[i]];
            }

            total += produto;
        }

        return new Resultado(total, contas);
    }

    /// <summary>
    /// A FORMULA DE RYSER, de 1963: inclusao e exclusao sobre os subconjuntos de
    /// colunas.
    ///
    /// Para cada subconjunto S de colunas, some os produtos das somas das linhas
    /// restritas a S, e alterne o sinal pela paridade do que ficou de fora. O que
    /// sai disso e a permanente, e custa dois elevado a n vezes n.
    ///
    /// Continua exponencial, e o problema continua sendo dificil. Mas com ordem doze
    /// a diferenca entre os dois metodos e a diferenca entre cinquenta mil contas e
    /// seis bilhoes.
    /// </summary>
    public static Resultado Ryser(Matriz m)
    {
        if (m.Ordem > 24) throw new ArgumentException("Ryser so vai ate ordem 24", nameof(m));
        if (m.Ordem == 0) return new Resultado(1, 0);

        var n = m.Ordem;
        var total = 0L;
        var contas = 0L;

        for (var mascara = 1; mascara < 1 << n; mascara++)
        {
            var produto = 1L;

            for (var i = 0; i < n; i++)
            {
                var soma = 0L;

                for (var j = 0; j < n; j++)
                    if ((mascara & (1 << j)) != 0) soma += m[i, j];

                contas++;
                produto *= soma;

                if (produto == 0) break;
            }

            // o sinal vem da paridade do numero de colunas DE FORA
            var deFora = n - System.Numerics.BitOperations.PopCount((uint)mascara);

            total += deFora % 2 == 0 ? produto : -produto;
        }

        return new Resultado(total, contas);
    }

    /// <summary>
    /// O DETERMINANTE por ELIMINACAO DE GAUSS, em fracoes exatas.
    ///
    /// Em n ao cubo, e e esse o contraste do repositorio: a conta que tem o sinal
    /// sai em tempo polinomial, e a que nao tem nao sai.
    ///
    /// A eliminacao e feita em fracoes exatas, e nao em ponto flutuante: um
    /// determinante inteiro calculado em double sai como 5,999999999999998 e a
    /// comparacao com a definicao viraria uma discussao sobre tolerancia em vez de
    /// uma conferencia.
    /// </summary>
    public static Resultado DeterminantePorGauss(Matriz m)
    {
        var n = m.Ordem;
        var a = new (long Cima, long Baixo)[n, n];
        var contas = 0L;

        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++) a[i, j] = (m[i, j], 1);

        var sinal = 1;

        for (var coluna = 0; coluna < n; coluna++)
        {
            var pivo = -1;

            for (var i = coluna; i < n && pivo < 0; i++)
                if (a[i, coluna].Cima != 0) pivo = i;

            if (pivo < 0) return new Resultado(0, contas);

            if (pivo != coluna)
            {
                for (var j = 0; j < n; j++) (a[coluna, j], a[pivo, j]) = (a[pivo, j], a[coluna, j]);

                sinal = -sinal;
            }

            for (var i = coluna + 1; i < n; i++)
            {
                if (a[i, coluna].Cima == 0) continue;

                var fator = Dividir(a[i, coluna], a[coluna, coluna]);

                for (var j = coluna; j < n; j++)
                {
                    contas++;
                    a[i, j] = Subtrair(a[i, j], Multiplicar(fator, a[coluna, j]));
                }
            }
        }

        var resultado = (Cima: (long)sinal, Baixo: 1L);

        for (var i = 0; i < n; i++) resultado = Multiplicar(resultado, a[i, i]);

        return new Resultado(resultado.Cima / resultado.Baixo, contas);
    }

    /// <summary>
    /// O DETERMINANTE por BAREISS: a mesma eliminacao, SEM FRACAO NENHUMA.
    ///
    /// Cada passo divide por um pivo anterior, e a divisao e sempre exata: todos os
    /// numeros intermediarios sao determinantes de submatrizes, que sao inteiros.
    /// Tambem em n ao cubo.
    ///
    /// Ele esta aqui por dois motivos. O primeiro e conferir Gauss: as fracoes do
    /// Gauss crescem, e um numerador que estoura o inteiro de sessenta e quatro bits
    /// nao avisa, so devolve um numero errado. O segundo e o contraste do
    /// repositorio: o determinante tem VARIOS metodos polinomiais, e a permanente nao
    /// tem nenhum.
    /// </summary>
    public static Resultado DeterminantePorBareiss(Matriz m)
    {
        var n = m.Ordem;

        if (n == 0) return new Resultado(1, 0);

        var a = new long[n, n];
        var contas = 0L;

        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++) a[i, j] = m[i, j];

        var sinal = 1L;
        var anterior = 1L;

        for (var coluna = 0; coluna < n - 1; coluna++)
        {
            if (a[coluna, coluna] == 0)
            {
                var troca = -1;

                for (var i = coluna + 1; i < n && troca < 0; i++)
                    if (a[i, coluna] != 0) troca = i;

                if (troca < 0) return new Resultado(0, contas);

                for (var j = 0; j < n; j++) (a[coluna, j], a[troca, j]) = (a[troca, j], a[coluna, j]);

                sinal = -sinal;
            }

            for (var i = coluna + 1; i < n; i++)
                for (var j = coluna + 1; j < n; j++)
                {
                    contas++;
                    a[i, j] = (a[i, j] * a[coluna, coluna] - a[i, coluna] * a[coluna, j]) / anterior;
                }

            anterior = a[coluna, coluna];
        }

        return new Resultado(sinal * a[n - 1, n - 1], contas);
    }

    private static (long Cima, long Baixo) Simplificar((long Cima, long Baixo) f)
    {
        if (f.Cima == 0) return (0, 1);

        var a = Math.Abs(f.Cima);
        var b = Math.Abs(f.Baixo);

        while (b != 0) (a, b) = (b, a % b);

        var sinal = f.Baixo < 0 ? -1 : 1;

        return (sinal * f.Cima / a, sinal * f.Baixo / a);
    }

    private static (long, long) Multiplicar((long Cima, long Baixo) x, (long Cima, long Baixo) y) =>
        Simplificar((x.Cima * y.Cima, x.Baixo * y.Baixo));

    private static (long, long) Dividir((long Cima, long Baixo) x, (long Cima, long Baixo) y) =>
        Simplificar((x.Cima * y.Baixo, x.Baixo * y.Cima));

    private static (long, long) Subtrair((long Cima, long Baixo) x, (long Cima, long Baixo) y) =>
        Simplificar((x.Cima * y.Baixo - y.Cima * x.Baixo, x.Baixo * y.Baixo));

    /// <summary>
    /// O SINAL de uma permutacao: mais um com um numero par de inversoes, menos um
    /// com impar.
    /// </summary>
    public static int Sinal(IReadOnlyList<int> p)
    {
        var inversoes = 0;

        for (var i = 0; i < p.Count; i++)
            for (var j = i + 1; j < p.Count; j++)
                if (p[i] > p[j]) inversoes++;

        return inversoes % 2 == 0 ? 1 : -1;
    }

    /// <summary>
    /// Todas as permutacoes de n elementos, pelo algoritmo de Heap, SEM RECURSAO.
    ///
    /// Sem recursao porque a profundidade seria a ordem da matriz e porque o
    /// algoritmo de Heap iterativo gera cada permutacao com UMA troca a partir da
    /// anterior, o que e o que torna possivel percorrer milhoes delas.
    /// </summary>
    public static IEnumerable<int[]> Permutacoes(int n)
    {
        var atual = Enumerable.Range(0, n).ToArray();
        var contador = new int[n];

        yield return atual;

        var i = 0;

        while (i < n)
        {
            if (contador[i] >= i) { contador[i] = 0; i++; continue; }

            var j = i % 2 == 0 ? 0 : contador[i];

            (atual[i], atual[j]) = (atual[j], atual[i]);

            yield return atual;

            contador[i]++;
            i = 0;
        }
    }

    public static long Fatorial(int n)
    {
        var f = 1L;

        for (var i = 2; i <= n; i++) f *= i;

        return f;
    }
}
