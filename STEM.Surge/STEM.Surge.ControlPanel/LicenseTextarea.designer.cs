namespace STEM.Surge.ControlPanel
{
    partial class LicenseTextarea
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseTextarea));
            this.licenseKeys = new System.Windows.Forms.RichTextBox();
            this.save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // licenseKeys
            // 
            this.licenseKeys.Location = new System.Drawing.Point(2, 2);
            this.licenseKeys.Name = "licenseKeys";
            this.licenseKeys.Size = new System.Drawing.Size(401, 103);
            this.licenseKeys.TabIndex = 0;
            this.licenseKeys.Text = "";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(328, 111);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 0;
            this.save.Text = "Apply";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // LicenseTextarea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(407, 138);
            this.Controls.Add(this.save);
            this.Controls.Add(this.licenseKeys);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LicenseTextarea";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "License Keys";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox licenseKeys;
        private System.Windows.Forms.Button save;
    }
}