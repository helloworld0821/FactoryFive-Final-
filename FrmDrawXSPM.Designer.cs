namespace DXApplication1
{
    partial class FrmDrawXSPM
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
            this.chartXSPM = new DevExpress.XtraCharts.ChartControl();
            ((System.ComponentModel.ISupportInitialize)(this.chartXSPM)).BeginInit();
            this.SuspendLayout();
            // 
            // chartXSPM
            // 
            this.chartXSPM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartXSPM.Location = new System.Drawing.Point(0, 0);
            this.chartXSPM.Name = "chartXSPM";
            this.chartXSPM.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartXSPM.Size = new System.Drawing.Size(673, 397);
            this.chartXSPM.TabIndex = 0;
            // 
            // FrmDrawXSPM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 397);
            this.Controls.Add(this.chartXSPM);
            this.Name = "FrmDrawXSPM";
            this.Text = "水井吸水剖面图";
            this.Load += new System.EventHandler(this.FrmDrawXSPM_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartXSPM)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraCharts.ChartControl chartXSPM;
    }
}