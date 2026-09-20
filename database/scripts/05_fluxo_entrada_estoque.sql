USE DB_GestaoComercial;
GO

/*
    Projeto: Gestão Comercial SQL
    Etapa: Fluxo de entrada de mercadorias

    Procedimentos:
    1. Criar uma entrada
    2. Adicionar produtos à entrada
    3. Confirmar a entrada e atualizar o estoque
*/


/* =========================================================
   1. CRIAR ENTRADA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_CriarEntrada
    @FornecedorID INT,
    @UsuarioID INT,
    @EntradaID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Fornecedores
        WHERE FornecedorID = @FornecedorID
          AND Ativo = 1
    )
        THROW 50001, 'Fornecedor inexistente ou inativo.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuarios
        WHERE UsuarioID = @UsuarioID
          AND Ativo = 1
    )
        THROW 50002, 'Usuário inexistente ou inativo.', 1;

    INSERT INTO dbo.Entradas
    (
        FornecedorID,
        UsuarioID
    )
    VALUES
    (
        @FornecedorID,
        @UsuarioID
    );

    SET @EntradaID = CONVERT(INT, SCOPE_IDENTITY());

    SELECT
        EntradaID,
        FornecedorID,
        UsuarioID,
        DataEntrada,
        Status,
        ValorTotal
    FROM dbo.Entradas
    WHERE EntradaID = @EntradaID;
END;
GO


/* =========================================================
   2. ADICIONAR ITEM À ENTRADA
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_AdicionarItemEntrada
    @EntradaID INT,
    @ProdutoID INT,
    @Quantidade INT,
    @CustoUnitario DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Quantidade <= 0
        THROW 50003, 'A quantidade deve ser maior que zero.', 1;

    IF @CustoUnitario < 0
        THROW 50004, 'O custo unitário não pode ser negativo.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Entradas WITH (UPDLOCK, HOLDLOCK)
            WHERE EntradaID = @EntradaID
              AND Status = 'ABERTA'
        )
            THROW 50005, 'Entrada inexistente ou não está aberta.', 1;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Produtos
            WHERE ProdutoID = @ProdutoID
              AND Ativo = 1
        )
            THROW 50006, 'Produto inexistente ou inativo.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.ItensEntrada
            WHERE EntradaID = @EntradaID
              AND ProdutoID = @ProdutoID
        )
            THROW 50007, 'O produto já foi adicionado a esta entrada.', 1;

        INSERT INTO dbo.ItensEntrada
        (
            EntradaID,
            ProdutoID,
            Quantidade,
            CustoUnitario
        )
        VALUES
        (
            @EntradaID,
            @ProdutoID,
            @Quantidade,
            @CustoUnitario
        );

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
        WHERE EntradaID = @EntradaID
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
   3. CONFIRMAR ENTRADA E ATUALIZAR ESTOQUE
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ConfirmarEntrada
    @EntradaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @UsuarioID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @UsuarioID = UsuarioID
        FROM dbo.Entradas WITH (UPDLOCK, HOLDLOCK)
        WHERE EntradaID = @EntradaID
          AND Status = 'ABERTA';

        IF @UsuarioID IS NULL
            THROW 50008, 'Entrada inexistente ou não está aberta.', 1;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.ItensEntrada
            WHERE EntradaID = @EntradaID
        )
            THROW 50009, 'Não é possível confirmar uma entrada sem itens.', 1;

        /* Atualiza estoques que já existem */
        UPDATE e
        SET
            e.QuantidadeAtual = e.QuantidadeAtual + i.Quantidade,
            e.AtualizadoEm = SYSDATETIME()
        FROM dbo.Estoques AS e
        INNER JOIN dbo.ItensEntrada AS i
            ON i.ProdutoID = e.ProdutoID
        WHERE i.EntradaID = @EntradaID;

        /* Cria estoque caso algum produto ainda não possua registro */
        INSERT INTO dbo.Estoques
        (
            ProdutoID,
            QuantidadeAtual
        )
        SELECT
            i.ProdutoID,
            i.Quantidade
        FROM dbo.ItensEntrada AS i
        WHERE i.EntradaID = @EntradaID
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.Estoques AS e
              WHERE e.ProdutoID = i.ProdutoID
          );

        /* Registra o histórico de movimentações */
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
            'ENTRADA',
            i.Quantidade,
            'ENTRADA',
            @EntradaID,
            N'Entrada de mercadorias confirmada'
        FROM dbo.ItensEntrada AS i
        WHERE i.EntradaID = @EntradaID;

        UPDATE dbo.Entradas
        SET
            Status = 'CONFIRMADA',
            ValorTotal =
            (
                SELECT COALESCE(SUM(Subtotal), 0)
                FROM dbo.ItensEntrada
                WHERE EntradaID = @EntradaID
            )
        WHERE EntradaID = @EntradaID;

        COMMIT TRANSACTION;

        SELECT
            EntradaID,
            FornecedorID,
            UsuarioID,
            DataEntrada,
            Status,
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
    'usp_CriarEntrada',
    'usp_AdicionarItemEntrada',
    'usp_ConfirmarEntrada'
)
ORDER BY name;
GO