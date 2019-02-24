namespace ScottLane.DataTidy.Client.Forms
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.overallSolidGauge = new LiveCharts.WinForms.SolidGauge();
            this.extractSolidGauge = new LiveCharts.WinForms.SolidGauge();
            this.transformSolidGauge = new LiveCharts.WinForms.SolidGauge();
            this.loadSolidGauge = new LiveCharts.WinForms.SolidGauge();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // overallSolidGauge
            // 
            this.overallSolidGauge.BackColor = System.Drawing.Color.Transparent;
            this.overallSolidGauge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overallSolidGauge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overallSolidGauge.ForeColor = System.Drawing.SystemColors.ControlText;
            this.overallSolidGauge.Location = new System.Drawing.Point(1044, 44);
            this.overallSolidGauge.Name = "overallSolidGauge";
            this.overallSolidGauge.Size = new System.Drawing.Size(343, 158);
            this.overallSolidGauge.TabIndex = 0;
            // 
            // extractSolidGauge
            // 
            this.extractSolidGauge.BackColor = System.Drawing.Color.Transparent;
            this.extractSolidGauge.Cursor = System.Windows.Forms.Cursors.Default;
            this.extractSolidGauge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extractSolidGauge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.extractSolidGauge.Location = new System.Drawing.Point(3, 44);
            this.extractSolidGauge.Name = "extractSolidGauge";
            this.extractSolidGauge.Size = new System.Drawing.Size(341, 158);
            this.extractSolidGauge.TabIndex = 0;
            // 
            // transformSolidGauge
            // 
            this.transformSolidGauge.BackColor = System.Drawing.Color.Transparent;
            this.transformSolidGauge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transformSolidGauge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transformSolidGauge.Location = new System.Drawing.Point(350, 44);
            this.transformSolidGauge.Name = "transformSolidGauge";
            this.transformSolidGauge.Size = new System.Drawing.Size(341, 158);
            this.transformSolidGauge.TabIndex = 0;
            // 
            // loadSolidGauge
            // 
            this.loadSolidGauge.BackColor = System.Drawing.Color.Transparent;
            this.loadSolidGauge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadSolidGauge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadSolidGauge.Location = new System.Drawing.Point(697, 44);
            this.loadSolidGauge.Name = "loadSolidGauge";
            this.loadSolidGauge.Size = new System.Drawing.Size(341, 158);
            this.loadSolidGauge.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.overallSolidGauge, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.extractSolidGauge, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.transformSolidGauge, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.loadSolidGauge, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1390, 205);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(341, 41);
            this.label1.TabIndex = 1;
            this.label1.Text = "Extract";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(350, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(341, 41);
            this.label2.TabIndex = 2;
            this.label2.Text = "Transform";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(697, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(341, 41);
            this.label3.TabIndex = 3;
            this.label3.Text = "Load";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(1044, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(343, 41);
            this.label4.TabIndex = 4;
            this.label4.Text = "Overall";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1390, 205);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProgressForm";
            this.Text = "Progress";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProgressForm_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private LiveCharts.WinForms.SolidGauge overallSolidGauge;
        private LiveCharts.WinForms.SolidGauge extractSolidGauge;
        private LiveCharts.WinForms.SolidGauge transformSolidGauge;
        private LiveCharts.WinForms.SolidGauge loadSolidGauge;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}