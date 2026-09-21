# Modelagem Lógica

## Objetivo

Transformar as entidades da modelagem conceitual em tabelas relacionais, definindo colunas, tipos de dados, chaves, relacionamentos, valores padrão e restrições de integridade.

O modelo foi preparado para o banco:

```text
DB_GestaoComercial
```

## Legenda

* **PK:** chave primária.
* **FK:** chave estrangeira.
* **UK:** restrição ou índice de unicidade.
* **NN:** campo obrigatório.
* **NULL:** campo opcional.
* **IDENTITY:** valor numérico gerado automaticamente.
* **DEFAULT:** valor padrão.
* **CHECK:** validação aplicada pelo banco.
* **COMPUTED:** valor calculado pelo SQL Server.
* **PERSISTED:** resultado calculado e armazenado fisicamente.

## Convenções utilizadas

* Identificadores utilizam números inteiros.
* Textos com possibilidade de acentuação utilizam `NVARCHAR`.
* Valores monetários utilizam `DECIMAL`.
* Datas são registradas com `DATETIME2`.
* Datas automáticas utilizam `SYSUTCDATETIME()`.
* Registros comerciais são desativados por meio da coluna `Ativo`.
* Registros históricos não possuem exclusão em cascata.
* Operações críticas de estoque utilizam transações.

## Tabela `Perfis`

| Coluna      | Tipo       | Regras          |
| ----------- | ---------- | --------------- |
| `PerfilID`  | `INT`      | PK, IDENTITY    |
| `Nome`      | `NVARCHAR` | NN, UK          |
| `Descricao` | `NVARCHAR` | NULL            |
| `Ativo`     | `BIT`      | NN, DEFAULT `1` |

Armazena os perfis Administrador, Estoquista e Caixa.

## Tabela `Usuarios`

| Coluna      | Tipo            | Regras                         |
| ----------- | --------------- | ------------------------------ |
| `UsuarioID` | `INT`           | PK, IDENTITY                   |
| `PerfilID`  | `INT`           | FK, NN                         |
| `Nome`      | `NVARCHAR(100)` | NN                             |
| `Email`     | `NVARCHAR(150)` | NN, UK                         |
| `SenhaHash` | `VARCHAR(255)`  | NN                             |
| `Ativo`     | `BIT`           | NN, DEFAULT `1`                |
| `CriadoEm`  | `DATETIME2`     | NN, DEFAULT `SYSUTCDATETIME()` |

### Relacionamento

`Usuarios.PerfilID` referencia `Perfis.PerfilID`.

A senha não será armazenada em texto simples. A futura API será responsável por gerar e validar o hash.

## Tabela `Categorias`

| Coluna        | Tipo       | Regras          |
| ------------- | ---------- | --------------- |
| `CategoriaID` | `INT`      | PK, IDENTITY    |
| `Nome`        | `NVARCHAR` | NN, UK          |
| `Descricao`   | `NVARCHAR` | NULL            |
| `Ativo`       | `BIT`      | NN, DEFAULT `1` |

## Tabela `Produtos`

| Coluna          | Tipo            | Regras                          |
| --------------- | --------------- | ------------------------------- |
| `ProdutoID`     | `INT`           | PK, IDENTITY                    |
| `CategoriaID`   | `INT`           | FK, NN                          |
| `CodigoBarras`  | `VARCHAR`       | NULL, UK quando informado       |
| `Nome`          | `NVARCHAR`      | NN                              |
| `Descricao`     | `NVARCHAR`      | NULL                            |
| `PrecoCusto`    | `DECIMAL(10,2)` | NN, CHECK maior ou igual a zero |
| `PrecoVenda`    | `DECIMAL(10,2)` | NN, CHECK maior ou igual a zero |
| `EstoqueMinimo` | `INT`           | NN, CHECK maior ou igual a zero |
| `Ativo`         | `BIT`           | NN, DEFAULT `1`                 |
| `CriadoEm`      | `DATETIME2`     | NN, DEFAULT `SYSUTCDATETIME()`  |

### Relacionamento

`Produtos.CategoriaID` referencia `Categorias.CategoriaID`.

O código de barras utiliza um índice único filtrado, permitindo que vários produtos tenham `NULL`, mas impedindo códigos repetidos quando informados.

## Tabela `Estoques`

| Coluna            | Tipo        | Regras                                       |
| ----------------- | ----------- | -------------------------------------------- |
| `ProdutoID`       | `INT`       | PK, FK                                       |
| `QuantidadeAtual` | `INT`       | NN, DEFAULT `0`, CHECK maior ou igual a zero |
| `AtualizadoEm`    | `DATETIME2` | NN, DEFAULT `SYSUTCDATETIME()`               |

