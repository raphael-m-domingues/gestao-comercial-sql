# Gestão Comercial SQL

Sistema de gestão comercial e controle de estoque desenvolvido para o **Minimercado Domingues**.

O projeto integra um banco de dados SQL Server a uma aplicação web ASP.NET Core MVC. Ele permite administrar produtos, categorias, fornecedores, entradas de mercadorias, vendas, estoque, usuários, movimentações e relatórios gerenciais.

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
| Gerenciar fornecedores | Sim | Sim | Não |
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

### Fornecedores

- Listagem de fornecedores.
- Cadastro de novos fornecedores.
- Edição das informações cadastradas.
- Ativação e desativação de registros.
- Cadastro de razão social, nome fantasia, CNPJ, telefone e e-mail.
- Normalização do CNPJ e telefone antes do armazenamento.
- Formatação do CNPJ para exibição.
- Validação do formato do CNPJ e do endereço de e-mail.
- Proteção contra o cadastro de CNPJ duplicado.
- Preservação de fornecedores inativos no histórico de entradas.
- Acesso restrito ao Administrador e Estoquista.

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