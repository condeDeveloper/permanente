namespace Conde.Permanente;

/// <summary>
/// O SORTEIO, escrito aqui em vez de vir da biblioteca.
///
/// A documentacao do sorteio de biblioteca nao promete a mesma sequencia entre
/// versoes nem entre sistemas, e a integracao continua roda em tres. "A permanente
/// media de uma matriz de zeros e uns de 8 por 8 deu 1.247" com matrizes diferentes
/// em cada sistema nao e uma medida: sao tres medidas que ninguem pode comparar.
/// </summary>
public sealed class Sorteio
{
    private ulong estado;

    public Sorteio(ulong semente) => estado = semente == 0 ? 1 : semente;

    public uint Proximo()
    {
        estado = estado * 6364136223846793005UL + 1442695040888963407UL;
        return (uint)(estado >> 32);
    }

    public int Ate(int limite) => (int)(Proximo() % (uint)limite);

    public double Fracao() => Proximo() / 4294967296.0;
}

/// <summary>
/// Uma MATRIZ QUADRADA de inteiros, e as duas contas que se fazem sobre ela.
///
/// O DETERMINANTE e a PERMANENTE tem a MESMA formula, com uma diferenca de um
/// sinal: as duas somam, sobre todas as permutacoes, o produto de uma entrada por
/// linha; o determinante multiplica cada parcela pelo sinal da permutacao, e a
/// permanente nao.
///
/// E ai esta um dos fatos mais estranhos da computacao. O determinante sai em
/// tempo polinomial por eliminacao de Gauss, e e ensinado no primeiro semestre. A
/// permanente nao: Valiant provou em 1979 que ela e completa para a classe de
/// contagem, e ninguem conhece metodo rapido nem para matrizes de zeros e uns.
///
/// Tirar os sinais de uma formula transforma um exercicio num problema aberto.
/// </summary>
public sealed class Matriz
{
    private readonly long[,] valores;

    public Matriz(int ordem)
    {
        Ordem = ordem;
        valores = new long[ordem, ordem];
    }

    public Matriz(long[,] valores)
    {
        if (valores.GetLength(0) != valores.GetLength(1)) throw new ArgumentException("a matriz precisa ser quadrada", nameof(valores));

        Ordem = valores.GetLength(0);
        this.valores = (long[,])valores.Clone();
    }

    public int Ordem { get; }

    public long this[int linha, int coluna]
    {
        get => valores[linha, coluna];
        set => valores[linha, coluna] = value;
    }

    /// <summary>A matriz so tem ZEROS E UNS? E nesse caso que a permanente conta coisas.</summary>
    public bool EhDeZerosEUns()
    {
        for (var i = 0; i < Ordem; i++)
            for (var j = 0; j < Ordem; j++)
                if (valores[i, j] != 0 && valores[i, j] != 1) return false;

        return true;
    }

    public Matriz Transposta()
    {
        var outra = new Matriz(Ordem);

        for (var i = 0; i < Ordem; i++)
            for (var j = 0; j < Ordem; j++) outra[j, i] = valores[i, j];

        return outra;
    }

    /// <summary>A matriz com duas linhas trocadas: o determinante troca de sinal e a permanente nao.</summary>
    public Matriz ComLinhasTrocadas(int a, int b)
    {
        var outra = new Matriz(valores);

        for (var j = 0; j < Ordem; j++) (outra[a, j], outra[b, j]) = (outra[b, j], outra[a, j]);

        return outra;
    }

    public override string ToString() => $"matriz {Ordem}x{Ordem}";
}

/// <summary>
/// As MATRIZES que separam as contas, e por que elas sao assim.
///
/// Matriz sorteada de inteiros grandes nao separa quase nada: as duas contas dao
/// numeros diferentes e nenhum deles quer dizer coisa alguma. O que separa sao as
/// matrizes de ZEROS E UNS, em que a permanente CONTA alguma coisa: o numero de
/// maneiras de casar linhas com colunas usando so os uns.
/// </summary>
public static class Matrizes
{
    /// <summary>A IDENTIDADE: determinante um, permanente um.</summary>
    public static Matriz Identidade(int ordem)
    {
        var m = new Matriz(ordem);

        for (var i = 0; i < ordem; i++) m[i, i] = 1;

        return m;
    }

    /// <summary>
    /// TODOS UNS: determinante ZERO, permanente n FATORIAL.
    ///
    /// E o caso que mostra que as duas contas nao sao parentes proximas. Com seis
    /// por seis, o determinante e zero e a permanente e 720; com dez por dez, zero e
    /// tres milhoes e meio.
    /// </summary>
    public static Matriz TodosUns(int ordem)
    {
        var m = new Matriz(ordem);

        for (var i = 0; i < ordem; i++)
            for (var j = 0; j < ordem; j++) m[i, j] = 1;

        return m;
    }

