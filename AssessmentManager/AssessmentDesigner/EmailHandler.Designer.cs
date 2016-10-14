namespace AssessmentManager
{
    partial class EmailHandler
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
            this.components = new System.ComponentModel.Container();
            this.lblDisplay = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lblSentCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblDisplay
            // 
            this.lblDisplay.AutoSize = true;
            this.lblDisplay.Location = new System.Drawing.Point(119, 23);
            this.lblDisplay.Name = "lblDisplay";
            this.lblDisplay.Size = new System.Drawing.Size(142, 13);
            this.lblDisplay.TabIndex = 0;
            this.lblDisplay.Text = "Sending email ... Please wait";
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(52, 53);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(276, 23);
            this.progress.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lblSentCount
            // 
            this.lblSentCount.AutoSize = true;
            this.lblSentCount.Location = new System.Drawing.Point(175, 90);
            this.lblSentCount.Name = "lblSentCount";
            this.lblSentCount.Size = new System.Drawing.Size(30, 13);
            this.lblSentCount.TabIndex = 2;
            this.lblSentCount.Text = "(0/0)";
            // 
            // EmailHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 122);
            this.Controls.Add(this.lblSentCount);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.lblDisplay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EmailHandler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EmailHandler";
            this.Shown += new System.EventHandler(this.EmailHandler_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDisplay;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblSentCount;
    }
}