using Data.Enums;
using Logic;
using Logic.Contracts;
using Logic.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;

namespace VisualizerTestProject
{
    public class StatisticsTests
    {
        protected readonly IStatisticsService _statisticsService;
        protected readonly IOptions<StatisticsSettings> _statisticsSettings;


        public StatisticsTests()
        {
            var services = new ServiceCollection();
            var configuration = InitConfiguration();

            services.ConfigureStatisticsLogic(configuration);

            var serviceProvider = services.BuildServiceProvider();

            _statisticsService = serviceProvider.GetService<IStatisticsService>() ?? 
                                 throw new InvalidOperationException("Couldn't find IStatisticsService!");

            _statisticsSettings = serviceProvider.GetService<IOptions<StatisticsSettings>>() ??
                                  throw new InvalidOperationException("Couldn't find IStatisticsSettings!");

        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                             .AddJsonFile(@"C:\Users\Sergey\source\repos\ComplexityVisualizer\VisualizerAPI\appsettings.json")
                             .Build();

            return config;
        }

        [Fact]
        public void CheckSettings()
        {
            Assert.True(_statisticsSettings.Value.DebugMode);
            Assert.Equal(50, _statisticsSettings.Value.DelayInMilliseconds);
            Assert.Equal(CollectionSize.Small, _statisticsSettings.Value.CollectionSize);
            Assert.Equal(ProcessTime.Long, _statisticsSettings.Value.ProcessTime);
        }
    }
}