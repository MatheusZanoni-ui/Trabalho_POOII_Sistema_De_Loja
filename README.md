# LojaVirtual - POO II - Implementacao para revisao do grupo

Aplicacao desktop C# / WPF / .NET 10 / Entity Framework Core / SQL Server LocalDB.

> **Importante:** esta versao foi gerada como proposta de implementacao e **nao foi compilada nem executada em Windows** neste ambiente. Os autores precisam revisar, compilar, testar e assumir responsabilidade pelo codigo antes da entrega. O enunciado permite IA ate nivel IA-2 e solicita transparencia, revisao humana e comparacao com versao sem IA.

## Requisitos

- Windows 10/11, Visual Studio com workload **Desenvolvimento para desktop com .NET** e SDK .NET 10
- SQL Server Express LocalDB (instancia `(localdb)\\MSSQLLocalDB`)
- Acesso aos pacotes NuGet na primeira restauracao

## Como executar

1. Abra `WpfAppSistemaDeLoja.slnx` no Visual Studio com suporte a `.slnx`.
2. Restaure pacotes e compile a solucao (Build > Rebuild Solution).
3. Confirme que a instancia SQL Server LocalDB esta instalada. Por padrao a conexao usa `LojaVirtualDb` e autenticacao integrada do Windows.
4. Pressione F5. A aplicacao aplicara as migrations existentes durante a inicializacao e deve abrir a tela principal.
5. Se quiser usar outro servidor, configure a variavel de ambiente `LOJA_CONNECTION_STRING` com uma connection string SQL Server apropriada.
6. Em caso de erro, leia a mensagem e confira instancia, permissoes e instalacao do SQL Server LocalDB.

### Ordem de demonstracao

1. Aba **Clientes**: cadastre PF e PJ; edite um cliente.
2. Aba **Produtos e servicos**: cadastre um produto com estoque 10 e um servico.
3. Aba **Categorias**: cadastre uma categoria e associe-a a um item (N:N).
4. Aba **Pedidos**: selecione cliente e produto, quantidade 2 e salve; verifique o estoque diminuir.
5. Aba **Pagamentos**: escolha pedido aberto e processe um Pix ou Cartao de **demonstracao**.
6. Aba **Visao geral**: veja contagens reais; clique em **Contar pedidos via SQL direto**.
7. Encerre e reinicie: confirme que os dados persistiram.

## Estrutura

- `Models`: modelo de dominio, heranca e relacionamentos.
- `Data/LojaContext`: mapeamento Code First e configuracao EF Core.
- `Migrations`: migration original preservada.
- `Services/LojaService`: logica de CRUD, pedidos, pagamentos e consulta SQL direto.
- `MainWindow.xaml` + `.cs`: interface e controladores de eventos WPF.
- `docs`: diagramas e material auxiliar para revisao humana.

## Conceitos e implementacao

- GUI: abas WPF com CRUD basico e fluxo de pedidos.
- MVC em camadas **adaptado para WPF**: models (`Models`), view (`MainWindow.xaml`) e camada de controle (`MainWindow.xaml.cs`) delegando para `LojaService`. Nao e MVC formal de ASP.NET.
- SOLID / SRP: interface trata entrada e exibicao, service cuida de regras e contexto cuida da persistencia. O design pode ser aprimorado com interfaces e DI.
- Clean Code: nomes explicitos, validacoes e operacoes separadas por responsabilidade.
- EF Core Code First: herancas TPH, Cliente-Pedido 1:N, Pedido-Pagamento 1:1, Item-Categoria N:N.
- SQL direto (ADO.NET): `LojaService.ContarPedidosViaSql` executa `SELECT COUNT(*) FROM [Pedidos]` usando `DbCommand`.
- Polimorfismo: `Pagamento.Processar()` sobrescrito em Cartao/Boleto/Pix e interface `IPagamento` implementada por `Pagamento`.

## Limitacoes conhecidas

- Pagamento nao integra gateways reais: e somente simulacao academica.
- Um pedido nesta versao contempla um tipo de item por operacao, embora o modelo aceite varios itens.
- Estorno e cancelamento de pedido, controle de concorrencia do estoque, validacao rigorosa de CPF/CNPJ e autenticacao nao foram implementados.
- Exclusao de clientes/itens com pedidos pode falhar pelas restricoes de chave estrangeira (correto para nao violar integridade).
- Para producao, migracoes automaticas na inicializacao e connection string local precisam ser substituidas por implantacao e seguranca adequadas.

## Checklist de validacao antes de entregar

- [ ] Solucao compila em **Visual Studio/Windows** sem erros.
- [ ] Aplicacao inicia com banco novo e reaplica migrations sem erro.
- [ ] Criar, editar, excluir (sem dependencias), reiniciar e consultar produtos/clientes/categorias.
- [ ] Criar pedido; conferir estoque decrementado e linhas do pedido no SQL Server.
- [ ] Processar pagamento simulado e verificar status pago.
- [ ] Acionar a consulta SQL direta e conferir contagem.
- [ ] Conferir diagramas de caso de uso e classe com professor/equipe.
- [ ] Revisar criticamente todo codigo gerado com IA, registrar prompts e diferencas.
- [ ] Todos os integrantes participam do unico video de ate 10 minutos e submetem individualmente.

## Atencao a data

O PDF enviado informa **13/11/2026** como data de entrega/apresentacao. O usuario informou novo prazo **20/11/2026 as 23h**. Confirme no sistema de entrega do curso qual data vale.
