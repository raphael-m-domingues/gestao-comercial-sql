USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Fluxo de venda e baixa de estoque

    Procedimentos:
    1. Criar uma venda
    2. Adicionar produtos à venda
    3. Concluir a venda e baixar o estoque
*/


/* =========================================================
   1. CRIAR VENDA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_CriarVenda
    @UsuarioID INT,
    @FormaPagamentoID INT,
    @VendaID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuarios
        WHERE UsuarioID = @UsuarioID
          AND Ativo = 1
    )
        THROW 52001, 'Usuário inexistente ou inativo.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.FormasPagamento
        WHERE FormaPagamentoID = @FormaPagamentoID
          AND Ativo = 1
    )
        THROW 52002, 'Forma de pagamento inexistente ou inativa.', 1;

    INSERT INTO dbo.Vendas
    (
        UsuarioID,
        FormaPagamentoID
    )
    VALUES
    (
        @UsuarioID,
        @FormaPagamentoID
    );

    SET @VendaID = CONVERT(INT, SCOPE_IDENTITY());

    SELECT
        VendaID,
        UsuarioID,
        FormaPagamentoID,
        DataVenda,
        Status,
        ValorTotal
    FROM dbo.Vendas
    WHERE VendaID = @VendaID;
END;
GO


/* =========================================================
   2. ADICIONAR ITEM À VENDA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_AdicionarItemVenda
    @VendaID INT,
    @ProdutoID INT,
    @Quantidade INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @PrecoUnitario DECIMAL(10,2);
    DECLARE @QuantidadeDisponivel INT;

    IF @Quantidade <= 0
        THROW 52003, 'A quantidade deve ser maior que zero.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Vendas WITH (UPDLOCK, HOLDLOCK)
            WHERE VendaID = @VendaID
              AND Status = 'ABERTA'
        )
            THROW 52004, 'Venda inexistente ou não está aberta.', 1;

        SELECT
            @PrecoUnitario = PrecoVenda
        FROM dbo.Produtos
        WHERE ProdutoID = @ProdutoID
          AND Ativo = 1;

        IF @PrecoUnitario IS NULL
            THROW 52005, 'Produto inexistente ou inativo.', 1;

        SELECT
            @QuantidadeDisponivel = QuantidadeAtual
        FROM dbo.Estoques
        WHERE ProdutoID = @ProdutoID;

        IF COALESCE(@QuantidadeDisponivel, 0) < @Quantidade
            THROW 52006, 'Estoque insuficiente para adicionar o produto.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.ItensVenda
            WHERE VendaID = @VendaID
              AND ProdutoID = @ProdutoID
        )
            THROW 52007, 'O produto já foi adicionado a esta venda.', 1;

        INSERT INTO dbo.ItensVenda
        (
            VendaID,
            ProdutoID,
            Quantidade,
            PrecoUnitario
        )
        VALUES
        (
            @VendaID,
            @ProdutoID,
            @Quantidade,
            @PrecoUnitario
        );

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
        WHERE VendaID = @VendaID
          AND ProdutoID = @ProdutoID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


/* =========================================================
   3. CONCLUIR VENDA E BAIXAR ESTOQUE
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ConcluirVenda
    @VendaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @UsuarioID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @UsuarioID = UsuarioID
        FROM dbo.Vendas WITH (UPDLOCK, HOLDLOCK)
        WHERE VendaID = @VendaID
          AND Status = 'ABERTA';

        IF @UsuarioID IS NULL
            THROW 52008, 'Venda inexistente ou não está aberta.', 1;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.ItensVenda
            WHERE VendaID = @VendaID
        )
            THROW 52009, 'Não é possível concluir uma venda sem itens.', 1;

        /*
            A verificação é repetida na conclusão porque o estoque
            pode ter mudado depois da inclusão dos itens.
        */
        IF EXISTS
        (
            SELECT 1
            FROM dbo.ItensVenda AS i
            LEFT JOIN dbo.Estoques AS e WITH (UPDLOCK, HOLDLOCK)
                ON e.ProdutoID = i.ProdutoID
            WHERE i.VendaID = @VendaID
              AND COALESCE(e.QuantidadeAtual, 0) < i.Quantidade
        )
            THROW 52010, 'Estoque insuficiente para concluir a venda.', 1;

        /* Realiza a baixa no estoque */
        UPDATE e
        SET
            e.QuantidadeAtual = e.QuantidadeAtual - i.Quantidade,
            e.AtualizadoEm = SYSDATETIME()
        FROM dbo.Estoques AS e
        INNER JOIN dbo.ItensVenda AS i
            ON i.ProdutoID = e.ProdutoID
        WHERE i.VendaID = @VendaID;

        /* Registra o histórico da saída */
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
            'SAIDA',
            i.Quantidade,
            'VENDA',
            @VendaID,
            N'Baixa de estoque referente à venda concluída'
        FROM dbo.ItensVenda AS i
        WHERE i.VendaID = @VendaID;

        UPDATE dbo.Vendas
        SET
            Status = 'CONCLUIDA',
            ValorTotal =
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
   VALIDAÇÃO DOS PROCEDIMENTOS CRIADOS
   ========================================================= */

SELECT
    SCHEMA_NAME(schema_id) AS Esquema,
    name AS Procedimento,
    create_date AS DataCriacao,
    modify_date AS DataModificacao
FROM sys.procedures
WHERE name IN
(
    'usp_CriarVenda',
    'usp_AdicionarItemVenda',
    'usp_ConcluirVenda'
)
ORDER BY name;
GO