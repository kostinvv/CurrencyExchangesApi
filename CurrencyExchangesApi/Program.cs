var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<ICurrenciesRepository, CurrenciesRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<ExchangeRateService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
