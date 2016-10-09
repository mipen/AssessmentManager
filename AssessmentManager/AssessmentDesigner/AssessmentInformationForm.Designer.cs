namespace AssessmentManager
{
    partial class AssessmentInformationForm
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
            this.labelAuthor = new System.Windows.Forms.Label();
            this.textBoxAuthor = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.lblAssessmentName = new System.Windows.Forms.Label();
            this.tbAssessmentName = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblAssessmentWeighting = new System.Windows.Forms.Label();
            this.nudAssessmentWeighting = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudAssessmentWeighting)).BeginInit();
            this.SuspendLayout();
            // 
            // labelAuthor
            // 
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Location = new System.Drawing.Point(85, 52);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(41, 13);
            this.labelAuthor.TabIndex = 0;
            this.labelAuthor.Text = "Author:";
            this.toolTip1.SetToolTip(this.labelAuthor, "The name of the author");
            // 
            // textBoxAuthor
            // 
            this.textBoxAuthor.Location = new System.Drawing.Point(132, 49);
            this.textBoxAuthor.Name = "textBoxAuthor";
            this.textBoxAuthor.Size = new System.Drawing.Size(218, 20);
            this.textBoxAuthor.TabIndex = 2;
            this.toolTip1.SetToolTip(this.textBoxAuthor, "The name of the author");
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(12, 131);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(275, 131);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // lblAssessmentName
            // 
            this.lblAssessmentName.AutoSize = true;
            this.lblAssessmentName.Location = new System.Drawing.Point(25, 23);
            this.lblAssessmentName.Name = "lblAssessmentName";
            this.lblAssessmentName.Size = new System.Drawing.Size(101, 13);
            this.lblAssessmentName.TabIndex = 9;
            this.lblAssessmentName.Text = "Assessment Name*:";
            this.toolTip1.SetToolTip(this.lblAssessmentName, "The name of the assessment. \\n ExampIe: Test 1 \\n This is required.");
            // 
            // tbAssessmentName
            // 
            this.tbAssessmentName.Location = new System.Drawing.Point(132, 20);
            this.tbAssessmentName.Name = "tbAssessmentName";
            this.tbAssessmentName.Size = new System.Drawing.Size(218, 20);
            this.tbAssessmentName.TabIndex = 1;
            this.toolTip1.SetToolTip(this.tbAssessmentName, "The name of the assessment. \\n ExampIe: Test 1 \\n This is required.");
            // 
            // lblAssessmentWeighting
            // 
            this.lblAssessmentWeighting.AutoSize = true;
            this.lblAssessmentWeighting.Location = new System.Drawing.Point(9, 81);
            this.lblAssessmentWeighting.Name = "lblAssessmentWeighting";
            this.lblAssessmentWeighting.Size = new System.Drawing.Size(117, 13);
            this.lblAssessmentWeighting.TabIndex = 10;
            this.lblAssessmentWeighting.Text = "Assessment Weighting:";
            this.toolTip1.SetToolTip(this.lblAssessmentWeighting, "The weighting for the assessment (in %)");
            // 
            // nudAssessmentWeighting
            // 
            this.nudAssessmentWeighting.Location = new System.Drawing.Point(132, 79);
            this.nudAssessmentWeighting.Name = "nudAssessmentWeighting";
            this.nudAssessmentWeighting.Size = new System.Drawing.Size(48, 20);
            this.nudAssessmentWeighting.TabIndex = 3;
            this.toolTip1.SetToolTip(this.nudAssessmentWeighting, "The weighting for the assessment (in %)");
            // 
            // AssessmentInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(363, 171);
            this.Controls.Add(this.nudAssessmentWeighting);
            this.Controls.Add(this.lblAssessmentWeighting);
            this.Controls.Add(this.tbAssessmentName);
            this.Controls.Add(this.lblAssessmentName);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxAuthor);
            this.Controls.Add(this.labelAuthor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AssessmentInformationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Assessment Information";
            ((System.ComponentModel.ISupportInitialize)(this.nudAssessmentWeighting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.TextBox textBoxAuthor;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label lblAssessmentName;
        private System.Windows.Forms.TextBox tbAssessmentName;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblAssessmentWeighting;
        private System.Windows.Forms.NumericUpDown nudAssessmentWeighting;
    }
}