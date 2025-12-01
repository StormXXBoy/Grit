using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    class Sprite
    {
        public Bitmap image;

        private Color _color = Color.HotPink;
        public Color color
        {
            get => _color;
            set
            {
                _color = value;
                brush = new SolidBrush(_color);
            }
        }

        public Brush brush = Brushes.HotPink;

        public Sprite() { }

        public Sprite(Bitmap img)
        {
            image = img;
        }

        public Sprite(Brush b)
        {
            brush = b;
        }

        public Sprite(Color c)
        {
            color = c;
        }
    }

    internal class Entity
    {
        public string id = Guid.NewGuid().ToString();

        public Color color = Color.HotPink;

        public Size size = new Size(10, 20);
        public Vector position = new Vector(0, 0);
        public Vector center => new Vector(position.X + size.Width / 2, position.Y + size.Height / 2);

        public DateTime lastJumpTime = DateTime.MinValue;

        public Sprite sprite = new Sprite();

        public Entity() { }

        public Entity(Color newColor)
        {
            sprite.color = newColor;
        }

        public void Draw(Graphics graphics)
        {
            if (sprite.image != null)
            {
                graphics.DrawImage(sprite.image, position.X, position.Y, size.Width, size.Height);
                return;
            }
            graphics.FillRectangle(sprite.brush, position.X, position.Y, size.Width, size.Height);
        }
    }

    class PhysicsEntity : Entity
    {
        public Vector acceleration = new Vector(0, 0);
        public Vector velocity = new Vector(0, 0);

        public PhysicsEntity() : base() { }
        public PhysicsEntity(Color color) : base(color) { }
    }

    interface ICollidable
    {
        RectangleF bounds { get; }
    }

    class CollisionEntity : Entity, ICollidable
    {
        public RectangleF bounds => new RectangleF(position.X, position.Y, size.Width, size.Height);

        public CollisionEntity() : base() {}

        public CollisionEntity(float x, float y, float width, float height)
        {
            position = new Vector(x, y);
            size = new Size((int)width, (int)height);
        }
    }

    class PlayerEntity : PhysicsEntity
    {
        public int health = 100;
        public float speed = 1;

        public void MoveHorizontal(float dt, int dir)
        {
            this.acceleration.X += (dir ^ 0) * this.speed * dt;
        }
    }

    class PlatformEntity : CollisionEntity
    {
        public PlatformEntity() : base()
        {
            sprite = new Sprite(Platformer.Properties.Resources.brick);
        }

        public PlatformEntity(float x, float y, float width, float height)
        {
            position = new Vector(x, y);
            size = new Size((int)width, (int)height);
            sprite = new Sprite(Platformer.Properties.Resources.brick);
        }
    }
}
