USE DB_GestaoComercial;
GO

/* =========================================================
   Tabela: Perfis
   ========================================================= */

IF OBJECT_ID(N'dbo.Perfis', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Perfis
    (
        PerfilID INT IDENTITY(1,1) NOT NULL,
        Nome NVARCHAR(30) NOT NULL,
        Descricao NVARCHAR(150) NULL,
        Ativo BIT NOT NULL
            CONSTRAINT DF_Perfis_Ativo DEFAULT (1),

        CONSTRAINT PK_Perfis
            PRIMARY KEY (PerfilID),

        CONSTRAINT UQ_Perfis_Nome
            UNIQUE (Nome)
    );

    PRINT 'Tabela Perfis criada.';
END;
GO

/* =========================================================
   Tabela: Usuarios
   ========================================================= */

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        UsuarioID INT IDENTITY(1,1) NOT NULL,
        PerfilID INT NOT NULL,
        Nome NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL,
        SenhaHash VARCHAR(255) NOT NULL,
        Ativo BIT NOT NULL
            CONSTRAINT DF_Usuarios_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL
            CONSTRAINT DF_Usuarios_CriadoEm
            DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Usuarios
            PRIMARY KEY (UsuarioID),

        CONSTRAINT UQ_Usuarios_Email
            UNIQUE (Email),

        CONSTRAINT FK_Usuarios_Perfis
            FOREIGN KEY (PerfilID)
            REFERENCES dbo.Perfis (PerfilID)
    );

    PRINT 'Tabela Usuarios criada.';
END;
GO

/* =========================================================
   Tabela: Categorias
   ========================================================= */

IF OBJECT_ID(N'dbo.Categorias', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias
    (
        CategoriaID INT IDENTITY(1,1) NOT NULL,
        Nome NVARCHAR(60) NOT NULL,
        Descricao NVARCHAR(150) NULL,
        Ativo BIT NOT NULL
            CONSTRAINT DF_Categorias_Ativo DEFAULT (1),

        CONSTRAINT PK_Categorias
            PRIMARY KEY (CategoriaID),

        CONSTRAINT UQ_Categorias_Nome
            UNIQUE (Nome)
    );

    PRINT 'Tabela Categorias criada.';
END;
GO

/* =========================================================
   Tabela: Produtos
   ========================================================= */

IF OBJECT_ID(N'dbo.Produtos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Produtos
    (
        ProdutoID INT IDENTITY(1,1) NOT NULL,
        CategoriaID INT NOT NULL,
        CodigoBarras VARCHAR(14) NULL,
        Nome NVARCHAR(120) NOT NULL,
        Descricao NVARCHAR(255) NULL,
        PrecoCusto DECIMAL(10,2) NOT NULL,
        PrecoVenda DECIMAL(10,2) NOT NULL,
        EstoqueMinimo INT NOT NULL,
        Ativo BIT NOT NULL
            CONSTRAINT DF_Produtos_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL
            CONSTRAINT DF_Produtos_CriadoEm
            DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Produtos
            PRIMARY KEY (ProdutoID),

        CONSTRAINT FK_Produtos_Categorias
            FOREIGN KEY (CategoriaID)
            REFERENCES dbo.Categorias (CategoriaID),

        CONSTRAINT CK_Produtos_PrecoCusto
            CHECK (PrecoCusto >= 0),

        CONSTRAINT CK_Produtos_PrecoVenda
            CHECK (PrecoVenda >= 0),

        CONSTRAINT CK_Produtos_EstoqueMinimo
            CHECK (EstoqueMinimo >= 0)
    );

    CREATE UNIQUE INDEX UX_Produtos_CodigoBarras
        ON dbo.Produtos (CodigoBarras)
        WHERE CodigoBarras IS NOT NULL;

    PRINT 'Tabela Produtos criada.';
END;
GO

/* =========================================================
   Tabela: Estoques
   ========================================================= */

IF OBJECT_ID(N'dbo.Estoques', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Estoques
    (
        ProdutoID INT NOT NULL,
        QuantidadeAtual INT NOT NULL
            CONSTRAINT DF_Estoques_QuantidadeAtual DEFAULT (0),
        AtualizadoEm DATETIME2(0) NOT NULL
            CONSTRAINT DF_Estoques_AtualizadoEm
            DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Estoques
            PRIMARY KEY (ProdutoID),

        CONSTRAINT FK_Estoques_Produtos
            FOREIGN KEY (ProdutoID)
            REFERENCES dbo.Produtos (ProdutoID),

        CONSTRAINT CK_Estoques_QuantidadeAtual
            CHECK (QuantidadeAtual >= 0)
    );

    PRINT 'Tabela Estoques criada.';
END;
GO

/* =========================================================
   Tabela: Fornecedores
   ========================================================= */

IF OBJECT_ID(N'dbo.Fornecedores', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Fornecedores
    (
        FornecedorID INT IDENTITY(1,1) NOT NULL,
        RazaoSocial NVARCHAR(120) NOT NULL,
        NomeFantasia NVARCHAR(120) NULL,
        CNPJ CHAR(14) NULL,
        Telefone VARCHAR(20) NULL,
        Email NVARCHAR(150) NULL,
        Ativo BIT NOT NULL
            CONSTRAINT DF_Fornecedores_Ativo DEFAULT (1),

        CONSTRAINT PK_Fornecedores
            PRIMARY KEY (FornecedorID)
    );

    CREATE UNIQUE INDEX UX_Fornecedores_CNPJ
        ON dbo.Fornecedores (CNPJ)
        WHERE CNPJ IS NOT NULL;

    PRINT 'Tabela Fornecedores criada.';
END;
GO

/* =========================================================
   Tabela: FormasPagamento
   ========================================================= */

IF OBJECT_ID(N'dbo.FormasPagamento', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FormasPagamento
    (
        FormaPagamentoID INT IDENTITY(1,1) NOT NULL,
        Nome NVARCHAR(40) NOT NULL,
        Ativo BIT NOT NULL
            CONSTRAINT DF_FormasPagamento_Ativo DEFAULT (1),

        CONSTRAINT PK_FormasPagamento
            PRIMARY KEY (FormaPagamentoID),

        CONSTRAINT UQ_FormasPagamento_Nome
            UNIQUE (Nome)
    );

    PRINT 'Tabela FormasPagamento criada.';
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
ORDER BY t.name;
GO