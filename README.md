# Gestão Comercial SQL

Sistema de gestão comercial e controle de estoque desenvolvido para o **Minimercado Domingues**.

O projeto integra um banco de dados SQL Server a uma aplicação web ASP.NET Core MVC. Ele permite administrar produtos, categorias, entradas de mercadorias, vendas, estoque e movimentações.

## Objetivos

- Praticar modelagem de banco de dados relacional.
- Implementar regras de negócio com T-SQL.
- Integrar SQL Server e ASP.NET Core MVC.
- Controlar entradas e saídas de produtos.
- Manter o histórico das movimentações de estoque.
- Evitar vendas com estoque insuficiente.
- Identificar produtos sem estoque ou com estoque baixo.
- Aplicar transações e validações em operações críticas.
- Documentar e versionar a evolução do projeto.

## Tecnologias

### Banco de dados

- SQL Server 2022
- SQL Server Management Studio — SSMS
- T-SQL

### Aplicação web

- .NET 10
- ASP.NET Core MVC
- C#
- Dapper
- Microsoft.Data.SqlClient
- Bootstrap
- Razor Views

### Desenvolvimento e versionamento

- Visual Studio Code
- Git
- GitHub

## Funcionalidades implementadas

### Painel inicial

- Resumo geral do estoque.
- Quantidade de produtos ativos.
- Total de unidades armazenadas.
- Valor do estoque pelo preço de custo.
- Valor potencial do estoque pelo preço de venda.
- Quantidade de produtos sem estoque.
- Quantidade de produtos com estoque baixo.

### Categorias

- Listagem de categorias.
- Cadastro de novas categorias.
- Edição de categorias.
- Ativação e desativação de registros.
- Validação das informações cadastradas.

### Produtos

- Listagem de produtos.
- Cadastro de novos produtos.
- Edição de produtos.
- Ativação e desativação de registros.
- Associação do produto com uma categoria.
- Configuração dos preços de custo e venda.
- Configuração do estoque mínimo.
- Exibição da quantidade atual em estoque.
- Identificação visual da situação do estoque.

### Entrada de mercadorias

- Criação de entradas.
- Seleção do fornecedor e do responsável.
- Inclusão de produtos na entrada.
- Alteração da quantidade e do custo unitário.
- Remoção de produtos antes da confirmação.
- Cálculo automático dos subtotais e do valor total.
- Confirmação do recebimento.
- Aumento automático das quantidades em estoque.
- Registro das movimentações do tipo `ENTRADA`.
- Proteção contra o processamento repetido da entrada.

### Venda de produtos

- Criação de vendas.
- Seleção do responsável e da forma de pagamento.
- Inclusão de produtos na venda.
- Utilização do preço de venda cadastrado.
- Alteração da quantidade antes da conclusão.
- Remoção de produtos da venda.
- Cálculo automático dos subtotais e do valor total.
- Validação do estoque disponível.
- Conclusão da venda.
- Baixa automática das quantidades em estoque.
- Registro das movimentações do tipo `SAIDA`.
- Proteção contra estoque negativo.
- Proteção contra o processamento repetido da venda.

### Controle de estoque

- Quantidade atual por produto.
- Estoque mínimo configurável.
- Histórico de entradas e saídas.
- Identificação de produtos sem estoque.
- Identificação de produtos com estoque baixo.
- Cálculo da quantidade necessária para reposição.
- Atualização automática após entradas e vendas.

### Consultas gerenciais no banco

- Resumo geral do estoque.
- Produtos sem estoque.
- Produtos com estoque baixo.
- Vendas por período.
- Total vendido.
- Ticket médio.
- Ranking de produtos mais vendidos.

## Principais tabelas

| Grupo | Tabelas |
| --- | --- |
| Acesso | `Perfis`, `Usuarios` |
| Cadastros | `Categorias`, `Produtos`, `Fornecedores`, `FormasPagamento` |
| Estoque | `Estoques`, `MovimentacoesEstoque` |
| Entradas | `Entradas`, `ItensEntrada` |
| Vendas | `Vendas`, `ItensVenda` |

## Objetos programáveis

### Procedures de entrada

- `usp_CriarEntrada`
- `usp_AdicionarItemEntrada`
- `usp_AtualizarItemEntrada`
- `usp_RemoverItemEntrada`
- `usp_ConfirmarEntrada`

### Procedures de venda

- `usp_CriarVenda`
- `usp_AdicionarItemVenda`
- `usp_AtualizarItemVenda`
- `usp_RemoverItemVenda`
- `usp_ConcluirVenda`

### Estoque baixo

- `vw_ProdutosEstoqueBaixo`
- `usp_ListarProdutosEstoqueBaixo`

### Consultas gerenciais

