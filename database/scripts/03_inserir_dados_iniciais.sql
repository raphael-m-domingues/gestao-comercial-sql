USE DB_GestaoComercial;
GO

SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    /* =====================================================
       Perfis
       ===================================================== */

    INSERT INTO dbo.Perfis
    (
        Nome,
        Descricao
    )
    SELECT
        v.Nome,
        v.Descricao
    FROM
    (
        VALUES
            (
                N'Administrador',
                N'Acesso completo ao sistema'
            ),
            (
                N'Estoquista',
                N'Gerenciamento de produtos e estoque'
            ),
            (
                N'Caixa',
                N'Registro e consulta de vendas'
            )
    ) AS v
    (
        Nome,
        Descricao
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Perfis AS p
        WHERE p.Nome = v.Nome
    );

    /* =====================================================
       Usuário operacional de demonstração
       ===================================================== */

    INSERT INTO dbo.Usuarios
    (
        PerfilID,
        Nome,
        Email,
        SenhaHash
    )
    SELECT
        p.PerfilID,
        N'Operador de Estoque',
        N'estoque@minimercado.local',
        CONVERT
        (
            VARCHAR(64),
            HASHBYTES
            (
                'SHA2_256',
                N'usuario-demonstracao-sem-autenticacao'
            ),
            2
        )
    FROM dbo.Perfis AS p
    WHERE p.Nome = N'Estoquista'
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.Usuarios AS u
          WHERE u.Email =
              N'estoque@minimercado.local'
      );

    /* =====================================================
       Formas de pagamento
       ===================================================== */

    INSERT INTO dbo.FormasPagamento
    (
        Nome
    )
    SELECT
        v.Nome
    FROM
    (
        VALUES
            (N'Dinheiro'),
            (N'Pix'),
            (N'Cartão de débito'),
            (N'Cartão de crédito')
    ) AS v
    (
        Nome
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.FormasPagamento AS fp
        WHERE fp.Nome = v.Nome
    );

    /* =====================================================
       Categorias
       ===================================================== */

    INSERT INTO dbo.Categorias
    (
        Nome,
        Descricao
    )
    SELECT
        v.Nome,
        v.Descricao
    FROM
    (
        VALUES
            (
                N'Alimentos',
                N'Produtos alimentícios em geral'
            ),
            (
                N'Bebidas',
                N'Bebidas não alcoólicas'
            ),
            (
                N'Higiene',
                N'Produtos de higiene pessoal'
            ),
            (
                N'Limpeza',
                N'Produtos para limpeza doméstica'
            ),
            (
                N'Utilidades',
                N'Utensílios, acessórios e utilidades domésticas'
            )
    ) AS v
    (
        Nome,
        Descricao
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Categorias AS c
        WHERE c.Nome = v.Nome
    );

    /* =====================================================
       Fornecedores fictícios
       ===================================================== */

    INSERT INTO dbo.Fornecedores
    (
        RazaoSocial,
        NomeFantasia,
        Telefone,
        Email
    )
    SELECT
        v.RazaoSocial,
        v.NomeFantasia,
        v.Telefone,
        v.Email
    FROM
    (
        VALUES
        (
            N'Distribuidora Vila Nova Ltda.',
            N'Distribuidora Vila Nova',
            '11999990001',
            N'contato@vilanova.exemplo'
        ),
        (
            N'Atacado Boa Compra Ltda.',
            N'Atacado Boa Compra',
            '11999990002',
            N'contato@boacompra.exemplo'
        )
    ) AS v
    (
        RazaoSocial,
        NomeFantasia,
        Telefone,
        Email
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Fornecedores AS f
        WHERE f.RazaoSocial = v.RazaoSocial
    );

    /* =====================================================
       Produtos fictícios
       ===================================================== */

    INSERT INTO dbo.Produtos
    (
        CategoriaID,
        Nome,
        Descricao,
        PrecoCusto,
        PrecoVenda,
        EstoqueMinimo
    )
    SELECT
        c.CategoriaID,
        v.Nome,
        v.Descricao,
        v.PrecoCusto,
        v.PrecoVenda,
        v.EstoqueMinimo
    FROM
    (
        VALUES
        (
            N'Alimentos',
            N'Arroz 5 kg',
            N'Pacote de arroz tipo 1',
            CAST(20.00 AS DECIMAL(10,2)),
            CAST(27.90 AS DECIMAL(10,2)),
            10
        ),
        (
            N'Alimentos',
            N'Feijão 1 kg',
            N'Pacote de feijão carioca',
            CAST(5.50 AS DECIMAL(10,2)),
            CAST(8.49 AS DECIMAL(10,2)),
            10
        ),
        (
            N'Alimentos',
            N'Macarrão 500 g',
            N'Pacote de macarrão espaguete',
            CAST(3.20 AS DECIMAL(10,2)),
            CAST(5.49 AS DECIMAL(10,2)),
            8
        ),
        (
            N'Bebidas',
            N'Refrigerante 2 L',
            N'Refrigerante sabor cola',
            CAST(6.50 AS DECIMAL(10,2)),
            CAST(9.99 AS DECIMAL(10,2)),
            12
        ),
        (
            N'Bebidas',
            N'Água mineral 500 ml',
            N'Água mineral sem gás',
            CAST(1.20 AS DECIMAL(10,2)),
            CAST(2.50 AS DECIMAL(10,2)),
            20
        ),
        (
            N'Higiene',
            N'Sabonete 90 g',
            N'Sabonete em barra',
            CAST(1.80 AS DECIMAL(10,2)),
            CAST(3.49 AS DECIMAL(10,2)),
            10
        ),
        (
            N'Limpeza',
            N'Detergente 500 ml',
            N'Detergente líquido neutro',
            CAST(1.70 AS DECIMAL(10,2)),
            CAST(3.29 AS DECIMAL(10,2)),
            10
        ),
        (
            N'Limpeza',
            N'Água sanitária 1 L',
            N'Produto para limpeza doméstica',
            CAST(2.80 AS DECIMAL(10,2)),
            CAST(5.49 AS DECIMAL(10,2)),
            8
        ),
        (
            N'Utilidades',
            N'Papel toalha 2 rolos',
            N'Pacote de papel toalha com dois rolos',
            CAST(4.75 AS DECIMAL(10,2)),
            CAST(8.49 AS DECIMAL(10,2)),
            8
        )
    ) AS v
    (
        CategoriaNome,
        Nome,
        Descricao,
        PrecoCusto,
        PrecoVenda,
        EstoqueMinimo
    )
    INNER JOIN dbo.Categorias AS c
        ON c.Nome = v.CategoriaNome
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Produtos AS p
        WHERE p.Nome = v.Nome
    );

    /* =====================================================
       Registro inicial de estoque
       ===================================================== */

    INSERT INTO dbo.Estoques
    (
        ProdutoID
    )
    SELECT
        p.ProdutoID
    FROM dbo.Produtos AS p
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Estoques AS e
        WHERE e.ProdutoID = p.ProdutoID
    );

    COMMIT TRANSACTION;

    PRINT 'Dados iniciais inseridos com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

