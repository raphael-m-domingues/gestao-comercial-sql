USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Consulta e alerta de estoque baixo

    A view retorna produtos cuja quantidade atual
    está igual ou abaixo do estoque mínimo definido.
*/

CREATE OR ALTER VIEW dbo.vw_ProdutosEstoqueBaixo
AS
    SELECT
        p.ProdutoID,
        p.Nome AS Produto,
        c.Nome AS Categoria,
        COALESCE(e.QuantidadeAtual, 0) AS QuantidadeAtual,
        p.EstoqueMinimo,

        CASE
            WHEN COALESCE(e.QuantidadeAtual, 0) = 0
                THEN 'SEM ESTOQUE'
            ELSE 'ESTOQUE BAIXO'
        END AS SituacaoEstoque,

        CASE
            WHEN p.EstoqueMinimo > COALESCE(e.QuantidadeAtual, 0)
                THEN p.EstoqueMinimo - COALESCE(e.QuantidadeAtual, 0)
            ELSE 0
        END AS QuantidadeParaReposicao,

        e.AtualizadoEm
    FROM dbo.Produtos AS p
    INNER JOIN dbo.Categorias AS c
        ON c.CategoriaID = p.CategoriaID
    LEFT JOIN dbo.Estoques AS e
        ON e.ProdutoID = p.ProdutoID
    WHERE p.Ativo = 1
      AND COALESCE(e.QuantidadeAtual, 0) <= p.EstoqueMinimo;
GO


/* =========================================================
   PROCEDIMENTO PARA CONSULTAR OS ALERTAS
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ListarProdutosEstoqueBaixo
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ProdutoID,
        Produto,
        Categoria,
        QuantidadeAtual,
        EstoqueMinimo,
        SituacaoEstoque,
        QuantidadeParaReposicao,
        AtualizadoEm
    FROM dbo.vw_ProdutosEstoqueBaixo
    ORDER BY
        QuantidadeAtual,
        Produto;
END;
GO


/* =========================================================
   VALIDAÇÃO
   ========================================================= */

SELECT
    SCHEMA_NAME(schema_id) AS Esquema,
    name AS Objeto,
    type_desc AS Tipo
FROM sys.objects
WHERE name IN
(
    'vw_ProdutosEstoqueBaixo',
    'usp_ListarProdutosEstoqueBaixo'
)
ORDER BY name;
GO

EXEC dbo.usp_ListarProdutosEstoqueBaixo;
GO