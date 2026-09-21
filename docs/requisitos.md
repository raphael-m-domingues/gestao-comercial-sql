# Requisitos do Sistema

## Projeto

**Gestão Comercial SQL — Sistema de Estoque e Vendas do Minimercado Domingues**

## Objetivo

Desenvolver um sistema de gestão comercial para controlar produtos, fornecedores, entradas de mercadorias, estoque, vendas e informações gerenciais de um minimercado fictício.

O projeto aplica conhecimentos de modelagem de dados, SQL Server, T-SQL, administração de banco de dados, desenvolvimento de API e aplicação web.

## Status dos requisitos

* **Implementado:** disponível atualmente no banco de dados.
* **Planejado:** será desenvolvido na API ou na aplicação web.

## Perfis de usuário

Os perfis estão cadastrados no banco. A aplicação das permissões será implementada posteriormente na API.

### Administrador

* Gerenciar usuários e perfis.
* Cadastrar produtos, categorias e fornecedores.
* Registrar entradas de mercadorias.
* Consultar e cancelar vendas.
* Consultar relatórios e movimentações de estoque.

### Estoquista

* Cadastrar e consultar produtos.
* Cadastrar e consultar fornecedores.
* Registrar entradas de mercadorias.
* Consultar o estoque.
* Consultar alertas de reposição.

### Caixa

* Consultar produtos disponíveis.
* Registrar vendas.
* Selecionar a forma de pagamento.
* Consultar vendas realizadas.

## Requisitos funcionais implementados

| Código | Requisito                                       | Status                |
| ------ | ----------------------------------------------- | --------------------- |
| RF01   | Cadastrar categorias                            | Implementado          |
| RF02   | Cadastrar produtos                              | Implementado          |
| RF03   | Cadastrar fornecedores                          | Implementado          |
| RF04   | Cadastrar formas de pagamento                   | Implementado          |
| RF05   | Manter perfis e usuários                        | Implementado no banco |
| RF06   | Registrar entradas de mercadorias               | Implementado          |
| RF07   | Adicionar um ou mais produtos a uma entrada     | Implementado          |
| RF08   | Confirmar uma entrada e aumentar o estoque      | Implementado          |
| RF09   | Registrar vendas com um ou mais produtos        | Implementado          |
| RF10   | Utilizar o preço de venda cadastrado no produto | Implementado          |
| RF11   | Validar o estoque antes da conclusão da venda   | Implementado          |
| RF12   | Concluir uma venda e reduzir o estoque          | Implementado          |
| RF13   | Impedir que o estoque fique negativo            | Implementado          |
| RF14   | Registrar o histórico de movimentações          | Implementado          |
| RF15   | Identificar produtos sem estoque                | Implementado          |
| RF16   | Identificar produtos com estoque baixo          | Implementado          |
| RF17   | Calcular a quantidade necessária para reposição | Implementado          |
| RF18   | Consultar o resumo geral do estoque             | Implementado          |
| RF19   | Consultar vendas por período                    | Implementado          |
| RF20   | Consultar produtos mais vendidos                | Implementado          |

## Requisitos funcionais planejados

| Código | Requisito                                                      | Etapa         |
| ------ | -------------------------------------------------------------- | ------------- |
| RF21   | Autenticar usuários com e-mail e senha                         | API           |
| RF22   | Aplicar permissões conforme o perfil do usuário                | API           |
| RF23   | Disponibilizar os cadastros pela aplicação web                 | Aplicação web |
| RF24   | Registrar entradas pela aplicação web                          | Aplicação web |
| RF25   | Registrar vendas pela aplicação web                            | Aplicação web |
| RF26   | Exibir alertas de estoque baixo no painel                      | Aplicação web |
| RF27   | Exibir indicadores e relatórios gerenciais                     | Aplicação web |
| RF28   | Cancelar uma entrada aberta                                    | Banco e API   |
| RF29   | Cancelar uma venda aberta                                      | Banco e API   |
| RF30   | Estornar uma venda concluída e devolver os produtos ao estoque | Banco e API   |

## Regras de negócio implementadas

