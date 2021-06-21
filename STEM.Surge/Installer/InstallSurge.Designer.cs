namespace Installer
{
    partial class InstallSurge
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.certificateFriendlyName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.useSSL = new System.Windows.Forms.CheckBox();
            this.postmortemOutputDir = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.remoteConfigurationDir = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.processorOverload = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.envManagers = new System.Windows.Forms.TextBox();
            this.envPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.userGroup = new System.Windows.Forms.GroupBox();
            this.password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.userName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.runAsUserCB = new System.Windows.Forms.CheckBox();
            this.uiRB = new System.Windows.Forms.RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.branchRB = new System.Windows.Forms.RadioButton();
            this.managerRB = new System.Windows.Forms.RadioButton();
            this.eventLogName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.userGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.uiRB);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.branchRB);
            this.panel1.Controls.Add(this.managerRB);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(680, 563);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.eventLogName);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.certificateFriendlyName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.useSSL);
            this.groupBox1.Controls.Add(this.postmortemOutputDir);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.remoteConfigurationDir);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.processorOverload);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.envManagers);
            this.groupBox1.Controls.Add(this.envPort);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.userGroup);
            this.groupBox1.Controls.Add(this.runAsUserCB);
            this.groupBox1.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(41, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(607, 391);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "For Service Installations";
            // 
            // certificateFriendlyName
            // 
            this.certificateFriendlyName.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.certificateFriendlyName.Location = new System.Drawing.Point(319, 219);
            this.certificateFriendlyName.Name = "certificateFriendlyName";
            this.certificateFriendlyName.Size = new System.Drawing.Size(174, 21);
            this.certificateFriendlyName.TabIndex = 42;
            this.certificateFriendlyName.Text = "STEM.Surge";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(17, 218);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 20);
            this.label6.TabIndex = 41;
            this.label6.Text = "Certificate Friendly Name";
            // 
            // useSSL
            // 
            this.useSSL.AutoSize = true;
            this.useSSL.Location = new System.Drawing.Point(499, 219);
            this.useSSL.Name = "useSSL";
            this.useSSL.Size = new System.Drawing.Size(94, 24);
            this.useSSL.TabIndex = 40;
            this.useSSL.Text = "Use SSL";
            this.useSSL.UseVisualStyleBackColor = true;
            // 
            // postmortemOutputDir
            // 
            this.postmortemOutputDir.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.postmortemOutputDir.Location = new System.Drawing.Point(319, 312);
            this.postmortemOutputDir.Name = "postmortemOutputDir";
            this.postmortemOutputDir.Size = new System.Drawing.Size(282, 21);
            this.postmortemOutputDir.TabIndex = 39;
            this.postmortemOutputDir.Text = "D:\\STEM.Workforce\\PostMortem";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(17, 311);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(215, 20);
            this.label9.TabIndex = 38;
            this.label9.Text = "PostMortem Output Directory";
            // 
            // remoteConfigurationDir
            // 
            this.remoteConfigurationDir.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remoteConfigurationDir.Location = new System.Drawing.Point(319, 281);
            this.remoteConfigurationDir.Name = "remoteConfigurationDir";
            this.remoteConfigurationDir.Size = new System.Drawing.Size(282, 21);
            this.remoteConfigurationDir.TabIndex = 37;
            this.remoteConfigurationDir.Text = "\\\\[MANAGERS]\\STEM.Workforce\\Configuration";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(17, 280);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(279, 20);
            this.label8.TabIndex = 36;
            this.label8.Text = "Surge Remote Configuration Directory";
            // 
            // processorOverload
            // 
            this.processorOverload.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processorOverload.Location = new System.Drawing.Point(319, 250);
            this.processorOverload.Name = "processorOverload";
            this.processorOverload.Size = new System.Drawing.Size(90, 21);
            this.processorOverload.TabIndex = 23;
            this.processorOverload.Text = "2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 249);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 20);
            this.label3.TabIndex = 22;
            this.label3.Text = "Processor Overload Factor";
            // 
            // envManagers
            // 
            this.envManagers.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.envManagers.Location = new System.Drawing.Point(319, 188);
            this.envManagers.Name = "envManagers";
            this.envManagers.Size = new System.Drawing.Size(230, 21);
            this.envManagers.TabIndex = 21;
            this.envManagers.Text = "172.19.12.50#172.19.24.50";
            // 
            // envPort
            // 
            this.envPort.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.envPort.Location = new System.Drawing.Point(319, 157);
            this.envPort.Name = "envPort";
            this.envPort.Size = new System.Drawing.Size(90, 21);
            this.envPort.TabIndex = 20;
            this.envPort.Text = "7847";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(288, 20);
            this.label4.TabIndex = 19;
            this.label4.Text = "Surge Deployment Manager Addresses";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(296, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "Surge Environment Communication Port ";
            // 
            // userGroup
            // 
            this.userGroup.Controls.Add(this.password);
            this.userGroup.Controls.Add(this.label2);
            this.userGroup.Controls.Add(this.userName);
            this.userGroup.Controls.Add(this.label1);
            this.userGroup.Location = new System.Drawing.Point(44, 44);
            this.userGroup.Name = "userGroup";
            this.userGroup.Size = new System.Drawing.Size(557, 98);
            this.userGroup.TabIndex = 1;
            this.userGroup.TabStop = false;
            // 
            // password
            // 
            this.password.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password.Location = new System.Drawing.Point(95, 52);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(368, 21);
            this.password.TabIndex = 3;
            this.password.Text = "password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // userName
            // 
            this.userName.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userName.Location = new System.Drawing.Point(95, 17);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(368, 21);
            this.userName.TabIndex = 1;
            this.userName.Text = "DOMAIN\\Username";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // runAsUserCB
            // 
            this.runAsUserCB.AutoSize = true;
            this.runAsUserCB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runAsUserCB.Location = new System.Drawing.Point(21, 19);
            this.runAsUserCB.Name = "runAsUserCB";
            this.runAsUserCB.Size = new System.Drawing.Size(264, 24);
            this.runAsUserCB.TabIndex = 0;
            this.runAsUserCB.Text = "Run the service as a specific user";
            this.runAsUserCB.UseVisualStyleBackColor = true;
            this.runAsUserCB.CheckedChanged += new System.EventHandler(this.runAsUserCB_CheckedChanged);
            // 
            // uiRB
            // 
            this.uiRB.AutoSize = true;
            this.uiRB.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiRB.Location = new System.Drawing.Point(41, 71);
            this.uiRB.Name = "uiRB";
            this.uiRB.Size = new System.Drawing.Size(291, 24);
            this.uiRB.TabIndex = 9;
            this.uiRB.Text = "Install Surge Control Panel Only";
            this.uiRB.UseVisualStyleBackColor = true;
            this.uiRB.CheckedChanged += new System.EventHandler(this.uiRB_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.AutoSize = true;
            this.button2.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(602, 527);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 30);
            this.button2.TabIndex = 8;
            this.button2.Text = "Exit";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.AutoSize = true;
            this.button1.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(521, 527);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 7;
            this.button1.Text = "Install";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // branchRB
            // 
            this.branchRB.AutoSize = true;
            this.branchRB.Checked = true;
            this.branchRB.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchRB.Location = new System.Drawing.Point(41, 46);
            this.branchRB.Name = "branchRB";
            this.branchRB.Size = new System.Drawing.Size(268, 24);
            this.branchRB.TabIndex = 3;
            this.branchRB.TabStop = true;
            this.branchRB.Text = "Install Surge Branch Manager";
            this.branchRB.UseVisualStyleBackColor = true;
            this.branchRB.CheckedChanged += new System.EventHandler(this.branchRB_CheckedChanged);
            // 
            // managerRB
            // 
            this.managerRB.AutoSize = true;
            this.managerRB.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.managerRB.Location = new System.Drawing.Point(41, 21);
            this.managerRB.Name = "managerRB";
            this.managerRB.Size = new System.Drawing.Size(311, 24);
            this.managerRB.TabIndex = 2;
            this.managerRB.Text = "Install Surge Deployment Manager";
            this.managerRB.UseVisualStyleBackColor = true;
            this.managerRB.CheckedChanged += new System.EventHandler(this.managerRB_CheckedChanged);
            // 
            // eventLogName
            // 
            this.eventLogName.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventLogName.Location = new System.Drawing.Point(319, 343);
            this.eventLogName.Name = "eventLogName";
            this.eventLogName.Size = new System.Drawing.Size(282, 21);
            this.eventLogName.TabIndex = 44;
            this.eventLogName.Text = "Application";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(17, 342);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(191, 20);
            this.label7.TabIndex = 43;
            this.label7.Text = "Windows EventLog Name";
            // 
            // InstallSurge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Name = "InstallSurge";
            this.Size = new System.Drawing.Size(680, 563);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.userGroup.ResumeLayout(false);
            this.userGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox userGroup;
        private System.Windows.Forms.CheckBox runAsUserCB;
        private System.Windows.Forms.RadioButton branchRB;
        private System.Windows.Forms.RadioButton managerRB;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox userName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton uiRB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox processorOverload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox envManagers;
        private System.Windows.Forms.TextBox envPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox postmortemOutputDir;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox remoteConfigurationDir;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox useSSL;
        private System.Windows.Forms.TextBox certificateFriendlyName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox eventLogName;
        private System.Windows.Forms.Label label7;
    }
}
