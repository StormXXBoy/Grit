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

        public Chat(string ip, int port)
        {
            InitializeComponent();
            client = new NetwerkrClient(ip, port);
        }

        private void chatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chatMessages.Items.Add(chatInput.Text);
                chatInput.Text = "";
            }
        }
    }
}