    /// <summary>
    /// TRIANGULAR SUPERIOR com uns na diagonal: determinante um, permanente um.
    ///
    /// E o unico feitio em que as duas contas coincidem por motivo estrutural: so a
    /// permutacao identidade da produto nao nulo.
    /// </summary>
    public static Matriz Triangular(int ordem, ulong semente)
    {
        var sorteio = new Sorteio(semente);
        var m = new Matriz(ordem);

        for (var i = 0; i < ordem; i++)
        {
            m[i, i] = 1;

            for (var j = i + 1; j < ordem; j++) m[i, j] = sorteio.Ate(5);
        }

        return m;
    }

    /// <summary>Uma matriz de ZEROS E UNS sorteada: a permanente dela conta emparelhamentos.</summary>
    public static Matriz ZerosEUns(int ordem, double chance, ulong semente)
    {
        var sorteio = new Sorteio(semente);
        var m = new Matriz(ordem);

        for (var i = 0; i < ordem; i++)
            for (var j = 0; j < ordem; j++) m[i, j] = sorteio.Fracao() < chance ? 1 : 0;

        return m;
    }

    /// <summary>Inteiros pequenos sorteados: o caso em que nenhuma das duas contas conta nada.</summary>
    public static Matriz Sorteada(int ordem, int maior, ulong semente)
    {
        var sorteio = new Sorteio(semente);
        var m = new Matriz(ordem);

        for (var i = 0; i < ordem; i++)
            for (var j = 0; j < ordem; j++) m[i, j] = sorteio.Ate(maior);

        return m;
    }

    /// <summary>
    /// A matriz com uma LINHA DE ZEROS: as duas contas dao zero, por motivos
    /// diferentes.
    ///
    /// O determinante da zero porque a matriz e singular; a permanente da zero
    /// porque nenhum casamento consegue usar aquela linha. A mesma resposta por duas
    /// razoes que nao se parecem.
    /// </summary>
    public static Matriz ComLinhaDeZeros(int ordem)
    {
        var m = TodosUns(ordem);

        for (var j = 0; j < ordem; j++) m[0, j] = 0;

        return m;
    }

    public static IEnumerable<(string Nome, Matriz Matriz)> Pequenas(int sorteadas = 10)
    {
        yield return ("identidade 5", Identidade(5));
        yield return ("todos uns 5", TodosUns(5));
        yield return ("todos uns 7", TodosUns(7));
        yield return ("triangular 6", Triangular(6, 3));
        yield return ("linha de zeros 5", ComLinhaDeZeros(5));

        for (ulong semente = 1; semente <= (ulong)sorteadas; semente++)
            yield return ($"zeros e uns 6 ({semente})", ZerosEUns(6, 0.5, semente));

        for (ulong semente = 1; semente <= (ulong)sorteadas; semente++)
            yield return ($"sorteada 6 ({semente})", Sorteada(6, 5, semente));
    }

    /// <summary>
    /// As matrizes em que o SINAL aparece: aquelas em que as duas contas se afastam.
    /// </summary>
    public static IEnumerable<(string Nome, Matriz Matriz)> ParaOSinal()
    {
        yield return ("identidade 4", Identidade(4));
        yield return ("todos uns 4", TodosUns(4));
        yield return ("todos uns 6", TodosUns(6));
        yield return ("todos uns 8", TodosUns(8));
        yield return ("triangular 5", Triangular(5, 9));
        yield return ("linha de zeros 4", ComLinhaDeZeros(4));
        yield return ("zeros e uns 5", ZerosEUns(5, 0.5, 7));
        yield return ("zeros e uns 6", ZerosEUns(6, 0.6, 11));
        yield return ("sorteada 5", Sorteada(5, 4, 13));
    }

    /// <summary>
    /// So as de ZEROS E UNS, que sao as que o juiz independente sabe julgar.
    /// </summary>
    public static IEnumerable<(string Nome, Matriz Matriz)> DeZerosEUns()
    {
        yield return ("identidade 6", Identidade(6));
        yield return ("todos uns 6", TodosUns(6));
        yield return ("todos uns 8", TodosUns(8));
        yield return ("linha de zeros 6", ComLinhaDeZeros(6));

        foreach (var chance in new[] { 0.3, 0.5, 0.8 })
            for (ulong semente = 1; semente <= 4; semente++)
                yield return ($"zeros e uns 8 a {chance:0.0} ({semente})", ZerosEUns(8, chance, semente * 31 + 5));
    }
}
