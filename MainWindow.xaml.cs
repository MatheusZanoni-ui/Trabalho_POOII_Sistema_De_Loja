using LojaVirtual.Models;
using LojaVirtual.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfAppSistemaDeLoja;

// Controller da interface: coleta entradas e delega regras ao service.
public partial class MainWindow : Window
{
    private readonly LojaService _service = new();
    private int? _clienteId;
    private int? _itemId;
    private int? _categoriaId;
    private bool _carregando;

    public MainWindow()
    {
        InitializeComponent();
        Atualizar();
    }

    private void Executar(Action acao)
    {
        try { acao(); Atualizar(); }
        catch (Exception ex) { MessageBox.Show(ex.GetBaseException().Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void Atualizar()
    {
        _carregando = true;
        try
        {
            var clientes = _service.ListarClientes();
            var itens = _service.ListarItens();
            var categorias = _service.ListarCategorias();
            var pedidos = _service.ListarPedidos();
            ClientesGrid.ItemsSource = clientes.Select(c => new ClienteLinha(c)).ToList();
            ItensGrid.ItemsSource = itens.Select(i => new ItemLinha(i)).ToList();
            CategoriasGrid.ItemsSource = categorias.Select(c => new CategoriaLinha(c)).ToList();
            PedidosGrid.ItemsSource = pedidos.Select(p => new PedidoLinha(p)).ToList();
            PagamentosGrid.ItemsSource = pedidos.Where(p => p.Pagamento != null).Select(p => new PagamentoLinha(p)).ToList();
            PedidoClienteCombo.ItemsSource = clientes;
            PedidoItemCombo.ItemsSource = itens;
            CategoriaItemCombo.ItemsSource = itens;
            CategoriaCombo.ItemsSource = categorias;
            PagamentoPedidoCombo.ItemsSource = pedidos.Where(p => p.Status == StatusPedido.Aberto)
                .Select(p => new PedidoOpcao(p.Id, $"#{p.Id} - {p.Cliente.Nome} - {p.Total:C}")).ToList();
            var resumo = _service.Resumo();
            TotalClientes.Text = resumo.clientes.ToString();
            TotalItens.Text = resumo.itens.ToString();
            TotalPedidos.Text = resumo.pedidos.ToString();
            TotalVendas.Text = resumo.vendas.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
            PedidosPagos.Text = pedidos.Count(p => p.Status == StatusPedido.Pago).ToString();
            PedidosAbertos.Text = pedidos.Count(p => p.Status == StatusPedido.Aberto).ToString();
            PedidosCancelados.Text = pedidos.Count(p => p.Status == StatusPedido.Cancelado).ToString();
            PedidosRecentesGrid.ItemsSource = pedidos.OrderByDescending(p => p.Data)
                .Take(5).Select(p => new PedidoLinha(p)).ToList();
        }
        finally { _carregando = false; }
    }

    private static string Tipo(ComboBox cb) => (cb.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
    private static int Escolhido(ComboBox cb) => cb.SelectedValue is int id ? id : throw new ArgumentException("Escolha um registro na lista.");
    private static int Inteiro(string texto) => int.TryParse(texto, out int valor) ? valor : throw new ArgumentException("Informe um numero inteiro valido.");
    private static decimal Dinheiro(string texto)
    {
        if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out var n)) return n;
        if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out n)) return n;
        throw new ArgumentException("Informe um preco valido, exemplo: 19,90.");
    }
    private static void Confirmar(Action acao)
    {
        if (MessageBox.Show("Confirmar exclusao do registro selecionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) acao();
    }

    private void Abas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // A interface carrega os dados apos cada alteracao; nao consulta banco a cada clique.
    }
    private void Atualizar_Click(object sender, RoutedEventArgs e) => Executar(() => { });
    private void Sql_Click(object sender, RoutedEventArgs e) => Executar(() =>
        MessageBox.Show($"Consulta SQL direta executada:\nSELECT COUNT(*) FROM [Pedidos]\n\nTotal: {_service.ContarPedidosViaSql()}", "SQL ADO.NET"));

    private void ClienteSalvar_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        _service.SalvarCliente(_clienteId, ClienteNome.Text, ClienteEmail.Text, Tipo(ClienteTipo), ClienteDocumento.Text);
        LimparCliente();
    });
    private void ClienteLimpar_Click(object sender, RoutedEventArgs e) => LimparCliente();
    private void LimparCliente()
    {
        _clienteId = null; ClientesGrid.SelectedItem = null;
        ClienteNome.Clear(); ClienteEmail.Clear(); ClienteDocumento.Clear(); ClienteTipo.SelectedIndex = 0;
    }
    private void ClientesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_carregando || ClientesGrid.SelectedItem is not ClienteLinha c) return;
        _clienteId = c.Id; ClienteNome.Text = c.Nome; ClienteEmail.Text = c.Email;
        ClienteTipo.SelectedIndex = c.Tipo == "PF" ? 0 : 1; ClienteDocumento.Text = c.Documento;
    }
    private void ClienteExcluir_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        if (_clienteId is not int id) throw new ArgumentException("Selecione um cliente.");
        Confirmar(() => { _service.ExcluirCliente(id); LimparCliente(); });
    });

    private void ItemSalvar_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        _service.SalvarItem(_itemId, ItemNome.Text, Dinheiro(ItemPreco.Text), Tipo(ItemTipo), Inteiro(ItemQuantidade.Text));
        LimparItem();
    });
    private void ItemLimpar_Click(object sender, RoutedEventArgs e) => LimparItem();
    private void LimparItem()
    {
        _itemId = null; ItensGrid.SelectedItem = null; ItemNome.Clear(); ItemPreco.Clear(); ItemQuantidade.Clear(); ItemTipo.SelectedIndex = 0;
    }
    private void ItensGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_carregando || ItensGrid.SelectedItem is not ItemLinha item) return;
        _itemId = item.Id; ItemNome.Text = item.Nome; ItemPreco.Text = item.Preco.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR"));
        ItemQuantidade.Text = item.Quantidade.ToString(); ItemTipo.SelectedIndex = item.Tipo == "Produto" ? 0 : 1;
    }
    private void ItemExcluir_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        if (_itemId is not int id) throw new ArgumentException("Selecione um item.");
        Confirmar(() => { _service.ExcluirItem(id); LimparItem(); });
    });

    private void CategoriaSalvar_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        _service.SalvarCategoria(_categoriaId, CategoriaNome.Text); LimparCategoria();
    });
    private void CategoriaLimpar_Click(object sender, RoutedEventArgs e) => LimparCategoria();
    private void LimparCategoria() { _categoriaId = null; CategoriasGrid.SelectedItem = null; CategoriaNome.Clear(); }
    private void CategoriasGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_carregando || CategoriasGrid.SelectedItem is not CategoriaLinha c) return;
        _categoriaId = c.Id; CategoriaNome.Text = c.Nome;
    }
    private void CategoriaExcluir_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        if (_categoriaId is not int id) throw new ArgumentException("Selecione uma categoria.");
        Confirmar(() => { _service.ExcluirCategoria(id); LimparCategoria(); });
    });
    private void Associar_Click(object sender, RoutedEventArgs e) => Executar(() =>
        _service.AssociarCategoria(Escolhido(CategoriaItemCombo), Escolhido(CategoriaCombo)));
    private void PedidoCriar_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        int id = _service.CriarPedido(Escolhido(PedidoClienteCombo), Escolhido(PedidoItemCombo), Inteiro(PedidoQuantidade.Text));
        MessageBox.Show($"Pedido #{id} criado e salvo no SQL Server.", "Pedido registrado");
    });
    private void Pagar_Click(object sender, RoutedEventArgs e) => Executar(() =>
    {
        _service.ProcessarPagamento(Escolhido(PagamentoPedidoCombo), Tipo(PagamentoForma));
        MessageBox.Show("Pagamento SIMULADO registrado com sucesso.", "Pagamento registrado");
    });

    // A navegacao preserva a sidebar e dashboard originais do grupo.
    private void IrPara(int indice, string titulo)
    {
        Abas.SelectedIndex = indice;
        PageTitleText.Text = titulo;
        BreadcrumbText.Text = "PÁGINAS  /  " + titulo.ToUpperInvariant();
    }

    private void Dashboard_Click(object sender, RoutedEventArgs e) => IrPara(0, "Visão geral");

    private void Navigation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button botao || botao.Tag is not string secao) return;
        switch (secao)
        {
            case "Clientes": IrPara(1, "Clientes"); break;
            case "Produtos": IrPara(2, "Produtos e serviços"); break;
            case "Categorias": IrPara(3, "Categorias"); break;
            case "Pedidos": IrPara(4, "Pedidos"); break;
            case "Pagamentos": IrPara(5, "Pagamentos"); break;
            case "Configurações":
                MessageBox.Show("Banco: SQL Server LocalDB (LojaVirtualDb).\n" +
                    "Os dados sao persistidos pelo Entity Framework Core.\n" +
                    "Para demonstrar SQL direto, volte ao painel e clique em Consulta SQL direta.",
                    "Informacoes do sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                break;
        }
    }

    private void NewOrder_Click(object sender, RoutedEventArgs e) => IrPara(4, "Pedidos");
    private void ViewOrders_Click(object sender, RoutedEventArgs e) => IrPara(4, "Pedidos");

    // View-models simples impedem DataGrid de percorrer propriedades de navegacao recursivas.
    private sealed record ClienteLinha(int Id, string Nome, string Email, string Tipo, string Documento)
    {
        public ClienteLinha(Cliente c) : this(c.Id, c.Nome, c.Email,
            c is PessoaFisica ? "PF" : "PJ", c is PessoaFisica pf ? pf.Cpf : ((PessoaJuridica)c).Cnpj) { }
    }
    private sealed record ItemLinha(int Id, string Nome, decimal Preco, string Tipo, int Quantidade)
    {
        public ItemLinha(Item i) : this(i.Id, i.Nome, i.Preco,
            i is Produto ? "Produto" : "Servico", i is Produto p ? p.Estoque : ((Servico)i).DuracaoHoras) { }
    }
    private sealed record CategoriaLinha(int Id, string Nome)
    {
        public CategoriaLinha(Categoria c) : this(c.Id, c.Nome) { }
    }
    private sealed record PedidoLinha(int Id, string Cliente, DateTime Data, string Status, decimal Total, int QuantidadeItens)
    {
        public PedidoLinha(Pedido p) : this(p.Id, p.Cliente.Nome, p.Data, p.Status.ToString(), p.Total, p.Itens.Sum(i => i.Quantidade)) { }
    }
    private sealed record PagamentoLinha(int PedidoId, string Tipo, decimal Valor, bool Processado)
    {
        public PagamentoLinha(Pedido p) : this(p.Id, p.Pagamento!.GetType().Name, p.Pagamento.Valor, p.Pagamento.Processado) { }
    }
    private sealed record PedidoOpcao(int Id, string Descricao);
}
