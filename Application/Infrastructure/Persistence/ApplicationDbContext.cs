using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Application.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection(string connectionString = "DefaultConnection")
        {
            string? connection = _configuration.GetConnectionString(connectionString);
            return new NpgsqlConnection(connection);
        }
    }
}
