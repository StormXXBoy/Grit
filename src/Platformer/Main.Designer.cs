namespace Platformer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menu = new System.Windows.Forms.Panel();
            this.menuStrip = new System.Windows.Forms.ToolStrip();
            this.toolstripTest = new System.Windows.Forms.ToolStripButton();
            this.ipInput = new System.Windows.Forms.TextBox();
            this.HostButton = new System.Windows.Forms.Button();
            this.FPScounter = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.gameScreen = new System.Windows.Forms.PictureBox();
            this.gameUI = new System.Windows.Forms.ToolStripDropDownButton();
            this.menu.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.SystemColors.Control;
            this.menu.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("menu.BackgroundImage")));
            this.menu.Controls.Add(this.menuStrip);
            this.menu.Controls.Add(this.ipInput);
            this.menu.Controls.Add(this.HostButton);
            this.menu.Controls.Add(this.FPScounter);
            this.menu.Controls.Add(this.ConnectButton);
            this.menu.Location = new System.Drawing.Point(50, 50);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(400, 400);
            this.menu.TabIndex = 1;
            this.menu.Visible = false;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameUI,
            this.toolstripTest});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip.Size = new System.Drawing.Size(400, 25);
            this.menuStrip.TabIndex = 5;
            this.menuStrip.Text = "menuStrip";
            // 
            // toolstripTest
            // 
            this.toolstripTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolstripTest.Image = ((System.Drawing.Image)(resources.GetObject("toolstripTest.Image")));
            this.toolstripTest.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.toolstripTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripTest.Name = "toolstripTest";
            this.toolstripTest.Size = new System.Drawing.Size(31, 22);
            this.toolstripTest.Text = "Test";
            this.toolstripTest.Click += new System.EventHandler(this.toolstripTest_Click);
            // 
            // ipInput
            // 
            this.ipInput.Location = new System.Drawing.Point(219, 376);
            this.ipInput.Name = "ipInput";
            this.ipInput.Size = new System.Drawing.Size(86, 20);
            this.ipInput.TabIndex = 4;
            this.ipInput.Text = "127.0.0.1";
            // 
            // HostButton
            // 
            this.HostButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HostButton.AutoSize = true;
            this.HostButton.Location = new System.Drawing.Point(311, 345);
            this.HostButton.Name = "HostButton";
            this.HostButton.Size = new System.Drawing.Size(86, 23);
            this.HostButton.TabIndex = 3;
            this.HostButton.Text = "Host Local";
            this.HostButton.UseVisualStyleBackColor = true;
            this.HostButton.Click += new System.EventHandler(this.HostButton_Click);
            // 
            // FPScounter
            // 
            this.FPScounter.AutoSize = true;
            this.FPScounter.Location = new System.Drawing.Point(3, 384);
            this.FPScounter.Name = "FPScounter";
            this.FPScounter.Size = new System.Drawing.Size(42, 13);
            this.FPScounter.TabIndex = 2;
            this.FPScounter.Text = "67 FPS";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnectButton.AutoSize = true;
            this.ConnectButton.Location = new System.Drawing.Point(311, 374);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(86, 23);
            this.ConnectButton.TabIndex = 0;
            this.ConnectButton.Text = "Connect Local";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.Connect_Click);
            // 
            // gameScreen
            // 
            this.gameScreen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gameScreen.BackColor = System.Drawing.SystemColors.ControlDark;
            this.gameScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.gameScreen.Location = new System.Drawing.Point(0, 0);
            this.gameScreen.Margin = new System.Windows.Forms.Padding(0);
            this.gameScreen.Name = "gameScreen";
            this.gameScreen.Size = new System.Drawing.Size(500, 500);
            this.gameScreen.TabIndex = 0;
            this.gameScreen.TabStop = false;
            // 
            // gameUI
            // 
            this.gameUI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.gameUI.Image = ((System.Drawing.Image)(resources.GetObject("gameUI.Image")));
            this.gameUI.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gameUI.Name = "gameUI";
            this.gameUI.Size = new System.Drawing.Size(65, 22);
            this.gameUI.Text = "Game UI";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.gameScreen);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(550, 550);
            this.Name = "Main";
            this.Text = "Grit";
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox gameScreen;
        private System.Windows.Forms.Panel menu;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label FPScounter;
        private System.Windows.Forms.Button HostButton;
        private System.Windows.Forms.TextBox ipInput;
        private System.Windows.Forms.ToolStripButton toolstripTest;
        private System.Windows.Forms.ToolStripDropDownButton gameUI;
        private System.Windows.Forms.ToolStrip menuStrip;
    }
}

