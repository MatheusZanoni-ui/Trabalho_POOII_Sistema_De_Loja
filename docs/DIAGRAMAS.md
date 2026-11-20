# Diagramas propostos para revisao do grupo

> Esbocos textuais Mermaid; verificar e ajustar com a modelagem efetivamente implementada e os requisitos cobrados.

## Classes e relacionamentos

```mermaid
classDiagram
    Cliente <|-- PessoaFisica
    Cliente <|-- PessoaJuridica
    Cliente "1" --> "*" Pedido
    Pedido "1" --> "*" ItemPedido
    Item "1" --> "*" ItemPedido
    Item "*" -- "*" Categoria
    Pedido "1" --> "0..1" Pagamento
    Item <|-- Produto
    Item <|-- Servico
    Pagamento <|-- Pix
    Pagamento <|-- Cartao
    Pagamento <|-- Boleto
    IPagamento <|.. Pagamento
    class IPagamento { <<interface>> Processar() }
```

## Casos de uso - descricao para desenhar em UML

Ator: Administrador da loja.

- Cadastrar/editar/excluir clientes (PF e PJ).
- Cadastrar/editar/excluir produtos e servicos.
- Gerenciar categorias e associar itens.
- Abrir pedido para cliente e incluir item/quantidade.
- Processar pagamento simulado por Pix, cartao ou boleto.
- Consultar resumo de vendas e quantidade de pedidos por SQL direto.

## Fluxo de demonstracao

```mermaid
flowchart TD
 A[Cadastrar cliente] --> B[Cadastrar produto e categoria]
 B --> C[Associar categoria ao produto]
 C --> D[Criar pedido]
 D --> E[Baixar estoque]
 E --> F[Processar pagamento simulado]
 F --> G[Consultar resumo e SQL direto]
```
