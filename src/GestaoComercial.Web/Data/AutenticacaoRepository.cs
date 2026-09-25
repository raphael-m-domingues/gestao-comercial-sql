using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class AutenticacaoRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public AutenticacaoRepository(
        SqlConnectionFactory connectionFactory
    )
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UsuarioAutenticacaoViewModel?>
        ObterPorEmailAsync(string email)
    {
        const string sql = """
            SELECT
                u.UsuarioID,
                u.Nome,
                u.Email,
                u.SenhaHash,
                p.Nome AS Perfil,
                u.Ativo
            FROM dbo.Usuarios AS u
            INNER JOIN dbo.Perfis AS p
                ON p.PerfilID = u.PerfilID
            WHERE u.Email = @Email;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<UsuarioAutenticacaoViewModel>(
                sql,
                new
                {
                    Email = email.Trim().ToLowerInvariant()
                }
            );
    }

    public async Task<IEnumerable<UsuarioListagemViewModel>>
        ListarUsuariosAsync()
    {
        const string sql = """
            SELECT
                u.UsuarioID,
                u.Nome,
                u.Email,
                p.Nome AS Perfil,
                u.Ativo,
                u.CriadoEm
            FROM dbo.Usuarios AS u
            INNER JOIN dbo.Perfis AS p
                ON p.PerfilID = u.PerfilID
            ORDER BY
                u.Nome,
                u.UsuarioID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<UsuarioListagemViewModel>(sql);
    }

    public async Task<UsuarioListagemViewModel?>
        ObterUsuarioPorIdAsync(int usuarioID)
    {
        const string sql = """
            SELECT
                u.UsuarioID,
                u.Nome,
                u.Email,
                p.Nome AS Perfil,
                u.Ativo,
                u.CriadoEm
            FROM dbo.Usuarios AS u
            INNER JOIN dbo.Perfis AS p
                ON p.PerfilID = u.PerfilID
            WHERE u.UsuarioID = @UsuarioID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<UsuarioListagemViewModel>(
                sql,
                new
                {
                    UsuarioID = usuarioID
                }
            );
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarPerfisAtivosAsync()
    {
        const string sql = """
            SELECT
                PerfilID AS ID,
                Nome
            FROM dbo.Perfis
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task CriarUsuarioAsync(
        UsuarioCriarViewModel usuario,
        string senhaHash
    )
    {
        const string sql = """
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
                @Nome,
                @Email,
                @SenhaHash
            );
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                usuario.PerfilID,
                Nome = usuario.Nome.Trim(),
                Email =
                    usuario.Email.Trim().ToLowerInvariant(),
                SenhaHash = senhaHash
            }
        );
    }

    public async Task<bool> AlterarStatusAsync(
        int usuarioID,
        bool ativo
    )
    {
        const string sql = """
            UPDATE dbo.Usuarios
            SET Ativo = @Ativo
            WHERE UsuarioID = @UsuarioID;

            SELECT CONVERT(BIT, @@ROWCOUNT);
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<bool>(
            sql,
            new
            {
                UsuarioID = usuarioID,
                Ativo = ativo
            }
        );
    }

    public async Task AtualizarSenhaHashAsync(
        int usuarioID,
        string senhaHash
    )
    {
        const string sql = """
            UPDATE dbo.Usuarios
            SET SenhaHash = @SenhaHash
            WHERE UsuarioID = @UsuarioID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                UsuarioID = usuarioID,
                SenhaHash = senhaHash
            }
        );
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = """
            SELECT CONVERT
            (
                BIT,
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM dbo.Usuarios AS u
                        INNER JOIN dbo.Perfis AS p
                            ON p.PerfilID = u.PerfilID
                        WHERE p.Nome = N'Administrador'
                          AND u.Ativo = 1
                    )
                    THEN 1
                    ELSE 0
                END
            );
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<bool>(sql);
    }

    public async Task CriarAdministradorAsync(
        string nome,
        string email,
        string senhaHash
    )
    {
        const string sql = """
            INSERT INTO dbo.Usuarios
            (
                PerfilID,
                Nome,
                Email,
                SenhaHash
            )
            SELECT
                p.PerfilID,
                @Nome,
                @Email,
                @SenhaHash
            FROM dbo.Perfis AS p
            WHERE p.Nome = N'Administrador'
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM dbo.Usuarios AS u
                  WHERE u.Email = @Email
              );

            IF @@ROWCOUNT = 0
                THROW 55001,
                    'Não foi possível criar o administrador inicial.',
                    1;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                Nome = nome.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                SenhaHash = senhaHash
            }
        );
    }
}