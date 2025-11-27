namespace Platformer
{
    partial class Form1
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
            this.gameScreen = new System.Windows.Forms.PictureBox();
            this.menu = new System.Windows.Forms.Panel();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.TestButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).BeginInit();
            this.menu.SuspendLayout();
            this.SuspendLayout();
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
            // menu
            // 
            this.menu.Controls.Add(this.TestButton);
            this.menu.Controls.Add(this.ConnectButton);
            this.menu.Location = new System.Drawing.Point(50, 50);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(400, 400);
            this.menu.TabIndex = 1;
            this.menu.Visible = false;
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
            this.ConnectButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(322, 345);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(75, 23);
            this.TestButton.TabIndex = 1;
            this.TestButton.Text = "Test";
            this.TestButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.gameScreen);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).EndInit();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox gameScreen;
        private System.Windows.Forms.Panel menu;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button TestButton;
    }
}

