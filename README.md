# Gestão Comercial SQL

Sistema de gestão comercial e controle de estoque desenvolvido para o **Minimercado Domingues**.

O projeto integra um banco de dados SQL Server a uma aplicação web ASP.NET Core MVC. Ele permite administrar produtos, categorias, entradas de mercadorias, vendas, estoque, usuários, movimentações e relatórios gerenciais.

## Objetivos

- Praticar modelagem de banco de dados relacional.
- Implementar regras de negócio com T-SQL.
- Integrar SQL Server e ASP.NET Core MVC.
- Controlar entradas e saídas de produtos.
- Manter o histórico das movimentações de estoque.
- Evitar vendas com estoque insuficiente.
- Identificar produtos sem estoque ou com estoque baixo.
- Aplicar transações e validações em operações críticas.
- Implementar autenticação e autorização por perfil.
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
- ASP.NET Core Identity PasswordHasher
- Autenticação por cookie
- Bootstrap
- Razor Views

### Desenvolvimento e versionamento

- Visual Studio Code
- Git
- GitHub

## Funcionalidades implementadas

### Autenticação e segurança

- Login por e-mail e senha.
- Armazenamento seguro das senhas com `PasswordHasher`.
- Autenticação por cookie.
- Opção para manter o usuário conectado.
- Redirecionamento para a página originalmente solicitada após o login.
- Encerramento da sessão por logout.
- Bloqueio de acesso para usuários inativos.
- Encerramento automático da sessão após a desativação do usuário.
- Validação do perfil armazenado no cookie.
- Página personalizada para acesso negado.
- Proteção contra requisições falsificadas com antiforgery token.
- Connection string e credenciais iniciais armazenadas em User Secrets.
- Mensagem genérica para tentativas de login inválidas.

### Perfis de acesso

| Funcionalidade | Administrador | Estoquista | Caixa |
| --- | :---: | :---: | :---: |
| Painel de estoque | Sim | Sim | Não |
| Consultar produtos | Sim | Sim | Sim |
| Gerenciar produtos | Sim | Sim | Não |
| Gerenciar categorias | Sim | Sim | Não |
| Registrar entradas | Sim | Sim | Não |
| Registrar vendas | Sim | Não | Sim |
| Cancelar vendas concluídas | Sim | Não | Não |
| Consultar estoque baixo | Sim | Sim | Não |
| Consultar relatórios | Sim | Não | Não |
| Gerenciar usuários | Sim | Não | Não |

### Gerenciamento de usuários

- Listagem das contas cadastradas.
- Cadastro de novos usuários.
- Associação do usuário a um perfil.
- Ativação e desativação de contas.
- Redefinição segura de senha.
- Proteção contra a desativação da própria conta.
- Criação segura do primeiro administrador.
- Revogação da sessão de usuários desativados.

### Painel inicial

- Resumo geral do estoque.
- Quantidade de produtos ativos.
- Total de unidades armazenadas.
- Valor do estoque pelo preço de custo.
- Valor potencial do estoque pelo preço de venda.
- Quantidade de produtos sem estoque.
- Quantidade de produtos com estoque baixo.
- Redirecionamento do perfil Caixa diretamente para Vendas.

### Categorias

- Listagem de categorias.
- Cadastro de novas categorias.
- Edição de categorias.
- Ativação e desativação de registros.
- Validação das informações cadastradas.
- Acesso restrito ao Administrador e Estoquista.

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
- Ocultação do preço de custo e das ações administrativas para o Caixa.

### Entrada de mercadorias

- Criação de entradas.
- Seleção do fornecedor.
- Identificação automática do usuário conectado como responsável.
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
- Identificação automática do usuário conectado como responsável.
- Seleção da forma de pagamento.
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

### Cancelamento de vendas

- Cancelamento restrito ao Administrador.
- Cancelamento permitido somente para vendas concluídas.
- Devolução automática dos produtos ao estoque.
- Registro das movimentações de devolução.
- Proteção contra cancelamento repetido.
- Preservação do histórico da venda e dos itens.

### Controle de estoque

- Quantidade atual por produto.
- Estoque mínimo configurável.
- Histórico de entradas, saídas e devoluções.
- Identificação de produtos sem estoque.
- Identificação de produtos com estoque baixo.
- Cálculo da quantidade necessária para reposição.
- Atualização automática após entradas, vendas e cancelamentos.

### Relatórios gerenciais

