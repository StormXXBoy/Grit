using Netwerkr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    public partial class Chat : Form
    {
        private NetwerkrClient client;
        public Action<string> messageSent;

        public Chat()
        {
            InitializeComponent();
            Chat_Resize(null, null);
        }

        public Chat(NetwerkrClient netwerkrClient)
        {
            InitializeComponent();
            Chat_Resize(null, null);
            client = netwerkrClient;
            client.listen("message", (data) =>
            {
                addMessage(data);
            });
        }

        public void addMessage(string message)
        {
            chatMessages.Items.Insert(0, message);
            messageSent?.Invoke(message);
        }

        private void chatInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (client != null)
                {
                    client.fire("message", chatInput.Text);
                }
                else
                {
                    addMessage(chatInput.Text);
                }
                chatInput.Text = "";
                chatMessages.TopIndex = 0;
                e.Handled = true;
            }
        }

        private void Chat_Resize(object sender, EventArgs e)
        {
            chatMessages.Location = new Point(0, 0);
            chatMessages.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - chatInput.Height);
            chatInput.Location = new Point(0, this.ClientSize.Height - chatInput.Height);
            chatInput.Size = new Size(this.ClientSize.Width, chatInput.Height);
        }

        private void chatMessages_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = chatMessages.IndexFromPoint(e.Location);

                if (index != ListBox.NoMatches)
                {
                    chatMessages.SelectionMode = SelectionMode.One;
                    chatMessages.SelectedIndex = index;
                    contextStrip.Show(chatMessages, e.Location);
                }
                else
                {
                    chatMessages.SelectionMode = SelectionMode.None;
                    chatMessages.ClearSelected();
                }
            }
            else
            {
                chatMessages.SelectionMode = SelectionMode.None;
                chatMessages.ClearSelected();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (chatMessages.SelectedItem != null)
            {
                Clipboard.SetText(chatMessages.SelectedItem.ToString());
            }
        }
    }
}