- `usp_ResumoEstoque`
- `usp_VendasPorPeriodo`
- `usp_ProdutosMaisVendidos`

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
│       ├── 11_testar_consultas_gerenciais.sql
│       ├── 12_gerenciar_itens_entrada.sql
│       ├── 13_padronizar_horario_movimentacoes.sql
│       ├── 14_gerenciar_itens_venda.sql
│       └── 15_padronizar_horario_vendas.sql
├── docs/
│   ├── evidencias/
│   ├── modelagem-conceitual.md
│   ├── modelagem-logica.md
│   └── requisitos.md
├── src/
│   └── GestaoComercial.Web/
│       ├── Controllers/
│       ├── Data/
│       ├── Models/
│       ├── Views/
│       ├── wwwroot/
│       └── Program.cs
├── GestaoComercial.slnx
└── README.md
```

## Preparação do banco de dados

O banco utilizado pelo projeto é:

```text
DB_GestaoComercial
```

Execute os scripts da pasta `database/scripts` seguindo a ordem numérica dos arquivos.

Os scripts de teste utilizam transações e `ROLLBACK` para evitar que dados temporários permaneçam no banco.

O modelo de recuperação utilizado é `SIMPLE`, adequado ao ambiente local de desenvolvimento e estudos.

## Configuração da aplicação

A aplicação utiliza uma connection string chamada `DefaultConnection`.

Na pasta do projeto web, configure a conexão por meio do User Secrets:

```powershell
cd src/GestaoComercial.Web

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=SERVIDOR;Database=DB_GestaoComercial;Trusted_Connection=True;TrustServerCertificate=True;"
```

Substitua `SERVIDOR` pelo nome da sua instância do SQL Server.

O uso do User Secrets evita armazenar a connection string diretamente no repositório.

## Execução da aplicação

Na raiz do repositório, restaure e compile a solução:

```powershell
dotnet restore GestaoComercial.slnx
dotnet build GestaoComercial.slnx
```

Depois execute o projeto web:

```powershell
dotnet run --project src/GestaoComercial.Web --launch-profile https
```

Acesse no navegador o endereço HTTPS exibido no terminal.

## Regras de negócio importantes

- Uma entrada somente pode ser confirmada se possuir itens.
- Uma entrada confirmada aumenta o estoque.
- Uma venda somente pode ser concluída se possuir itens.
- Uma venda não pode ser concluída sem estoque suficiente.
- Uma venda concluída reduz o estoque.
- Entradas e vendas geram registros em `MovimentacoesEstoque`.
- Operações críticas são executadas dentro de transações.
- Entradas ou vendas concluídas não podem ser processadas novamente.
- Produtos inativos não podem ser utilizados em novas operações.
- Dados históricos não são excluídos em cascata.
- Valores monetários utilizam duas casas decimais.
- A aplicação utiliza a cultura `pt-BR` para datas e valores.

## Arquitetura da aplicação

A aplicação utiliza a estrutura padrão do ASP.NET Core MVC:

- **Models:** representam os dados utilizados pelas telas.
- **Views:** apresentam as informações e formulários ao usuário.
- **Controllers:** recebem as requisições e controlam o fluxo da aplicação.
- **Repositories:** executam consultas e procedures no SQL Server com Dapper.
- **SQL Server:** armazena os dados e aplica as principais regras de negócio.

## Documentação

A pasta `docs` contém:

- requisitos funcionais;
- regras de negócio;
- modelagem conceitual;
- modelagem lógica;
- evidências das execuções realizadas no SQL Server.

## Status do projeto

### Concluído

- Levantamento de requisitos.
- Modelagem conceitual.
- Modelagem lógica.
- Criação do banco de dados.
- Tabelas de cadastro e transacionais.
- Dados iniciais.
- Procedures e regras de negócio.
- Consultas gerenciais no banco.
- Testes transacionais com `ROLLBACK`.
- Integração entre ASP.NET Core MVC e SQL Server.
- Painel inicial com indicadores.
- Cadastro e gerenciamento de categorias.
- Cadastro e gerenciamento de produtos.
- Consulta de estoque baixo.
- Fluxo completo de entrada de mercadorias.
- Fluxo completo de vendas.
- Atualização automática do estoque.
- Registro do histórico de movimentações.

### Próximas etapas

- Criar telas para os relatórios gerenciais.
- Implementar autenticação e autorização de usuários.
- Restringir funcionalidades conforme o perfil de acesso.
- Adicionar filtros e paginação às listagens.
- Ampliar os testes da aplicação.
- Preparar a aplicação para publicação.

## Autor

**Raphael Domingues**

Projeto desenvolvido para estudo e prática de SQL Server, modelagem de dados, T-SQL, C#, ASP.NET Core MVC e integração entre banco de dados e aplicação web.