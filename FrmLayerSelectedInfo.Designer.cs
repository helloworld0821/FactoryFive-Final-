namespace DXApplication1
{
    partial class FrmLayerSelectedInfo
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
            this.btnOpenSMFile = new DevExpress.XtraEditors.SimpleButton();
            this.btnOpenCWDY = new DevExpress.XtraEditors.SimpleButton();
            this.txtOpenSMFile = new DevExpress.XtraEditors.TextEdit();
            this.txtOpenCWDY = new DevExpress.XtraEditors.TextEdit();
            this.txtOpenJHDY = new DevExpress.XtraEditors.TextEdit();
            this.btnOpenJHDY = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblOpenJHDY = new DevExpress.XtraEditors.LabelControl();
            this.lblOpenCWDY = new DevExpress.XtraEditors.LabelControl();
            this.lblOpenSMFile = new DevExpress.XtraEditors.LabelControl();
            this.btnSubmit = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::DXApplication1.WaitForm1), true, true);
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenSMFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenCWDY.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenJHDY.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenSMFile
            // 
            this.btnOpenSMFile.Location = new System.Drawing.Point(459, 50);
            this.btnOpenSMFile.Name = "btnOpenSMFile";
            this.btnOpenSMFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenSMFile.TabIndex = 0;
            this.btnOpenSMFile.Text = "打开";
            this.btnOpenSMFile.Click += new System.EventHandler(this.btnOpenSMFile_Click);
            // 
            // btnOpenCWDY
            // 
            this.btnOpenCWDY.Location = new System.Drawing.Point(459, 134);
            this.btnOpenCWDY.Name = "btnOpenCWDY";
            this.btnOpenCWDY.Size = new System.Drawing.Size(75, 23);
            this.btnOpenCWDY.TabIndex = 1;
            this.btnOpenCWDY.Text = "打开";
            this.btnOpenCWDY.Click += new System.EventHandler(this.btnOpenCWDY_Click);
            // 
            // txtOpenSMFile
            // 
            this.txtOpenSMFile.Enabled = false;
            this.txtOpenSMFile.Location = new System.Drawing.Point(98, 51);
            this.txtOpenSMFile.Name = "txtOpenSMFile";
            this.txtOpenSMFile.Size = new System.Drawing.Size(349, 20);
            this.txtOpenSMFile.TabIndex = 2;
            // 
            // txtOpenCWDY
            // 
            this.txtOpenCWDY.Enabled = false;
            this.txtOpenCWDY.Location = new System.Drawing.Point(98, 135);
            this.txtOpenCWDY.Name = "txtOpenCWDY";
            this.txtOpenCWDY.Size = new System.Drawing.Size(349, 20);
            this.txtOpenCWDY.TabIndex = 3;
            // 
            // txtOpenJHDY
            // 
            this.txtOpenJHDY.Enabled = false;
            this.txtOpenJHDY.Location = new System.Drawing.Point(98, 223);
            this.txtOpenJHDY.Name = "txtOpenJHDY";
            this.txtOpenJHDY.Size = new System.Drawing.Size(349, 20);
            this.txtOpenJHDY.TabIndex = 4;
            // 
            // btnOpenJHDY
            // 
            this.btnOpenJHDY.Location = new System.Drawing.Point(459, 222);
            this.btnOpenJHDY.Name = "btnOpenJHDY";
            this.btnOpenJHDY.Size = new System.Drawing.Size(75, 23);
            this.btnOpenJHDY.TabIndex = 5;
            this.btnOpenJHDY.Text = "打开";
            this.btnOpenJHDY.Click += new System.EventHandler(this.btnOpenJHDY_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControl1.Controls.Add(this.lblOpenJHDY);
            this.panelControl1.Controls.Add(this.lblOpenCWDY);
            this.panelControl1.Controls.Add(this.lblOpenSMFile);
            this.panelControl1.Controls.Add(this.txtOpenSMFile);
            this.panelControl1.Controls.Add(this.btnOpenJHDY);
            this.panelControl1.Controls.Add(this.btnOpenSMFile);
            this.panelControl1.Controls.Add(this.txtOpenJHDY);
            this.panelControl1.Controls.Add(this.txtOpenCWDY);
            this.panelControl1.Controls.Add(this.btnOpenCWDY);
            this.panelControl1.Location = new System.Drawing.Point(44, 34);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(589, 280);
            this.panelControl1.TabIndex = 6;
            // 
            // lblOpenJHDY
            // 
            this.lblOpenJHDY.Location = new System.Drawing.Point(24, 104);
            this.lblOpenJHDY.Name = "lblOpenJHDY";
            this.lblOpenJHDY.Size = new System.Drawing.Size(183, 14);
            this.lblOpenJHDY.TabIndex = 8;
            this.lblOpenJHDY.Text = "数模与实际层位对应文件选择(.xls)";
            // 
            // lblOpenCWDY
            // 
            this.lblOpenCWDY.Location = new System.Drawing.Point(24, 190);
            this.lblOpenCWDY.Name = "lblOpenCWDY";
            this.lblOpenCWDY.Size = new System.Drawing.Size(183, 14);
            this.lblOpenCWDY.TabIndex = 7;
            this.lblOpenCWDY.Text = "数模与实际井号对应文件选择(.xls)";
            // 
            // lblOpenSMFile
            // 
            this.lblOpenSMFile.Location = new System.Drawing.Point(24, 18);
            this.lblOpenSMFile.Name = "lblOpenSMFile";
            this.lblOpenSMFile.Size = new System.Drawing.Size(102, 14);
            this.lblOpenSMFile.TabIndex = 6;
            this.lblOpenSMFile.Text = "数模文件选择(.prt)";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(422, 333);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 7;
            this.btnSubmit.Text = "确定";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(526, 333);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // FrmInfoChoose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 373);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmInfoChoose";
            this.Text = "数模文件选择";
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenSMFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenCWDY.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenJHDY.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnOpenSMFile;
        private DevExpress.XtraEditors.SimpleButton btnOpenCWDY;
        private DevExpress.XtraEditors.TextEdit txtOpenSMFile;
        private DevExpress.XtraEditors.TextEdit txtOpenCWDY;
        private DevExpress.XtraEditors.TextEdit txtOpenJHDY;
        private DevExpress.XtraEditors.SimpleButton btnOpenJHDY;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSubmit;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl lblOpenJHDY;
        private DevExpress.XtraEditors.LabelControl lblOpenCWDY;
        private DevExpress.XtraEditors.LabelControl lblOpenSMFile;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
    }
}