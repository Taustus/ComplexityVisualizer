using Data.Enums;
using LiveCharts;
using LiveCharts.Wpf;
using Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComplexityVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsWorking { get; set; } = true;

        private Task _mainTask;
        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();

            ChartAny.Series = CreateSeriesCollection();
            ChartMin.Series = CreateSeriesCollection();
            ChartMax.Series = CreateSeriesCollection();

            InitMainTask();
        }

        private void InitMainTask()
        {
            _cts = new();

            var (list, enumerable, hashSet) = CreateWorkers<int>();

            _mainTask = new Task(() =>
            {
                try
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        var intAnyTuple = MeasureAny(list, hashSet, enumerable);
                        var intMinTuple = MeasureMin(list, hashSet, enumerable);
                        var intMaxTuple = MeasureMax(list, hashSet, enumerable);

                        AddTupleValuesToSeries(ChartAny.Series, intAnyTuple);
                        AddTupleValuesToSeries(ChartMin.Series, intMinTuple);
                        AddTupleValuesToSeries(ChartMax.Series, intMaxTuple);

                        // Sleep a bit
                        Thread.Sleep(100);
                    }
                }
                catch (TaskCanceledException)
                {

                }
            }, _cts.Token);

            _mainTask.Start();
        }

        private static void AddTupleValuesToSeries(SeriesCollection series, (long f, long s, long t) tuple)
        {
            static void addValue(IChartValues values, long newValue)
            {
                values.Add(newValue);

                if (values.Count > 15)
                {
                    values.RemoveAt(0);
                }
            }
            addValue(series[0].Values, tuple.f);
            addValue(series[1].Values, tuple.s);
            addValue(series[2].Values, tuple.t);
        }

        private static SeriesCollection CreateSeriesCollection()
        {
            return new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "List",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "HashSet",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Enumerable",
                },
            };
        }

        public static (long anyList, long anyHashSet, long anyEnumerable) MeasureAny<T>(CollectionWorker<T> list,
                                                                                        CollectionWorker<T> hashSet,
                                                                                        CollectionWorker<T> enumerable)
        {
            var stopWatch = Stopwatch.StartNew();

            list.Any();
            var anyList = stopWatch.ElapsedTicks;
            hashSet.Any();
            var anyHashSet = stopWatch.ElapsedTicks;
            enumerable.Any();
            var anyEnumerable = stopWatch.ElapsedTicks;

            stopWatch.Stop();

            return new(anyList, anyHashSet, anyEnumerable);
        }

        public static (long maxList, long maxHashSet, long maxEnumerable) MeasureMax<T>(CollectionWorker<T> list,
                                                                                        CollectionWorker<T> hashSet,
                                                                                        CollectionWorker<T> enumerable)
        {
            var stopWatch = Stopwatch.StartNew();

            list.Max();
            var maxList = stopWatch.ElapsedTicks;
            hashSet.Max();
            var maxHashSet = stopWatch.ElapsedTicks;
            enumerable.Max();
            var maxEnumerable = stopWatch.ElapsedTicks;

            stopWatch.Stop();

            return new(maxList, maxHashSet, maxEnumerable);
        }

        public static (long minList, long minHashSet, long minEnumerable) MeasureMin<T>(CollectionWorker<T> list,
                                                                                        CollectionWorker<T> hashSet,
                                                                                        CollectionWorker<T> enumerable)
        {
            var stopWatch = Stopwatch.StartNew();

            list.Min();
            var minList = stopWatch.ElapsedTicks;
            hashSet.Min();
            var minHashSet = stopWatch.ElapsedTicks;
            enumerable.Min();
            var minEnumerable = stopWatch.ElapsedTicks;

            stopWatch.Stop();

            return new(minList, minHashSet, minEnumerable);
        }

        public static (CollectionWorker<T> list, CollectionWorker<T> enumerable, CollectionWorker<T> hashSet) CreateWorkers<T>()
        {
            var list = new CollectionWorker<T>(EnumerableType.List, CollectionSize.Small);
            var enumerable = new CollectionWorker<T>(EnumerableType.IEnumerable, CollectionSize.Small);
            var hashSet = new CollectionWorker<T>(EnumerableType.HashSet, CollectionSize.Small);

            return new(list, enumerable, hashSet);
        }

        private void ButtonStartPlay_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (IsWorking)
            {
                button.Content = "Start";
                _cts.Cancel();
                _cts.Dispose();
            }
            else
            {
                button.Content = "Stop";
                InitMainTask();
            }

            IsWorking = !IsWorking;
        }
    }
}
