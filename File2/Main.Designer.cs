namespace File2
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.buttonAggregateCancel = new System.Windows.Forms.Button();
            this.groupBoxAggregate = new System.Windows.Forms.GroupBox();
            this.labelAggregateMessage = new System.Windows.Forms.Label();
            this.buttonAggregateGo = new System.Windows.Forms.Button();
            this.buttonAggregateTarget = new System.Windows.Forms.Button();
            this.buttonAggregateSource = new System.Windows.Forms.Button();
            this.textBoxAggregateTarget = new System.Windows.Forms.TextBox();
            this.textBoxAggregateSource = new System.Windows.Forms.TextBox();
            this.tabPageSetting = new System.Windows.Forms.TabPage();
            this.folderBrowserDialogMain = new System.Windows.Forms.FolderBrowserDialog();
            this.timerProgress = new System.Windows.Forms.Timer(this.components);
            this.tabControlMain.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.groupBoxAggregate.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageMain);
            this.tabControlMain.Controls.Add(this.tabPageSetting);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(782, 433);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.buttonAggregateCancel);
            this.tabPageMain.Controls.Add(this.groupBoxAggregate);
            this.tabPageMain.Location = new System.Drawing.Point(4, 25);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(774, 404);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "Home";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // buttonAggregateCancel
            // 
            this.buttonAggregateCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAggregateCancel.Enabled = false;
            this.buttonAggregateCancel.Location = new System.Drawing.Point(660, 161);
            this.buttonAggregateCancel.Name = "buttonAggregateCancel";
            this.buttonAggregateCancel.Size = new System.Drawing.Size(105, 25);
            this.buttonAggregateCancel.TabIndex = 5;
            this.buttonAggregateCancel.Text = "Cancel";
            this.buttonAggregateCancel.UseVisualStyleBackColor = true;
            this.buttonAggregateCancel.Click += new System.EventHandler(this.ButtonAggregateCancel_Click);
            // 
            // groupBoxAggregate
            // 
            this.groupBoxAggregate.Controls.Add(this.labelAggregateMessage);
            this.groupBoxAggregate.Controls.Add(this.buttonAggregateGo);
            this.groupBoxAggregate.Controls.Add(this.buttonAggregateTarget);
            this.groupBoxAggregate.Controls.Add(this.buttonAggregateSource);
            this.groupBoxAggregate.Controls.Add(this.textBoxAggregateTarget);
            this.groupBoxAggregate.Controls.Add(this.textBoxAggregateSource);
            this.groupBoxAggregate.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxAggregate.Location = new System.Drawing.Point(3, 3);
            this.groupBoxAggregate.Name = "groupBoxAggregate";
            this.groupBoxAggregate.Size = new System.Drawing.Size(768, 141);
            this.groupBoxAggregate.TabIndex = 0;
            this.groupBoxAggregate.TabStop = false;
            this.groupBoxAggregate.Text = "Aggregate";
            // 
            // labelAggregateMessage
            // 
            this.labelAggregateMessage.AutoSize = true;
            this.labelAggregateMessage.Font = new System.Drawing.Font("SimSun", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelAggregateMessage.ForeColor = System.Drawing.Color.Red;
            this.labelAggregateMessage.Location = new System.Drawing.Point(2, 102);
            this.labelAggregateMessage.Name = "labelAggregateMessage";
            this.labelAggregateMessage.Size = new System.Drawing.Size(149, 19);
            this.labelAggregateMessage.TabIndex = 5;
            this.labelAggregateMessage.Text = "Invalid source";
            // 
            // buttonAggregateGo
            // 
            this.buttonAggregateGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAggregateGo.Location = new System.Drawing.Point(657, 105);
            this.buttonAggregateGo.Name = "buttonAggregateGo";
            this.buttonAggregateGo.Size = new System.Drawing.Size(105, 25);
            this.buttonAggregateGo.TabIndex = 4;
            this.buttonAggregateGo.Text = "Go";
            this.buttonAggregateGo.UseVisualStyleBackColor = true;
            this.buttonAggregateGo.Click += new System.EventHandler(this.ButtonAggregateGo_Click);
            // 
            // buttonAggregateTarget
            // 
            this.buttonAggregateTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAggregateTarget.Location = new System.Drawing.Point(657, 65);
            this.buttonAggregateTarget.Name = "buttonAggregateTarget";
            this.buttonAggregateTarget.Size = new System.Drawing.Size(105, 25);
            this.buttonAggregateTarget.TabIndex = 3;
            this.buttonAggregateTarget.Text = "target...";
            this.buttonAggregateTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAggregateTarget.UseVisualStyleBackColor = true;
            this.buttonAggregateTarget.Click += new System.EventHandler(this.ButtonAggregateTarget_Click);
            // 
            // buttonAggregateSource
            // 
            this.buttonAggregateSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAggregateSource.Location = new System.Drawing.Point(657, 24);
            this.buttonAggregateSource.Name = "buttonAggregateSource";
            this.buttonAggregateSource.Size = new System.Drawing.Size(105, 25);
            this.buttonAggregateSource.TabIndex = 2;
            this.buttonAggregateSource.Text = "source...";
            this.buttonAggregateSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAggregateSource.UseVisualStyleBackColor = true;
            this.buttonAggregateSource.Click += new System.EventHandler(this.ButtonAggregateSource_Click);
            // 
            // textBoxAggregateTarget
            // 
            this.textBoxAggregateTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAggregateTarget.Location = new System.Drawing.Point(6, 65);
            this.textBoxAggregateTarget.Name = "textBoxAggregateTarget";
            this.textBoxAggregateTarget.Size = new System.Drawing.Size(645, 25);
            this.textBoxAggregateTarget.TabIndex = 1;
            this.textBoxAggregateTarget.TextChanged += new System.EventHandler(this.TextBoxAggregateTarget_TextChanged);
            // 
            // textBoxAggregateSource
            // 
            this.textBoxAggregateSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAggregateSource.Location = new System.Drawing.Point(6, 24);
            this.textBoxAggregateSource.Name = "textBoxAggregateSource";
            this.textBoxAggregateSource.Size = new System.Drawing.Size(645, 25);
            this.textBoxAggregateSource.TabIndex = 0;
            this.textBoxAggregateSource.TextChanged += new System.EventHandler(this.TextBoxAggregateSource_TextChanged);
            // 
            // tabPageSetting
            // 
            this.tabPageSetting.Location = new System.Drawing.Point(4, 25);
            this.tabPageSetting.Name = "tabPageSetting";
            this.tabPageSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSetting.Size = new System.Drawing.Size(774, 404);
            this.tabPageSetting.TabIndex = 1;
            this.tabPageSetting.Text = "Setting";
            this.tabPageSetting.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 433);
            this.Controls.Add(this.tabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File2";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.groupBoxAggregate.ResumeLayout(false);
            this.groupBoxAggregate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.TabPage tabPageSetting;
        private System.Windows.Forms.GroupBox groupBoxAggregate;
        private System.Windows.Forms.Button buttonAggregateGo;
        private System.Windows.Forms.Button buttonAggregateTarget;
        private System.Windows.Forms.Button buttonAggregateSource;
        private System.Windows.Forms.TextBox textBoxAggregateTarget;
        private System.Windows.Forms.TextBox textBoxAggregateSource;
        private System.Windows.Forms.Label labelAggregateMessage;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogMain;
        private System.Windows.Forms.Button buttonAggregateCancel;
        private System.Windows.Forms.Timer timerProgress;
    }
}

