namespace MCHAPP
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.bnInit = new System.Windows.Forms.Button();
            this.bnOpen = new System.Windows.Forms.Button();
            this.bnEnroll = new System.Windows.Forms.Button();
            this.bnVerify = new System.Windows.Forms.Button();
            this.bnFree = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.bnIdentify = new System.Windows.Forms.Button();
            this.textRes = new System.Windows.Forms.TextBox();
            this.picFPImg = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbIdx = new System.Windows.Forms.ComboBox();
            this.lblName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnChangeFingerPrint = new System.Windows.Forms.Button();
            this.lblHealthFacilityName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picFPImg)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnInit
            // 
            this.bnInit.Location = new System.Drawing.Point(33, 124);
            this.bnInit.Margin = new System.Windows.Forms.Padding(6);
            this.bnInit.Name = "bnInit";
            this.bnInit.Size = new System.Drawing.Size(274, 76);
            this.bnInit.TabIndex = 0;
            this.bnInit.Text = "Initialize";
            this.bnInit.UseVisualStyleBackColor = true;
            this.bnInit.Click += new System.EventHandler(this.bnInit_Click);
            // 
            // bnOpen
            // 
            this.bnOpen.Enabled = false;
            this.bnOpen.Location = new System.Drawing.Point(383, 124);
            this.bnOpen.Margin = new System.Windows.Forms.Padding(6);
            this.bnOpen.Name = "bnOpen";
            this.bnOpen.Size = new System.Drawing.Size(274, 76);
            this.bnOpen.TabIndex = 1;
            this.bnOpen.Text = "Open";
            this.bnOpen.UseVisualStyleBackColor = true;
            this.bnOpen.Click += new System.EventHandler(this.bnOpen_Click);
            // 
            // bnEnroll
            // 
            this.bnEnroll.Enabled = false;
            this.bnEnroll.Location = new System.Drawing.Point(33, 237);
            this.bnEnroll.Margin = new System.Windows.Forms.Padding(6);
            this.bnEnroll.Name = "bnEnroll";
            this.bnEnroll.Size = new System.Drawing.Size(274, 73);
            this.bnEnroll.TabIndex = 2;
            this.bnEnroll.Text = "Enrol";
            this.bnEnroll.UseVisualStyleBackColor = true;
            this.bnEnroll.Click += new System.EventHandler(this.bnEnroll_Click);
            // 
            // bnVerify
            // 
            this.bnVerify.Enabled = false;
            this.bnVerify.Location = new System.Drawing.Point(383, 237);
            this.bnVerify.Margin = new System.Windows.Forms.Padding(6);
            this.bnVerify.Name = "bnVerify";
            this.bnVerify.Size = new System.Drawing.Size(274, 73);
            this.bnVerify.TabIndex = 3;
            this.bnVerify.Text = "Verify";
            this.bnVerify.UseVisualStyleBackColor = true;
            this.bnVerify.Click += new System.EventHandler(this.bnVerify_Click);
            // 
            // bnFree
            // 
            this.bnFree.Enabled = false;
            this.bnFree.Location = new System.Drawing.Point(232, 916);
            this.bnFree.Margin = new System.Windows.Forms.Padding(6);
            this.bnFree.Name = "bnFree";
            this.bnFree.Size = new System.Drawing.Size(150, 48);
            this.bnFree.TabIndex = 4;
            this.bnFree.Text = "Finalize";
            this.bnFree.UseVisualStyleBackColor = true;
            this.bnFree.Visible = false;
            this.bnFree.Click += new System.EventHandler(this.bnFree_Click);
            // 
            // bnClose
            // 
            this.bnClose.Enabled = false;
            this.bnClose.Location = new System.Drawing.Point(383, 356);
            this.bnClose.Margin = new System.Windows.Forms.Padding(6);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(274, 78);
            this.bnClose.TabIndex = 5;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // bnIdentify
            // 
            this.bnIdentify.Enabled = false;
            this.bnIdentify.Location = new System.Drawing.Point(60, 916);
            this.bnIdentify.Margin = new System.Windows.Forms.Padding(6);
            this.bnIdentify.Name = "bnIdentify";
            this.bnIdentify.Size = new System.Drawing.Size(150, 48);
            this.bnIdentify.TabIndex = 6;
            this.bnIdentify.Text = "Identiy";
            this.bnIdentify.UseVisualStyleBackColor = true;
            this.bnIdentify.Visible = false;
            this.bnIdentify.Click += new System.EventHandler(this.bnIdentify_Click);
            // 
            // textRes
            // 
            this.textRes.Cursor = System.Windows.Forms.Cursors.Default;
            this.textRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textRes.Location = new System.Drawing.Point(33, 564);
            this.textRes.Margin = new System.Windows.Forms.Padding(6);
            this.textRes.Multiline = true;
            this.textRes.Name = "textRes";
            this.textRes.ReadOnly = true;
            this.textRes.Size = new System.Drawing.Size(648, 219);
            this.textRes.TabIndex = 7;
            // 
            // picFPImg
            // 
            this.picFPImg.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.picFPImg.Location = new System.Drawing.Point(853, 124);
            this.picFPImg.Margin = new System.Windows.Forms.Padding(6);
            this.picFPImg.Name = "picFPImg";
            this.picFPImg.Size = new System.Drawing.Size(600, 670);
            this.picFPImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFPImg.TabIndex = 8;
            this.picFPImg.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(627, 939);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 26);
            this.label1.TabIndex = 9;
            this.label1.Text = "Index:";
            this.label1.Visible = false;
            // 
            // cmbIdx
            // 
            this.cmbIdx.FormattingEnabled = true;
            this.cmbIdx.Location = new System.Drawing.Point(467, 931);
            this.cmbIdx.Margin = new System.Windows.Forms.Padding(6);
            this.cmbIdx.Name = "cmbIdx";
            this.cmbIdx.Size = new System.Drawing.Size(76, 33);
            this.cmbIdx.TabIndex = 10;
            this.cmbIdx.Visible = false;
            this.cmbIdx.SelectedIndexChanged += new System.EventHandler(this.cmbIdx_SelectedIndexChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(259, 55);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(150, 46);
            this.lblName.TabIndex = 11;
            this.lblName.Text = "Mother";
            this.lblName.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.btnChangeFingerPrint);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.cmbIdx);
            this.groupBox1.Controls.Add(this.picFPImg);
            this.groupBox1.Controls.Add(this.bnFree);
            this.groupBox1.Controls.Add(this.bnIdentify);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bnInit);
            this.groupBox1.Controls.Add(this.textRes);
            this.groupBox1.Controls.Add(this.bnOpen);
            this.groupBox1.Controls.Add(this.bnEnroll);
            this.groupBox1.Controls.Add(this.bnClose);
            this.groupBox1.Controls.Add(this.bnVerify);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(46, 115);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1502, 1013);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Finger Prints";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnChangeFingerPrint
            // 
            this.btnChangeFingerPrint.Enabled = false;
            this.btnChangeFingerPrint.Location = new System.Drawing.Point(33, 356);
            this.btnChangeFingerPrint.Margin = new System.Windows.Forms.Padding(6);
            this.btnChangeFingerPrint.Name = "btnChangeFingerPrint";
            this.btnChangeFingerPrint.Size = new System.Drawing.Size(274, 73);
            this.btnChangeFingerPrint.TabIndex = 12;
            this.btnChangeFingerPrint.Text = "Change Fingerprints";
            this.btnChangeFingerPrint.UseVisualStyleBackColor = true;
            this.btnChangeFingerPrint.Click += new System.EventHandler(this.btnChangeFingerPrint_Click);
            // 
            // lblHealthFacilityName
            // 
            this.lblHealthFacilityName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblHealthFacilityName.AutoSize = true;
            this.lblHealthFacilityName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHealthFacilityName.ForeColor = System.Drawing.Color.SandyBrown;
            this.lblHealthFacilityName.Location = new System.Drawing.Point(491, 57);
            this.lblHealthFacilityName.Name = "lblHealthFacilityName";
            this.lblHealthFacilityName.Size = new System.Drawing.Size(158, 55);
            this.lblHealthFacilityName.TabIndex = 13;
            this.lblHealthFacilityName.Text = "label2";
            this.lblHealthFacilityName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1697, 1273);
            this.Controls.Add(this.lblHealthFacilityName);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "MCHMIS";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picFPImg)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnInit;
        private System.Windows.Forms.Button bnOpen;
        private System.Windows.Forms.Button bnEnroll;
        private System.Windows.Forms.Button bnVerify;
        private System.Windows.Forms.Button bnFree;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Button bnIdentify;
        private System.Windows.Forms.TextBox textRes;
        private System.Windows.Forms.PictureBox picFPImg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbIdx;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblHealthFacilityName;
        private System.Windows.Forms.Button btnChangeFingerPrint;
    }
}

