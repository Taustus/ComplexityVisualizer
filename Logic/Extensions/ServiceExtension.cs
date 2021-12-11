using Data;
using Data.Contracts;
using Logic.Contracts;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureStatisticsLogic(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MySqlContext>(options => options.UseMySQL(configuration.GetSection("ConnectionStrings")["MySQL"]));
            services.Configure<StatisticsSettings>(configuration.GetSection("StatisticsSettings"));
            services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddHostedService<HostedService>();
        }
    }
}
