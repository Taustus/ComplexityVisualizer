using Logic.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseUrls("http://*:5000", "https://*:5001");

// Add services to the container.
var configuration = builder.Configuration;
var services = builder.Services;
services.ConfigureStatisticsLogic(configuration);

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

app.MapControllers();
app.UseCors();
app.Run();