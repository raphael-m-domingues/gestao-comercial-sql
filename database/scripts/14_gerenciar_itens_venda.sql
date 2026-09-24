USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Gerenciamento dos itens da venda

    Procedimentos:
    1. Atualizar a quantidade de um item
    2. Remover um item
*/


/* =========================================================
   1. ATUALIZAR ITEM DA VENDA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_AtualizarItemVenda
    @ItemVendaID INT,
    @Quantidade INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @VendaID INT;
    DECLARE @ProdutoID INT;
    DECLARE @QuantidadeDisponivel INT;

    IF @Quantidade <= 0
        THROW 52011, 'A quantidade deve ser maior que zero.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @VendaID = i.VendaID,
            @ProdutoID = i.ProdutoID
        FROM dbo.ItensVenda AS i
        INNER JOIN dbo.Vendas AS v WITH (UPDLOCK, HOLDLOCK)
            ON v.VendaID = i.VendaID
        WHERE i.ItemVendaID = @ItemVendaID
          AND v.Status = 'ABERTA';

        IF @VendaID IS NULL
            THROW 52012, 'Item inexistente ou venda não está aberta.', 1;

        SELECT
            @QuantidadeDisponivel = e.QuantidadeAtual
        FROM dbo.Estoques AS e WITH (UPDLOCK, HOLDLOCK)
        WHERE e.ProdutoID = @ProdutoID;

        IF COALESCE(@QuantidadeDisponivel, 0) < @Quantidade
            THROW 52013, 'Estoque insuficiente para atualizar o item.', 1;

        UPDATE dbo.ItensVenda
        SET Quantidade = @Quantidade
        WHERE ItemVendaID = @ItemVendaID;

        UPDATE dbo.Vendas
        SET ValorTotal =
        (
            SELECT COALESCE(SUM(Subtotal), 0)
            FROM dbo.ItensVenda
            WHERE VendaID = @VendaID
        )
        WHERE VendaID = @VendaID;

        COMMIT TRANSACTION;

        SELECT
            ItemVendaID,
            VendaID,
            ProdutoID,
            Quantidade,
            PrecoUnitario,
            Subtotal
        FROM dbo.ItensVenda
        WHERE ItemVendaID = @ItemVendaID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


/* =========================================================
   2. REMOVER ITEM DA VENDA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_RemoverItemVenda
    @ItemVendaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @VendaID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @VendaID = i.VendaID
        FROM dbo.ItensVenda AS i
        INNER JOIN dbo.Vendas AS v WITH (UPDLOCK, HOLDLOCK)
            ON v.VendaID = i.VendaID
        WHERE i.ItemVendaID = @ItemVendaID
          AND v.Status = 'ABERTA';

        IF @VendaID IS NULL
            THROW 52014, 'Item inexistente ou venda não está aberta.', 1;

        DELETE FROM dbo.ItensVenda
        WHERE ItemVendaID = @ItemVendaID;

        UPDATE dbo.Vendas
        SET ValorTotal =
        (
            SELECT COALESCE(SUM(Subtotal), 0)
            FROM dbo.ItensVenda
            WHERE VendaID = @VendaID
        )
        WHERE VendaID = @VendaID;

        COMMIT TRANSACTION;

        SELECT
            VendaID,
            UsuarioID,
            FormaPagamentoID,
            DataVenda,
            Status,
            ValorTotal
        FROM dbo.Vendas
        WHERE VendaID = @VendaID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


/* =========================================================
   VALIDAÇÃO DOS PROCEDIMENTOS
   ========================================================= */

SELECT
    SCHEMA_NAME(schema_id) AS Esquema,
    name AS Procedimento,
    create_date AS DataCriacao,
    modify_date AS DataModificacao
FROM sys.procedures
WHERE name IN
(
    'usp_AtualizarItemVenda',
    'usp_RemoverItemVenda'
)
ORDER BY name;
GO