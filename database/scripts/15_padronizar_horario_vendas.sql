USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Padronização do horário das vendas

    Objetivos:
    1. Alterar o horário padrão de UTC para o horário local.
    2. Corrigir as vendas registradas com o padrão anterior.
    3. Permitir nova execução sem corrigir os horários novamente.
*/

SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS
    (
        SELECT 1
        FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.object_id = dc.parent_object_id
           AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id =
            OBJECT_ID(N'dbo.Vendas')
          AND c.name = N'DataVenda'
          AND dc.definition LIKE
              '%sysutcdatetime%'
    )
    BEGIN
        /*
            Corrige as vendas criadas usando o horário UTC.
            O horário local está três horas atrás do UTC.
        */
        UPDATE dbo.Vendas
        SET DataVenda =
            DATEADD(HOUR, -3, DataVenda);

        /*
            Remove o valor padrão anterior.
        */
        ALTER TABLE dbo.Vendas
            DROP CONSTRAINT DF_Vendas_DataVenda;

        /*
            Define o horário local do servidor como padrão.
        */
        ALTER TABLE dbo.Vendas
            ADD CONSTRAINT DF_Vendas_DataVenda
            DEFAULT SYSDATETIME()
            FOR DataVenda;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

/* =========================================================
   Validação da configuração
   ========================================================= */

SELECT
    dc.name AS RestricaoPadrao,
    c.name AS Coluna,
    dc.definition AS ValorPadrao
FROM sys.default_constraints AS dc
INNER JOIN sys.columns AS c
    ON c.object_id = dc.parent_object_id
   AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id =
    OBJECT_ID(N'dbo.Vendas')
  AND c.name = N'DataVenda';

/* =========================================================
   Validação da venda utilizada no teste
   ========================================================= */

SELECT
    VendaID,
    DataVenda,
    Status,
    ValorTotal
FROM dbo.Vendas
WHERE VendaID = 1002;
GO