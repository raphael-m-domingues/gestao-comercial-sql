USE DB_GestaoComercial;
GO

/*
    Teste integrado do fluxo de entrada.

    Todos os dados criados por este teste serão desfeitos
    pelo ROLLBACK ao final.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @PerfilID INT;
DECLARE @UsuarioID INT;
DECLARE @FornecedorID INT;
DECLARE @ProdutoID1 INT;
DECLARE @ProdutoID2 INT;
DECLARE @EntradaID INT;

BEGIN TRY
    BEGIN TRANSACTION;

    /* Seleciona um perfil para o usuário temporário */
    SELECT TOP (1)
        @PerfilID = PerfilID
    FROM dbo.Perfis
    WHERE Ativo = 1
    ORDER BY PerfilID;

    IF @PerfilID IS NULL
        THROW 51001, 'Nenhum perfil ativo foi encontrado.', 1;

    /* Cria um usuário somente para este teste */
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
        N'teste.fluxo@local.invalid',
        'HASH_NAO_UTILIZAVEL_APENAS_PARA_TESTE'
    );

    SET @UsuarioID = CONVERT(INT, SCOPE_IDENTITY());

    /* Seleciona um fornecedor ativo */
    SELECT TOP (1)
        @FornecedorID = FornecedorID
    FROM dbo.Fornecedores
    WHERE Ativo = 1
    ORDER BY FornecedorID;

    IF @FornecedorID IS NULL
        THROW 51002, 'Nenhum fornecedor ativo foi encontrado.', 1;

    /* Seleciona dois produtos ativos */
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

    IF @ProdutoID1 IS NULL
       OR @ProdutoID2 IS NULL
       OR @ProdutoID1 = @ProdutoID2
        THROW 51003, 'São necessários pelo menos dois produtos ativos.', 1;

    /* Estoque antes da entrada */
    SELECT
        e.ProdutoID,
        p.Nome AS Produto,
        e.QuantidadeAtual AS EstoqueAntes
    FROM dbo.Estoques AS e
    INNER JOIN dbo.Produtos AS p
        ON p.ProdutoID = e.ProdutoID
    WHERE e.ProdutoID IN (@ProdutoID1, @ProdutoID2)
    ORDER BY e.ProdutoID;

    /* Cria a entrada */
    EXEC dbo.usp_CriarEntrada
        @FornecedorID = @FornecedorID,
        @UsuarioID = @UsuarioID,
        @EntradaID = @EntradaID OUTPUT;

    /* Adiciona o primeiro produto */
    EXEC dbo.usp_AdicionarItemEntrada
        @EntradaID = @EntradaID,
        @ProdutoID = @ProdutoID1,
        @Quantidade = 10,
        @CustoUnitario = 20.00;

    /* Adiciona o segundo produto */
    EXEC dbo.usp_AdicionarItemEntrada
        @EntradaID = @EntradaID,
        @ProdutoID = @ProdutoID2,
        @Quantidade = 5,
        @CustoUnitario = 3.00;

    /* Confirma a entrada e atualiza o estoque */
    EXEC dbo.usp_ConfirmarEntrada
        @EntradaID = @EntradaID;

    /* Resultado consolidado da entrada */
    SELECT
        en.EntradaID,
        en.Status,
        en.ValorTotal,
        f.NomeFantasia AS Fornecedor,
        u.Nome AS Usuario
    FROM dbo.Entradas AS en
    INNER JOIN dbo.Fornecedores AS f
        ON f.FornecedorID = en.FornecedorID
    INNER JOIN dbo.Usuarios AS u
        ON u.UsuarioID = en.UsuarioID
    WHERE en.EntradaID = @EntradaID;

    /* Estoque depois da confirmação */
    SELECT
        e.ProdutoID,
        p.Nome AS Produto,
        e.QuantidadeAtual AS EstoqueDepois
    FROM dbo.Estoques AS e
    INNER JOIN dbo.Produtos AS p
        ON p.ProdutoID = e.ProdutoID
    WHERE e.ProdutoID IN (@ProdutoID1, @ProdutoID2)
    ORDER BY e.ProdutoID;

    /* Histórico das movimentações */
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
    WHERE m.OrigemTipo = 'ENTRADA'
      AND m.OrigemID = @EntradaID
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