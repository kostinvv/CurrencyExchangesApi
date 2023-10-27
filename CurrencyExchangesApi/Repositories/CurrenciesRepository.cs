using CurrencyExchangesApi.Dapper;
using CurrencyExchangesApi.Models;
using CurrencyExchangesApi.Repositories.Interfaces;
using Dapper;

namespace CurrencyExchangesApi.Repositories
{
    public class CurrenciesRepository : ICurrenciesRepository
    {
        private readonly DapperContext _context;

        private const string SELECT_CURRENCY = "SELECT * FROM Currencies";
        private const string SELECT_CURRENCIES = "SELECT * FROM Currencies WHERE Code = @Code";
        private const string INSERT_CURRENCY = "INSERT INTO Currencies (Code, FullName, Sign) " +
            "VALUES (@Code, @FullName, @Sign)";

        public CurrenciesRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Currency> Get(string code)
        {
            var parametr = new { Code = code };

            using (var connection = _context.CreateSqliteConnection())
            {
                var currency = await connection
                    .QueryFirstOrDefaultAsync<Currency>(SELECT_CURRENCIES, parametr);

                return currency!;
            }
        }

        public async Task<IEnumerable<Currency>> Get()
        {
            using (var connection = _context.CreateSqliteConnection())
            {
                var currencies = await connection.QueryAsync<Currency>(SELECT_CURRENCY);

                return currencies;
            }
        }

        public async Task Insert(Currency currency)
        {
            var parameters = new
            {
                Code = currency.Code,
                FullName = currency.FullName,
                Sign = currency.Sign,
            };

            using (var connection = _context.CreateSqliteConnection())
            {
                await connection.QueryAsync<Currency>(INSERT_CURRENCY, parameters);
            }
        }
    }
}
