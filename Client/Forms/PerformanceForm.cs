using System;
using ScottLane.DataTidy.Client.Model;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace ScottLane.DataTidy.Client.Forms
{
    public partial class PerformanceForm : ChildForm
    {
        private readonly TimeSpan MINIMUM_UPDATE_DURATION = TimeSpan.FromSeconds(1);

        private DateTime processStartedOn;
        private TimeSpan processDuration;
        private DateTime extractLastUpdatedOn;
        private int previousExecutedItemCount;
        private DateTime previousUpdatedOn;

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        SeriesCollection series;
        LineSeries operationPerformance;

        public PerformanceForm()
        {
            InitializeComponent();
            ResetCounters();

            ApplicationState.Default.AsyncProcessStarted += Current_AsyncProcessStarted;
            ApplicationState.Default.AsyncProgressChanged += Current_AsyncProgressChanged;
        }

        private void ResetCounters()
        {
            processStartedOn = DateTime.MinValue;
            processDuration = TimeSpan.MinValue;
            extractLastUpdatedOn = DateTime.MinValue;
            previousExecutedItemCount = 0;
            previousUpdatedOn = DateTime.MinValue;
            performanceChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Load",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = null,
                    LineSmoothness = 0
                }
            };

            performanceChart.LegendLocation = LegendLocation.None;

            series = new SeriesCollection();
            operationPerformance = new LineSeries()
            {
                Title = "Operations",
                Values = new ChartValues<ObservablePoint>()
            };
            series.Add(operationPerformance);
        }

        private void Current_AsyncProcessStarted(object sender, AsyncEventArgs e)
        {
            ResetCounters();
            processStartedOn = DateTime.Now;
        }

        private void Current_AsyncProgressChanged(object sender, ProgressEventArgs e)
        {
            if (e.Progress.ExecutionStage == Core.ExecutionStage.Load)
            {
                DateTime updatedOn = DateTime.Now;
                TimeSpan durationSincePreviousUpdate = updatedOn - (previousUpdatedOn == DateTime.MinValue ? processStartedOn : previousUpdatedOn);

                if (durationSincePreviousUpdate >= MINIMUM_UPDATE_DURATION)
                {
                    int itemCount = e.Progress.ExecutedItemCount - previousExecutedItemCount;
                    double operationsPerSecond = itemCount / durationSincePreviousUpdate.TotalSeconds;

                    TimeSpan totalDuration = updatedOn - processStartedOn;
                    performanceChart.Series[0].Values.Add(new ObservablePoint(Math.Round(totalDuration.TotalSeconds, 0), Math.Round(operationsPerSecond, 0)));

                    previousExecutedItemCount = e.Progress.ExecutedItemCount;
                    previousUpdatedOn = updatedOn;
                }
            }
        }

        private void PerformanceForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            ApplicationState.Default.AsyncProgressChanged -= Current_AsyncProgressChanged;
            ApplicationState.Default.AsyncProcessStarted -= Current_AsyncProcessStarted;
        }
    }
}
