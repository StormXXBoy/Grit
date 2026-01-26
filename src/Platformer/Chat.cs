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

        private void initChat()
        {
            chatMessages.Columns.Clear();
            chatMessages.Columns.Add("", chatMessages.ClientSize.Width);

            Chat_Resize(null, null);
        }

        public Chat()
        {
            InitializeComponent();
            initChat();
        }

        public Chat(NetwerkrClient netwerkrClient)
        {
            InitializeComponent();
            initChat();
            client = netwerkrClient;
            client.listen("message", (data) =>
            {
                addMessage(data);
            });
        }

        public void addMessage(string message)
        {
            var item = new ListViewItem(message);
            item.ToolTipText = message;
            chatMessages.Items.Insert(0, item);
            messageSent?.Invoke(message);

            Chat_Resize(null, null);
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
                e.Handled = true;
            }
        }

        private void Chat_Resize(object sender, EventArgs e)
        {
            chatMessages.Location = new Point(0, 0);
            chatMessages.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - chatInput.Height);
            chatInput.Location = new Point(0, this.ClientSize.Height - chatInput.Height);
            chatInput.Size = new Size(this.ClientSize.Width, chatInput.Height);

            if (chatMessages.Columns.Count > 0)
            {
                chatMessages.Columns[0].Width = chatMessages.ClientSize.Width;
            }
        }

        private void chatMessages_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var item = chatMessages.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    item.Selected = true;
                    contextStrip.Show(chatMessages, e.Location);
                }
                else
                {
                    chatMessages.SelectedItems.Clear();
                }
            }
            else
            {
                chatMessages.SelectedItems.Clear();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (chatMessages.FocusedItem != null)
            {
                Clipboard.SetText(chatMessages.FocusedItem.Text.ToString());
                chatMessages.SelectedItems.Clear();
            }
        }
    }
}