| Código | Regra                                                                                        |
| ------ | -------------------------------------------------------------------------------------------- |
| RN01   | Cada produto deve pertencer a uma categoria.                                                 |
| RN02   | Cada produto possui preço de custo, preço de venda e estoque mínimo.                         |
| RN03   | Cada produto possui somente um registro de estoque.                                          |
| RN04   | Produtos, categorias, fornecedores, usuários e formas de pagamento podem ser desativados.    |
| RN05   | Uma entrada deve possuir um fornecedor e um usuário responsáveis.                            |
| RN06   | Uma entrada aberta pode receber vários produtos.                                             |
| RN07   | O mesmo produto não pode aparecer mais de uma vez na mesma entrada.                          |
| RN08   | Uma entrada somente pode ser confirmada se possuir itens.                                    |
| RN09   | A confirmação da entrada aumenta o estoque.                                                  |
| RN10   | Uma entrada confirmada não pode ser confirmada novamente.                                    |
| RN11   | Uma venda deve possuir um usuário responsável e uma forma de pagamento.                      |
| RN12   | Uma venda aberta pode receber vários produtos.                                               |
| RN13   | O mesmo produto não pode aparecer mais de uma vez na mesma venda.                            |
| RN14   | O preço registrado no item da venda deve representar o preço do produto no momento da venda. |
| RN15   | Uma venda somente pode ser concluída se possuir itens.                                       |
| RN16   | A conclusão da venda deve validar novamente o estoque disponível.                            |
| RN17   | A conclusão da venda reduz o estoque.                                                        |
| RN18   | Uma venda concluída não pode ser concluída novamente.                                        |
| RN19   | O sistema não permite estoque negativo.                                                      |
| RN20   | Toda entrada ou saída de estoque gera uma movimentação.                                      |
| RN21   | Cada movimentação registra produto, usuário, tipo, quantidade, data e origem.                |
| RN22   | Produtos com quantidade igual ou inferior ao estoque mínimo aparecem no alerta de reposição. |
| RN23   | Operações de confirmação são executadas dentro de transações.                                |
| RN24   | Se ocorrer um erro durante uma operação transacional, todas as alterações são desfeitas.     |
| RN25   | Registros históricos não são excluídos em cascata.                                           |

## Regras de negócio planejadas

| Código | Regra                                                                        |
| ------ | ---------------------------------------------------------------------------- |
| RN26   | Senhas deverão ser armazenadas somente como hashes seguros gerados pela API. |
| RN27   | Usuários inativos não poderão acessar a aplicação.                           |
| RN28   | As funcionalidades disponíveis serão limitadas pelo perfil do usuário.       |
| RN29   | O cancelamento de uma entrada aberta não alterará o estoque.                 |
| RN30   | O cancelamento de uma venda aberta não alterará o estoque.                   |
| RN31   | O estorno de uma venda concluída devolverá os produtos ao estoque.           |
| RN32   | O estorno gerará movimentações de devolução vinculadas à venda original.     |
| RN33   | Uma venda concluída não poderá ser simplesmente excluída.                    |

## Status das operações

### Entradas

* `ABERTA`
* `CONFIRMADA`
* `CANCELADA`

### Vendas

* `ABERTA`
* `CONCLUIDA`
* `CANCELADA`

Embora o banco aceite o status `CANCELADA`, os fluxos de cancelamento e estorno fazem parte dos requisitos planejados.

## Tipos de movimentação de estoque

* `ENTRADA`
* `SAIDA`
* `DEVOLUCAO`
* `AJUSTE`

Atualmente, os fluxos de entrada e venda utilizam os tipos `ENTRADA` e `SAIDA`. Os tipos `DEVOLUCAO` e `AJUSTE` serão utilizados em funcionalidades futuras.

## Formas de pagamento iniciais

* Dinheiro
* Pix
* Cartão de débito
* Cartão de crédito

## Consultas gerenciais

O sistema disponibiliza consultas para:

* total de produtos ativos;
* total de unidades em estoque;
* valor do estoque pelo preço de custo;
* valor potencial do estoque pelo preço de venda;
* produtos sem estoque;
* produtos com estoque baixo;
* vendas concluídas por período;
* total vendido;
* ticket médio;
* produtos mais vendidos.

## Requisitos não funcionais

| Código | Requisito                                                                           |
| ------ | ----------------------------------------------------------------------------------- |
| RNF01  | Utilizar SQL Server como sistema gerenciador de banco de dados.                     |
| RNF02  | Utilizar transações nas operações que alteram estoque.                              |
| RNF03  | Manter integridade por meio de chaves primárias e estrangeiras.                     |
| RNF04  | Aplicar restrições `CHECK`, `UNIQUE`, `DEFAULT` e campos obrigatórios.              |
| RNF05  | Armazenar textos que aceitam acentuação utilizando tipos Unicode quando necessário. |
| RNF06  | Manter scripts versionados no GitHub.                                               |
| RNF07  | Manter evidências e documentação das etapas do projeto.                             |
| RNF08  | Não armazenar senhas em texto simples.                                              |
| RNF09  | Utilizar consultas parametrizadas na futura API.                                    |
| RNF10  | Preservar o histórico das operações comerciais.                                     |

## Escopo inicial

A primeira versão atende somente uma unidade do Minimercado Domingues e trabalha com produtos vendidos por unidade.

Não fazem parte do escopo inicial:

* múltiplas filiais;
* vendas por peso;
* emissão fiscal;
* integração com meios de pagamento;
* integração com fornecedores;
* comércio eletrônico;
* controle financeiro completo;
* programa de fidelidade.

## Critérios de conclusão da etapa de banco de dados

A etapa principal do banco será considerada concluída quando:

* todos os scripts puderem ser executados na ordem documentada;
* as tabelas e restrições forem criadas corretamente;
* as entradas aumentarem o estoque;
* as vendas reduzirem o estoque;
* vendas sem estoque suficiente forem rejeitadas;
* as movimentações forem registradas;
* os alertas de estoque baixo funcionarem;
* as consultas gerenciais retornarem os resultados esperados;
* os testes integrados forem executados sem deixar dados temporários no banco.
