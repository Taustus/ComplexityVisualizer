using Data;
using Data.Contracts;
using Data.Enums;
using Logic.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Logic
{
    public class StatisticsService : IStatisticsService
    {
        protected readonly IStatisticsRepository _repository;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly StatisticsRepository _statisticsRepository;
        protected readonly ILogger<StatisticsService> _logger;
        protected readonly ProcessTime _processTime;
        protected readonly CollectionSize _collectionSize;
        protected readonly TimeSpan _timeToWorkInMilliseconds;
        protected readonly int _delayInMiliseconds;
        protected readonly bool _debugMode;

        static object _lock;

        public StatisticsService(ILogger<StatisticsService> logger,
                                 IServiceScopeFactory ServiceScopeFactory,
                                 IOptions<StatisticsSettings> StatisticsSettings)
        {
            _logger = logger;
            _serviceScopeFactory = ServiceScopeFactory;
            _repository = GetRepository();

            _processTime = StatisticsSettings.Value.ProcessTime;
            _collectionSize = StatisticsSettings.Value.CollectionSize;
            _debugMode = StatisticsSettings.Value.DebugMode;
            _delayInMiliseconds = StatisticsSettings.Value.DelayInMilliseconds;
            _timeToWorkInMilliseconds = TimeSpan.FromMilliseconds((long)_processTime);

            _lock = new object();

            _logger.LogInformation("StatisticsService start!");
        }

        public void RunTasks()
        {
            Task[] tasks = GenerateTasks();

            foreach (var task in tasks)
            {
                task.Start();
            }
        }

        private Task[] GenerateTasks()
        {
            Task[] tasks = new[]
            {
                CreateNewStatisticsTask(EnumerableType.IEnumerable),
                CreateNewStatisticsTask(EnumerableType.List),
                CreateNewStatisticsTask(EnumerableType.Array),
                CreateNewStatisticsTask(EnumerableType.HashSet)
            };

            return tasks;
        }

        private Task CreateNewStatisticsTask(EnumerableType enumerableType)
        {
            CollectionWorker<int> collectionWorker = new(enumerableType, _collectionSize);

            Task collectionWorkerTask = new(() => CollectionWorkerTask(collectionWorker));

            return collectionWorkerTask;
        }

        private async void CollectionWorkerTask<T>(CollectionWorker<T> collectionWorker)
        {
            List<StatisticsModel> min = new();
            List<StatisticsModel> max = new();
            List<StatisticsModel> any = new();

            Stopwatch stopWatch = new();
            var timeAsTicks = _timeToWorkInMilliseconds.Ticks;

            long passedTicks = 0;
            int delay = _delayInMiliseconds;
            var debugMode = _debugMode;
            var enumerableType = collectionWorker.EnumerableType;

            while (passedTicks < timeAsTicks)
            {
                var minTime = MeasureMethod(new Func<T>(collectionWorker.Min), stopWatch);
                var maxTime = MeasureMethod(new Func<T>(collectionWorker.Max), stopWatch);
                var anyTime = MeasureMethod(new Func<bool>(collectionWorker.Any), stopWatch);

                min.Add(new StatisticsModel(minTime, "min", enumerableType.ToString()));
                max.Add(new StatisticsModel(maxTime, "max", enumerableType.ToString()));
                any.Add(new StatisticsModel(anyTime, "any", enumerableType.ToString()));

                // We don't have to check count of all lists
                // Counts between them are equal
                if (min.Count > 100)
                {
                    AddDataToDb(min, max, any);

                    ClearLists(min, max, any);
                }

                passedTicks += minTime + maxTime + anyTime;

                await Task.Delay(delay);
            }
        }

        private static long MeasureMethod<T>(Func<T> func, Stopwatch sw)
        {
            sw.Restart();
            func.Invoke();
            sw.Stop();

            return sw.ElapsedTicks;
        }

        private void AddDataToDb(List<StatisticsModel> min, List<StatisticsModel> max, List<StatisticsModel> any)
        {
            var union = min.Union(max).Union(any);

            _logger.LogInformation("Add new data to db!");
            
            lock(_lock)
            {
                _repository.AddRange(union);
            }
        }

        private List<StatisticsModel> GetDataFromDb()
        {
            var dataAsList = _repository.GetAll().ToList();

            return dataAsList;
        }

        private static void ClearLists(List<StatisticsModel> min, List<StatisticsModel> max, List<StatisticsModel> any)
        {
            min.Clear();
            max.Clear();
            any.Clear();
        }

        private IStatisticsRepository GetRepository()
        {
            var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetService<IStatisticsRepository>();

            if (repository is not null)
            {
                return repository;
            }
            else
            {
                throw new InvalidOperationException("Couldn't get repository!");
            }
        }

        private static float CalculateMedian(List<StatisticsModel> data, string enumerableTypeAsString)
        {
            float median = data.Where(x => x.EnumerableType == enumerableTypeAsString)
                               // Ex.: (3 + 4) / 2 == 3 / 2 + 4 / 2
                               // We don't want to see overflow here (even if it's nearly possible)
                               .Sum(x => x.Ticks / data.Count);

            return median;
        }

        public float GetMedianByType(EnumerableType enumerableType, string method)
        {
            var data = GetDataFromDb();
            var median = CalculateMedian(data, enumerableType.ToString());

            return median;
        }
    }
}
