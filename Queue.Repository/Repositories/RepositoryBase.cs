using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Queue.Repository.Interfaces;

namespace Queue.Repository.Repositories;
public class RepositoryBase(IConfiguration config) : IRepositoryBase
{
    private readonly string _connString = config["CONNECTION_STRING"]!;

    public async Task<bool> Insert<T>(T entity, CancellationToken cancellationToken)
    {
        var tableName = entity!.GetType().Name;
        var properties = entity.GetType().GetProperties();

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        var insertSql = $"INSERT INTO dbo.{tableName} ({columnNames}) VALUES ({parameterNames}) ";

        using var conn = new SqlConnection(_connString);
        await conn.OpenAsync(cancellationToken);
        return await conn.ExecuteAsync(insertSql, param: entity) == 1;
    }

    public async Task<bool> Update<T>(T entity, CancellationToken cancellationToken)
    {
        var tableName = entity!.GetType().Name;
        var properties = entity.GetType().GetProperties();

        var keyProperty = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception("Chave primária 'Id' não encontrada.");

        var setClause = string.Join(", ", properties
            .Where(p => p.Name != keyProperty.Name)
            .Select(p => $"{p.Name} = @{p.Name}"));

        var updateSql = $"UPDATE dbo.{tableName} SET {setClause} WHERE {keyProperty.Name} = @{keyProperty.Name}";

        using var conn = new SqlConnection(_connString);
        await conn.OpenAsync(cancellationToken);
        return await conn.ExecuteAsync(updateSql, param: entity) == 1;
    }

    public async Task<bool> Delete<T>(Guid id, CancellationToken cancellationToken)
    {
        var tableName = typeof(T).Name;

        var sql = $"DELETE FROM dbo.{tableName} WHERE Id = @id";

        using var conn = new SqlConnection(_connString);
        await conn.OpenAsync(cancellationToken);
        return await conn.ExecuteAsync(sql, param: new { id }) == 1;
    }
}
