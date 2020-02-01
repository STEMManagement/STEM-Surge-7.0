namespace STEM.Surge.ControlPanel
{
    partial class SwitchboardEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.fileSourcesGridView = new System.Windows.Forms.DataGridView();
            this.Notes = new System.Windows.Forms.DataGridViewImageColumn();
            this.enableDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.sourceDirectoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.directoryFilterDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileFilterDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recurseDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PingOnPoll = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.scanDelaySecondsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ControllerFilename = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.maxBranchLoadDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ipLooselyBoundDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Sandbox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UseAltAssembliesOnly = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CoordinatedManagerIPs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LimitToBranchIPs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImpersonateUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LocalUserImpersonation = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ImpersonationPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileSourcesDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Save = new System.Windows.Forms.ToolStripButton();
            this.Cancel = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.offlineTol = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grafanaUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.synchronizedManagers = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ipPreferenceMap = new System.Windows.Forms.Button();
            this.managerMacroMap = new System.Windows.Forms.Button();
            this.configurationMacroMapDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterMask = new System.Windows.Forms.ToolStripTextBox();
            this.managerConfiguration = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fileSourcesGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSourcesDataTableBindingSource)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurationMacroMapDataTableBindingSource)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileSourcesGridView
            // 
            this.fileSourcesGridView.AutoGenerateColumns = false;
            this.fileSourcesGridView.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.fileSourcesGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.fileSourcesGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.fileSourcesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileSourcesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Notes,
            this.enableDataGridViewCheckBoxColumn,
            this.sourceDirectoryDataGridViewTextBoxColumn,
            this.directoryFilterDataGridViewTextBoxColumn,
            this.fileFilterDataGridViewTextBoxColumn,
            this.recurseDataGridViewCheckBoxColumn,
            this.PingOnPoll,
            this.scanDelaySecondsDataGridViewTextBoxColumn,
            this.ControllerFilename,
            this.maxBranchLoadDataGridViewTextBoxColumn,
            this.ipLooselyBoundDataGridViewCheckBoxColumn,
            this.Sandbox,
            this.UseAltAssembliesOnly,
            this.CoordinatedManagerIPs,
            this.LimitToBranchIPs,
            this.ImpersonateUser,
            this.LocalUserImpersonation,
            this.ImpersonationPassword});
            this.fileSourcesGridView.DataSource = this.fileSourcesDataTableBindingSource;
            this.fileSourcesGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileSourcesGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.fileSourcesGridView.GridColor = System.Drawing.Color.LightGray;
            this.fileSourcesGridView.Location = new System.Drawing.Point(0, 0);
            this.fileSourcesGridView.Name = "fileSourcesGridView";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.fileSourcesGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.fileSourcesGridView.Size = new System.Drawing.Size(1285, 250);
            this.fileSourcesGridView.TabIndex = 0;
            // 
            // Notes
            // 
            this.Notes.HeaderText = "";
            this.Notes.Image = global::STEM.Surge.ControlPanel.Properties.Resources.notes;
            this.Notes.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.Notes.MinimumWidth = 20;
            this.Notes.Name = "Notes";
            this.Notes.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Notes.Width = 20;
            // 
            // enableDataGridViewCheckBoxColumn
            // 
            this.enableDataGridViewCheckBoxColumn.DataPropertyName = "Enable";
            this.enableDataGridViewCheckBoxColumn.HeaderText = "Enable";
            this.enableDataGridViewCheckBoxColumn.Name = "enableDataGridViewCheckBoxColumn";
            this.enableDataGridViewCheckBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.enableDataGridViewCheckBoxColumn.Width = 60;
            // 
            // sourceDirectoryDataGridViewTextBoxColumn
            // 
            this.sourceDirectoryDataGridViewTextBoxColumn.DataPropertyName = "SourceDirectory";
            this.sourceDirectoryDataGridViewTextBoxColumn.HeaderText = "Source";
            this.sourceDirectoryDataGridViewTextBoxColumn.Name = "sourceDirectoryDataGridViewTextBoxColumn";
            this.sourceDirectoryDataGridViewTextBoxColumn.ToolTipText = "Use STEM IP Spanning Notation to save configuration time";
            this.sourceDirectoryDataGridViewTextBoxColumn.Width = 300;
            // 
            // directoryFilterDataGridViewTextBoxColumn
            // 
            this.directoryFilterDataGridViewTextBoxColumn.DataPropertyName = "DirectoryFilter";
            this.directoryFilterDataGridViewTextBoxColumn.HeaderText = "Directory Filter";
            this.directoryFilterDataGridViewTextBoxColumn.Name = "directoryFilterDataGridViewTextBoxColumn";
            this.directoryFilterDataGridViewTextBoxColumn.ToolTipText = "Use \'|\' to separate multiple entries. Use \'!\' to specify an exclusive filter.";
            // 
            // fileFilterDataGridViewTextBoxColumn
            // 
            this.fileFilterDataGridViewTextBoxColumn.DataPropertyName = "FileFilter";
            this.fileFilterDataGridViewTextBoxColumn.HeaderText = "File Filter";
            this.fileFilterDataGridViewTextBoxColumn.Name = "fileFilterDataGridViewTextBoxColumn";
            this.fileFilterDataGridViewTextBoxColumn.ToolTipText = "Use \'|\' to separate multiple entries. Use \'!\' to specify an exclusive filter.";
            // 
            // recurseDataGridViewCheckBoxColumn
            // 
            this.recurseDataGridViewCheckBoxColumn.DataPropertyName = "Recurse";
            this.recurseDataGridViewCheckBoxColumn.HeaderText = "Recurse";
            this.recurseDataGridViewCheckBoxColumn.Name = "recurseDataGridViewCheckBoxColumn";
            this.recurseDataGridViewCheckBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.recurseDataGridViewCheckBoxColumn.Width = 60;
            // 
            // PingOnPoll
            // 
            this.PingOnPoll.DataPropertyName = "PingOnPoll";
            this.PingOnPoll.HeaderText = "Pingable Source";
            this.PingOnPoll.Name = "PingOnPoll";
            this.PingOnPoll.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // scanDelaySecondsDataGridViewTextBoxColumn
            // 
            this.scanDelaySecondsDataGridViewTextBoxColumn.DataPropertyName = "ScanDelaySeconds";
            this.scanDelaySecondsDataGridViewTextBoxColumn.HeaderText = "Scan Delay Seconds";
            this.scanDelaySecondsDataGridViewTextBoxColumn.Name = "scanDelaySecondsDataGridViewTextBoxColumn";
            this.scanDelaySecondsDataGridViewTextBoxColumn.ToolTipText = "The shorter the delay, the higher the scan priority";
            this.scanDelaySecondsDataGridViewTextBoxColumn.Width = 60;
            // 
            // ControllerFilename
            // 
            this.ControllerFilename.DataPropertyName = "ControllerFilename";
            this.ControllerFilename.HeaderText = "Controller";
            this.ControllerFilename.Name = "ControllerFilename";
            this.ControllerFilename.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ControllerFilename.Width = 300;
            // 
            // maxBranchLoadDataGridViewTextBoxColumn
            // 
            this.maxBranchLoadDataGridViewTextBoxColumn.DataPropertyName = "MaxBranchLoad";
            this.maxBranchLoadDataGridViewTextBoxColumn.HeaderText = "Maximum Branch Load";
            this.maxBranchLoadDataGridViewTextBoxColumn.Name = "maxBranchLoadDataGridViewTextBoxColumn";
            this.maxBranchLoadDataGridViewTextBoxColumn.ToolTipText = "Consider the worst case in concert with all other rows in this Switchboard";
            this.maxBranchLoadDataGridViewTextBoxColumn.Width = 60;
            // 
            // ipLooselyBoundDataGridViewCheckBoxColumn
            // 
            this.ipLooselyBoundDataGridViewCheckBoxColumn.DataPropertyName = "IpLooselyBound";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.NullValue = false;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ipLooselyBoundDataGridViewCheckBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.ipLooselyBoundDataGridViewCheckBoxColumn.HeaderText = "Allow Cross Branch Assignment";
            this.ipLooselyBoundDataGridViewCheckBoxColumn.Name = "ipLooselyBoundDataGridViewCheckBoxColumn";
            this.ipLooselyBoundDataGridViewCheckBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ipLooselyBoundDataGridViewCheckBoxColumn.ToolTipText = "Allow files sourced by this row to run on a different Branch when found on an Act" +
    "ive Branch.";
            this.ipLooselyBoundDataGridViewCheckBoxColumn.Width = 60;
            // 
            // Sandbox
            // 
            this.Sandbox.DataPropertyName = "Sandbox";
            this.Sandbox.HeaderText = "Sandbox";
            this.Sandbox.Name = "Sandbox";
            this.Sandbox.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // UseAltAssembliesOnly
            // 
            this.UseAltAssembliesOnly.DataPropertyName = "UseAltAssembliesOnly";
            this.UseAltAssembliesOnly.HeaderText = "Sandbox With Alt Assemblies Only";
            this.UseAltAssembliesOnly.Name = "UseAltAssembliesOnly";
            this.UseAltAssembliesOnly.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // CoordinatedManagerIPs
            // 
            this.CoordinatedManagerIPs.DataPropertyName = "CoordinatedManagerIPs";
            this.CoordinatedManagerIPs.HeaderText = "Coordinated Manager IPs";
            this.CoordinatedManagerIPs.Name = "CoordinatedManagerIPs";
            this.CoordinatedManagerIPs.ToolTipText = "Coordinate assignment with a Deployment Manager that is also assigning files from" +
    " this source";
            this.CoordinatedManagerIPs.Width = 120;
            // 
            // LimitToBranchIPs
            // 
            this.LimitToBranchIPs.DataPropertyName = "LimitToBranchIPs";
            this.LimitToBranchIPs.HeaderText = "Limit To Branch IPs";
            this.LimitToBranchIPs.Name = "LimitToBranchIPs";
            // 
            // ImpersonateUser
            // 
            this.ImpersonateUser.DataPropertyName = "ImpersonateUser";
            this.ImpersonateUser.HeaderText = "Impersonate User";
            this.ImpersonateUser.Name = "ImpersonateUser";
            this.ImpersonateUser.ToolTipText = "Is a different user account needed in order to access these files";
            // 
            // LocalUserImpersonation
            // 
            this.LocalUserImpersonation.DataPropertyName = "LocalUserImpersonation";
            this.LocalUserImpersonation.HeaderText = "Local User Impersonation";
            this.LocalUserImpersonation.Name = "LocalUserImpersonation";
            this.LocalUserImpersonation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.LocalUserImpersonation.ToolTipText = "Is the impersonated user account on this machine or the data source";
            // 
            // ImpersonationPassword
            // 
            this.ImpersonationPassword.DataPropertyName = "ImpersonationPassword";
            this.ImpersonationPassword.HeaderText = "Impersonation Password";
            this.ImpersonationPassword.Name = "ImpersonationPassword";
            // 
            // fileSourcesDataTableBindingSource
            // 
            this.fileSourcesDataTableBindingSource.DataSource = typeof(STEM.Surge.SwitchboardConfig.FileSourcesDataTable);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Save,
            this.Cancel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1291, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Save
            // 
            this.Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Save.Image = global::STEM.Surge.ControlPanel.Properties.Resources.save;
            this.Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(28, 28);
            this.Save.Text = "Save Settings";
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Cancel
            // 
            this.Cancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Cancel.Image = global::STEM.Surge.ControlPanel.Properties.Resources.undo;
            this.Cancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(28, 28);
            this.Cancel.Text = "Undo and reload last saved configuration";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.offlineTol);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.grafanaUrl);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.synchronizedManagers);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(0, 34);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(548, 117);
            this.panel2.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(402, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "minutes.";
            // 
            // offlineTol
            // 
            this.offlineTol.Location = new System.Drawing.Point(335, 92);
            this.offlineTol.Name = "offlineTol";
            this.offlineTol.Size = new System.Drawing.Size(61, 20);
            this.offlineTol.TabIndex = 11;
            this.offlineTol.Text = "2";
            this.offlineTol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.offlineTol.TextChanged += new System.EventHandler(this.offlineTol_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(332, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Allow cross site assignment when site appears \'Offline\' for  more than ";
            // 
            // grafanaUrl
            // 
            this.grafanaUrl.Location = new System.Drawing.Point(141, 29);
            this.grafanaUrl.Name = "grafanaUrl";
            this.grafanaUrl.Size = new System.Drawing.Size(358, 20);
            this.grafanaUrl.TabIndex = 9;
            this.grafanaUrl.Text = "http://localhost:8080";
            this.grafanaUrl.TextChanged += new System.EventHandler(this.grafanaUrl_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Grafana URL";
            // 
            // synchronizedManagers
            // 
            this.synchronizedManagers.Location = new System.Drawing.Point(141, 3);
            this.synchronizedManagers.Name = "synchronizedManagers";
            this.synchronizedManagers.Size = new System.Drawing.Size(358, 20);
            this.synchronizedManagers.TabIndex = 2;
            this.synchronizedManagers.Text = "127.0.0.1";
            this.synchronizedManagers.TextChanged += new System.EventHandler(this.synchronizedManagers_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Synchronized Managers";
            // 
            // ipPreferenceMap
            // 
            this.ipPreferenceMap.Location = new System.Drawing.Point(554, 86);
            this.ipPreferenceMap.Name = "ipPreferenceMap";
            this.ipPreferenceMap.Size = new System.Drawing.Size(218, 23);
            this.ipPreferenceMap.TabIndex = 7;
            this.ipPreferenceMap.Text = "IP Preference Map...";
            this.ipPreferenceMap.UseVisualStyleBackColor = true;
            this.ipPreferenceMap.Click += new System.EventHandler(this.IpPreferenceMap_Click);
            // 
            // managerMacroMap
            // 
            this.managerMacroMap.Location = new System.Drawing.Point(554, 60);
            this.managerMacroMap.Name = "managerMacroMap";
            this.managerMacroMap.Size = new System.Drawing.Size(218, 23);
            this.managerMacroMap.TabIndex = 6;
            this.managerMacroMap.Text = "Global Placeholders...";
            this.managerMacroMap.UseVisualStyleBackColor = true;
            this.managerMacroMap.Click += new System.EventHandler(this.ManagerMacroMap_Click);
            // 
            // configurationMacroMapDataTableBindingSource
            // 
            this.configurationMacroMapDataTableBindingSource.DataSource = typeof(STEM.Surge.SwitchboardConfig.ConfigurationMacroMapDataTable);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.toolStrip2);
            this.panel3.Location = new System.Drawing.Point(3, 157);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1285, 285);
            this.panel3.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.fileSourcesGridView);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 35);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1285, 250);
            this.panel4.TabIndex = 0;
            // 
            // toolStrip2
            // 
            this.toolStrip2.AutoSize = false;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFilter,
            this.filterMask});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1285, 35);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // clearFilter
            // 
            this.clearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(24, 32);
            this.clearFilter.Text = "Clear Filter";
            this.clearFilter.Click += new System.EventHandler(this.clearFilter_Click);
            // 
            // filterMask
            // 
            this.filterMask.AutoSize = false;
            this.filterMask.BackColor = System.Drawing.Color.Lavender;
            this.filterMask.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterMask.Name = "filterMask";
            this.filterMask.Size = new System.Drawing.Size(400, 20);
            this.filterMask.ToolTipText = "Filter the Switchboard Grid Rows";
            this.filterMask.TextChanged += new System.EventHandler(this.filterMask_TextChanged);
            // 
            // managerConfiguration
            // 
            this.managerConfiguration.Location = new System.Drawing.Point(554, 34);
            this.managerConfiguration.Name = "managerConfiguration";
            this.managerConfiguration.Size = new System.Drawing.Size(218, 23);
            this.managerConfiguration.TabIndex = 8;
            this.managerConfiguration.Text = "Deployment Manager Configuration...";
            this.managerConfiguration.UseVisualStyleBackColor = true;
            this.managerConfiguration.Click += new System.EventHandler(this.managerConfiguration_Click);
            // 
            // SwitchboardEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.managerConfiguration);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.ipPreferenceMap);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.managerMacroMap);
            this.Name = "SwitchboardEditor";
            this.Size = new System.Drawing.Size(1291, 445);
            ((System.ComponentModel.ISupportInitialize)(this.fileSourcesGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSourcesDataTableBindingSource)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurationMacroMapDataTableBindingSource)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private System.Windows.Forms.DataGridView fileSourcesGridView;
        private System.Windows.Forms.BindingSource fileSourcesDataTableBindingSource;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Save;
        private System.Windows.Forms.ToolStripButton Cancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterMask;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox synchronizedManagers;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.BindingSource configurationMacroMapDataTableBindingSource;
        private System.Windows.Forms.Button managerMacroMap;
        private System.Windows.Forms.Button ipPreferenceMap;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox offlineTol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox grafanaUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button managerConfiguration;
        private System.Windows.Forms.DataGridViewImageColumn Notes;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enableDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceDirectoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn directoryFilterDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileFilterDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn recurseDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PingOnPoll;
        private System.Windows.Forms.DataGridViewTextBoxColumn scanDelaySecondsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ControllerFilename;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxBranchLoadDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ipLooselyBoundDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Sandbox;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseAltAssembliesOnly;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoordinatedManagerIPs;
        private System.Windows.Forms.DataGridViewTextBoxColumn LimitToBranchIPs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImpersonateUser;
        private System.Windows.Forms.DataGridViewCheckBoxColumn LocalUserImpersonation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImpersonationPassword;
    }
}