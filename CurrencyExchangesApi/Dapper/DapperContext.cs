using System.Data.SQLite;

namespace CurrencyExchangesApi.Dapper
{
    public class DapperContext
    {
        private const string NAME = "SQLiteDbPath";

        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SQLiteConnection CreateSqliteConnection()
        {
            var connectionString = _configuration.GetConnectionString(NAME);

            return new SQLiteConnection(connectionString);
        }
    }
}
