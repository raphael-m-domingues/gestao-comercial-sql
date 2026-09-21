USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Consultas gerenciais

    Procedimentos:
    1. Resumo geral do estoque
    2. Vendas por período
    3. Produtos mais vendidos
*/


/* =========================================================
   1. RESUMO GERAL DO ESTOQUE
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ResumoEstoque
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*) AS TotalProdutosAtivos,

        COALESCE
        (
            SUM(COALESCE(e.QuantidadeAtual, 0)),
            0
        ) AS TotalUnidadesEstoque,

        COALESCE
        (
            SUM
            (
                CONVERT(DECIMAL(18,2), p.PrecoCusto)
                * COALESCE(e.QuantidadeAtual, 0)
            ),
            0
        ) AS ValorCustoEstoque,

        COALESCE
        (
            SUM
            (
                CONVERT(DECIMAL(18,2), p.PrecoVenda)
                * COALESCE(e.QuantidadeAtual, 0)
            ),
            0
        ) AS ValorVendaEstoque,

        SUM
        (
            CASE
                WHEN COALESCE(e.QuantidadeAtual, 0) = 0
                    THEN 1
                ELSE 0
            END
        ) AS ProdutosSemEstoque,

        SUM
        (
            CASE
                WHEN COALESCE(e.QuantidadeAtual, 0)
                     <= p.EstoqueMinimo
                    THEN 1
                ELSE 0
            END
        ) AS ProdutosEstoqueBaixo

    FROM dbo.Produtos AS p
    LEFT JOIN dbo.Estoques AS e
        ON e.ProdutoID = p.ProdutoID
    WHERE p.Ativo = 1;
END;
GO


/* =========================================================
   2. VENDAS POR PERÍODO
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_VendasPorPeriodo
    @DataInicial DATE,
    @DataFinal DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF @DataInicial IS NULL OR @DataFinal IS NULL
        THROW 54001, 'As datas inicial e final são obrigatórias.', 1;

    IF @DataInicial > @DataFinal
        THROW 54002, 'A data inicial não pode ser posterior à data final.', 1;

    SELECT
        CAST(v.DataVenda AS DATE) AS Data,
        COUNT(*) AS QuantidadeVendas,
        SUM(v.ValorTotal) AS TotalVendido,
        AVG(v.ValorTotal) AS TicketMedio
    FROM dbo.Vendas AS v
    WHERE v.Status = 'CONCLUIDA'
      AND v.DataVenda >= @DataInicial
      AND v.DataVenda < DATEADD(DAY, 1, @DataFinal)
    GROUP BY
        CAST(v.DataVenda AS DATE)
    ORDER BY
        Data;
END;
GO


/* =========================================================
   3. PRODUTOS MAIS VENDIDOS
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ProdutosMaisVendidos
    @DataInicial DATE,
    @DataFinal DATE,
    @Limite INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @DataInicial IS NULL OR @DataFinal IS NULL
        THROW 54003, 'As datas inicial e final são obrigatórias.', 1;

    IF @DataInicial > @DataFinal
        THROW 54004, 'A data inicial não pode ser posterior à data final.', 1;

    IF @Limite <= 0 OR @Limite > 100
        THROW 54005, 'O limite deve estar entre 1 e 100.', 1;

    SELECT TOP (@Limite)
        p.ProdutoID,
        p.Nome AS Produto,
        c.Nome AS Categoria,
        SUM(iv.Quantidade) AS QuantidadeVendida,
        SUM(iv.Subtotal) AS TotalVendido
    FROM dbo.ItensVenda AS iv
    INNER JOIN dbo.Vendas AS v
        ON v.VendaID = iv.VendaID
    INNER JOIN dbo.Produtos AS p
        ON p.ProdutoID = iv.ProdutoID
    INNER JOIN dbo.Categorias AS c
        ON c.CategoriaID = p.CategoriaID
    WHERE v.Status = 'CONCLUIDA'
      AND v.DataVenda >= @DataInicial
      AND v.DataVenda < DATEADD(DAY, 1, @DataFinal)
    GROUP BY
        p.ProdutoID,
        p.Nome,
        c.Nome
    ORDER BY
        QuantidadeVendida DESC,
        TotalVendido DESC,
        Produto;
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
    'usp_ResumoEstoque',
    'usp_VendasPorPeriodo',
    'usp_ProdutosMaisVendidos'
)
ORDER BY name;
GO

EXEC dbo.usp_ResumoEstoque;
GO