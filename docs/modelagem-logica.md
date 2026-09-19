# Modelagem Lógica

## Objetivo

Transformar as entidades da modelagem conceitual em tabelas, definindo suas colunas, chaves primárias, chaves estrangeiras e principais restrições.

## Legenda

- **PK:** chave primária, que identifica cada registro.
- **FK:** chave estrangeira, que relaciona uma tabela com outra.
- **UK:** valor único, que não poderá ser repetido.
- **NN:** campo obrigatório, que não aceitará valor nulo.

## Tabela Perfis

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| PerfilID | INT | PK, identidade |
| Nome | VARCHAR(30) | NN, UK |
| Descricao | VARCHAR(150) | Opcional |
| Ativo | BIT | NN |

## Tabela Usuarios

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| UsuarioID | INT | PK, identidade |
| PerfilID | INT | FK, NN |
| Nome | VARCHAR(100) | NN |
| Email | VARCHAR(150) | NN, UK |
| SenhaHash | VARCHAR(255) | NN |
| Ativo | BIT | NN |
| CriadoEm | DATETIME2 | NN |

A senha nunca será armazenada como texto comum. A aplicação armazenará somente o hash produzido por um algoritmo apropriado.

## Tabela Categorias

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| CategoriaID | INT | PK, identidade |
| Nome | VARCHAR(60) | NN, UK |
| Descricao | VARCHAR(150) | Opcional |
| Ativo | BIT | NN |

## Tabela Produtos

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| ProdutoID | INT | PK, identidade |
| CategoriaID | INT | FK, NN |
| CodigoBarras | VARCHAR(14) | UK, opcional |
| Nome | VARCHAR(120) | NN |
| Descricao | VARCHAR(255) | Opcional |
| PrecoCusto | DECIMAL(10,2) | NN |
| PrecoVenda | DECIMAL(10,2) | NN |
| EstoqueMinimo | INT | NN |
| Ativo | BIT | NN |
| CriadoEm | DATETIME2 | NN |

## Tabela Estoques

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| ProdutoID | INT | PK, FK |
| QuantidadeAtual | INT | NN |
| AtualizadoEm | DATETIME2 | NN |

Cada produto terá somente um registro nessa tabela. A quantidade atual não poderá ser negativa.

## Tabela Fornecedores

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| FornecedorID | INT | PK, identidade |
| RazaoSocial | VARCHAR(120) | NN |
| NomeFantasia | VARCHAR(120) | Opcional |
| CNPJ | CHAR(14) | UK, opcional |
| Telefone | VARCHAR(20) | Opcional |
| Email | VARCHAR(150) | Opcional |
| Ativo | BIT | NN |

## Tabela FormasPagamento

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| FormaPagamentoID | INT | PK, identidade |
| Nome | VARCHAR(40) | NN, UK |
| Ativo | BIT | NN |

## Tabela Entradas

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| EntradaID | INT | PK, identidade |
| FornecedorID | INT | FK, NN |
| UsuarioID | INT | FK, NN |
| DataEntrada | DATETIME2 | NN |
| Status | VARCHAR(20) | NN |
| ValorTotal | DECIMAL(12,2) | NN |

Status permitidos: `ABERTA`, `CONFIRMADA` e `CANCELADA`.

## Tabela ItensEntrada

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| ItemEntradaID | INT | PK, identidade |
| EntradaID | INT | FK, NN |
| ProdutoID | INT | FK, NN |
| Quantidade | INT | NN |
| CustoUnitario | DECIMAL(10,2) | NN |
| Subtotal | DECIMAL(12,2) | NN |

## Tabela Vendas

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| VendaID | INT | PK, identidade |
| UsuarioID | INT | FK, NN |
| FormaPagamentoID | INT | FK, opcional enquanto aberta |
| DataVenda | DATETIME2 | NN |
| Status | VARCHAR(20) | NN |
| ValorTotal | DECIMAL(12,2) | NN |

Status permitidos: `ABERTA`, `CONCLUIDA` e `CANCELADA`.

## Tabela ItensVenda

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| ItemVendaID | INT | PK, identidade |
| VendaID | INT | FK, NN |
| ProdutoID | INT | FK, NN |
| Quantidade | INT | NN |
| PrecoUnitario | DECIMAL(10,2) | NN |
| Subtotal | DECIMAL(12,2) | NN |

O preço unitário será registrado no item para preservar o preço praticado no momento da venda, mesmo que o cadastro do produto seja alterado posteriormente.

## Tabela MovimentacoesEstoque

| Coluna | Tipo sugerido | Regras |
|---|---|---|
| MovimentacaoID | BIGINT | PK, identidade |
| ProdutoID | INT | FK, NN |
| UsuarioID | INT | FK, NN |
| Tipo | VARCHAR(20) | NN |
| Quantidade | INT | NN |
| DataMovimentacao | DATETIME2 | NN |
| OrigemTipo | VARCHAR(20) | NN |
| OrigemID | INT | NN |
| Observacao | VARCHAR(255) | Opcional |

Tipos previstos: `ENTRADA`, `SAIDA`, `DEVOLUCAO` e `AJUSTE`.

## Relacionamentos principais

- `Usuarios.PerfilID` referencia `Perfis.PerfilID`.
- `Produtos.CategoriaID` referencia `Categorias.CategoriaID`.
- `Estoques.ProdutoID` referencia `Produtos.ProdutoID`.
- `Entradas.FornecedorID` referencia `Fornecedores.FornecedorID`.
- `Entradas.UsuarioID` referencia `Usuarios.UsuarioID`.
- `ItensEntrada.EntradaID` referencia `Entradas.EntradaID`.
- `ItensEntrada.ProdutoID` referencia `Produtos.ProdutoID`.
- `Vendas.UsuarioID` referencia `Usuarios.UsuarioID`.
- `Vendas.FormaPagamentoID` referencia `FormasPagamento.FormaPagamentoID`.
- `ItensVenda.VendaID` referencia `Vendas.VendaID`.
- `ItensVenda.ProdutoID` referencia `Produtos.ProdutoID`.
- `MovimentacoesEstoque.ProdutoID` referencia `Produtos.ProdutoID`.
- `MovimentacoesEstoque.UsuarioID` referencia `Usuarios.UsuarioID`.