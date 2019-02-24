namespace ScottLane.DataTidy.Client.Forms
{
    partial class PerformanceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformanceForm));
            this.performanceChart = new LiveCharts.WinForms.CartesianChart();
            this.SuspendLayout();
            // 
            // performanceChart
            // 
            this.performanceChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.performanceChart.Location = new System.Drawing.Point(0, 0);
            this.performanceChart.Name = "performanceChart";
            this.performanceChart.Size = new System.Drawing.Size(1494, 253);
            this.performanceChart.TabIndex = 0;
            this.performanceChart.Text = "cartesianChart1";
            // 
            // PerformanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1494, 253);
            this.Controls.Add(this.performanceChart);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PerformanceForm";
            this.Text = "Performance";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PerformanceForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private LiveCharts.WinForms.CartesianChart performanceChart;
    }
}