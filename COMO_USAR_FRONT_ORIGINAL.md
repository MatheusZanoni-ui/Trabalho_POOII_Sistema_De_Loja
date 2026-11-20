# Front-end original integrado ao projeto funcional

Esta versao preserva a estrutura de dashboard, menu lateral, cartoes, cabecalho e paleta azul-marinho do ZIP original do grupo e incorpora os modulos funcionais da versao revisada e testada pelo aluno.

## Como executar

1. Extraia o ZIP numa nova pasta e abra `WpfAppSistemaDeLoja.slnx` no Visual Studio.
2. Compile e execute (F5). Requisitos: Windows, .NET 10, SQL Server LocalDB e acesso aos pacotes NuGet.
3. Na barra lateral, abra **Clientes**, **Produtos e servicos**, **Categorias**, **Pedidos** e **Pagamentos**.
4. Cadastre um cliente e um produto antes de criar pedidos.
5. Em **Visao geral**, o botao **Consulta SQL direta** executa a contagem de pedidos pelo SQL, sem LINQ.
6. Os valores, status e pedidos recentes no dashboard agora vem do banco. O grafico estatico original foi retirado porque ele nao representava vendas reais.

## Importante

- Os arquivos de `Models`, `Data`, `Migrations`, `Services` e o `App.xaml.cs` sao iguais aos da versao revisada anterior.
- Mudancas concentram-se em `MainWindow.xaml` e `MainWindow.xaml.cs`.
- Nao foi possivel compilar WPF neste ambiente Linux. Verifique erros de compilacao no Visual Studio antes do commit.
- Se voce ja usou a versao anterior e ela compila corretamente, copie somente os dois arquivos `MainWindow.xaml` e `MainWindow.xaml.cs` desta versao para o projeto local e teste.
- Nao adicione as pastas `bin`, `obj` nem `.vs` ao Git.
