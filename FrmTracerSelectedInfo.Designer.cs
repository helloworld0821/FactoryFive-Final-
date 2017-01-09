namespace DXApplication1
{
    partial class FrmTracerSelectedInfo
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblSJDY = new DevExpress.XtraEditors.LabelControl();
            this.txtSJDY = new DevExpress.XtraEditors.TextEdit();
            this.btnSJDY = new DevExpress.XtraEditors.SimpleButton();
            this.lblOpenJHDY = new DevExpress.XtraEditors.LabelControl();
            this.lblOpenCWDY = new DevExpress.XtraEditors.LabelControl();
            this.lblOpenTracerFile = new DevExpress.XtraEditors.LabelControl();
            this.txtOpenTracerFile = new DevExpress.XtraEditors.TextEdit();
            this.btnOpenJHDY = new DevExpress.XtraEditors.SimpleButton();
            this.btnOpenTracerFile = new DevExpress.XtraEditors.SimpleButton();
            this.txtOpenJHDY = new DevExpress.XtraEditors.TextEdit();
            this.txtOpenCWDY = new DevExpress.XtraEditors.TextEdit();
            this.btnOpenCWDY = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSubmit = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::DXApplication1.WaitForm1), true, true);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSJDY.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenTracerFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenJHDY.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenCWDY.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.lblSJDY);
            this.panelControl1.Controls.Add(this.txtSJDY);
            this.panelControl1.Controls.Add(this.btnSJDY);
            this.panelControl1.Controls.Add(this.lblOpenJHDY);
            this.panelControl1.Controls.Add(this.lblOpenCWDY);
            this.panelControl1.Controls.Add(this.lblOpenTracerFile);
            this.panelControl1.Controls.Add(this.txtOpenTracerFile);
            this.panelControl1.Controls.Add(this.btnOpenJHDY);
            this.panelControl1.Controls.Add(this.btnOpenTracerFile);
            this.panelControl1.Controls.Add(this.txtOpenJHDY);
            this.panelControl1.Controls.Add(this.txtOpenCWDY);
            this.panelControl1.Controls.Add(this.btnOpenCWDY);
            this.panelControl1.Location = new System.Drawing.Point(44, 34);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(589, 336);
            this.panelControl1.TabIndex = 0;
            // 
            // lblSJDY
            // 
            this.lblSJDY.Location = new System.Drawing.Point(39, 99);
            this.lblSJDY.Name = "lblSJDY";
            this.lblSJDY.Size = new System.Drawing.Size(114, 14);
            this.lblSJDY.TabIndex = 20;
            this.lblSJDY.Text = "示踪剂对应水井(.txt)";
            // 
            // txtSJDY
            // 
            this.txtSJDY.Enabled = false;
            this.txtSJDY.Location = new System.Drawing.Point(113, 130);
            this.txtSJDY.Name = "txtSJDY";
            this.txtSJDY.Size = new System.Drawing.Size(349, 20);
            this.txtSJDY.TabIndex = 19;
            // 
            // btnSJDY
            // 
            this.btnSJDY.Location = new System.Drawing.Point(474, 129);
            this.btnSJDY.Name = "btnSJDY";
            this.btnSJDY.Size = new System.Drawing.Size(75, 23);
            this.btnSJDY.TabIndex = 18;
            this.btnSJDY.Text = "打开";
            this.btnSJDY.Click += new System.EventHandler(this.btnSJDY_Click);
            // 
            // lblOpenJHDY
            // 
            this.lblOpenJHDY.Location = new System.Drawing.Point(39, 170);
            this.lblOpenJHDY.Name = "lblOpenJHDY";
            this.lblOpenJHDY.Size = new System.Drawing.Size(195, 14);
            this.lblOpenJHDY.TabIndex = 17;
            this.lblOpenJHDY.Text = "示踪剂与实际层位对应文件选择(.xls)";
            // 
            // lblOpenCWDY
            // 
            this.lblOpenCWDY.Location = new System.Drawing.Point(39, 256);
            this.lblOpenCWDY.Name = "lblOpenCWDY";
            this.lblOpenCWDY.Size = new System.Drawing.Size(195, 14);
            this.lblOpenCWDY.TabIndex = 16;
            this.lblOpenCWDY.Text = "示踪剂与实际井号对应文件选择(.xls)";
            // 
            // lblOpenTracerFile
            // 
            this.lblOpenTracerFile.Location = new System.Drawing.Point(39, 27);
            this.lblOpenTracerFile.Name = "lblOpenTracerFile";
            this.lblOpenTracerFile.Size = new System.Drawing.Size(114, 14);
            this.lblOpenTracerFile.TabIndex = 15;
            this.lblOpenTracerFile.Text = "示踪剂文件选择(.prt)";
            // 
            // txtOpenTracerFile
            // 
            this.txtOpenTracerFile.Enabled = false;
            this.txtOpenTracerFile.Location = new System.Drawing.Point(113, 60);
            this.txtOpenTracerFile.Name = "txtOpenTracerFile";
            this.txtOpenTracerFile.Size = new System.Drawing.Size(349, 20);
            this.txtOpenTracerFile.TabIndex = 11;
            // 
            // btnOpenJHDY
            // 
            this.btnOpenJHDY.Location = new System.Drawing.Point(474, 288);
            this.btnOpenJHDY.Name = "btnOpenJHDY";
            this.btnOpenJHDY.Size = new System.Drawing.Size(75, 23);
            this.btnOpenJHDY.TabIndex = 14;
            this.btnOpenJHDY.Text = "打开";
            this.btnOpenJHDY.Click += new System.EventHandler(this.btnOpenJHDY_Click);
            // 
            // btnOpenTracerFile
            // 
            this.btnOpenTracerFile.Location = new System.Drawing.Point(474, 59);
            this.btnOpenTracerFile.Name = "btnOpenTracerFile";
            this.btnOpenTracerFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenTracerFile.TabIndex = 9;
            this.btnOpenTracerFile.Text = "打开";
            this.btnOpenTracerFile.Click += new System.EventHandler(this.btnOpenTracerFile_Click);
            // 
            // txtOpenJHDY
            // 
            this.txtOpenJHDY.Enabled = false;
            this.txtOpenJHDY.Location = new System.Drawing.Point(113, 289);
            this.txtOpenJHDY.Name = "txtOpenJHDY";
            this.txtOpenJHDY.Size = new System.Drawing.Size(349, 20);
            this.txtOpenJHDY.TabIndex = 13;
            // 
            // txtOpenCWDY
            // 
            this.txtOpenCWDY.Enabled = false;
            this.txtOpenCWDY.Location = new System.Drawing.Point(113, 201);
            this.txtOpenCWDY.Name = "txtOpenCWDY";
            this.txtOpenCWDY.Size = new System.Drawing.Size(349, 20);
            this.txtOpenCWDY.TabIndex = 12;
            // 
            // btnOpenCWDY
            // 
            this.btnOpenCWDY.Location = new System.Drawing.Point(474, 200);
            this.btnOpenCWDY.Name = "btnOpenCWDY";
            this.btnOpenCWDY.Size = new System.Drawing.Size(75, 23);
            this.btnOpenCWDY.TabIndex = 10;
            this.btnOpenCWDY.Text = "打开";
            this.btnOpenCWDY.Click += new System.EventHandler(this.btnOpenCWDY_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(535, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(431, 376);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 25);
            this.btnSubmit.TabIndex = 9;
            this.btnSubmit.Text = "确定";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // FrmTracerSelectedInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 422);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmTracerSelectedInfo";
            this.Text = "示踪剂文件选择";
            this.Load += new System.EventHandler(this.FrmTracerSelectedInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSJDY.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenTracerFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenJHDY.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenCWDY.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblOpenJHDY;
        private DevExpress.XtraEditors.LabelControl lblOpenCWDY;
        private DevExpress.XtraEditors.LabelControl lblOpenTracerFile;
        private DevExpress.XtraEditors.TextEdit txtOpenTracerFile;
        private DevExpress.XtraEditors.SimpleButton btnOpenJHDY;
        private DevExpress.XtraEditors.SimpleButton btnOpenTracerFile;
        private DevExpress.XtraEditors.TextEdit txtOpenJHDY;
        private DevExpress.XtraEditors.TextEdit txtOpenCWDY;
        private DevExpress.XtraEditors.SimpleButton btnOpenCWDY;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnSubmit;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.LabelControl lblSJDY;
        private DevExpress.XtraEditors.TextEdit txtSJDY;
        private DevExpress.XtraEditors.SimpleButton btnSJDY;
    }
}