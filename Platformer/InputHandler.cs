using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    public enum InputState
    {
        Down,
        Presses,
        Up
    }

    public enum InputType
    {
        Key,
        Mouse
    }

    public readonly struct InputInfo
    {
        public InputType type { get; }
        public InputState state { get; }
        public Keys key { get; }
        public MouseButtons button { get; }
        public Vector position { get; }

        public InputInfo(InputType Type, InputState State, Keys Key)
        {
            type = Type;
            state = State;
            key = Key;
            button = MouseButtons.None;
            position = Vector.Zero;
        }

        public InputInfo(InputType Type, InputState State, MouseButtons Button, Vector Position)
        {
            type = Type;
            state = State;
            key = Keys.None;
            button = Button;
            position = Position;
        }

        public override string ToString()
        {
            return $"Type: {type}, State: {state}, Key: {key}, Button: {button}, Position: ({position.X}, {position.Y})";
        }
    }

    public class InputHandler
    {
        private HashSet<Keys> keysDown = new HashSet<Keys>();
        private readonly Dictionary<Keys, Action> keyDownEvents = new Dictionary<Keys, Action>();
        private readonly Dictionary<Keys, Action> keyUpEvents = new Dictionary<Keys, Action>();
        private readonly List<Action<InputInfo>> inputEvents = new List<Action<InputInfo>>();

        private MouseMessageFilter mouseFilter = new MouseMessageFilter();

        public InputHandler(Form form)
        {
            form.KeyDown += HandleKeyDown;
            form.KeyUp += HandleKeyUp;

            Application.AddMessageFilter(mouseFilter);

            mouseFilter.MouseDown += HandleMouseDown;
            mouseFilter.MouseUp += HandleMouseUp;
        }

        private void handleInputEvent(InputInfo info)
        {
            foreach (var action in inputEvents)
            {
                action.Invoke(info);
            }
        }

        public void SubscribeInputEvent(Action<InputInfo> action)
        {
            inputEvents.Add(action);
        }

        public bool IsKeyDown(Keys key)
        {
            return keysDown.Contains(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return !IsKeyDown(key);
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
                handleInputEvent(new InputInfo(InputType.Key, InputState.Down, e.KeyCode));
                if (keyDownEvents.TryGetValue(e.KeyCode, out var action)) action?.Invoke();
            }
        }
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            handleInputEvent(new InputInfo(InputType.Key, InputState.Up, e.KeyCode));
            if (keysDown.Remove(e.KeyCode))
            {
                if (keyUpEvents.TryGetValue(e.KeyCode, out var action)) action?.Invoke();
            }
        }
        private void HandleMouseDown(MouseButtons btn, int x, int y)
        {
            handleInputEvent(new InputInfo(InputType.Mouse, InputState.Down, btn, new Vector(x, y)));
        }

        private void HandleMouseUp(MouseButtons btn, int x, int y)
        {
            handleInputEvent(new InputInfo(InputType.Mouse, InputState.Up, btn, new Vector(x, y)));
        }

    }

    public class MouseMessageFilter : IMessageFilter
    {
        public event Action<MouseButtons, int, int> MouseDown;
        public event Action<MouseButtons, int, int> MouseUp;

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                // ========== LEFT BUTTON ==========
                case 0x201: // WM_LBUTTONDOWN
                case 0x203: // WM_LBUTTONDBLCLK
                    MouseDown?.Invoke(MouseButtons.Left, Cursor.Position.X, Cursor.Position.Y);
                    break;

                case 0x202: // WM_LBUTTONUP
                    MouseUp?.Invoke(MouseButtons.Left, Cursor.Position.X, Cursor.Position.Y);
                    break;

                // ========== RIGHT BUTTON ==========
                case 0x204: // WM_RBUTTONDOWN
                case 0x206: // WM_RBUTTONDBLCLK
                    MouseDown?.Invoke(MouseButtons.Right, Cursor.Position.X, Cursor.Position.Y);
                    break;

                case 0x205: // WM_RBUTTONUP
                    MouseUp?.Invoke(MouseButtons.Right, Cursor.Position.X, Cursor.Position.Y);
                    break;

                // ========== MIDDLE BUTTON ==========
                case 0x207: // WM_MBUTTONDOWN
                case 0x209: // WM_MBUTTONDBLCLK
                    MouseDown?.Invoke(MouseButtons.Middle, Cursor.Position.X, Cursor.Position.Y);
                    break;

                case 0x208: // WM_MBUTTONUP
                    MouseUp?.Invoke(MouseButtons.Middle, Cursor.Position.X, Cursor.Position.Y);
                    break;

                //// ========== NON-CLIENT (TITLE BAR, BORDERS) ==========
                //case 0xA1: // WM_NCLBUTTONDOWN
                //    MouseDown?.Invoke(MouseButtons.Left, Cursor.Position.X, Cursor.Position.Y);
                //    break;

                //case 0xA0: // WM_NCLBUTTONUP
                //    MouseUp?.Invoke(MouseButtons.Left, Cursor.Position.X, Cursor.Position.Y);
                //    break;
            }

            return false; // Let the event continue to controls
        }
    }
}