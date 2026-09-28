USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Consulta do histórico de movimentações de estoque

    Procedimento:
    1. Listar movimentações com filtros opcionais
*/


/* =========================================================
   CONSULTAR MOVIMENTAÇÕES DE ESTOQUE
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ListarMovimentacoesEstoque
    @DataInicial DATE = NULL,
    @DataFinal DATE = NULL,
    @ProdutoID INT = NULL,
    @UsuarioID INT = NULL,
    @Tipo VARCHAR(20) = NULL,
    @Limite INT = 200
AS
BEGIN
    SET NOCOUNT ON;

    IF @DataInicial IS NOT NULL
       AND @DataFinal IS NOT NULL
       AND @DataInicial > @DataFinal
    BEGIN
        THROW 56001,
            'A data inicial não pode ser posterior à data final.',
            1;
    END;

    IF @Tipo IS NOT NULL
       AND @Tipo NOT IN
       (
           'ENTRADA',
           'SAIDA',
           'DEVOLUCAO',
           'AJUSTE'
       )
    BEGIN
        THROW 56002,
            'O tipo de movimentação informado é inválido.',
            1;
    END;

    IF @Limite < 1 OR @Limite > 1000
    BEGIN
        THROW 56003,
            'O limite deve estar entre 1 e 1000.',
            1;
    END;

    SELECT TOP (@Limite)
        m.MovimentacaoID,
        m.DataMovimentacao,
        m.ProdutoID,
        p.Nome AS Produto,
        m.UsuarioID,
        u.Nome AS Usuario,
        m.Tipo,
        m.Quantidade,
        m.OrigemTipo,
        m.OrigemID,
        m.Observacao
    FROM dbo.MovimentacoesEstoque AS m
    INNER JOIN dbo.Produtos AS p
        ON p.ProdutoID = m.ProdutoID
    INNER JOIN dbo.Usuarios AS u
        ON u.UsuarioID = m.UsuarioID
    WHERE
        (
            @DataInicial IS NULL
            OR m.DataMovimentacao >= @DataInicial
        )
        AND
        (
            @DataFinal IS NULL
            OR m.DataMovimentacao
                < DATEADD(DAY, 1, @DataFinal)
        )
        AND
        (
            @ProdutoID IS NULL
            OR m.ProdutoID = @ProdutoID
        )
        AND
        (
            @UsuarioID IS NULL
            OR m.UsuarioID = @UsuarioID
        )
        AND
        (
            @Tipo IS NULL
            OR m.Tipo = @Tipo
        )
    ORDER BY
        m.DataMovimentacao DESC,
        m.MovimentacaoID DESC;
END;
GO


/* =========================================================
   VALIDAÇÃO DO PROCEDIMENTO
   ========================================================= */

SELECT
    SCHEMA_NAME(schema_id) AS Esquema,
    name AS Procedimento,
    create_date AS DataCriacao,
    modify_date AS DataModificacao
FROM sys.procedures
WHERE name = N'usp_ListarMovimentacoesEstoque';
GO