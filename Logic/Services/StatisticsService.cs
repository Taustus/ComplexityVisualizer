﻿using Data;
using Data.Enums;
using Logic.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Logic
{
    public class StatisticsService : IStatisticsService
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;

        protected readonly ProcessTime _processTime;
        protected readonly CollectionSize _collectionSize;
        protected readonly int _delayInMiliseconds;

        public StatisticsService(IServiceScopeFactory ServiceScopeFactory,
                                 IOptions<StatisticsSettings> StatisticsSettings)
        {
            _serviceScopeFactory = ServiceScopeFactory;
            _processTime = StatisticsSettings.Value.ProcessTime;
            _collectionSize = StatisticsSettings.Value.CollectionSize;

            SetUpTasks();
        }

        private void SetUpTasks()
        {
            Task[] tasks = GenerateTasks();

            Task.WaitAny(tasks);
        }

        private Task[] GenerateTasks()
        {
            var timeToWork = TimeSpan.FromHours((int)_processTime);

            Task[] tasks = new[]
            {
                CreateNewStatisticsTask(timeToWork, EnumerableType.IEnumerable),
                CreateNewStatisticsTask(timeToWork, EnumerableType.List),
                CreateNewStatisticsTask(timeToWork, EnumerableType.Array),
                CreateNewStatisticsTask(timeToWork, EnumerableType.HashSet)
            };

            return tasks;
        }

        private Task CreateNewStatisticsTask(TimeSpan timeToWork, EnumerableType enumerableType)
        {
            CollectionWorker<int> collectionWorker = new(enumerableType, _collectionSize);

            Task collectionWorkerTask = CollectionWorkerTask(collectionWorker, timeToWork);

            return collectionWorkerTask;
        }

        private async Task CollectionWorkerTask<T>(CollectionWorker<T> collectionWorker, TimeSpan timeToWork)
        {
            List<StatisticsModel> min = new();
            List<StatisticsModel> max = new();
            List<StatisticsModel> any = new();

            Stopwatch stopWatch = new();
            var timeAsTicks = timeToWork.Ticks;

            long passedTicks = 0;
            int delay = _delayInMiliseconds;
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
            var repository = GetRepository();

            repository.AddRange(union);
        }

        private List<StatisticsModel> GetDataFromDb()
        {
            var repository = GetRepository();
            var dataAsList = repository.GetAll().ToList();

            return dataAsList;
        }

        private static void ClearLists(List<StatisticsModel> min, List<StatisticsModel> max, List<StatisticsModel> any)
        {
            min.Clear();
            max.Clear();
            any.Clear();
        }

        private StatisticsRepository GetRepository()
        {
            var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetService<StatisticsRepository>();

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
