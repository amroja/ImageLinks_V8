using System.Data;
using System.Linq;
using Dapper;
using ImageLinks.Application.Interfaces;
using ImageLinks.Domain.Enums;
using ImageLinks.Domain.Users;

namespace ImageLinks.Infrastructure.Persistence.Dapper
{
    public sealed class UsersService : IUsersService
    {
        private const string UsersTableProjection = "REC_ID, USER_NAME, PASSWORD";
        private readonly IGenericService _genericService;

        public UsersService(IGenericService genericService)
            => _genericService = genericService;

        public async Task<IEnumerable<User>> GetUsersAsync(
            string? connectionString = null,
            CancellationToken ct = default)
        {
            var sql = $"SELECT {UsersTableProjection} FROM USERS";
            var table = await _genericService.GetDataTableAsync(sql, null, connectionString, ct);
            return MapUsers(table).ToArray();
        }

        public async Task<User?> GetUserByIdAsync(
            int recId,
            string? connectionString = null,
            CancellationToken ct = default)
        {
            var provider = _genericService.GetDatabaseType(connectionString);
            var paramPrefix = GetParameterPrefix(provider);
            var sql = $"SELECT {UsersTableProjection} FROM USERS WHERE REC_ID = {paramPrefix}Rec_ID";

            var parameters = new DynamicParameters();
            parameters.Add("Rec_ID", recId, DbType.Int32, ParameterDirection.Input);

            var table = await _genericService.GetDataTableAsync(sql, parameters, connectionString, ct);
            return MapUsers(table).FirstOrDefault();
        }

        public async Task<int> CreateUserAsync(
            User user,
            string? connectionString = null,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(user);

            var provider = _genericService.GetDatabaseType(connectionString);
            var paramPrefix = GetParameterPrefix(provider);

            var parameters = new DynamicParameters();
            parameters.Add("User_Name", user.User_Name, DbType.String, ParameterDirection.Input);
            parameters.Add("Password", user.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("Rec_ID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var sql = provider switch
            {
                DatabaseProvider.Oracle => $@"
BEGIN
    INSERT INTO USERS (REC_ID, USER_NAME, PASSWORD)
    VALUES ((SELECT NVL(MAX(REC_ID), 0) + 1 FROM USERS), {paramPrefix}User_Name, {paramPrefix}Password)
    RETURNING REC_ID INTO {paramPrefix}Rec_ID;
END;",
                _ => $@"
INSERT INTO USERS (User_Name, Password)
VALUES ({paramPrefix}User_Name, {paramPrefix}Password);
SET {paramPrefix}Rec_ID = CAST(SCOPE_IDENTITY() AS INT);"
            };

            await _genericService.ExecuteNonQueryAsync(sql, parameters, connectionString, ct);
            return parameters.Get<int?>("Rec_ID") ?? 0;
        }

        public async Task<bool> UpdateUserAsync(
            User user,
            string? connectionString = null,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(user);

            var provider = _genericService.GetDatabaseType(connectionString);
            var paramPrefix = GetParameterPrefix(provider);

            var sql = $@"UPDATE USERS
SET USER_NAME = {paramPrefix}User_Name,
    PASSWORD = {paramPrefix}Password
WHERE REC_ID = {paramPrefix}Rec_ID";

            var parameters = new DynamicParameters();
            parameters.Add("User_Name", user.User_Name, DbType.String, ParameterDirection.Input);
            parameters.Add("Password", user.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("Rec_ID", user.Rec_ID, DbType.Int32, ParameterDirection.Input);

            var affected = await _genericService.ExecuteNonQueryAsync(sql, parameters, connectionString, ct);
            return affected > 0;
        }

        public async Task<bool> DeleteUserAsync(
            int recId,
            string? connectionString = null,
            CancellationToken ct = default)
        {
            var provider = _genericService.GetDatabaseType(connectionString);
            var paramPrefix = GetParameterPrefix(provider);

            var sql = $"DELETE FROM USERS WHERE REC_ID = {paramPrefix}Rec_ID";

            var parameters = new DynamicParameters();
            parameters.Add("Rec_ID", recId, DbType.Int32, ParameterDirection.Input);

            var affected = await _genericService.ExecuteNonQueryAsync(sql, parameters, connectionString, ct);
            return affected > 0;
        }

        private static string GetParameterPrefix(DatabaseProvider provider)
            => provider == DatabaseProvider.Oracle ? ":" : "@";

        private static IEnumerable<User> MapUsers(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                yield return new User
                {
                    Rec_ID = Convert.ToInt32(row["REC_ID"]),
                    User_Name = row.Field<string?>("USER_NAME") ?? string.Empty,
                    Password = row.Field<string?>("PASSWORD") ?? string.Empty,
                };
            }
        }
    }
}
