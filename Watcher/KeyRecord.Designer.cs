namespace Watcher
{
    partial class KeyRecord
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyRecord));
            this.txtRecordInfo = new System.Windows.Forms.TextBox();
            this.gpbKeyboard = new System.Windows.Forms.TextBox();
            this.lblMarginLeft = new System.Windows.Forms.Label();
            this.lblMarginTop = new System.Windows.Forms.Label();
            this.lblMarginRight = new System.Windows.Forms.Label();
            this.lblMarginBottom = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.gpbMouseInfo = new System.Windows.Forms.TextBox();
            this.lsvRegistry = new System.Windows.Forms.ListView();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.txtCaption = new System.Windows.Forms.TextBox();
            this.lblXCoor = new System.Windows.Forms.Label();
            this.lblYCoor = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtRecordInfo
            // 
            this.txtRecordInfo.Location = new System.Drawing.Point(396, 12);
            this.txtRecordInfo.Multiline = true;
            this.txtRecordInfo.Name = "txtRecordInfo";
            this.txtRecordInfo.Size = new System.Drawing.Size(242, 304);
            this.txtRecordInfo.TabIndex = 0;
            // 
            // gpbKeyboard
            // 
            this.gpbKeyboard.Location = new System.Drawing.Point(234, 12);
            this.gpbKeyboard.Name = "gpbKeyboard";
            this.gpbKeyboard.Size = new System.Drawing.Size(145, 21);
            this.gpbKeyboard.TabIndex = 1;
            // 
            // lblMarginLeft
            // 
            this.lblMarginLeft.AutoSize = true;
            this.lblMarginLeft.Location = new System.Drawing.Point(12, 158);
            this.lblMarginLeft.Name = "lblMarginLeft";
            this.lblMarginLeft.Size = new System.Drawing.Size(41, 12);
            this.lblMarginLeft.TabIndex = 2;
            this.lblMarginLeft.Text = "label1";
            // 
            // lblMarginTop
            // 
            this.lblMarginTop.AutoSize = true;
            this.lblMarginTop.Location = new System.Drawing.Point(160, 9);
            this.lblMarginTop.Name = "lblMarginTop";
            this.lblMarginTop.Size = new System.Drawing.Size(41, 12);
            this.lblMarginTop.TabIndex = 3;
            this.lblMarginTop.Text = "label1";
            // 
            // lblMarginRight
            // 
            this.lblMarginRight.AutoSize = true;
            this.lblMarginRight.Location = new System.Drawing.Point(266, 157);
            this.lblMarginRight.Name = "lblMarginRight";
            this.lblMarginRight.Size = new System.Drawing.Size(41, 12);
            this.lblMarginRight.TabIndex = 4;
            this.lblMarginRight.Text = "label1";
            // 
            // lblMarginBottom
            // 
            this.lblMarginBottom.AutoSize = true;
            this.lblMarginBottom.Location = new System.Drawing.Point(140, 304);
            this.lblMarginBottom.Name = "lblMarginBottom";
            this.lblMarginBottom.Size = new System.Drawing.Size(41, 12);
            this.lblMarginBottom.TabIndex = 5;
            this.lblMarginBottom.Text = "label1";
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Interval = 10000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // gpbMouseInfo
            // 
            this.gpbMouseInfo.Location = new System.Drawing.Point(234, 56);
            this.gpbMouseInfo.Name = "gpbMouseInfo";
            this.gpbMouseInfo.Size = new System.Drawing.Size(100, 21);
            this.gpbMouseInfo.TabIndex = 6;
            // 
            // lsvRegistry
            // 
            this.lsvRegistry.Location = new System.Drawing.Point(656, 15);
            this.lsvRegistry.Name = "lsvRegistry";
            this.lsvRegistry.Size = new System.Drawing.Size(121, 97);
            this.lsvRegistry.TabIndex = 8;
            this.lsvRegistry.UseCompatibleStateImageBehavior = false;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // txtCaption
            // 
            this.txtCaption.Location = new System.Drawing.Point(14, 12);
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new System.Drawing.Size(117, 21);
            this.txtCaption.TabIndex = 9;
            this.txtCaption.Text = "开始";
            // 
            // lblXCoor
            // 
            this.lblXCoor.AutoSize = true;
            this.lblXCoor.Location = new System.Drawing.Point(422, 338);
            this.lblXCoor.Name = "lblXCoor";
            this.lblXCoor.Size = new System.Drawing.Size(53, 12);
            this.lblXCoor.TabIndex = 2;
            this.lblXCoor.Text = "lblXCoor";
            // 
            // lblYCoor
            // 
            this.lblYCoor.AutoSize = true;
            this.lblYCoor.Location = new System.Drawing.Point(546, 338);
            this.lblYCoor.Name = "lblYCoor";
            this.lblYCoor.Size = new System.Drawing.Size(53, 12);
            this.lblYCoor.TabIndex = 2;
            this.lblYCoor.Text = "lblXCoor";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(783, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(785, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(785, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(785, 241);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(785, 290);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "label5";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(45, 338);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // KeyRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(973, 390);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCaption);
            this.Controls.Add(this.lsvRegistry);
            this.Controls.Add(this.gpbMouseInfo);
            this.Controls.Add(this.lblMarginBottom);
            this.Controls.Add(this.lblMarginRight);
            this.Controls.Add(this.lblMarginTop);
            this.Controls.Add(this.lblYCoor);
            this.Controls.Add(this.lblXCoor);
            this.Controls.Add(this.lblMarginLeft);
            this.Controls.Add(this.gpbKeyboard);
            this.Controls.Add(this.txtRecordInfo);
            this.Name = "KeyRecord";
            this.Text = "Form1";
            this.MinimumSizeChanged += new System.EventHandler(this.KeyRecord_MinimumSizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KeyRecord_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRecordInfo;
        private System.Windows.Forms.TextBox gpbKeyboard;
        private System.Windows.Forms.Label lblMarginLeft;
        private System.Windows.Forms.Label lblMarginTop;
        private System.Windows.Forms.Label lblMarginRight;
        private System.Windows.Forms.Label lblMarginBottom;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.TextBox gpbMouseInfo;
        private System.Windows.Forms.ListView lsvRegistry;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TextBox txtCaption;
        private System.Windows.Forms.Label lblXCoor;
        private System.Windows.Forms.Label lblYCoor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
    }
}

