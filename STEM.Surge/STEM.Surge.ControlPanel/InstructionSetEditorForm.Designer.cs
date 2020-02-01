namespace STEM.Surge.ControlPanel
{
    partial class InstructionSetEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstructionSetEditorForm));
            this.instructionSetEditor1 = new STEM.Surge.ControlPanel.InstructionSetEditor();
            this.SuspendLayout();
            // 
            // instructionSetEditor1
            // 
            this.instructionSetEditor1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.instructionSetEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionSetEditor1.Location = new System.Drawing.Point(0, 0);
            this.instructionSetEditor1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.instructionSetEditor1.Name = "instructionSetEditor1";
            this.instructionSetEditor1.Size = new System.Drawing.Size(1361, 740);
            this.instructionSetEditor1.TabIndex = 0;
            // 
            // InstructionSetEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1361, 740);
            this.Controls.Add(this.instructionSetEditor1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InstructionSetEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InstructionSetEditorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private InstructionSetEditor instructionSetEditor1;
    }
}