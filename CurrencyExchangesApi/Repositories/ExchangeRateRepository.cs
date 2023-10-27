using CurrencyExchangesApi.Models;
using Dapper;
using System.Data.SQLite;

namespace CurrencyExchangesApi.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly DapperContext _context;

        private const string GET_RATE_EXCHANGES = "SELECT e.ExchangeRateId, e.Rate, " +
            "a.CurrencyId as BaseCurrency, a.Code as bCode, a.FullName as bFullname, a.Sign as bSign," +
            "b.CurrencyId as TargetCurrency, b.Code as tCode, b.FullName as tFullname, b.Sign as tSign " +
            "FROM Currencies as a, Currencies as b, ExchangeRates as e " +
            "WHERE BaseCurrency = (SELECT BaseCurrencyId FROM ExchangeRates WHERE ExchangeRateId = e.ExchangeRateId) " +
            "AND TargetCurrency = (SELECT TargetCurrencyId FROM ExchangeRates WHERE ExchangeRateId = e.ExchangeRateId);";

        private const string GET_RATE_EXCHANGE = "SELECT e.ExchangeRateId, e.Rate, " +
            "a.CurrencyId as BaseCurrency, a.Code as bCode, a.FullName as bFullname, a.Sign as bSign," +
            "b.CurrencyId as TargetCurrency, b.Code as tCode, b.FullName as tFullname, b.Sign as tSign " +
            "FROM Currencies as a, Currencies as b, ExchangeRates as e " +
            "WHERE BaseCurrency = (SELECT BaseCurrencyId FROM ExchangeRates WHERE ExchangeRateId = e.ExchangeRateId) " +
            "AND TargetCurrency = (SELECT TargetCurrencyId FROM ExchangeRates WHERE ExchangeRateId = e.ExchangeRateId) " +
            "AND a.Code = @FirstCode " +
            "AND b.Code = @SecondCode;";

        private const string INSERT_RATE_EXCHANGE = "INSERT INTO ExchangeRates (BaseCurrencyId, TargetCurrencyId, Rate) " +
            "VALUES (" +
                "(SELECT CurrencyId FROM Currencies WHERE Code = @CurrentCode), " +
                "(SELECT CurrencyId FROM Currencies WHERE Code = @TargetCode), " +
                "@Rate);";

        private const string UPDATE_RATE = "UPDATE ExchangeRates SET Rate = @Rate WHERE Rate = (" +
            "SELECT Rate FROM ExchangeRates " +
            "WHERE BaseCurrencyId = (SELECT CurrencyId FROM Currencies WHERE Code = @CurrentCode) " +
            "AND TargetCurrencyId = (SELECT CurrencyId FROM Currencies WHERE Code = @TargetCode));";

        public ExchangeRateRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExchangeRate>> Get()
        {
            using (var connection = new SQLiteConnection(_context.CreateSqliteConnection()))
            {
                var entities = await connection.QueryAsync(GET_RATE_EXCHANGES);

                var exchangeRates = new List<ExchangeRate>();

                foreach (var item in entities)
                {
                    exchangeRates.Add(new ExchangeRate()
                    {
                        ExchangeRateId = (int)item.ExchangeRateId,
                        BaseCurrency = new Currency
                        {
                            CurrencyId = (int)item.BaseCurrency,
                            Code = item.bCode,
                            FullName = item.bFullname,
                            Sign = item.bSign,
                        },
                        TargetCurrency = new Currency
                        {
                            CurrencyId = (int)item.TargetCurrency,
                            Code = item.tCode,
                            FullName = item.tFullname,
                            Sign = item.tSign,
                        },
                        Rate = item.Rate,
                    });
                }

                return exchangeRates;
            }
        }

        public async Task<ExchangeRate> Get(string currentCode, string targetCode)
        {
            using (var connection = new SQLiteConnection(_context.CreateSqliteConnection()))
            {
                var parametrs = new 
                { 
                    FirstCode = currentCode, 
                    SecondCode = targetCode
                };

                var item = await connection.QueryFirstOrDefaultAsync(GET_RATE_EXCHANGE, parametrs);

                ExchangeRate exchangeRate;

                if (item != null)
                {
                    exchangeRate = new ExchangeRate
                    {
                        ExchangeRateId = (int)item!.ExchangeRateId,
                        BaseCurrency = new Currency
                        {
                            CurrencyId = (int)item.BaseCurrency,
                            Code = item.bCode,
                            FullName = item.bFullname,
                            Sign = item.bSign,
                        },
                        TargetCurrency = new Currency
                        {
                            CurrencyId = (int)item.TargetCurrency,
                            Code = item.tCode,
                            FullName = item.tFullname,
                            Sign = item.tSign,
                        },
                        Rate = item.Rate,
                    };

                    return exchangeRate;
                }

                return item!;
            }
        }

        public async Task Insert(string currentCode, string targetCode, decimal rate)
        {
            var parameters = new
            {
                CurrentCode = currentCode,
                TargetCode = targetCode,
                Rate = rate,
            };

            using (var connection = new SQLiteConnection(_context.CreateSqliteConnection()))
            {
                await connection.QueryAsync<ExchangeRate>(INSERT_RATE_EXCHANGE, parameters);
            }
        }

        public async Task Update(string currentCode, string targetCode, decimal rate)
        {
            var parameter = new 
            {
                CurrentCode = currentCode,
                TargetCode = targetCode,
                Rate = rate,
            };

            using (var connection = new SQLiteConnection(_context.CreateSqliteConnection()))
            {
                await connection.QueryAsync<ExchangeRate>(UPDATE_RATE, parameter);
            }
        }
    }
}
