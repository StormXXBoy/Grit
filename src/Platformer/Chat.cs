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
        NetwerkrClient client;

        public Chat()
        {
            InitializeComponent();
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
            chatMessages.Items.Add(message);
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
                chatMessages.TopIndex = chatMessages.Items.Count-1;
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
    }
}
