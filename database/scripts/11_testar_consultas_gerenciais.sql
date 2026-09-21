USE DB_GestaoComercial;
GO

/*
    Teste das consultas gerenciais:

    - Entrada de dois produtos
    - Realização de duas vendas
    - Resumo do estoque
    - Vendas por período
    - Produtos mais vendidos
    - ROLLBACK ao final
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
DECLARE @VendaID1 INT;
DECLARE @VendaID2 INT;
DECLARE @DataTeste DATE = CAST(SYSUTCDATETIME() AS DATE);

BEGIN TRY
    BEGIN TRANSACTION;

    /* Registros auxiliares */
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
        THROW 55001, 'Nenhum perfil ativo foi encontrado.', 1;

    IF @FornecedorID IS NULL
        THROW 55002, 'Nenhum fornecedor ativo foi encontrado.', 1;

    IF @FormaPagamentoID IS NULL
        THROW 55003, 'Nenhuma forma de pagamento ativa foi encontrada.', 1;

    IF @ProdutoID1 IS NULL
       OR @ProdutoID2 IS NULL
       OR @ProdutoID1 = @ProdutoID2
        THROW 55004, 'São necessários pelo menos dois produtos ativos.', 1;

    /* Usuário temporário */
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
        N'Usuário Gerencial de Teste',
        N'teste.gerencial@local.invalid',
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
        @Quantidade = 20,
        @CustoUnitario = 20.00;

    EXEC dbo.usp_AdicionarItemEntrada
        @EntradaID = @EntradaID,
        @ProdutoID = @ProdutoID2,
        @Quantidade = 15,
        @CustoUnitario = 5.00;

    EXEC dbo.usp_ConfirmarEntrada
        @EntradaID = @EntradaID;


    /* =====================================================
       PRIMEIRA VENDA
       ===================================================== */

    EXEC dbo.usp_CriarVenda
        @UsuarioID = @UsuarioID,
        @FormaPagamentoID = @FormaPagamentoID,
        @VendaID = @VendaID1 OUTPUT;

    EXEC dbo.usp_AdicionarItemVenda
        @VendaID = @VendaID1,
        @ProdutoID = @ProdutoID1,
        @Quantidade = 3;

    EXEC dbo.usp_AdicionarItemVenda
        @VendaID = @VendaID1,
        @ProdutoID = @ProdutoID2,
        @Quantidade = 2;

    EXEC dbo.usp_ConcluirVenda
        @VendaID = @VendaID1;


    /* =====================================================
       SEGUNDA VENDA
       ===================================================== */

    EXEC dbo.usp_CriarVenda
        @UsuarioID = @UsuarioID,
        @FormaPagamentoID = @FormaPagamentoID,
        @VendaID = @VendaID2 OUTPUT;

    EXEC dbo.usp_AdicionarItemVenda
        @VendaID = @VendaID2,
        @ProdutoID = @ProdutoID1,
        @Quantidade = 1;

    EXEC dbo.usp_ConcluirVenda
        @VendaID = @VendaID2;


    /* =====================================================
       RELATÓRIOS GERENCIAIS
       ===================================================== */

    SELECT 'RESUMO DO ESTOQUE' AS Relatorio;

    EXEC dbo.usp_ResumoEstoque;


    SELECT 'VENDAS POR PERÍODO' AS Relatorio;

    EXEC dbo.usp_VendasPorPeriodo
        @DataInicial = @DataTeste,
        @DataFinal = @DataTeste;


    SELECT 'PRODUTOS MAIS VENDIDOS' AS Relatorio;

    EXEC dbo.usp_ProdutosMaisVendidos
        @DataInicial = @DataTeste,
        @DataFinal = @DataTeste,
        @Limite = 10;


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