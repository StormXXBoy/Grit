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
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).BeginInit();
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
            this.menu.Location = new System.Drawing.Point(50, 50);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(400, 400);
            this.menu.TabIndex = 1;
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox gameScreen;
        private System.Windows.Forms.Panel menu;
    }
}

