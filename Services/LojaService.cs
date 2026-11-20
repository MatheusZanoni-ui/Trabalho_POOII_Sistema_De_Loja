using LojaVirtual.Data;
using LojaVirtual.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LojaVirtual.Services;

// Service: regras de negocio fora da interface (separacao em camadas/SRP).
public sealed class LojaService
{
    public List<Cliente> ListarClientes()
    {
        using var db = new LojaContext();
        return db.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToList();
    }

    public List<Item> ListarItens()
    {
        using var db = new LojaContext();
        return db.Itens.AsNoTracking().OrderBy(i => i.Nome).ToList();
    }

    public List<Categoria> ListarCategorias()
    {
        using var db = new LojaContext();
        return db.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToList();
    }

    public List<Pedido> ListarPedidos()
    {
        using var db = new LojaContext();
        return db.Pedidos.AsNoTracking().Include(p => p.Cliente)
            .Include(p => p.Itens).ThenInclude(i => i.Item)
            .Include(p => p.Pagamento)
            .OrderByDescending(p => p.Data).ToList();
    }

    public void SalvarCliente(int? id, string nome, string email, string tipo, string documento)
    {
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(documento))
            throw new ArgumentException("Nome, e-mail e documento sao obrigatorios.");
        if (!email.Contains('@')) throw new ArgumentException("Informe um e-mail valido.");
        using var db = new LojaContext();
        if (id is int existente)
        {
            var cliente = db.Clientes.Single(c => c.Id == existente);
            if ((cliente is PessoaFisica && tipo != "Pessoa fisica") ||
                (cliente is PessoaJuridica && tipo != "Pessoa juridica"))
                throw new InvalidOperationException("O tipo do cliente nao pode ser alterado. Exclua e cadastre novamente.");
            cliente.Nome = nome.Trim(); cliente.Email = email.Trim();
            if (cliente is PessoaFisica pf) pf.Cpf = documento.Trim();
            else if (cliente is PessoaJuridica pj) pj.Cnpj = documento.Trim();
        }
        else
        {
            Cliente cliente = tipo == "Pessoa juridica"
                ? new PessoaJuridica { Nome = nome.Trim(), Email = email.Trim(), Cnpj = documento.Trim() }
                : new PessoaFisica { Nome = nome.Trim(), Email = email.Trim(), Cpf = documento.Trim() };
            db.Clientes.Add(cliente);
        }
        db.SaveChanges();
    }

    public void ExcluirCliente(int id)
    {
        using var db = new LojaContext();
        var cliente = db.Clientes.Find(id) ?? throw new InvalidOperationException("Cliente nao encontrado.");
        db.Clientes.Remove(cliente); db.SaveChanges();
    }

    public void SalvarItem(int? id, string nome, decimal preco, string tipo, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(nome) || preco < 0 || quantidade < 0)
            throw new ArgumentException("Informe nome, preco e quantidade validos.");
        using var db = new LojaContext();
        if (id is int existente)
        {
            var item = db.Itens.Single(i => i.Id == existente);
            if ((item is Produto && tipo != "Produto") || (item is Servico && tipo != "Servico"))
                throw new InvalidOperationException("O tipo do item nao pode ser alterado.");
            item.Nome = nome.Trim(); item.Preco = preco;
            if (item is Produto produto) produto.Estoque = quantidade;
            else if (item is Servico servico) servico.DuracaoHoras = quantidade;
        }
        else
        {
            Item item = tipo == "Servico"
                ? new Servico { Nome = nome.Trim(), Preco = preco, DuracaoHoras = quantidade }
                : new Produto { Nome = nome.Trim(), Preco = preco, Estoque = quantidade };
            db.Itens.Add(item);
        }
        db.SaveChanges();
    }

    public void ExcluirItem(int id)
    {
        using var db = new LojaContext();
        var item = db.Itens.Find(id) ?? throw new InvalidOperationException("Item nao encontrado.");
        db.Itens.Remove(item); db.SaveChanges();
    }

    public void SalvarCategoria(int? id, string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Informe o nome da categoria.");
        using var db = new LojaContext();
        if (id is int existente)
        {
            var categoria = db.Categorias.Find(existente) ?? throw new InvalidOperationException("Categoria nao encontrada.");
            categoria.Nome = nome.Trim();
        }
        else db.Categorias.Add(new Categoria { Nome = nome.Trim() });
        db.SaveChanges();
    }

    public void ExcluirCategoria(int id)
    {
        using var db = new LojaContext();
        var categoria = db.Categorias.Find(id) ?? throw new InvalidOperationException("Categoria nao encontrada.");
        db.Categorias.Remove(categoria); db.SaveChanges();
    }

    // N:N: uma categoria pode conter diversos itens e vice-versa.
    public void AssociarCategoria(int itemId, int categoriaId)
    {
        using var db = new LojaContext();
        var item = db.Itens.Include(i => i.Categorias).Single(i => i.Id == itemId);
        var categoria = db.Categorias.Single(c => c.Id == categoriaId);
        if (!item.Categorias.Any(c => c.Id == categoriaId)) item.Categorias.Add(categoria);
        db.SaveChanges();
    }

    public int CriarPedido(int clienteId, int itemId, int quantidade)
    {
        if (quantidade < 1) throw new ArgumentException("Quantidade deve ser maior que zero.");
        using var db = new LojaContext();
        using var tx = db.Database.BeginTransaction();
        if (!db.Clientes.Any(c => c.Id == clienteId)) throw new ArgumentException("Cliente nao encontrado.");
        var item = db.Itens.Single(i => i.Id == itemId);
        if (item is Produto produto)
        {
            if (produto.Estoque < quantidade) throw new InvalidOperationException("Estoque insuficiente.");
            produto.Estoque -= quantidade;
        }
        var pedido = new Pedido { ClienteId = clienteId };
        pedido.Itens.Add(new ItemPedido { ItemId = itemId, Quantidade = quantidade, PrecoUnitario = item.Preco });
        db.Pedidos.Add(pedido);
        db.SaveChanges(); tx.Commit();
        return pedido.Id;
    }

    // Polimorfismo: implementacoes distintas da mesma operacao de pagamento.
    public void ProcessarPagamento(int pedidoId, string forma)
    {
        using var db = new LojaContext();
        var pedido = db.Pedidos.Include(p => p.Itens).Include(p => p.Pagamento)
            .Single(p => p.Id == pedidoId);
        if (pedido.Status != StatusPedido.Aberto || pedido.Pagamento != null)
            throw new InvalidOperationException("Este pedido nao esta aberto ou ja possui pagamento.");
        Pagamento pagamento = forma switch
        {
            "Cartao" => new Cartao { Bandeira = "Demonstracao", UltimosDigitos = "0000" },
            "Boleto" => new Boleto { CodigoBarras = "SIMULADO" + pedido.Id },
            "Pix" => new Pix { IdentificadorTransacao = "DEMO-" + Guid.NewGuid().ToString("N")[..12] },
            _ => throw new ArgumentException("Forma de pagamento invalida.")
        };
        pagamento.PedidoId = pedido.Id;
        pagamento.Valor = pedido.Total;
        pagamento.Processar();
        pedido.Pagamento = pagamento;
        pedido.Status = StatusPedido.Pago;
        db.SaveChanges();
    }

    public (int clientes, int itens, int pedidos, decimal vendas) Resumo()
    {
        using var db = new LojaContext();
        var pedidos = db.Pedidos.AsNoTracking().Include(p => p.Itens)
            .Where(p => p.Status == StatusPedido.Pago).ToList();
        return (db.Clientes.Count(), db.Itens.Count(), db.Pedidos.Count(),
            pedidos.Sum(p => p.Total));
    }

    // Exigencia da atividade: uma funcionalidade em SQL direto, sem LINQ/EF para a consulta.
    // Usa conexao gerenciada pelo provider e parametro fixo, sem concatenar entrada do usuario.
    public int ContarPedidosViaSql()
    {
        using var db = new LojaContext();
        using var conexao = db.Database.GetDbConnection();
        conexao.Open();
        using var comando = conexao.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM [Pedidos]";
        return Convert.ToInt32(comando.ExecuteScalar());
    }
}
