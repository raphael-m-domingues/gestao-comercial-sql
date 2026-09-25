USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Cancelamento de venda

    A procedure:
    1. Valida se a venda está concluída.
    2. Devolve os produtos ao estoque.
    3. Registra as movimentações de devolução.
    4. Altera o status da venda para CANCELADA.
*/


CREATE OR ALTER PROCEDURE dbo.usp_CancelarVenda
    @VendaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @UsuarioID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        /*
            Bloqueia a venda durante o cancelamento e permite
            cancelar somente vendas concluídas.
        */
        SELECT
            @UsuarioID = UsuarioID
        FROM dbo.Vendas WITH (UPDLOCK, HOLDLOCK)
        WHERE VendaID = @VendaID
          AND Status = 'CONCLUIDA';

        IF @UsuarioID IS NULL
            THROW 52015,
                'Venda inexistente ou não está concluída.',
                1;

        /*
            Confirma que todos os produtos possuem
            um registro correspondente no estoque.
        */
        IF EXISTS
        (
            SELECT 1
            FROM dbo.ItensVenda AS i
            LEFT JOIN dbo.Estoques AS e
                ON e.ProdutoID = i.ProdutoID
            WHERE i.VendaID = @VendaID
              AND e.ProdutoID IS NULL
        )
            THROW 52016,
                'Um ou mais produtos não possuem registro de estoque.',
                1;

        /*
            Bloqueia os registros de estoque que serão atualizados.
        */
        SELECT
            e.ProdutoID
        FROM dbo.Estoques AS e WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN dbo.ItensVenda AS i
            ON i.ProdutoID = e.ProdutoID
        WHERE i.VendaID = @VendaID;

        /*
            Devolve ao estoque as quantidades que haviam
            sido retiradas na conclusão da venda.
        */
        UPDATE e
        SET
            e.QuantidadeAtual =
                e.QuantidadeAtual + i.Quantidade,

            e.AtualizadoEm =
                SYSDATETIME()
        FROM dbo.Estoques AS e
        INNER JOIN dbo.ItensVenda AS i
            ON i.ProdutoID = e.ProdutoID
        WHERE i.VendaID = @VendaID;

        /*
            Registra o histórico da devolução ao estoque.
        */
        INSERT INTO dbo.MovimentacoesEstoque
        (
            ProdutoID,
            UsuarioID,
            Tipo,
            Quantidade,
            OrigemTipo,
            OrigemID,
            Observacao
        )
        SELECT
            i.ProdutoID,
            @UsuarioID,
            'DEVOLUCAO',
            i.Quantidade,
            'VENDA',
            @VendaID,
            N'Estorno de estoque referente ao cancelamento da venda'
        FROM dbo.ItensVenda AS i
        WHERE i.VendaID = @VendaID;

        /*
            Mantém o valor total para preservar o histórico,
            alterando somente o status da venda.
        */
        UPDATE dbo.Vendas
        SET Status = 'CANCELADA'
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
   Validação da procedure
   ========================================================= */

SELECT
    SCHEMA_NAME(schema_id) AS Esquema,
    name AS Procedimento,
    create_date AS DataCriacao,
    modify_date AS DataModificacao
FROM sys.procedures
WHERE name = 'usp_CancelarVenda';
GO