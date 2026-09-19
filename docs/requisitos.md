# Requisitos do Sistema

## Projeto

**Gestão Comercial SQL — Sistema de Estoque e Vendas do Minimercado Domingues**

## Objetivo

Desenvolver um sistema de gestão comercial para controlar produtos, fornecedores, entradas de mercadorias, estoque e vendas de um minimercado fictício.

O projeto será utilizado para aplicar conhecimentos de modelagem de dados, SQL Server, administração de banco de dados, desenvolvimento de API e aplicação web.

## Perfis de usuário

### Administrador

- Gerenciar usuários.
- Cadastrar produtos, categorias e fornecedores.
- Registrar entradas de mercadorias.
- Consultar e cancelar vendas.
- Acessar relatórios e movimentações de estoque.

### Estoquista

- Cadastrar e consultar produtos.
- Cadastrar e consultar fornecedores.
- Registrar entradas de mercadorias.
- Consultar o estoque e os alertas de reposição.

### Caixa

- Consultar produtos disponíveis.
- Registrar vendas.
- Selecionar a forma de pagamento.
- Consultar vendas realizadas.

## Requisitos funcionais

- Cadastrar categorias.
- Cadastrar produtos.
- Cadastrar fornecedores.
- Registrar entradas de mercadorias.
- Registrar vendas com um ou mais produtos.
- Atualizar automaticamente o estoque.
- Impedir vendas sem estoque suficiente.
- Alertar sobre produtos com estoque baixo.
- Registrar o histórico de movimentações.
- Permitir o cancelamento de vendas.
- Controlar usuários e permissões.
- Emitir consultas e relatórios básicos.

## Regras de negócio

- Cada produto deverá pertencer a uma categoria.
- Cada produto terá preço de custo, preço de venda, estoque atual e estoque mínimo.
- Produtos com movimentações não poderão ser apagados; apenas desativados.
- A entrada de mercadorias aumentará o estoque.
- A conclusão de uma venda reduzirá o estoque.
- O sistema não permitirá estoque negativo.
- Uma venda poderá conter vários produtos.
- Uma venda poderá estar aberta, concluída ou cancelada.
- O cancelamento devolverá os produtos ao estoque.
- Toda alteração de estoque deverá gerar uma movimentação.
- Produtos com quantidade igual ou inferior ao estoque mínimo deverão aparecer no alerta de reposição.

## Formas de pagamento

- Dinheiro
- Pix
- Cartão de débito
- Cartão de crédito

## Escopo inicial

A primeira versão atenderá somente uma unidade do Minimercado Domingues e trabalhará com produtos vendidos por unidade.