using CurrencyExchangesApi.Models;
using Dapper;

namespace CurrencyExchangesApi.Repositories
{
    public class CurrenciesRepository : ICurrenciesRepository
    {
        private readonly DapperContext _context;

        public CurrenciesRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Currency> Get(string code)
        {
            var query = "SELECT * FROM Currencies WHERE Code = @Code";

            using (var connection = _context.CreateSqliteConnection())
            {
                var currency = await connection
                    .QueryFirstOrDefaultAsync<Currency>(query, new { Code = code });

                return currency!;
            }
        }

        public async Task<IEnumerable<Currency>> Get()
        {
            var query = "SELECT * FROM Currencies";

            using (var connection = _context.CreateSqliteConnection())
            {
                var currencies = await connection.QueryAsync<Currency>(query);

                return currencies;
            }
        }

        public async Task Insert(Currency currency)
        {
            var query = @"
                INSERT INTO Currencies (Code, FullName, Sign) 
                VALUES (@Code, @FullName, @Sign)
            ";

            var parametrs = new { currency.Code, currency.FullName, currency.Sign };

            using (var connection = _context.CreateSqliteConnection())
            {
                await connection.QueryAsync<Currency>(query, parametrs);
            }
        }
    }
}
