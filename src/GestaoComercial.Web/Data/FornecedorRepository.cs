using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class FornecedorRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public FornecedorRepository(
        SqlConnectionFactory connectionFactory
    )
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<FornecedorViewModel>>
        ListarAsync()
    {
        const string sql = """
            SELECT
                FornecedorID,
                RazaoSocial,
                NomeFantasia,
                CNPJ,
                Telefone,
                Email,
                Ativo
            FROM dbo.Fornecedores
            ORDER BY
                RazaoSocial,
                FornecedorID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<FornecedorViewModel>(sql);
    }

    public async Task<FornecedorViewModel?>
        ObterPorIdAsync(int fornecedorID)
    {
        const string sql = """
            SELECT
                FornecedorID,
                RazaoSocial,
                NomeFantasia,
                CNPJ,
                Telefone,
                Email,
                Ativo
            FROM dbo.Fornecedores
            WHERE FornecedorID = @FornecedorID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<FornecedorViewModel>(
                sql,
                new
                {
                    FornecedorID = fornecedorID
                }
            );
    }

    public async Task<int> InserirAsync(
        FornecedorViewModel fornecedor
    )
    {
        const string sql = """
            INSERT INTO dbo.Fornecedores
            (
                RazaoSocial,
                NomeFantasia,
                CNPJ,
                Telefone,
                Email,
                Ativo
            )
            OUTPUT INSERTED.FornecedorID
            VALUES
            (
                @RazaoSocial,
                @NomeFantasia,
                @CNPJ,
                @Telefone,
                @Email,
                @Ativo
            );
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            sql,
            fornecedor
        );
    }

    public async Task<bool> AtualizarAsync(
        FornecedorViewModel fornecedor
    )
    {
        const string sql = """
            UPDATE dbo.Fornecedores
            SET
                RazaoSocial = @RazaoSocial,
                NomeFantasia = @NomeFantasia,
                CNPJ = @CNPJ,
                Telefone = @Telefone,
                Email = @Email,
                Ativo = @Ativo
            WHERE FornecedorID = @FornecedorID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        var registrosAfetados =
            await connection.ExecuteAsync(
                sql,
                fornecedor
            );

        return registrosAfetados == 1;
    }

    public async Task<bool> AlterarStatusAsync(
        int fornecedorID,
        bool ativo
    )
    {
        const string sql = """
            UPDATE dbo.Fornecedores
            SET Ativo = @Ativo
            WHERE FornecedorID = @FornecedorID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        var registrosAfetados =
            await connection.ExecuteAsync(
                sql,
                new
                {
                    FornecedorID = fornecedorID,
                    Ativo = ativo
                }
            );

        return registrosAfetados == 1;
    }
}