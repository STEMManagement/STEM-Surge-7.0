namespace STEM.Surge.ControlPanel
{
    partial class ControllerEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControllerEditorForm));
            this.controllerEditor1 = new STEM.Surge.ControlPanel.ControllerEditor();
            this.SuspendLayout();
            // 
            // controllerEditor1
            // 
            this.controllerEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controllerEditor1.Location = new System.Drawing.Point(0, 0);
            this.controllerEditor1.Name = "controllerEditor1";
            this.controllerEditor1.Size = new System.Drawing.Size(1351, 600);
            this.controllerEditor1.TabIndex = 0;
            // 
            // ControllerEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1351, 600);
            this.Controls.Add(this.controllerEditor1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ControllerEditorForm";
            this.Text = "Controller Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private ControllerEditor controllerEditor1;
    }
}