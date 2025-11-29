using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    class Vector
    {
        public float X = 0;
        public float Y = 0;

        public Vector() { }

        public Vector(object x, object y)
        {
            X = Convert.ToSingle(x);
            Y = Convert.ToSingle(y);
        }

        public Vector(Vector other)
        {
            X = other.X;
            Y = other.Y;
        }

        public float magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public Vector normalize()
        {
            float mag = this.magnitude();
            if (mag == 0) return new Vector(0, 0);
            return new Vector(X / mag, Y / mag);
        }

        public static Vector operator +(Vector a, Vector b) { return new Vector(a.X + b.X, a.Y + b.Y); }
        public static Vector operator -(Vector a, Vector b) { return new Vector(a.X - b.X, a.Y - b.Y); }
        public static Vector operator *(Vector a, float scalar) { return new Vector(a.X * scalar, a.Y * scalar); }
    }

    internal class Entity
    {
        public string id = Guid.NewGuid().ToString();

        public int health = 100;
        public float speed = 1;
        public Color color = Color.HotPink;

        public Size size = new Size(10, 20);
        public Vector position = new Vector(0, 0);
        public Vector acceleration = new Vector(0, 0);
        public Vector velocity = new Vector(0, 0);

        public DateTime lastJumpTime = DateTime.MinValue;

        private Pen pen;
        private Brush brush;

        public Entity()
        {
            pen = new Pen(color);
            brush = new SolidBrush(color);
        }

        public Entity(Color newColor)
        {
            color = newColor;
            pen = new Pen(color);
            brush = new SolidBrush(color);
        }

        public void Draw(Graphics graphics)
        {
            graphics.FillRectangle(brush, position.X, position.Y, size.Width, size.Height);
        }

        public void MoveHorizontal(float dt, int dir)
        {
            this.acceleration.X += (dir^0) * this.speed * dt;
        }
    }
}
