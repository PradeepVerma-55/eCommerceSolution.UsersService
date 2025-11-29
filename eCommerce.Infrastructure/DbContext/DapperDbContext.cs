using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Infrastructure.DbContext
{
    public class DapperDbContext
    {
        private readonly string _connectionString;

        public DapperDbContext(IConfiguration configuration)
        {
           string connectionStringTemplate = configuration.GetConnectionString("PostgreSqlConnection")!
                ?? throw new InvalidOperationException("Connection string 'PostgreSqlConnection' not found.");
            _connectionString = connectionStringTemplate.Replace("$POSTGRES_HOST", Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost")
                                                         .Replace("$POSTGRES_PASSWORD", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "admin");
        }

        // Return a new connection instance on each request (avoid sharing a single IDbConnection)
        public IDbConnection DbConnection => new NpgsqlConnection(_connectionString);
    }



}