### Relacionamento

`Estoques.ProdutoID` referencia `Produtos.ProdutoID`.

A chave primária também funciona como chave estrangeira, garantindo que cada produto possua no máximo um registro de estoque.

## Tabela `Fornecedores`

| Coluna         | Tipo            | Regras                    |
| -------------- | --------------- | ------------------------- |
| `FornecedorID` | `INT`           | PK, IDENTITY              |
| `RazaoSocial`  | `NVARCHAR`      | NN                        |
| `NomeFantasia` | `NVARCHAR`      | NULL                      |
| `CNPJ`         | `CHAR(14)`      | NULL, UK quando informado |
| `Telefone`     | `VARCHAR(20)`   | NULL                      |
| `Email`        | `NVARCHAR(150)` | NULL                      |
| `Ativo`        | `BIT`           | NN, DEFAULT `1`           |

O CNPJ utiliza um índice único filtrado, permitindo `NULL` e impedindo a repetição de valores informados.

## Tabela `FormasPagamento`

| Coluna             | Tipo           | Regras          |
| ------------------ | -------------- | --------------- |
| `FormaPagamentoID` | `INT`          | PK, IDENTITY    |
| `Nome`             | `NVARCHAR(40)` | NN, UK          |
| `Ativo`            | `BIT`          | NN, DEFAULT `1` |

## Tabela `Entradas`

| Coluna         | Tipo            | Regras                                       |
| -------------- | --------------- | -------------------------------------------- |
| `EntradaID`    | `INT`           | PK, IDENTITY                                 |
| `FornecedorID` | `INT`           | FK, NN                                       |
| `UsuarioID`    | `INT`           | FK, NN                                       |
| `DataEntrada`  | `DATETIME2`     | NN, DEFAULT `SYSUTCDATETIME()`               |
| `Status`       | `VARCHAR(20)`   | NN, DEFAULT `ABERTA`, CHECK                  |
| `ValorTotal`   | `DECIMAL(12,2)` | NN, DEFAULT `0`, CHECK maior ou igual a zero |

### Relacionamentos

* `Entradas.FornecedorID` referencia `Fornecedores.FornecedorID`.
* `Entradas.UsuarioID` referencia `Usuarios.UsuarioID`.

### Status permitidos

* `ABERTA`
* `CONFIRMADA`
* `CANCELADA`

## Tabela `ItensEntrada`

| Coluna          | Tipo            | Regras                          |
| --------------- | --------------- | ------------------------------- |
| `ItemEntradaID` | `INT`           | PK, IDENTITY                    |
| `EntradaID`     | `INT`           | FK, NN                          |
| `ProdutoID`     | `INT`           | FK, NN                          |
| `Quantidade`    | `INT`           | NN, CHECK maior que zero        |
| `CustoUnitario` | `DECIMAL(10,2)` | NN, CHECK maior ou igual a zero |
| `Subtotal`      | `DECIMAL(12,2)` | COMPUTED, PERSISTED             |

### Relacionamentos

* `ItensEntrada.EntradaID` referencia `Entradas.EntradaID`.
* `ItensEntrada.ProdutoID` referencia `Produtos.ProdutoID`.

### Cálculo

```text
Subtotal = Quantidade × CustoUnitario
```

A combinação `EntradaID + ProdutoID` é única, impedindo que o mesmo produto seja incluído duas vezes na mesma entrada.

## Tabela `Vendas`

| Coluna             | Tipo            | Regras                                       |
| ------------------ | --------------- | -------------------------------------------- |
| `VendaID`          | `INT`           | PK, IDENTITY                                 |
| `UsuarioID`        | `INT`           | FK, NN                                       |
| `FormaPagamentoID` | `INT`           | FK, NULL                                     |
| `DataVenda`        | `DATETIME2`     | NN, DEFAULT `SYSUTCDATETIME()`               |
| `Status`           | `VARCHAR(20)`   | NN, DEFAULT `ABERTA`, CHECK                  |
| `ValorTotal`       | `DECIMAL(12,2)` | NN, DEFAULT `0`, CHECK maior ou igual a zero |

### Relacionamentos

* `Vendas.UsuarioID` referencia `Usuarios.UsuarioID`.
* `Vendas.FormaPagamentoID` referencia `FormasPagamento.FormaPagamentoID`.

### Status permitidos

* `ABERTA`
* `CONCLUIDA`
* `CANCELADA`

A forma de pagamento é opcional na estrutura para permitir uma venda ainda em montagem. O fluxo atual exige uma forma de pagamento válida ao criar a venda.

## Tabela `ItensVenda`

