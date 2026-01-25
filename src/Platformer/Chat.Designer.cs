namespace Platformer
{
    partial class Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            this.chatInput = new System.Windows.Forms.TextBox();
            this.chatMessages = new System.Windows.Forms.ListBox();
            this.contextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // chatInput
            // 
            this.chatInput.AcceptsReturn = true;
            this.chatInput.Location = new System.Drawing.Point(12, 279);
            this.chatInput.Name = "chatInput";
            this.chatInput.Size = new System.Drawing.Size(459, 20);
            this.chatInput.TabIndex = 0;
            this.chatInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chatInput_KeyPress);
            // 
            // chatMessages
            // 
            this.chatMessages.BackColor = System.Drawing.SystemColors.Window;
            this.chatMessages.FormattingEnabled = true;
            this.chatMessages.Location = new System.Drawing.Point(12, 12);
            this.chatMessages.Name = "chatMessages";
            this.chatMessages.ScrollAlwaysVisible = true;
            this.chatMessages.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.chatMessages.Size = new System.Drawing.Size(460, 264);
            this.chatMessages.TabIndex = 1;
            this.chatMessages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chatMessages_MouseDown);
            // 
            // contextStrip
            // 
            this.contextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextStrip.Name = "contextStrip";
            this.contextStrip.Size = new System.Drawing.Size(103, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(484, 311);
            this.Controls.Add(this.chatMessages);
            this.Controls.Add(this.chatInput);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(250, 300);
            this.Name = "Chat";
            this.Text = "Chat";
            this.Resize += new System.EventHandler(this.Chat_Resize);
            this.contextStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatInput;
        private System.Windows.Forms.ListBox chatMessages;
        private System.Windows.Forms.ContextMenuStrip contextStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}