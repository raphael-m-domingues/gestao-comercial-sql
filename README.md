# Gestão Comercial SQL

Sistema de gestão de estoque e vendas do **Minimercado Domingues**, desenvolvido como projeto prático utilizando SQL Server.

O projeto contempla a modelagem e a implementação de um banco de dados comercial, incluindo cadastros, controle de entradas, registro de vendas, movimentações de estoque, alertas e consultas gerenciais.

A aplicação web responsável por consumir o banco de dados será desenvolvida na próxima etapa do projeto.

## Objetivos

* Praticar modelagem de banco de dados relacional.
* Implementar regras de negócio utilizando T-SQL.
* Controlar entradas e saídas de produtos.
* Manter um histórico das movimentações de estoque.
* Evitar vendas com estoque insuficiente.
* Identificar produtos com estoque baixo.
* Criar consultas gerenciais para uma futura aplicação web.
* Documentar e versionar a evolução do projeto.

## Tecnologias

* SQL Server 2022
* SQL Server Management Studio — SSMS
* T-SQL
* Git
* GitHub
* Visual Studio Code

## Funcionalidades implementadas

### Cadastros

* Perfis de acesso
* Usuários
* Categorias
* Produtos
* Estoques
* Fornecedores
* Formas de pagamento

### Entrada de mercadorias

* Criação de entradas.
* Inclusão de produtos na entrada.
* Cálculo automático do valor total.
* Confirmação da entrada.
* Aumento automático do estoque.
* Registro da movimentação de entrada.
* Validação de fornecedor, usuário e produtos.

### Venda de produtos

* Criação de vendas.
* Inclusão de produtos na venda.
* Utilização do preço de venda cadastrado.
* Cálculo automático do valor total.
* Validação do estoque disponível.
* Conclusão da venda.
* Baixa automática do estoque.
* Registro da movimentação de saída.
* Proteção contra estoque negativo.

### Controle de estoque

* Quantidade atual por produto.
* Estoque mínimo configurável.
* Histórico de entradas e saídas.
* Identificação de produtos sem estoque.
* Identificação de produtos com estoque baixo.
* Cálculo da quantidade necessária para reposição.

### Consultas gerenciais

* Resumo geral do estoque.
* Quantidade de produtos ativos.
* Total de unidades armazenadas.
* Valor do estoque pelo custo.
* Valor potencial do estoque pela venda.
* Produtos sem estoque.
* Produtos com estoque baixo.
* Vendas por período.
* Total vendido e ticket médio.
* Ranking de produtos mais vendidos.

## Principais tabelas

| Grupo     | Tabelas                                                     |
| --------- | ----------------------------------------------------------- |
| Acesso    | `Perfis`, `Usuarios`                                        |
| Cadastros | `Categorias`, `Produtos`, `Fornecedores`, `FormasPagamento` |
| Estoque   | `Estoques`, `MovimentacoesEstoque`                          |
| Entradas  | `Entradas`, `ItensEntrada`                                  |
| Vendas    | `Vendas`, `ItensVenda`                                      |

## Objetos programáveis

### Procedures de entrada

* `usp_CriarEntrada`
* `usp_AdicionarItemEntrada`
* `usp_ConfirmarEntrada`

### Procedures de venda

* `usp_CriarVenda`
* `usp_AdicionarItemVenda`
* `usp_ConcluirVenda`

### Estoque baixo

* `vw_ProdutosEstoqueBaixo`
* `usp_ListarProdutosEstoqueBaixo`

### Relatórios gerenciais

* `usp_ResumoEstoque`
* `usp_VendasPorPeriodo`
* `usp_ProdutosMaisVendidos`

## Estrutura do repositório

```text
gestao-comercial-sql/
├── database/
│   └── scripts/
│       ├── 01_criar_banco.sql
│       ├── 02_criar_tabelas_cadastro.sql
│       ├── 03_inserir_dados_iniciais.sql
│       ├── 04_criar_tabelas_transacionais.sql
│       ├── 05_fluxo_entrada_estoque.sql
│       ├── 06_testar_fluxo_entrada.sql
│       ├── 07_fluxo_venda_estoque.sql
│       ├── 08_testar_fluxo_venda.sql
│       ├── 09_consulta_estoque_baixo.sql
│       ├── 10_consultas_gerenciais.sql
│       └── 11_testar_consultas_gerenciais.sql
├── docs/
│   ├── evidencias/
│   ├── modelagem-conceitual.md
│   ├── modelagem-logica.md
│   └── requisitos.md
└── README.md
```

## Ordem de execução

Para criar o ambiente, execute os scripts nesta ordem:

1. `01_criar_banco.sql`
2. `02_criar_tabelas_cadastro.sql`
3. `03_inserir_dados_iniciais.sql`
4. `04_criar_tabelas_transacionais.sql`
5. `05_fluxo_entrada_estoque.sql`
6. `07_fluxo_venda_estoque.sql`
7. `09_consulta_estoque_baixo.sql`
8. `10_consultas_gerenciais.sql`

Os scripts abaixo são utilizados somente para testes:

* `06_testar_fluxo_entrada.sql`
* `08_testar_fluxo_venda.sql`
* `11_testar_consultas_gerenciais.sql`

Os testes são executados dentro de transações e utilizam `ROLLBACK` ao final, evitando que os dados temporários permaneçam no banco.

## Banco de dados

O banco utilizado pelo projeto é:

```text
DB_GestaoComercial
```

O modelo de recuperação utilizado é `SIMPLE`, adequado ao ambiente local de desenvolvimento e estudos.

## Regras importantes

* Uma entrada somente pode ser confirmada se possuir itens.
* Uma entrada confirmada aumenta o estoque.
* Uma venda somente pode ser concluída se possuir itens.
* Uma venda não pode ser concluída sem estoque suficiente.
* Uma venda concluída reduz o estoque.
* Entradas e saídas geram registros em `MovimentacoesEstoque`.
* Operações críticas são executadas dentro de transações.
* Entradas ou vendas já concluídas não podem ser processadas novamente.
* Dados históricos não são excluídos em cascata.

## Documentação

A pasta `docs` contém:

* requisitos funcionais e regras de negócio;
* modelagem conceitual;
* modelagem lógica;
* evidências das execuções realizadas no SQL Server.

## Status

### Concluído

* Levantamento de requisitos
* Modelagem conceitual
* Modelagem lógica
* Criação do banco de dados
* Tabelas de cadastro
* Tabelas transacionais
* Dados iniciais
* Fluxo de entrada
* Fluxo de venda
* Movimentações de estoque
* Consulta de estoque baixo
* Consultas gerenciais
* Testes integrados com `ROLLBACK`

### Próximas etapas

* Desenvolver a API de integração.
* Criar a aplicação web.
* Implementar autenticação de usuários.
* Criar telas de produtos, entradas e vendas.
* Criar painel com indicadores gerenciais.
* Integrar os alertas de estoque baixo à interface.

## Autor

**Raphael Domingues**

Projeto desenvolvido para estudo e prática de SQL Server, modelagem de dados, regras de negócio e integração com aplicação web.