| Coluna          | Tipo            | Regras                          |
| --------------- | --------------- | ------------------------------- |
| `ItemVendaID`   | `INT`           | PK, IDENTITY                    |
| `VendaID`       | `INT`           | FK, NN                          |
| `ProdutoID`     | `INT`           | FK, NN                          |
| `Quantidade`    | `INT`           | NN, CHECK maior que zero        |
| `PrecoUnitario` | `DECIMAL(10,2)` | NN, CHECK maior ou igual a zero |
| `Subtotal`      | `DECIMAL(12,2)` | COMPUTED, PERSISTED             |

### Relacionamentos

* `ItensVenda.VendaID` referencia `Vendas.VendaID`.
* `ItensVenda.ProdutoID` referencia `Produtos.ProdutoID`.

### Cálculo

```text
Subtotal = Quantidade × PrecoUnitario
```

A combinação `VendaID + ProdutoID` é única, impedindo que o mesmo produto seja incluído duas vezes na mesma venda.

O preço unitário é registrado no item para preservar o preço praticado, mesmo que o cadastro do produto seja alterado posteriormente.

## Tabela `MovimentacoesEstoque`

| Coluna             | Tipo            | Regras                         |
| ------------------ | --------------- | ------------------------------ |
| `MovimentacaoID`   | `BIGINT`        | PK, IDENTITY                   |
| `ProdutoID`        | `INT`           | FK, NN                         |
| `UsuarioID`        | `INT`           | FK, NN                         |
| `Tipo`             | `VARCHAR(20)`   | NN, CHECK                      |
| `Quantidade`       | `INT`           | NN, CHECK maior que zero       |
| `DataMovimentacao` | `DATETIME2`     | NN, DEFAULT `SYSUTCDATETIME()` |
| `OrigemTipo`       | `VARCHAR(20)`   | NULL                           |
| `OrigemID`         | `INT`           | NULL                           |
| `Observacao`       | `NVARCHAR(255)` | NULL                           |

### Relacionamentos

* `MovimentacoesEstoque.ProdutoID` referencia `Produtos.ProdutoID`.
* `MovimentacoesEstoque.UsuarioID` referencia `Usuarios.UsuarioID`.

### Tipos permitidos

* `ENTRADA`
* `SAIDA`
* `DEVOLUCAO`
* `AJUSTE`

`OrigemTipo` e `OrigemID` devem ser informados juntos ou permanecer ambos como `NULL`.

Essa regra permite:

* relacionar uma entrada à movimentação gerada;
* relacionar uma venda à movimentação gerada;
* registrar futuramente um ajuste sem uma operação comercial de origem.

## Restrições de unicidade

| Tabela            | Colunas                  | Finalidade                          |
| ----------------- | ------------------------ | ----------------------------------- |
| `Perfis`          | `Nome`                   | Impedir perfis duplicados           |
| `Usuarios`        | `Email`                  | Impedir usuários com o mesmo e-mail |
| `Categorias`      | `Nome`                   | Impedir categorias duplicadas       |
| `Produtos`        | `CodigoBarras`           | Impedir códigos de barras repetidos |
| `Fornecedores`    | `CNPJ`                   | Impedir CNPJs repetidos             |
| `FormasPagamento` | `Nome`                   | Impedir formas duplicadas           |
| `ItensEntrada`    | `EntradaID`, `ProdutoID` | Impedir produto repetido na entrada |
| `ItensVenda`      | `VendaID`, `ProdutoID`   | Impedir produto repetido na venda   |

## Integridade histórica

O modelo não utiliza exclusão em cascata nas tabelas comerciais.

Essa decisão evita que a exclusão de um cadastro remova automaticamente:

* entradas;
* vendas;
* itens;
* movimentações de estoque.

Cadastros utilizados em operações devem ser desativados em vez de excluídos.

## Objetos complementares

Além das tabelas, o banco possui:

### View

* `vw_ProdutosEstoqueBaixo`

### Procedures de entrada

* `usp_CriarEntrada`
* `usp_AdicionarItemEntrada`
* `usp_ConfirmarEntrada`

### Procedures de venda

* `usp_CriarVenda`
* `usp_AdicionarItemVenda`
* `usp_ConcluirVenda`

### Procedures de consulta

* `usp_ListarProdutosEstoqueBaixo`
* `usp_ResumoEstoque`
* `usp_VendasPorPeriodo`
* `usp_ProdutosMaisVendidos`

## Observação

Os scripts SQL são a fonte definitiva da implementação física. Este documento apresenta a estrutura lógica e as principais regras do modelo, enquanto detalhes como nomes de índices e constraints permanecem documentados nos scripts.
