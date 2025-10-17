
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventorySys.Infrastructure.Data;

public interface IDapperDbContext
{
    IDbConnection GetConnection();
}

public class DapperDbContext : IDisposable, IDapperDbContext
{
    private readonly string _connectionString;
    private IDbConnection? _connection;

    public DapperDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetApplicationDatabaseConnectionString();
    }
    public IDbConnection GetConnection()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }
        return _connection;
    }
    public void Dispose()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}