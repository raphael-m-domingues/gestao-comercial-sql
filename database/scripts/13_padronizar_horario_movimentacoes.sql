USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Padronização dos horários das movimentações

    Objetivo:
    Utilizar o horário local do servidor SQL Server
    no registro das movimentações de estoque.
*/

SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS
    (
        SELECT 1
        FROM sys.default_constraints
        WHERE parent_object_id =
            OBJECT_ID(N'dbo.MovimentacoesEstoque')
          AND name =
            N'DF_MovimentacoesEstoque_Data'
    )
    BEGIN
        ALTER TABLE dbo.MovimentacoesEstoque
            DROP CONSTRAINT
                DF_MovimentacoesEstoque_Data;
    END;

    ALTER TABLE dbo.MovimentacoesEstoque
        ADD CONSTRAINT
            DF_MovimentacoesEstoque_Data
        DEFAULT SYSDATETIME()
        FOR DataMovimentacao;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

/*
    Validação da configuração.
*/

SELECT
    dc.name AS RestricaoPadrao,
    c.name AS Coluna,
    dc.definition AS ValorPadrao
FROM sys.default_constraints AS dc
INNER JOIN sys.columns AS c
    ON c.object_id = dc.parent_object_id
   AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id =
    OBJECT_ID(N'dbo.MovimentacoesEstoque')
  AND c.name = N'DataMovimentacao';
GO