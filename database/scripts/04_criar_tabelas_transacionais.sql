USE DB_GestaoComercial;
GO

/* =========================================================
   Tabela: Entradas
   ========================================================= */

IF OBJECT_ID(N'dbo.Entradas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Entradas
    (
        EntradaID INT IDENTITY(1,1) NOT NULL,
        FornecedorID INT NOT NULL,
        UsuarioID INT NOT NULL,
        DataEntrada DATETIME2(0) NOT NULL
            CONSTRAINT DF_Entradas_DataEntrada
            DEFAULT (SYSUTCDATETIME()),
        Status VARCHAR(20) NOT NULL
            CONSTRAINT DF_Entradas_Status DEFAULT ('ABERTA'),
        ValorTotal DECIMAL(12,2) NOT NULL
            CONSTRAINT DF_Entradas_ValorTotal DEFAULT (0),

        CONSTRAINT PK_Entradas
            PRIMARY KEY (EntradaID),

        CONSTRAINT FK_Entradas_Fornecedores
            FOREIGN KEY (FornecedorID)
            REFERENCES dbo.Fornecedores (FornecedorID),

        CONSTRAINT FK_Entradas_Usuarios
            FOREIGN KEY (UsuarioID)
            REFERENCES dbo.Usuarios (UsuarioID),

        CONSTRAINT CK_Entradas_Status
            CHECK (Status IN ('ABERTA', 'CONFIRMADA', 'CANCELADA')),

        CONSTRAINT CK_Entradas_ValorTotal
            CHECK (ValorTotal >= 0)
    );

    CREATE INDEX IX_Entradas_FornecedorID
        ON dbo.Entradas (FornecedorID);

    CREATE INDEX IX_Entradas_UsuarioID
        ON dbo.Entradas (UsuarioID);

    PRINT 'Tabela Entradas criada.';
END;
GO

/* =========================================================
   Tabela: ItensEntrada
   ========================================================= */

IF OBJECT_ID(N'dbo.ItensEntrada', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItensEntrada
    (
        ItemEntradaID INT IDENTITY(1,1) NOT NULL,
        EntradaID INT NOT NULL,
        ProdutoID INT NOT NULL,
        Quantidade INT NOT NULL,
        CustoUnitario DECIMAL(10,2) NOT NULL,

        Subtotal AS
        (
            CONVERT
            (
                DECIMAL(12,2),
                Quantidade * CustoUnitario
            )
        ) PERSISTED,

        CONSTRAINT PK_ItensEntrada
            PRIMARY KEY (ItemEntradaID),

        CONSTRAINT FK_ItensEntrada_Entradas
            FOREIGN KEY (EntradaID)
            REFERENCES dbo.Entradas (EntradaID),

        CONSTRAINT FK_ItensEntrada_Produtos
            FOREIGN KEY (ProdutoID)
            REFERENCES dbo.Produtos (ProdutoID),

        CONSTRAINT UQ_ItensEntrada_Entrada_Produto
            UNIQUE (EntradaID, ProdutoID),

        CONSTRAINT CK_ItensEntrada_Quantidade
            CHECK (Quantidade > 0),

        CONSTRAINT CK_ItensEntrada_CustoUnitario
            CHECK (CustoUnitario >= 0)
    );

    CREATE INDEX IX_ItensEntrada_ProdutoID
        ON dbo.ItensEntrada (ProdutoID);

    PRINT 'Tabela ItensEntrada criada.';
END;
GO

/* =========================================================
   Tabela: Vendas
   ========================================================= */

IF OBJECT_ID(N'dbo.Vendas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Vendas
    (
        VendaID INT IDENTITY(1,1) NOT NULL,
        UsuarioID INT NOT NULL,
        FormaPagamentoID INT NULL,
        DataVenda DATETIME2(0) NOT NULL
            CONSTRAINT DF_Vendas_DataVenda
            DEFAULT (SYSUTCDATETIME()),
        Status VARCHAR(20) NOT NULL
            CONSTRAINT DF_Vendas_Status DEFAULT ('ABERTA'),
        ValorTotal DECIMAL(12,2) NOT NULL
            CONSTRAINT DF_Vendas_ValorTotal DEFAULT (0),

        CONSTRAINT PK_Vendas
            PRIMARY KEY (VendaID),

        CONSTRAINT FK_Vendas_Usuarios
            FOREIGN KEY (UsuarioID)
            REFERENCES dbo.Usuarios (UsuarioID),

        CONSTRAINT FK_Vendas_FormasPagamento
            FOREIGN KEY (FormaPagamentoID)
            REFERENCES dbo.FormasPagamento (FormaPagamentoID),

        CONSTRAINT CK_Vendas_Status
            CHECK (Status IN ('ABERTA', 'CONCLUIDA', 'CANCELADA')),

        CONSTRAINT CK_Vendas_ValorTotal
            CHECK (ValorTotal >= 0)
    );

    CREATE INDEX IX_Vendas_UsuarioID
        ON dbo.Vendas (UsuarioID);

    CREATE INDEX IX_Vendas_FormaPagamentoID
        ON dbo.Vendas (FormaPagamentoID);

    PRINT 'Tabela Vendas criada.';
END;
GO

/* =========================================================
   Tabela: ItensVenda
   ========================================================= */

IF OBJECT_ID(N'dbo.ItensVenda', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItensVenda
    (
        ItemVendaID INT IDENTITY(1,1) NOT NULL,
        VendaID INT NOT NULL,
        ProdutoID INT NOT NULL,
        Quantidade INT NOT NULL,
        PrecoUnitario DECIMAL(10,2) NOT NULL,

        Subtotal AS
        (
            CONVERT
            (
                DECIMAL(12,2),
                Quantidade * PrecoUnitario
            )
        ) PERSISTED,

        CONSTRAINT PK_ItensVenda
            PRIMARY KEY (ItemVendaID),

        CONSTRAINT FK_ItensVenda_Vendas
            FOREIGN KEY (VendaID)
            REFERENCES dbo.Vendas (VendaID),

        CONSTRAINT FK_ItensVenda_Produtos
            FOREIGN KEY (ProdutoID)
            REFERENCES dbo.Produtos (ProdutoID),

        CONSTRAINT UQ_ItensVenda_Venda_Produto
            UNIQUE (VendaID, ProdutoID),

        CONSTRAINT CK_ItensVenda_Quantidade
            CHECK (Quantidade > 0),

        CONSTRAINT CK_ItensVenda_PrecoUnitario
            CHECK (PrecoUnitario >= 0)
    );

    CREATE INDEX IX_ItensVenda_ProdutoID
        ON dbo.ItensVenda (ProdutoID);

    PRINT 'Tabela ItensVenda criada.';
END;
GO

/* =========================================================
   Tabela: MovimentacoesEstoque
   ========================================================= */

IF OBJECT_ID(N'dbo.MovimentacoesEstoque', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MovimentacoesEstoque
    (
        MovimentacaoID BIGINT IDENTITY(1,1) NOT NULL,
        ProdutoID INT NOT NULL,
        UsuarioID INT NOT NULL,
        Tipo VARCHAR(20) NOT NULL,
        Quantidade INT NOT NULL,
        DataMovimentacao DATETIME2(0) NOT NULL
            CONSTRAINT DF_MovimentacoesEstoque_Data
            DEFAULT (SYSUTCDATETIME()),
        OrigemTipo VARCHAR(20) NULL,
        OrigemID INT NULL,
        Observacao NVARCHAR(255) NULL,

        CONSTRAINT PK_MovimentacoesEstoque
            PRIMARY KEY (MovimentacaoID),

        CONSTRAINT FK_MovimentacoesEstoque_Produtos
            FOREIGN KEY (ProdutoID)
            REFERENCES dbo.Produtos (ProdutoID),

        CONSTRAINT FK_MovimentacoesEstoque_Usuarios
            FOREIGN KEY (UsuarioID)
            REFERENCES dbo.Usuarios (UsuarioID),

        CONSTRAINT CK_MovimentacoesEstoque_Tipo
            CHECK (Tipo IN ('ENTRADA', 'SAIDA', 'DEVOLUCAO', 'AJUSTE')),

        CONSTRAINT CK_MovimentacoesEstoque_Quantidade
            CHECK (Quantidade > 0),

        CONSTRAINT CK_MovimentacoesEstoque_Origem
            CHECK
            (
                (OrigemTipo IS NULL AND OrigemID IS NULL)
                OR
                (OrigemTipo IS NOT NULL AND OrigemID IS NOT NULL)
            )
    );

    CREATE INDEX IX_MovimentacoesEstoque_ProdutoID
        ON dbo.MovimentacoesEstoque (ProdutoID);

    CREATE INDEX IX_MovimentacoesEstoque_UsuarioID
        ON dbo.MovimentacoesEstoque (UsuarioID);

    CREATE INDEX IX_MovimentacoesEstoque_Data
        ON dbo.MovimentacoesEstoque (DataMovimentacao);

    PRINT 'Tabela MovimentacoesEstoque criada.';
END;
GO

/* =========================================================
   Validação
   ========================================================= */

SELECT
    s.name AS Esquema,
    t.name AS Tabela,
    t.create_date AS DataCriacao
FROM sys.tables AS t
INNER JOIN sys.schemas AS s
    ON s.schema_id = t.schema_id
WHERE t.name IN
(
    'Entradas',
    'ItensEntrada',
    'Vendas',
    'ItensVenda',
    'MovimentacoesEstoque'
)
ORDER BY t.name;
GO