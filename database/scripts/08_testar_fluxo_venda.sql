USE DB_GestaoComercial;
GO

/*
    Teste integrado:

    1. Cria usuário temporário
    2. Registra entrada de mercadorias
    3. Confirma a entrada
    4. Registra uma venda
    5. Confirma a venda
    6. Verifica a baixa do estoque
    7. Desfaz todos os dados com ROLLBACK
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @PerfilID INT;
DECLARE @UsuarioID INT;
DECLARE @FornecedorID INT;
DECLARE @FormaPagamentoID INT;
DECLARE @ProdutoID1 INT;
DECLARE @ProdutoID2 INT;
DECLARE @EntradaID INT;
DECLARE @VendaID INT;

BEGIN TRY
    BEGIN TRANSACTION;

    /* Seleciona registros auxiliares */
    SELECT TOP (1)
        @PerfilID = PerfilID
    FROM dbo.Perfis
    WHERE Ativo = 1
    ORDER BY PerfilID;

    SELECT TOP (1)
        @FornecedorID = FornecedorID
    FROM dbo.Fornecedores
    WHERE Ativo = 1
    ORDER BY FornecedorID;

    SELECT TOP (1)
        @FormaPagamentoID = FormaPagamentoID
    FROM dbo.FormasPagamento
    WHERE Ativo = 1
    ORDER BY FormaPagamentoID;

    SELECT
        @ProdutoID1 = MIN(ProdutoID),
        @ProdutoID2 = MAX(ProdutoID)
    FROM
    (
        SELECT TOP (2)
            ProdutoID
        FROM dbo.Produtos
        WHERE Ativo = 1
        ORDER BY ProdutoID
    ) AS ProdutosSelecionados;

    IF @PerfilID IS NULL
        THROW 53001, 'Nenhum perfil ativo foi encontrado.', 1;

    IF @FornecedorID IS NULL
        THROW 53002, 'Nenhum fornecedor ativo foi encontrado.', 1;

    IF @FormaPagamentoID IS NULL
        THROW 53003, 'Nenhuma forma de pagamento ativa foi encontrada.', 1;

    IF @ProdutoID1 IS NULL
       OR @ProdutoID2 IS NULL
       OR @ProdutoID1 = @ProdutoID2
        THROW 53004, 'São necessários pelo menos dois produtos ativos.', 1;

    /* Cria usuário temporário */
    INSERT INTO dbo.Usuarios
    (
        PerfilID,
        Nome,
        Email,
        SenhaHash
    )
    VALUES
    (
        @PerfilID,
        N'Usuário de Teste',
        N'teste.venda@local.invalid',
        'HASH_NAO_UTILIZAVEL_APENAS_PARA_TESTE'
    );

    SET @UsuarioID = CONVERT(INT, SCOPE_IDENTITY());


    /* =====================================================
       ENTRADA DE MERCADORIAS
       ===================================================== */

    EXEC dbo.usp_CriarEntrada
        @FornecedorID = @FornecedorID,
        @UsuarioID = @UsuarioID,
        @EntradaID = @EntradaID OUTPUT;

    EXEC dbo.usp_AdicionarItemEntrada
        @EntradaID = @EntradaID,
        @ProdutoID = @ProdutoID1,
        @Quantidade = 10,
        @CustoUnitario = 20.00;

    EXEC dbo.usp_AdicionarItemEntrada
        @EntradaID = @EntradaID,
        @ProdutoID = @ProdutoID2,
        @Quantidade = 8,
        @CustoUnitario = 5.00;

    EXEC dbo.usp_ConfirmarEntrada
        @EntradaID = @EntradaID;

    /* Estoque após a entrada */
    SELECT
        e.ProdutoID,
        p.Nome AS Produto,
        e.QuantidadeAtual AS EstoqueAposEntrada
    FROM dbo.Estoques AS e
    INNER JOIN dbo.Produtos AS p
        ON p.ProdutoID = e.ProdutoID
    WHERE e.ProdutoID IN (@ProdutoID1, @ProdutoID2)
    ORDER BY e.ProdutoID;


    /* =====================================================
       VENDA
       ===================================================== */

    EXEC dbo.usp_CriarVenda
        @UsuarioID = @UsuarioID,
        @FormaPagamentoID = @FormaPagamentoID,
        @VendaID = @VendaID OUTPUT;

    EXEC dbo.usp_AdicionarItemVenda
        @VendaID = @VendaID,
        @ProdutoID = @ProdutoID1,
        @Quantidade = 3;

    EXEC dbo.usp_AdicionarItemVenda
        @VendaID = @VendaID,
        @ProdutoID = @ProdutoID2,
        @Quantidade = 2;

    EXEC dbo.usp_ConcluirVenda
        @VendaID = @VendaID;

    /* Resultado consolidado da venda */
    SELECT
        v.VendaID,
        v.Status,
        v.ValorTotal,
        fp.Nome AS FormaPagamento,
        u.Nome AS Usuario
    FROM dbo.Vendas AS v
    INNER JOIN dbo.FormasPagamento AS fp
        ON fp.FormaPagamentoID = v.FormaPagamentoID
    INNER JOIN dbo.Usuarios AS u
        ON u.UsuarioID = v.UsuarioID
    WHERE v.VendaID = @VendaID;

    /* Estoque após a venda */
    SELECT
        e.ProdutoID,
        p.Nome AS Produto,
        e.QuantidadeAtual AS EstoqueAposVenda
    FROM dbo.Estoques AS e
    INNER JOIN dbo.Produtos AS p
        ON p.ProdutoID = e.ProdutoID
    WHERE e.ProdutoID IN (@ProdutoID1, @ProdutoID2)
    ORDER BY e.ProdutoID;

    /* Movimentações da venda */
    SELECT
        m.MovimentacaoID,
        m.ProdutoID,
        p.Nome AS Produto,
        m.Tipo,
        m.Quantidade,
        m.OrigemTipo,
        m.OrigemID,
        m.Observacao
    FROM dbo.MovimentacoesEstoque AS m
    INNER JOIN dbo.Produtos AS p
        ON p.ProdutoID = m.ProdutoID
    WHERE m.OrigemTipo = 'VENDA'
      AND m.OrigemID = @VendaID
    ORDER BY m.MovimentacaoID;

    ROLLBACK TRANSACTION;

    SELECT
        'TESTE CONCLUÍDO - DADOS DESFEITOS COM ROLLBACK' AS Resultado;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO