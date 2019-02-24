using ScottLane.DataTidy.Core;
using ScottLane.DataTidy.Client.Model;

namespace ScottLane.DataTidy.Client.Forms
{
    public partial class ProgressForm : ChildForm
    {
        int operationCount;
        int operationIndex;

        public ProgressForm()
        {
            InitializeComponent();
            InitialiseGauges();
            ResetGuages(true);

            ApplicationState.Default.AsyncProcessStarted += Current_AsyncProcessStarted;
            ApplicationState.Default.AsyncProgressChanged += Current_AsyncProgressChanged;
        }

        /// <summary>
        /// https://material.io/guidelines/style/color.html#color-color-palette
        /// </summary>
        private void InitialiseGauges()
        {
            extractSolidGauge.Uses360Mode = false;
            extractSolidGauge.LabelFormatter = x => string.Format("{0:n0}", x);
            extractSolidGauge.FromColor = System.Windows.Media.Color.FromArgb(255, 229, 115, 115);  // red
            extractSolidGauge.ToColor = System.Windows.Media.Color.FromArgb(255, 244, 67, 54);

            transformSolidGauge.Uses360Mode = false;
            transformSolidGauge.LabelFormatter = x => string.Format("{0:n0}", x);
            transformSolidGauge.FromColor = System.Windows.Media.Color.FromArgb(255, 255, 213, 79);    // amber
            transformSolidGauge.ToColor = System.Windows.Media.Color.FromArgb(255, 255, 193, 7);

            loadSolidGauge.Uses360Mode = false;
            loadSolidGauge.LabelFormatter = x => string.Format("{0:n0}", x);
            loadSolidGauge.FromColor = System.Windows.Media.Color.FromArgb(255, 100, 181, 246); // blue
            loadSolidGauge.ToColor = System.Windows.Media.Color.FromArgb(255, 33, 150, 243);

            overallSolidGauge.Uses360Mode = false;
            overallSolidGauge.LabelFormatter = x => string.Format("{0:p0}", x);
            overallSolidGauge.FromColor = System.Windows.Media.Color.FromArgb(255, 129, 199, 132);  // green
            overallSolidGauge.ToColor = System.Windows.Media.Color.FromArgb(255, 76, 175, 80);
        }

        private void ResetGuages(bool resetOverallGauge)
        {
            extractSolidGauge.Value = 0;
            extractSolidGauge.To = 1;
            transformSolidGauge.Value = 0;
            transformSolidGauge.To = 1;
            loadSolidGauge.Value = 0;
            loadSolidGauge.To = 1;

            if (resetOverallGauge)
            {
                overallSolidGauge.Value = 0;
                overallSolidGauge.To = 1;
            }
        }

        private void Current_AsyncProcessStarted(object sender, AsyncEventArgs e)
        {
            operationCount = e.OperationCount;
            operationIndex = 0;
            ResetGuages(true);
        }

        private void Current_AsyncProgressChanged(object sender, ProgressEventArgs e)
        {
            // Sample timing: Extract: 17 sec, Transform: 2 sec, Load: 280 sec => 0.05, 0.001, 93 %
            if (e.Progress.ProgressType == ProgressType.ItemProgress)
            {
                if (e.Progress.ExecutionStage == ExecutionStage.Extract)
                {
                    extractSolidGauge.To = e.Progress.TotalItemCount == 0 ? 1 : e.Progress.TotalItemCount;
                    extractSolidGauge.Value = e.Progress.ExecutedItemCount;
                }
                else if (e.Progress.ExecutionStage == ExecutionStage.Transform)
                {
                    transformSolidGauge.To = e.Progress.TotalItemCount == 0 ? 1 : e.Progress.TotalItemCount;
                    transformSolidGauge.Value = e.Progress.ExecutedItemCount;
                }
                if (e.Progress.ExecutionStage == ExecutionStage.Load)
                {
                    loadSolidGauge.To = e.Progress.TotalItemCount == 0 ? 1 : e.Progress.TotalItemCount;
                    loadSolidGauge.Value = e.Progress.ExecutedItemCount;

                    if (operationCount > 0 && e.Progress.TotalItemCount > 0)
                    {
                        double value = (operationIndex / (double)operationCount) + ((e.Progress.ExecutedItemCount) / (double)e.Progress.TotalItemCount / operationCount);
                        overallSolidGauge.Value = value > 1 ? 1 : value;
                    }
                }
            }
            else if (e.Progress.ProgressType == ProgressType.OperationProgress)
            {
                if (operationIndex != e.Progress.OperationIndex)
                {
                    operationIndex = e.Progress.OperationIndex;
                    ResetGuages(false);
                }
            }
        }

        private void ProgressForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            ApplicationState.Default.AsyncProgressChanged -= Current_AsyncProgressChanged;
            ApplicationState.Default.AsyncProcessStarted -= Current_AsyncProcessStarted;
        }
    }
}