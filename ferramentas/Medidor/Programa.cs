using Conde.Permanente;

var comando = args.Length > 0 ? args[0] : "tudo";

if (comando is "definicao" or "tudo") Definicao();
if (comando is "sinal" or "tudo") Sinal();
if (comando is "contar" or "tudo") Contar();
if (comando is "custo" or "tudo") Custo();
if (comando is "densidade" or "tudo") Densidade();
if (comando is "alcance" or "tudo") Alcance();

static void Definicao()
{
    Console.WriteLine();
    Console.WriteLine("RYSER contra a DEFINICAO: o mesmo numero, por caminhos de custos bem diferentes.");
    Console.WriteLine();
    Console.WriteLine($"  {"matriz",-24} {"ordem",5} {"pela definicao",15} {"por Ryser",12} {"batem",6} {"contas da definicao",20} {"contas de Ryser",16}");

    foreach (var l in Medidas.RyserContraADefinicao())
        Console.WriteLine($"  {l.Matriz,-24} {l.Ordem,5} {l.PelaDefinicao,15:n0} {l.PorRyser,12:n0} {(l.Batem ? "sim" : "NAO"),6} {l.ContasDaDefinicao,20:n0} {l.ContasDeRyser,16:n0}");

    Console.WriteLine();
}

static void Sinal()
{
    Console.WriteLine();
    Console.WriteLine("O SINAL: a mesma formula, e o que ele faz com ela.");
    Console.WriteLine();
    Console.WriteLine($"  {"matriz",-20} {"ordem",5} {"determinante",14} {"permanente",14} {"det. com 2 linhas trocadas",28} {"perm. trocada",14}");

    foreach (var l in Medidas.OQueOSinalFaz())
        Console.WriteLine($"  {l.Matriz,-20} {l.Ordem,5} {l.Determinante,14:n0} {l.Permanente,14:n0} {l.DeterminanteTrocado,28:n0} {l.PermanenteTrocada,14:n0}");

    Console.WriteLine();
}

static void Contar()
{
    Console.WriteLine();
    Console.WriteLine("CONTAR e outra coisa: a permanente de zeros e uns conta distribuicoes.");
    Console.WriteLine();
    Console.WriteLine($"  {"matriz",-26} {"permanente",12} {"pela arvore",12} {"pela tabela",12} {"batem",6} {"passos de quantas",18} {"passos de existe",17}");

    foreach (var l in Medidas.ContarEhOutraCoisa())
        Console.WriteLine($"  {l.Matriz,-26} {l.Permanente,12:n0} {l.Emparelhamentos,12:n0} {l.PorTabela,12:n0} {(l.Batem ? "sim" : "NAO"),6} {l.PassosDaArvore,18:n0} {l.PassosDoExiste,17:n0}");

    Console.WriteLine();
}

static void Custo()
{
    Console.WriteLine();
    Console.WriteLine("O ABISMO: as duas contas na mesma matriz, ordem por ordem.");
    Console.WriteLine();
    Console.WriteLine($"  {"ordem",5} {"determinante",14} {"permanente",14} {"batem",6} {"Gauss",8} {"Bareiss",8} {"Ryser",10} {"a definicao",20} {"Ryser sobre Gauss",18}");

    foreach (var l in Medidas.OCustoDasDuasContas())
        Console.WriteLine($"  {l.Ordem,5} {l.Determinante,14:n0} {l.Permanente,14:n0} {(l.OsDoisDeterminantesBatem ? "sim" : "NAO"),6} {l.ContasDeGauss,8:n0} {l.ContasDeBareiss,8:n0} {l.ContasDeRyser,10:n0} {l.ContasDaDefinicao,20:n0} {l.RyserSobreGauss,18:n1}");

    Console.WriteLine();
}

static void Densidade()
{
    Console.WriteLine();
    Console.WriteLine("A DENSIDADE dos uns: quantos uns sao precisos para a resposta deixar de ser zero.");
    Console.WriteLine();
    Console.WriteLine($"  {"chance de um",12} {"ordem",5} {"permanente media",17} {"maior permanente",17} {"quantas deram zero",19} {"passos medios da arvore",24}");

    foreach (var l in Medidas.ADensidadeDosUns())
        Console.WriteLine($"  {l.Chance,12:0.0} {l.Ordem,5} {l.PermanenteMedia,17:n0} {l.MaiorPermanente,17:n0} {l.QuantasTemZero,19} {l.PassosMedios,24:n0}");

    Console.WriteLine();
}

static void Alcance()
{
    Console.WriteLine();
    Console.WriteLine("ATE ONDE cada metodo chega, com um orcamento de um bilhao de contas.");
    Console.WriteLine();
    Console.WriteLine($"  {"ordem",6} {"definicao",22} {"Ryser",22} {"Gauss",16} {"quem cabe",16}");

    foreach (var l in Medidas.AteOndeCadaUmChega())
        Console.WriteLine($"  {l.Ordem,6} {l.Definicao,22:0.00e+0} {l.Ryser,22:0.00e+0} {l.Gauss,16:n0} {l.QuemCabe,16}");

    Console.WriteLine();
}
