using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Vector
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
        public static Vector operator /(Vector a, float scalar) { return new Vector(a.X / scalar, a.Y / scalar); }
    }
}