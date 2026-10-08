# permanente

O determinante e a permanente do zero em C# e .NET 8. As duas contas têm a **mesma
fórmula**: somam, sobre todas as permutações, o produto de uma entrada por linha. A
única diferença é que o determinante multiplica cada parcela pelo **sinal** da
permutação, e a permanente não.

O juiz é a **definição**, escrita exatamente assim. E há um **segundo juiz** que não
sabe o que é permanente: numa matriz de zeros e uns, a permanente **conta** os
emparelhamentos perfeitos do grafo bipartido, e contar é só percorrer a árvore de
distribuições.

Tirar os sinais de uma fórmula transforma um exercício de primeiro semestre num
problema aberto. Este repositório é sobre o tamanho desse abismo.

## O sinal, visto de perto

```
$ dotnet medidor.dll sinal

  matriz               ordem   determinante     permanente   det. com 2 linhas trocadas  perm. trocada
  identidade 4             4              1              1                           -1              1
  todos uns 4              4              0             24                            0             24
  todos uns 6              6              0            720                            0            720
  todos uns 8              8              0         40,320                            0         40,320
  triangular 5             5              1              1                           -1              1
  linha de zeros 4         4              0              0                            0              0
  zeros e uns 5            5              2              8                           -2              8
  zeros e uns 6            6              1             49                           -1             49
  sorteada 5               5             36            108                          -36            108
```

Trocar duas linhas inverte o determinante e **não mexe** na permanente: trocar duas
linhas troca a paridade de toda permutação, e a permanente não olha para paridade.

Na matriz de **todos uns** as duas contas estão o mais longe que podem estar. O
determinante é zero; a permanente é n fatorial.

## O abismo

```
$ dotnet medidor.dll custo

  ordem   determinante     permanente  batem    Gauss  Bareiss      Ryser          a definicao  Ryser sobre Gauss
      3              0              2    sim        2        5         19                   18                9.5
      5              0             12    sim       35       30        133                  600                3.8
      8              0            528    sim       62      140      1,647              322,560               26.6
     10              0          7,794    sim      176      285      8,985           36,288,000               51.1
     12              0        490,326    sim      214      415     46,889        5,748,019,200              219.1
     15            -24  1,716,495,124    sim      715    1,015    487,605       19,615,115,520,000           682.0
```

É a **mesma matriz** em cada linha. O determinante dela sai por eliminação, em n ao
cubo, por dois caminhos diferentes que concordam em toda linha. A permanente sai
pela fórmula de **Ryser**, de 1963, que é inclusão e exclusão sobre os
subconjuntos de colunas: dois elevado a n vezes n.

E a coluna da direita é o preço do sinal.

## Ryser não resolve o problema, só adia

```
$ dotnet medidor.dll alcance

   ordem              definicao                  Ryser            Gauss        quem cabe
       5                6.00e+2                1.60e+2              125          os tres
      10                3.63e+7                1.02e+4            1,000          os tres
      12                5.75e+9                4.92e+4            1,728    Gauss e Ryser
      25               3.88e+26                8.39e+8           15,625    Gauss e Ryser
      30               7.96e+33               3.22e+10           27,000         so Gauss
     100              9.33e+159               1.27e+32        1,000,000         so Gauss
    1000               Infinity              1.07e+304    1,000,000,000         so Gauss
```

Com um orçamento de **um bilhão de contas**: Gauss chega à ordem mil sem suar,
Ryser para perto de vinte e cinco, a definição para em doze.

Ryser move o teto de doze para vinte e cinco e para por aí. Dobrar a velocidade da
máquina move esse teto em **uma** ordem.

## A permanente de zeros e uns conta coisas

```
$ dotnet medidor.dll contar

  matriz                       permanente  pela arvore  pela tabela  batem  passos de quantas  passos de existe
  identidade 6                          1            1            1    sim                  7                21
  todos uns 6                         720          720          720    sim              1,957                56
  todos uns 8                      40,320       40,320       40,320    sim            109,601               120
  linha de zeros 6                      0            0            0    sim                  1                 6
  zeros e uns 8 a 0.5 (2)               8            8            8    sim                186               116
  zeros e uns 8 a 0.5 (4)             120          120          120    sim              1,094                86
  zeros e uns 8 a 0.8 (3)          14,376       14,376       14,376    sim             42,147               115
```

