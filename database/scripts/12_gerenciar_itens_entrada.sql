USE DB_GestaoComercial;
GO

/*
    Permite alterar ou remover itens somente enquanto
    a entrada estiver com o status ABERTA.
*/


/* =========================================================
   1. ATUALIZAR ITEM DA ENTRADA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_AtualizarItemEntrada
    @ItemEntradaID INT,
    @Quantidade INT,
    @CustoUnitario DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Quantidade <= 0
        THROW 50010, 'A quantidade deve ser maior que zero.', 1;

    IF @CustoUnitario < 0
        THROW 50011, 'O custo unitário não pode ser negativo.', 1;

    DECLARE @EntradaID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @EntradaID = ie.EntradaID
        FROM dbo.ItensEntrada AS ie
        INNER JOIN dbo.Entradas AS e WITH (UPDLOCK, HOLDLOCK)
            ON e.EntradaID = ie.EntradaID
        WHERE ie.ItemEntradaID = @ItemEntradaID
          AND e.Status = 'ABERTA';

        IF @EntradaID IS NULL
            THROW 50012, 'Item inexistente ou entrada não está aberta.', 1;

        UPDATE dbo.ItensEntrada
        SET
            Quantidade = @Quantidade,
            CustoUnitario = @CustoUnitario
        WHERE ItemEntradaID = @ItemEntradaID;

        UPDATE dbo.Entradas
        SET ValorTotal =
        (
            SELECT COALESCE(SUM(Subtotal), 0)
            FROM dbo.ItensEntrada
            WHERE EntradaID = @EntradaID
        )
        WHERE EntradaID = @EntradaID;

        COMMIT TRANSACTION;

        SELECT
            ItemEntradaID,
            EntradaID,
            ProdutoID,
            Quantidade,
            CustoUnitario,
            Subtotal
        FROM dbo.ItensEntrada
        WHERE ItemEntradaID = @ItemEntradaID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


/* =========================================================
   2. REMOVER ITEM DA ENTRADA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_RemoverItemEntrada
    @ItemEntradaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @EntradaID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @EntradaID = ie.EntradaID
        FROM dbo.ItensEntrada AS ie
        INNER JOIN dbo.Entradas AS e WITH (UPDLOCK, HOLDLOCK)
            ON e.EntradaID = ie.EntradaID
        WHERE ie.ItemEntradaID = @ItemEntradaID
          AND e.Status = 'ABERTA';

        IF @EntradaID IS NULL
            THROW 50013, 'Item inexistente ou entrada não está aberta.', 1;

        DELETE FROM dbo.ItensEntrada
        WHERE ItemEntradaID = @ItemEntradaID;

        UPDATE dbo.Entradas
        SET ValorTotal =
        (
            SELECT COALESCE(SUM(Subtotal), 0)
            FROM dbo.ItensEntrada
            WHERE EntradaID = @EntradaID
        )
        WHERE EntradaID = @EntradaID;

        COMMIT TRANSACTION;

        SELECT
            @EntradaID AS EntradaID,
            ValorTotal
        FROM dbo.Entradas
        WHERE EntradaID = @EntradaID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


/* =========================================================
   VALIDAÇÃO
   ========================================================= */

SELECT
    SCHEMA_NAME(schema_id) AS Esquema,
    name AS Procedimento,
    create_date AS DataCriacao,
    modify_date AS DataModificacao
FROM sys.procedures
WHERE name IN
(
    'usp_AtualizarItemEntrada',
    'usp_RemoverItemEntrada'
)
ORDER BY name;
GO