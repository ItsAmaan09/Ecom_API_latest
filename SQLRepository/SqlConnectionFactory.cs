using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace ECommerce.Core
{
    public class SqlConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateConnection()
            => new SqlConnection(_configuration.GetConnectionString("SQLConnectionString"));
    }
}