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
            _connectionString = configuration.GetConnectionString("PostgreSqlConnection")
                ?? throw new InvalidOperationException("Connection string 'PostgreSqlConnection' not found.");
        }

        // Return a new connection instance on each request (avoid sharing a single IDbConnection)
        public IDbConnection DbConnection => new NpgsqlConnection(_connectionString);
    }



}
