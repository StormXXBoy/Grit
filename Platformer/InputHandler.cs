using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    internal class InputHandler
    {
        private HashSet<Keys> keysDown = new HashSet<Keys>();

        public InputHandler(Form form)
        {
            form.KeyDown += HandleKeyDown;
            form.KeyUp += HandleKeyUp;
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            keysDown.Add(e.KeyCode);
        }
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            keysDown.Remove(e.KeyCode);
        }

        public bool IsKeyDown(Keys key)
        {
            return keysDown.Contains(key);
        }
    }
}
