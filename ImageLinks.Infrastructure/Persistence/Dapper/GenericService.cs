using Dapper;
using ImageLinks.Application.Interfaces;
using ImageLinks.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;

namespace ImageLinks.Infrastructure.Persistence.Dapper
{
    public sealed class GenericService : IGenericService
    {
        private readonly IConfiguration _configuration;
        public GenericService(IConfiguration configuration) => _configuration = configuration;

        private string GetConn(string? overrideConn = null)
            => !string.IsNullOrWhiteSpace(overrideConn)
               ? overrideConn
               : _configuration.GetConnectionString("DefaultConnection")!;

        private IDbConnection CreateConnection(string? overrideConn = null)
        {
            var connStr = ModifyConnectionString(GetConn(overrideConn));
            return GetDatabaseType(connStr) switch
            {
                DatabaseProvider.Oracle => new OracleConnection(connStr),
                _ => new SqlConnection(connStr)
            };
        }

        public DatabaseProvider GetDatabaseType(string? connectionString = null)
        {
            var conn = GetConn(connectionString);
            if (conn.Contains("Provider=MSDAORA", StringComparison.OrdinalIgnoreCase) ||
                conn.Contains("SERVICE_NAME", StringComparison.OrdinalIgnoreCase) ||
                (!conn.Contains("initial catalog", StringComparison.OrdinalIgnoreCase)))
                return DatabaseProvider.Oracle;

            return DatabaseProvider.SqlServer;
        }

        public async Task<string?> ExecuteScalarAsync(
            string sql,
            object? parameters = null,
            string? connectionString = null,
            CancellationToken ct = default)
        {
            IDbConnection? db = CreateConnection(connectionString);
            var result = await db.ExecuteScalarAsync<object?>(new CommandDefinition(sql, parameters, cancellationToken: ct));
            return result?.ToString();
        }

        public async Task<DataTable> GetDataTableAsync(
            string sql,
            object? parameters = null,
            string? connectionString = null,
            CancellationToken ct = default)
        {
            IDbConnection? db = CreateConnection(connectionString);
            using var reader = await db.ExecuteReaderAsync(new CommandDefinition(sql, parameters, cancellationToken: ct));
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            object? parameters = null,
            string? connectionString = null,
            CancellationToken ct = default)
        {
             IDbConnection? db = CreateConnection(connectionString);
            return await db.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: ct));
        }

        public async Task<int> ExecuteTransactionAsync(
    IEnumerable<string> sqlStatements,
    string? connectionString = null,
    CancellationToken ct = default)
        {
            using var db = CreateConnection(connectionString);
            using var tx = db.BeginTransaction();

            try
            {
                var affected = 0;
                foreach (var sql in sqlStatements)
                {
                    if (string.IsNullOrWhiteSpace(sql)) continue;

                    affected += await db.ExecuteAsync(
                        new CommandDefinition(sql.Trim(),
                                               transaction: tx,
                                               cancellationToken: ct));
                }
                tx.Commit();
                return affected;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public bool IsDatabaseEmpty()
        {
            var dbType = GetDatabaseType();
            var sql = dbType == DatabaseProvider.SqlServer
                ? "SELECT COUNT(*) FROM sysobjects WHERE type='U' AND name='users'"
                : "SELECT COUNT(table_name) FROM user_tables WHERE table_name='USERS'";
            return Convert.ToInt32(ExecuteScalarAsync(sql).GetAwaiter().GetResult()) == 0;
        }

        public string GetDatabaseName()
        {
            var conn = GetConn();
            return GetDatabaseType(conn) == DatabaseProvider.Oracle
                ? conn.Split("User ID")[1].Split(';')[0].Replace("=", "").Trim()
                : conn.Split("Initial Catalog")[1].Split(';')[0].Replace("=", "").Trim();
        }

        private static string ModifyConnectionString(string conn)
        {
            return string.Join(';',
                conn.Split(';').Where(p => !p.Trim().StartsWith("provider=", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
