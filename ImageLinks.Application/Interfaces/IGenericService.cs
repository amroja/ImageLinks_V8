using ImageLinks.Domain.Enums;
using System.Data;

namespace ImageLinks.Application.Interfaces
{
    public interface IGenericService
    {
        Task<string?> ExecuteScalarAsync(
            string sql,
            object? parameters = null,
            string? connectionString = null,
            CancellationToken ct = default);

        Task<DataTable> GetDataTableAsync(
            string sql,
            object? parameters = null,
            string? connectionString = null,
            CancellationToken ct = default);

        Task<int> ExecuteNonQueryAsync(
            string sql,
            object? parameters = null,
            string? connectionString = null,
            CancellationToken ct = default);

        Task<int> ExecuteTransactionAsync(
            IEnumerable<string> sqlStatements,
            string? connectionString = null,
            CancellationToken ct = default);

        DatabaseProvider GetDatabaseType(string? connectionString = null);
        bool IsDatabaseEmpty();
        string GetDatabaseName();
    }
}