/* =========================================================
   Validação
   ========================================================= */

SELECT
    PerfilID,
    Nome,
    Descricao,
    Ativo
FROM dbo.Perfis
ORDER BY PerfilID;

SELECT
    u.UsuarioID,
    p.Nome AS Perfil,
    u.Nome AS Usuario,
    u.Email,
    u.Ativo,
    u.CriadoEm
FROM dbo.Usuarios AS u
INNER JOIN dbo.Perfis AS p
    ON p.PerfilID = u.PerfilID
ORDER BY u.UsuarioID;

SELECT
    FormaPagamentoID,
    Nome,
    Ativo
FROM dbo.FormasPagamento
ORDER BY FormaPagamentoID;

SELECT
    CategoriaID,
    Nome,
    Descricao,
    Ativo
FROM dbo.Categorias
ORDER BY CategoriaID;

SELECT
    FornecedorID,
    RazaoSocial,
    NomeFantasia,
    Telefone,
    Email,
    Ativo
FROM dbo.Fornecedores
ORDER BY FornecedorID;

SELECT
    p.ProdutoID,
    c.Nome AS Categoria,
    p.Nome AS Produto,
    p.PrecoCusto,
    p.PrecoVenda,
    p.EstoqueMinimo,
    e.QuantidadeAtual,
    p.Ativo
FROM dbo.Produtos AS p
INNER JOIN dbo.Categorias AS c
    ON c.CategoriaID = p.CategoriaID
INNER JOIN dbo.Estoques AS e
    ON e.ProdutoID = p.ProdutoID
ORDER BY p.ProdutoID;
GO