Pense nas linhas como pessoas e nas colunas como tarefas, com um na posição i,j
quando a pessoa i sabe fazer a tarefa j. Uma permutação com produto um é
exatamente uma maneira de dar uma tarefa diferente para cada pessoa, e o produto de
qualquer outra é zero. Então a soma da definição **conta** as distribuições
possíveis.

Três caminhos, e dois deles não sabem o que é permanente: a árvore de distribuições
e a tabela sobre subconjuntos. Os três dão o mesmo número.

As duas últimas colunas são o que incomoda. Na matriz de todos uns de ordem oito,
saber que **existe** uma distribuição custa 120 passos; saber que são 40.320 custa
109.601. A mesma matriz, duas perguntas que soam parecidas.

E não é um detalhe de implementação: a primeira pergunta está em tempo polinomial
desde sempre, e a segunda é completa para a classe de contagem (Valiant, 1979).

## Quantos uns são precisos

```
$ dotnet medidor.dll densidade

  chance de um ordem  permanente media  maior permanente  quantas deram zero  passos medios da arvore
           0.1     8                 0                 0                 200                        3
           0.2     8                 0                 8                 191                       13
           0.3     8                 3                88                 123                       91
           0.4     8                25               324                  55                      281
           0.5     8               146             1,668                  19                    1,082
           0.7     8             2,433            10,340                   0                   10,210
           1.0     8            40,320            40,320                   0                  109,601
```

Duzentas matrizes oito por oito em cada linha. Com um décimo de uns, **todas** dão
zero. Com sete décimos, **nenhuma** dá. A faixa inteira em que a resposta é
interessante cabe entre trinta e cinquenta por cento.

## Como rodar

```
dotnet test testes/Permanente.Testes/Permanente.Testes.csproj
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- tudo
```

Os comandos do medidor são `definicao`, `sinal`, `contar`, `custo`, `densidade`,
`alcance` e `tudo`.

## O que as medidas me corrigiram

**A coluna de custo do Gauss não cresce com a ordem.** Eu escrevi a tabela do abismo
esperando duas colunas crescentes, uma como um cubo e a outra como uma potência de
dois. A do Gauss sobe, desce e sobe de novo: ordem doze custa 214 contas e ordem
onze custa 225.

O motivo é que o Gauss pula a linha inteira quando a entrada da coluna já é zero, e
**desiste** assim que acha uma coluna sem pivô. Numa matriz de zeros e uns singular
ele devolve zero no meio do caminho. É por isso que o Bareiss está no repositório
também: ele não pula nada, e a coluna dele cresce em toda linha.

**Ryser me parecia a solução, e ele só adia.** Antes da tabela de alcance eu tratava
a fórmula de 1963 como o conserto do problema: "a permanente tem sim um método
melhor que a definição". Tem, e ele leva o teto de doze para vinte e cinco. Depois
disso é a mesma parede. A tabela de alcance é a medida que tira o entusiasmo da
frase.

**O Bareiss entrou como conferência e virou a metade mais interessante da tabela.**
Ele estava previsto só para vigiar o Gauss: a eliminação com frações exatas tem um
numerador que cresce, e um numerador que estoura o inteiro de sessenta e quatro bits
não avisa, devolve um número errado em silêncio.

Foi ele que mostrou a não monotonia da coluna do Gauss, porque é a única coluna da
tabela que cresce do jeito que eu esperava. Sem a segunda coluna ao lado, a primeira
pareceria só ruidosa.

(As duas eliminações são em inteiros de propósito. Em `double`, um determinante
inteiro 6 sai como 5,999999999999998, e a conferência contra a definição vira uma
discussão sobre tolerância em vez de uma igualdade.)

**O segundo juiz foi o que tornou o repositório conferível acima de ordem onze.** A
definição para em doze, e sem ela não haveria contra o que medir Ryser. A contagem
de emparelhamentos não tem esse teto, não sabe o que é permutação, e confirma a
fórmula em toda matriz de zeros e uns. Dois métodos que chegam ao mesmo número por
caminhos que não se parecem valem mais do que um método conferido duas vezes.

## Licença

MIT.
