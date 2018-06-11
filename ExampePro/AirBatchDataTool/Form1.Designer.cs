namespace AirBatchDataTool
{
    partial class BatchSceduleFrm
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
            this.btnGenerate = new System.Windows.Forms.Button();
            this.dtpTmplBeginTime = new System.Windows.Forms.DateTimePicker();
            this.lblBeginTime = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpTmplEndTime = new System.Windows.Forms.DateTimePicker();
            this.lblNumber = new System.Windows.Forms.Label();
            this.rtbTip = new System.Windows.Forms.RichTextBox();
            this.dtpTargetEndTime = new System.Windows.Forms.DateTimePicker();
            this.lblTargetEndTime = new System.Windows.Forms.Label();
            this.lblTargetBeginTime = new System.Windows.Forms.Label();
            this.dtpTargetBeginTime = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTmplQueryNummber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(103, 128);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "生成";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // dtpTmplBeginTime
            // 
            this.dtpTmplBeginTime.Location = new System.Drawing.Point(103, 18);
            this.dtpTmplBeginTime.Name = "dtpTmplBeginTime";
            this.dtpTmplBeginTime.Size = new System.Drawing.Size(116, 21);
            this.dtpTmplBeginTime.TabIndex = 1;
            // 
            // lblBeginTime
            // 
            this.lblBeginTime.AutoSize = true;
            this.lblBeginTime.Location = new System.Drawing.Point(13, 24);
            this.lblBeginTime.Name = "lblBeginTime";
            this.lblBeginTime.Size = new System.Drawing.Size(77, 12);
            this.lblBeginTime.TabIndex = 2;
            this.lblBeginTime.Text = "模板开始时间";
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(103, 89);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(116, 21);
            this.txtNumber.TabIndex = 3;
            this.txtNumber.Text = "1";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(279, 24);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(17, 12);
            this.lblTo.TabIndex = 4;
            this.lblTo.Text = "至";
            // 
            // dtpTmplEndTime
            // 
            this.dtpTmplEndTime.Location = new System.Drawing.Point(314, 18);
            this.dtpTmplEndTime.Name = "dtpTmplEndTime";
            this.dtpTmplEndTime.Size = new System.Drawing.Size(116, 21);
            this.dtpTmplEndTime.TabIndex = 5;
            // 
            // lblNumber
            // 
            this.lblNumber.AutoSize = true;
            this.lblNumber.Location = new System.Drawing.Point(13, 98);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(71, 12);
            this.lblNumber.TabIndex = 6;
            this.lblNumber.Text = "创建数目/天";
            // 
            // rtbTip
            // 
            this.rtbTip.Location = new System.Drawing.Point(15, 175);
            this.rtbTip.Name = "rtbTip";
            this.rtbTip.Size = new System.Drawing.Size(415, 128);
            this.rtbTip.TabIndex = 7;
            this.rtbTip.Text = "";
            // 
            // dtpTargetEndTime
            // 
            this.dtpTargetEndTime.Location = new System.Drawing.Point(314, 50);
            this.dtpTargetEndTime.Name = "dtpTargetEndTime";
            this.dtpTargetEndTime.Size = new System.Drawing.Size(116, 21);
            this.dtpTargetEndTime.TabIndex = 11;
            // 
            // lblTargetEndTime
            // 
            this.lblTargetEndTime.AutoSize = true;
            this.lblTargetEndTime.Location = new System.Drawing.Point(279, 56);
            this.lblTargetEndTime.Name = "lblTargetEndTime";
            this.lblTargetEndTime.Size = new System.Drawing.Size(17, 12);
            this.lblTargetEndTime.TabIndex = 10;
            this.lblTargetEndTime.Text = "至";
            // 
            // lblTargetBeginTime
            // 
            this.lblTargetBeginTime.AutoSize = true;
            this.lblTargetBeginTime.Location = new System.Drawing.Point(13, 56);
            this.lblTargetBeginTime.Name = "lblTargetBeginTime";
            this.lblTargetBeginTime.Size = new System.Drawing.Size(77, 12);
            this.lblTargetBeginTime.TabIndex = 9;
            this.lblTargetBeginTime.Text = "航班开始时间";
            // 
            // dtpTargetBeginTime
            // 
            this.dtpTargetBeginTime.Location = new System.Drawing.Point(103, 50);
            this.dtpTargetBeginTime.Name = "dtpTargetBeginTime";
            this.dtpTargetBeginTime.Size = new System.Drawing.Size(116, 21);
            this.dtpTargetBeginTime.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(237, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "模板数目/天";
            // 
            // txtTmplQueryNummber
            // 
            this.txtTmplQueryNummber.Location = new System.Drawing.Point(314, 95);
            this.txtTmplQueryNummber.Name = "txtTmplQueryNummber";
            this.txtTmplQueryNummber.Size = new System.Drawing.Size(116, 21);
            this.txtTmplQueryNummber.TabIndex = 13;
            this.txtTmplQueryNummber.Text = "1";
            // 
            // BatchSceduleFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 315);
            this.Controls.Add(this.txtTmplQueryNummber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpTargetEndTime);
            this.Controls.Add(this.lblTargetEndTime);
            this.Controls.Add(this.lblTargetBeginTime);
            this.Controls.Add(this.dtpTargetBeginTime);
            this.Controls.Add(this.rtbTip);
            this.Controls.Add(this.lblNumber);
            this.Controls.Add(this.dtpTmplEndTime);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.txtNumber);
            this.Controls.Add(this.lblBeginTime);
            this.Controls.Add(this.dtpTmplBeginTime);
            this.Controls.Add(this.btnGenerate);
            this.Name = "BatchSceduleFrm";
            this.Text = "批量航班计划";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.DateTimePicker dtpTmplBeginTime;
        private System.Windows.Forms.Label lblBeginTime;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtpTmplEndTime;
        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.RichTextBox rtbTip;
        private System.Windows.Forms.DateTimePicker dtpTargetEndTime;
        private System.Windows.Forms.Label lblTargetEndTime;
        private System.Windows.Forms.Label lblTargetBeginTime;
        private System.Windows.Forms.DateTimePicker dtpTargetBeginTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTmplQueryNummber;
    }
}

