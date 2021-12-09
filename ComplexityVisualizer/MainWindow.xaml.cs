using Data.Enums;
using LiveCharts;
using LiveCharts.Wpf;
using Logic;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ComplexityVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var intWorkers = CreateWorkers<int>();

            //var intTimes = new List<int>();
            //var stringTimes = new List<int>();
            //var doubleTimes = new List<int>();
            //var longTimes = new List<int>();

            listChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Any",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Max",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Min",
                },
            };
            listChart.LegendLocation = LegendLocation.Top;

            enumChart.Series = new SeriesCollection
            {
               new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Any",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Max",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Min",
                },
            };
            enumChart.LegendLocation = LegendLocation.Top;

            hashSetChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Any",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Max",
                },
                new LineSeries
                {
                    Values = new ChartValues<long>(),
                    Title = "Min",
                },
            };
            hashSetChart.LegendLocation = LegendLocation.Top;


            Task.Run(() =>
            {
                int counter = 0;
                // Repeat until all values ​​are repeated 3 times
                while (counter < 100)
                {
                    var intListTuple = MeasureTime(intWorkers.Item1);
                    var intEnumTuple = MeasureTime(intWorkers.Item1);
                    var intHashSetTuple = MeasureTime(intWorkers.Item1);

                    listChart.Series[0].Values.Add(intListTuple.Item1);
                    listChart.Series[1].Values.Add(intListTuple.Item2);
                    listChart.Series[2].Values.Add(intListTuple.Item3);

                    enumChart.Series[0].Values.Add(intEnumTuple.Item1);
                    enumChart.Series[1].Values.Add(intEnumTuple.Item2);
                    enumChart.Series[2].Values.Add(intEnumTuple.Item3);

                    hashSetChart.Series[0].Values.Add(intHashSetTuple.Item1);
                    hashSetChart.Series[1].Values.Add(intHashSetTuple.Item2);
                    hashSetChart.Series[2].Values.Add(intHashSetTuple.Item3);

                    counter++;

                    // Sleep a bit
                    Thread.Sleep(100);
                }
            });
        }

        public static Tuple<long, long, long> MeasureTime<T>(CollectionWorker<T> worker)
        {
            var stopWatch = Stopwatch.StartNew();

            worker.Any();
            var any = stopWatch.ElapsedTicks;
            worker.Max();
            var min = stopWatch.ElapsedTicks;
            worker.Min();
            var max = stopWatch.ElapsedTicks;

            stopWatch.Stop();

            return new Tuple<long, long, long>(any, min, max);
        }

        public static Tuple<CollectionWorker<T>, CollectionWorker<T>, CollectionWorker<T>> CreateWorkers<T>()
        {
            var list = new CollectionWorker<T>(EnumerableType.List, CollectionSize.Small);
            var enumerable = new CollectionWorker<T>(EnumerableType.IEnumerable, CollectionSize.Small);
            var hashSet = new CollectionWorker<T>(EnumerableType.HashSet, CollectionSize.Small);

            return new Tuple<CollectionWorker<T>, CollectionWorker<T>, CollectionWorker<T>>(list, enumerable, hashSet);
        }
    }

}
