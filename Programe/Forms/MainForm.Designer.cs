namespace Programe.Forms
{
    partial class MainForm
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
            this.PlayerList = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.AccountMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.LoginMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.RegisterMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.shipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UploadMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.Display = new System.Windows.Forms.TreeView();
            this.UploadDialog = new System.Windows.Forms.OpenFileDialog();
            this.ZoomBar = new System.Windows.Forms.TrackBar();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomBar)).BeginInit();
            this.SuspendLayout();
            // 
            // PlayerList
            // 
            this.PlayerList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerList.FormattingEnabled = true;
            this.PlayerList.IntegralHeight = false;
            this.PlayerList.Location = new System.Drawing.Point(12, 27);
            this.PlayerList.Name = "PlayerList";
            this.PlayerList.Size = new System.Drawing.Size(185, 606);
            this.PlayerList.TabIndex = 0;
            this.PlayerList.SelectedIndexChanged += new System.EventHandler(this.PlayerListSelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AccountMenu,
            this.shipToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // AccountMenu
            // 
            this.AccountMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoginMenu,
            this.RegisterMenu});
            this.AccountMenu.Name = "AccountMenu";
            this.AccountMenu.Size = new System.Drawing.Size(64, 20);
            this.AccountMenu.Text = "Account";
            // 
            // LoginMenu
            // 
            this.LoginMenu.Name = "LoginMenu";
            this.LoginMenu.Size = new System.Drawing.Size(116, 22);
            this.LoginMenu.Text = "Login";
            this.LoginMenu.Click += new System.EventHandler(this.LoginMenuClick);
            // 
            // RegisterMenu
            // 
            this.RegisterMenu.Name = "RegisterMenu";
            this.RegisterMenu.Size = new System.Drawing.Size(116, 22);
            this.RegisterMenu.Text = "Register";
            this.RegisterMenu.Click += new System.EventHandler(this.RegisterMenuClick);
            // 
            // shipToolStripMenuItem
            // 
            this.shipToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UploadMenu});
            this.shipToolStripMenuItem.Name = "shipToolStripMenuItem";
            this.shipToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.shipToolStripMenuItem.Text = "Ship";
            // 
            // UploadMenu
            // 
            this.UploadMenu.Name = "UploadMenu";
            this.UploadMenu.Size = new System.Drawing.Size(112, 22);
            this.UploadMenu.Text = "Upload";
            this.UploadMenu.Click += new System.EventHandler(this.UploadMenuClick);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Interval = 15;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // Display
            // 
            this.Display.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Display.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Display.Location = new System.Drawing.Point(203, 27);
            this.Display.Name = "Display";
            this.Display.Size = new System.Drawing.Size(1049, 642);
            this.Display.TabIndex = 2;
            // 
            // UploadDialog
            // 
            this.UploadDialog.Filter = "Programe Executables (*.pge)|*.pge";
            this.UploadDialog.Title = "Ship Upload";
            // 
            // ZoomBar
            // 
            this.ZoomBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ZoomBar.Location = new System.Drawing.Point(12, 639);
            this.ZoomBar.Maximum = 250;
            this.ZoomBar.Minimum = 50;
            this.ZoomBar.Name = "ZoomBar";
            this.ZoomBar.Size = new System.Drawing.Size(185, 45);
            this.ZoomBar.TabIndex = 3;
            this.ZoomBar.Value = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.ZoomBar);
            this.Controls.Add(this.Display);
            this.Controls.Add(this.PlayerList);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Programe";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox PlayerList;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem AccountMenu;
        private System.Windows.Forms.ToolStripMenuItem shipToolStripMenuItem;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.TreeView Display;
        private System.Windows.Forms.ToolStripMenuItem LoginMenu;
        private System.Windows.Forms.ToolStripMenuItem RegisterMenu;
        private System.Windows.Forms.OpenFileDialog UploadDialog;
        private System.Windows.Forms.ToolStripMenuItem UploadMenu;
        private System.Windows.Forms.TrackBar ZoomBar;
    }
}