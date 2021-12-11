using Data;
using Data.Contracts;
using Logic.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureStatisticsLogic(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MySqlContext>(options => options.UseMySQL(configuration.GetSection("ConnectionsString")["MySQL"]));
            services.Configure<StatisticsSettings>(configuration.GetSection("StatisticsSettings"));
            services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            services.AddSingleton<IStatisticsService ,StatisticsService>();
        }
    }
}