- Resumo geral do estoque.
- Produtos sem estoque.
- Produtos com estoque baixo.
- Vendas por período.
- Quantidade de vendas.
- Total vendido.
- Ticket médio.
- Ranking de produtos mais vendidos.
- Filtro por período.
- Definição da quantidade de produtos exibidos no ranking.
- Acesso restrito ao Administrador.

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
- `usp_CancelarVenda`

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
│       ├── 15_padronizar_horario_vendas.sql
│       └── 16_cancelar_venda.sql
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
│       ├── Services/
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

Na raiz do repositório, configure a conexão por meio do User Secrets:

```powershell
dotnet user-secrets set `
    --project src/GestaoComercial.Web `
    "ConnectionStrings:DefaultConnection" `
    "Server=SERVIDOR;Database=DB_GestaoComercial;Trusted_Connection=True;TrustServerCertificate=True;"
```

Substitua `SERVIDOR` pelo nome da sua instância do SQL Server.

O uso do User Secrets evita armazenar a connection string diretamente no repositório.

## Criação do primeiro administrador

Em uma instalação nova, configure temporariamente os dados do primeiro administrador:

```powershell
dotnet user-secrets set `
    --project src/GestaoComercial.Web `
    "BootstrapAdmin:Nome" `
    "Administrador do Sistema"

dotnet user-secrets set `
    --project src/GestaoComercial.Web `
    "BootstrapAdmin:Email" `
    "admin@minimercado.local"

dotnet user-secrets set `
    --project src/GestaoComercial.Web `
    "BootstrapAdmin:Senha" `
    "DEFINA-UMA-SENHA-FORTE"
```

Na primeira inicialização, a aplicação cria o administrador utilizando um hash seguro.

Depois que a conta for criada, remova a senha temporária:

```powershell
dotnet user-secrets remove `
    --project src/GestaoComercial.Web `
    "BootstrapAdmin:Senha"
```

Os demais usuários devem ser cadastrados pelo Administrador na própria aplicação.

## Execução da aplicação

Na raiz do repositório, restaure e compile a solução:

```powershell
dotnet restore GestaoComercial.slnx
dotnet build GestaoComercial.slnx
```

Depois execute o projeto web:

```powershell
dotnet run --project src/GestaoComercial.Web
```

Acesse no navegador o endereço exibido no terminal.

## Regras de negócio importantes

- Uma entrada somente pode ser confirmada se possuir itens.
- Uma entrada confirmada aumenta o estoque.
- Uma venda somente pode ser concluída se possuir itens.
- Uma venda não pode ser concluída sem estoque suficiente.
- Uma venda concluída reduz o estoque.
- Uma venda cancelada devolve os produtos ao estoque.
- Entradas, vendas e cancelamentos geram movimentações de estoque.
- Operações críticas são executadas dentro de transações.
- Entradas ou vendas concluídas não podem ser processadas novamente.
- Vendas canceladas não podem ser canceladas novamente.
- Produtos inativos não podem ser utilizados em novas operações.
- Dados históricos não são excluídos em cascata.
- Valores monetários utilizam duas casas decimais.
- A aplicação utiliza a cultura `pt-BR` para datas e valores.
- Entradas e vendas são associadas automaticamente ao usuário conectado.
- Usuários inativos não podem acessar a aplicação.
- Funcionalidades são autorizadas de acordo com o perfil do usuário.

## Arquitetura da aplicação

A aplicação utiliza a estrutura padrão do ASP.NET Core MVC:

- **Models:** representam os dados utilizados pelas telas.
- **Views:** apresentam as informações e formulários ao usuário.
- **Controllers:** recebem as requisições e controlam o fluxo da aplicação.
- **Repositories:** executam consultas e procedures no SQL Server com Dapper.
- **Services:** executam inicialização e validações relacionadas à autenticação.
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
- Cancelamento de vendas e devolução ao estoque.
- Atualização automática do estoque.
- Registro do histórico de movimentações.
- Telas de relatórios gerenciais.
- Autenticação por cookie.
- Gerenciamento de usuários.
- Autorização por perfis.
- Revogação da sessão de usuários desativados.
- Identificação automática do responsável pelas operações.

### Próximas etapas

- Adicionar filtros e paginação às listagens.
- Ampliar os testes automatizados da aplicação.
- Criar uma API para integração com outros sistemas.
- Melhorar a observabilidade e o tratamento global de erros.
- Preparar a aplicação para publicação.

## Autor

**Raphael Domingues**

Projeto desenvolvido para estudo e prática de SQL Server, modelagem de dados, T-SQL, C#, ASP.NET Core MVC, segurança e integração entre banco de dados e aplicação web.