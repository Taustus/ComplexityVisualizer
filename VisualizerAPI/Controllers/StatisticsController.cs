using Data.Enums;
using Logic.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace VisualizerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(IStatisticsService statisticsService,
                                    ILogger<StatisticsController> logger)
        {
            _statisticsService = statisticsService;
            _logger = logger;

            _logger.LogInformation($"Logger was succesfully initialized at {nameof(StatisticsController)}");
        }

        [HttpGet("GetMedian")]
        public float GetMedian(string enumerableType, string method)
        {
            _logger.LogInformation("wasd");
            float median = enumerableType switch
            {
                "List" => _statisticsService.GetMedianByType(EnumerableType.List, method),
                "Array" => _statisticsService.GetMedianByType(EnumerableType.Array, method),
                "HashSet" => _statisticsService.GetMedianByType(EnumerableType.HashSet, method),
                "IEnumerable" => _statisticsService.GetMedianByType(EnumerableType.IEnumerable, method),
                _ => throw new NotImplementedException(),
            };

            string message = $"Median of {enumerableType} collection for {method} method is {Math.Round(median, 5)}";
            _logger.LogInformation(message);

            return median;
        }
    }
}