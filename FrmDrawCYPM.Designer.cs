namespace DXApplication1
{
    partial class FrmDrawCYPM
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
            this.chartCYPM = new DevExpress.XtraCharts.ChartControl();
            ((System.ComponentModel.ISupportInitialize)(this.chartCYPM)).BeginInit();
            this.SuspendLayout();
            // 
            // chartCYPM
            // 
            this.chartCYPM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartCYPM.Location = new System.Drawing.Point(0, 0);
            this.chartCYPM.Name = "chartCYPM";
            this.chartCYPM.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartCYPM.Size = new System.Drawing.Size(673, 397);
            this.chartCYPM.TabIndex = 0;
            // 
            // FrmDrawCYPM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 397);
            this.Controls.Add(this.chartCYPM);
            this.Name = "FrmDrawCYPM";
            this.Text = "油井产液剖面图";
            this.Load += new System.EventHandler(this.DrawResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartCYPM)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraCharts.ChartControl chartCYPM;
    }
}