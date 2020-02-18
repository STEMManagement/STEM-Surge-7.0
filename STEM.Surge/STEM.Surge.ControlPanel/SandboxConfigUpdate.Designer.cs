namespace STEM.Surge.ControlPanel
{
    partial class SandboxConfigUpdate
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SandboxConfigUpdate));
            this.deploymentControllerSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.controllerEditor1 = new STEM.Surge.ControlPanel.ControllerEditor();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.versionSelect = new System.Windows.Forms.DataGridView();
            this.AsmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrigAsmVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AsmVersion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.updateEntries = new System.Windows.Forms.DataGridView();
            this.UpdateOb = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ObType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrigVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.beginSandboxUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.versionSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateEntries)).BeginInit();
            this.SuspendLayout();
            // 
            // deploymentControllerSelect
            // 
            this.deploymentControllerSelect.FormattingEnabled = true;
            this.deploymentControllerSelect.Location = new System.Drawing.Point(128, 13);
            this.deploymentControllerSelect.Name = "deploymentControllerSelect";
            this.deploymentControllerSelect.Size = new System.Drawing.Size(434, 21);
            this.deploymentControllerSelect.TabIndex = 0;
            this.deploymentControllerSelect.SelectedIndexChanged += new System.EventHandler(this.deploymentControllerSelect_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Deployment Controller";
            // 
            // controllerEditor1
            // 
            this.controllerEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controllerEditor1.Location = new System.Drawing.Point(8, 260);
            this.controllerEditor1.Name = "controllerEditor1";
            this.controllerEditor1.Size = new System.Drawing.Size(850, 364);
            this.controllerEditor1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(8, 40);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.versionSelect);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.updateEntries);
            this.splitContainer1.Size = new System.Drawing.Size(850, 189);
            this.splitContainer1.SplitterDistance = 419;
            this.splitContainer1.TabIndex = 3;
            // 
            // versionSelect
            // 
            this.versionSelect.AllowUserToAddRows = false;
            this.versionSelect.AllowUserToDeleteRows = false;
            this.versionSelect.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.versionSelect.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.versionSelect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.versionSelect.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AsmName,
            this.OrigAsmVersion,
            this.AsmVersion});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.versionSelect.DefaultCellStyle = dataGridViewCellStyle3;
            this.versionSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.versionSelect.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.versionSelect.Location = new System.Drawing.Point(0, 0);
            this.versionSelect.Name = "versionSelect";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.versionSelect.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.versionSelect.RowHeadersVisible = false;
            this.versionSelect.Size = new System.Drawing.Size(419, 189);
            this.versionSelect.TabIndex = 0;
            // 
            // AsmName
            // 
            this.AsmName.HeaderText = "Assembley";
            this.AsmName.Name = "AsmName";
            this.AsmName.ReadOnly = true;
            this.AsmName.Width = 200;
            // 
            // OrigAsmVersion
            // 
            this.OrigAsmVersion.HeaderText = "OrigAsmVersion";
            this.OrigAsmVersion.Name = "OrigAsmVersion";
            this.OrigAsmVersion.ReadOnly = true;
            this.OrigAsmVersion.Visible = false;
            // 
            // AsmVersion
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.AsmVersion.DefaultCellStyle = dataGridViewCellStyle2;
            this.AsmVersion.HeaderText = "Version";
            this.AsmVersion.Name = "AsmVersion";
            this.AsmVersion.Width = 200;
            // 
            // updateEntries
            // 
            this.updateEntries.AllowUserToAddRows = false;
            this.updateEntries.AllowUserToDeleteRows = false;
            this.updateEntries.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.updateEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.updateEntries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UpdateOb,
            this.ObType,
            this.ObFile,
            this.OrigVersion,
            this.NewVersion});
            this.updateEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateEntries.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.updateEntries.Location = new System.Drawing.Point(0, 0);
            this.updateEntries.Name = "updateEntries";
            this.updateEntries.RowHeadersVisible = false;
            this.updateEntries.Size = new System.Drawing.Size(427, 189);
            this.updateEntries.TabIndex = 0;
            // 
            // UpdateOb
            // 
            this.UpdateOb.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.UpdateOb.HeaderText = "Update";
            this.UpdateOb.Name = "UpdateOb";
            this.UpdateOb.Width = 48;
            // 
            // ObType
            // 
            this.ObType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ObType.HeaderText = "Type";
            this.ObType.Name = "ObType";
            this.ObType.ReadOnly = true;
            this.ObType.Width = 56;
            // 
            // ObFile
            // 
            this.ObFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ObFile.HeaderText = "File";
            this.ObFile.Name = "ObFile";
            this.ObFile.ReadOnly = true;
            this.ObFile.Width = 48;
            // 
            // OrigVersion
            // 
            this.OrigVersion.HeaderText = "OrigVersion";
            this.OrigVersion.Name = "OrigVersion";
            this.OrigVersion.ReadOnly = true;
            this.OrigVersion.Visible = false;
            // 
            // NewVersion
            // 
            this.NewVersion.HeaderText = "NewVersion";
            this.NewVersion.Name = "NewVersion";
            this.NewVersion.ReadOnly = true;
            this.NewVersion.Visible = false;
            // 
            // beginSandboxUpdate
            // 
            this.beginSandboxUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.beginSandboxUpdate.Location = new System.Drawing.Point(8, 231);
            this.beginSandboxUpdate.Name = "beginSandboxUpdate";
            this.beginSandboxUpdate.Size = new System.Drawing.Size(850, 23);
            this.beginSandboxUpdate.TabIndex = 4;
            this.beginSandboxUpdate.Text = "Begin Sandbox Update";
            this.beginSandboxUpdate.UseVisualStyleBackColor = true;
            this.beginSandboxUpdate.Click += new System.EventHandler(this.beginSandboxUpdate_Click);
            // 
            // SandboxConfigUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 652);
            this.Controls.Add(this.beginSandboxUpdate);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.controllerEditor1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deploymentControllerSelect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SandboxConfigUpdate";
            this.Text = "Sandbox Configuration Update";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.versionSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateEntries)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox deploymentControllerSelect;
        private System.Windows.Forms.Label label1;
        private ControllerEditor controllerEditor1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView versionSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn AsmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrigAsmVersion;
        private System.Windows.Forms.DataGridViewComboBoxColumn AsmVersion;
        private System.Windows.Forms.DataGridView updateEntries;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UpdateOb;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrigVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewVersion;
        private System.Windows.Forms.Button beginSandboxUpdate;
    }
}