namespace AssessmentManager
{
    partial class EmailConfigForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkSSL = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbSmtp = new System.Windows.Forms.TextBox();
            this.gbLoginDetails = new System.Windows.Forms.GroupBox();
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbLoginDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Smtp:";
            // 
            // chkSSL
            // 
            this.chkSSL.AutoSize = true;
            this.chkSSL.Checked = true;
            this.chkSSL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSSL.Location = new System.Drawing.Point(203, 123);
            this.chkSSL.Name = "chkSSL";
            this.chkSSL.Size = new System.Drawing.Size(46, 17);
            this.chkSSL.TabIndex = 4;
            this.chkSSL.Text = "SSL";
            this.chkSSL.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(30, 368);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(186, 368);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(82, 23);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(167, 20);
            this.tbUserName.TabIndex = 7;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(82, 56);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(167, 20);
            this.tbPassword.TabIndex = 8;
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(82, 121);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(79, 20);
            this.tbPort.TabIndex = 9;
            this.tbPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPort_KeyPress);
            // 
            // tbSmtp
            // 
            this.tbSmtp.Location = new System.Drawing.Point(82, 90);
            this.tbSmtp.Name = "tbSmtp";
            this.tbSmtp.Size = new System.Drawing.Size(167, 20);
            this.tbSmtp.TabIndex = 10;
            // 
            // gbLoginDetails
            // 
            this.gbLoginDetails.Controls.Add(this.label3);
            this.gbLoginDetails.Controls.Add(this.tbSmtp);
            this.gbLoginDetails.Controls.Add(this.label1);
            this.gbLoginDetails.Controls.Add(this.tbPort);
            this.gbLoginDetails.Controls.Add(this.label2);
            this.gbLoginDetails.Controls.Add(this.tbPassword);
            this.gbLoginDetails.Controls.Add(this.label4);
            this.gbLoginDetails.Controls.Add(this.tbUserName);
            this.gbLoginDetails.Controls.Add(this.chkSSL);
            this.gbLoginDetails.Location = new System.Drawing.Point(12, 12);
            this.gbLoginDetails.Name = "gbLoginDetails";
            this.gbLoginDetails.Size = new System.Drawing.Size(268, 159);
            this.gbLoginDetails.TabIndex = 11;
            this.gbLoginDetails.TabStop = false;
            this.gbLoginDetails.Text = "Login Details";
            // 
            // rtbMessage
            // 
            this.rtbMessage.Location = new System.Drawing.Point(15, 190);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.Size = new System.Drawing.Size(268, 172);
            this.rtbMessage.TabIndex = 12;
            this.rtbMessage.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 174);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Email Message:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(212, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "What\'s this?";
            this.toolTip1.SetToolTip(this.label6, "This is the message which will be sent to students when you email their results t" +
        "o them");
            // 
            // EmailConfigForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(296, 400);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rtbMessage);
            this.Controls.Add(this.gbLoginDetails);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EmailConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Email Settings";
            this.gbLoginDetails.ResumeLayout(false);
            this.gbLoginDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkSSL;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbSmtp;
        private System.Windows.Forms.GroupBox gbLoginDetails;
        private System.Windows.Forms.RichTextBox rtbMessage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}