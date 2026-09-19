# Modelagem Conceitual

## Objetivo

Identificar as principais entidades do sistema Gestão Comercial SQL e os relacionamentos existentes entre elas.

## Entidades

### Perfil

Representa o nível de acesso do usuário ao sistema.

Exemplos:

- Administrador
- Estoquista
- Caixa

### Usuário

Representa a pessoa autorizada a utilizar o sistema.

Cada usuário deverá possuir um perfil e poderá ser responsável por vendas, entradas e movimentações de estoque.

### Categoria

Organiza os produtos por tipo.

Exemplos:

- Alimentos
- Bebidas
- Higiene
- Limpeza

### Produto

Representa um item comercializado pelo Minimercado Domingues.

Cada produto pertencerá a uma categoria e terá informações como preço de custo, preço de venda e estoque mínimo.

### Estoque

Armazena a quantidade atualmente disponível de cada produto.

Cada produto terá um único registro de estoque.

### Fornecedor

Representa a empresa responsável por fornecer mercadorias ao minimercado.

### Entrada

Representa o recebimento de mercadorias de um fornecedor.

Cada entrada será registrada por um usuário e poderá conter vários produtos.

### Item da entrada

Representa cada produto incluído em uma entrada de mercadorias, com sua quantidade e custo unitário.

### Venda

Representa uma operação de venda realizada no minimercado.

Cada venda será registrada por um usuário, terá uma forma de pagamento e poderá conter vários produtos.

### Item da venda

Representa cada produto incluído em uma venda, com sua quantidade, preço unitário e subtotal.

### Forma de pagamento

Representa a maneira utilizada para pagar uma venda.

Exemplos:

- Dinheiro
- Pix
- Cartão de débito
- Cartão de crédito

### Movimentação de estoque

Registra qualquer alteração na quantidade de um produto.

Exemplos:

- Entrada
- Saída por venda
- Devolução por cancelamento
- Ajuste de estoque

## Relacionamentos

- Um perfil poderá pertencer a vários usuários.
- Uma categoria poderá conter vários produtos.
- Cada produto terá um registro de estoque.
- Um fornecedor poderá possuir várias entradas.
- Um usuário poderá registrar várias entradas.
- Uma entrada deverá conter um ou mais itens.
- Um produto poderá aparecer em vários itens de entrada.
- Um usuário poderá registrar várias vendas.
- Uma forma de pagamento poderá ser utilizada em várias vendas.
- Uma venda deverá conter um ou mais itens.
- Um produto poderá aparecer em vários itens de venda.
- Um produto poderá possuir várias movimentações de estoque.
- Um usuário poderá ser responsável por várias movimentações.

## Diagrama conceitual

```mermaid
erDiagram
    PERFIL ||--o{ USUARIO : possui
    CATEGORIA ||--o{ PRODUTO : classifica
    PRODUTO ||--|| ESTOQUE : possui
    FORNECEDOR ||--o{ ENTRADA : fornece
    USUARIO ||--o{ ENTRADA : registra
    ENTRADA ||--|{ ITEM_ENTRADA : contem
    PRODUTO ||--o{ ITEM_ENTRADA : recebido
    USUARIO ||--o{ VENDA : registra
    FORMA_PAGAMENTO ||--o{ VENDA : utilizada
    VENDA ||--|{ ITEM_VENDA : contem
    PRODUTO ||--o{ ITEM_VENDA : vendido
    PRODUTO ||--o{ MOVIMENTACAO_ESTOQUE : movimenta
    USUARIO ||--o{ MOVIMENTACAO_ESTOQUE : registra
```