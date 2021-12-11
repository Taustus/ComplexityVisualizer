using Logic.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class HostedService : IHostedService, IDisposable
    {
        protected readonly ILogger _logger;
        protected readonly IStatisticsService _statisticsService;

        public HostedService(ILogger<HostedService> logger,
                             IStatisticsService statisticsService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _statisticsService.RunTasks();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
