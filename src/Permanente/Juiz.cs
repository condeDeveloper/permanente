namespace Conde.Permanente;

/// <summary>
/// O JUIZ INDEPENDENTE: a permanente de uma matriz de ZEROS E UNS conta os
/// EMPARELHAMENTOS PERFEITOS de um grafo bipartido.
///
/// Pense nas linhas como pessoas e nas colunas como tarefas, com um na posicao i,j
/// quando a pessoa i sabe fazer a tarefa j. Uma permutacao com produto um e
/// exatamente uma maneira de dar uma tarefa diferente para cada pessoa; o produto
/// de qualquer outra permutacao e zero. Entao a soma da definicao CONTA as
/// distribuicoes possiveis.
///
/// E esse e o segundo juiz do repositorio, e o importante dele e que ele NAO sabe
/// o que e permanente. Ele percorre o grafo escolhendo tarefa por tarefa e conta
/// quantas folhas da arvore chegam ao fim. Nenhuma multiplicacao, nenhuma soma
/// sobre permutacoes, nenhuma formula: so contagem.
///
/// Dois metodos que chegam ao mesmo numero por caminhos que nao se parecem valem
/// mais do que um metodo conferido duas vezes.
/// </summary>
public static class Juiz
{
    /// <param name="Quantos">quantos emparelhamentos perfeitos existem</param>
    /// <param name="Passos">quantos nos da arvore de busca foram visitados</param>
    public record Contagem(long Quantos, long Passos);

    /// <summary>
    /// Conta os EMPARELHAMENTOS PERFEITOS percorrendo a arvore de escolhas, com
    /// pilha explicita.
    ///
    /// Pilha explicita, e nao recursao, porque a profundidade e a ordem da matriz e
    /// porque ja tomei um estouro de pilha no macOS num repositorio anterior por
    /// confiar na profundidade "pequena" de uma arvore.
    /// </summary>
    public static Contagem Emparelhamentos(Matriz m)
    {
        if (!m.EhDeZerosEUns()) throw new ArgumentException("so faz sentido com zeros e uns", nameof(m));

        var n = m.Ordem;
        var quantos = 0L;
        var passos = 0L;

        // cada no da arvore: a linha que estamos distribuindo e quais colunas ja foram
        var pilha = new Stack<(int Linha, int Usadas)>();

        pilha.Push((0, 0));

        while (pilha.Count > 0)
        {
            var (linha, usadas) = pilha.Pop();

            passos++;

            if (linha == n) { quantos++; continue; }

            for (var coluna = 0; coluna < n; coluna++)
            {
                if (m[linha, coluna] == 0) continue;
                if ((usadas & (1 << coluna)) != 0) continue;

                pilha.Push((linha + 1, usadas | (1 << coluna)));
            }
        }

        return new Contagem(quantos, passos);
    }

    /// <summary>
    /// A mesma contagem por PROGRAMACAO DINAMICA sobre os subconjuntos de colunas.
    ///
    /// Em dois elevado a n vezes n, que e o mesmo custo de Ryser, mas sem nenhum
    /// sinal e sem nenhuma multiplicacao: e uma soma de caminhos. Serve de terceira
    /// conferencia e de medida: a permanente de zeros e uns tem um metodo que parece
    /// mais simples que Ryser, e mesmo assim nao e polinomial.
    /// </summary>
    public static long EmparelhamentosPorTabela(Matriz m)
    {
        if (!m.EhDeZerosEUns()) throw new ArgumentException("so faz sentido com zeros e uns", nameof(m));

        var n = m.Ordem;
        var tabela = new long[1 << n];

        tabela[0] = 1;

        for (var usadas = 0; usadas < 1 << n; usadas++)
        {
            if (tabela[usadas] == 0) continue;

            var linha = System.Numerics.BitOperations.PopCount((uint)usadas);

            if (linha == n) continue;

            for (var coluna = 0; coluna < n; coluna++)
            {
                if (m[linha, coluna] == 0) continue;
                if ((usadas & (1 << coluna)) != 0) continue;

                tabela[usadas | (1 << coluna)] += tabela[usadas];
            }
        }

        return tabela[(1 << n) - 1];
    }

    /// <summary>
    /// EXISTE algum emparelhamento perfeito? Pelo caminho aumentante, em tempo
    /// polinomial.
    ///
    /// E a medida mais estranha do repositorio. Perguntar SE existe uma distribuicao
    /// e facil; perguntar QUANTAS existem e completo para a classe de contagem. A
    /// mesma matriz, duas perguntas, dois mundos.
    /// </summary>
    public static (bool Existe, long Passos) ExisteEmparelhamento(Matriz m)
    {
        var n = m.Ordem;
        var casouCom = new int[n];
        var passos = 0L;

        Array.Fill(casouCom, -1);

        for (var linha = 0; linha < n; linha++)
        {
            var visitada = new bool[n];

            if (!Aumenta(m, linha, visitada, casouCom, ref passos)) return (false, passos);
        }

        return (true, passos);
    }

    /// <summary>
    /// O CAMINHO AUMENTANTE de Kuhn, recursivo.
    ///
    /// Recursivo aqui e nao em pilha explicita porque a profundidade e no maximo a
    /// ordem da matriz, e a ordem nunca passa de vinte e quatro neste repositorio:
    /// acima disso a propria formula de Ryser nao cabe no tempo de ninguem.
    /// </summary>
    private static bool Aumenta(Matriz m, int linha, bool[] visitada, int[] casouCom, ref long passos)
    {
        for (var coluna = 0; coluna < m.Ordem; coluna++)
        {
            passos++;

            if (m[linha, coluna] == 0 || visitada[coluna]) continue;

            visitada[coluna] = true;

            if (casouCom[coluna] == -1 || Aumenta(m, casouCom[coluna], visitada, casouCom, ref passos))
            {
                casouCom[coluna] = linha;
                return true;
            }
        }

        return false;
    }
}
