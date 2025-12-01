using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    public class InputHandler
    {
        private HashSet<Keys> keysDown = new HashSet<Keys>();
        private readonly Dictionary<Keys, Action> keyDownEvents = new Dictionary<Keys, Action>();
        private readonly Dictionary<Keys, Action> keyUpEvents = new Dictionary<Keys, Action>();

        public InputHandler(Form form)
        {
            form.KeyDown += HandleKeyDown;
            form.KeyUp += HandleKeyUp;
        }

        public void SubscribeKeyDown(Keys key, Action action)
        {
            if (!keyDownEvents.ContainsKey(key))
                keyDownEvents[key] = null;

            keyDownEvents[key] += action;
        }

        public void SubscribeKeyUp(Keys key, Action action)
        {
            if (!keyUpEvents.ContainsKey(key))
                keyUpEvents[key] = null;

            keyUpEvents[key] += action;
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (keysDown.Add(e.KeyCode))
            {
                if (keyDownEvents.TryGetValue(e.KeyCode, out var action)) action?.Invoke();
            }
        }
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (keysDown.Remove(e.KeyCode))
            {
                if (keyUpEvents.TryGetValue(e.KeyCode, out var action)) action?.Invoke();
            }
        }

        public bool IsKeyDown(Keys key)
        {
            return keysDown.Contains(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return !IsKeyDown(key);
        }
    }
}