using Data;
using Data.Contracts;
using Logic;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
var services = builder.Services;
services.AddDbContext<MySqlContext>(options => options.UseMySQL(configuration.GetSection("ConnectionsString")["MySQL"]));
services.Configure<StatisticsSettings>(configuration.GetSection("StatisticsSettings"));
services.AddScoped<IStatisticsRepository, StatisticsRepository>();
services.AddSingleton<StatisticsService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
