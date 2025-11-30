// ECS Core Components
using Platformer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

public interface IComponent { }

public struct Transform : IComponent
{
    public Vector Position;
    public Vector Velocity;
    public Vector Acceleration;
    public Size Size;

    public Transform(Vector position, Size size)
    {
        Position = position;
        Velocity = new Vector();
        Acceleration = new Vector();
        Size = size;
    }
}

public struct Renderable : IComponent
{
    public Color Color;
    public Bitmap Sprite;
}

public struct Physics : IComponent
{
    public bool UseGravity;
    public bool Collidable;
    public float Mass;
    public DateTime LastJumpTime;
}

public struct Player : IComponent
{
    public float MoveSpeed;
    public float JumpForce;
}

public struct Networked : IComponent
{
    public string NetworkId;
}

public struct Bullet : IComponent
{
    public float Lifetime;
    public float Damage;
}

public abstract class GameSystem
{
    protected World World;

    public GameSystem(World world)
    {
        World = world;
    }

    public abstract void Update(float deltaTime);
}

public class InputSystem : GameSystem
{
    private InputHandler input;

    public InputSystem(World world, InputHandler inputHandler) : base(world)
    {
        input = inputHandler;
    }

    public override void Update(float deltaTime)
    {
        var entities = World.GetEntities<Player, Transform, Physics>();

        foreach (var entity in entities)
        {
            var player = World.GetComponent<Player>(entity);
            var transform = World.GetComponent<Transform>(entity);
            var physics = World.GetComponent<Physics>(entity);

            if (input.IsKeyDown(Keys.Right) || input.IsKeyDown(Keys.D))
            {
                transform.Acceleration.X += player.MoveSpeed * deltaTime;
            }
            if (input.IsKeyDown(Keys.Left) || input.IsKeyDown(Keys.Q))
            {
                transform.Acceleration.X -= player.MoveSpeed * deltaTime;
            }
            if (input.IsKeyDown(Keys.Up) || input.IsKeyDown(Keys.Space))
            {
                physics.LastJumpTime = DateTime.Now;
            }

            World.SetComponent(entity, transform);
            World.SetComponent(entity, physics);
        }
    }
}

public class PhysicsSystem : GameSystem
{
    private float gravity = 0.2f;
    private float friction = 0.9f;

    public PhysicsSystem(World world) : base(world) { }

    public override void Update(float deltaTime)
    {
        var entities = World.GetEntities<Transform, Physics>();

        foreach (var entity in entities)
        {
            var transform = World.GetComponent<Transform>(entity);
            var physics = World.GetComponent<Physics>(entity);

            // Apply gravity
            if (physics.UseGravity && !IsGrounded(transform, entity))
            {
                transform.Acceleration.Y += gravity;
            }

            // Update velocity and position
            transform.Velocity += transform.Acceleration;
            transform.Velocity *= friction;
            transform.Acceleration *= friction;

            // Move and resolve collisions
            transform.Position.X += transform.Velocity.X;
            ResolveCollisions(entity, true);

            transform.Position.Y += transform.Velocity.Y;
            ResolveCollisions(entity, false);

            World.SetComponent(entity, transform);
        }
    }

    private bool IsGrounded(Transform transform, int currentEntity)
    {
        RectangleF feet = new RectangleF(
            transform.Position.X,
            transform.Position.Y + transform.Size.Height,
            transform.Size.Width,
            1
        );

        // Check against all collidable entities (excluding self)
        var collidables = World.GetEntities<Transform, Physics>()
            .Where(e => e != currentEntity)
            .Where(e => World.GetComponent<Physics>(e).Collidable);

        foreach (var collidable in collidables)
        {
            var collidableTransform = World.GetComponent<Transform>(collidable);
            RectangleF bounds = new RectangleF(
                collidableTransform.Position.X,
                collidableTransform.Position.Y,
                collidableTransform.Size.Width,
                collidableTransform.Size.Height
            );

            if (feet.IntersectsWith(bounds))
                return true;
        }

        return transform.Position.Y + transform.Size.Height >= 500;
    }

    private void ResolveCollisions(int entity, bool axisX)
    {
        var transform = World.GetComponent<Transform>(entity);
        var physics = World.GetComponent<Physics>(entity);

        RectangleF rect = new RectangleF(
            transform.Position.X,
            transform.Position.Y,
            transform.Size.Width,
            transform.Size.Height
        );

        // Check against all collidable entities (excluding self)
        var collidables = World.GetEntities<Transform, Physics>()
            .Where(e => e != entity)
            .Where(e => World.GetComponent<Physics>(e).Collidable);

        foreach (var collidable in collidables)
        {
            var collidableTransform = World.GetComponent<Transform>(collidable);
            RectangleF bounds = new RectangleF(
                collidableTransform.Position.X,
                collidableTransform.Position.Y,
                collidableTransform.Size.Width,
                collidableTransform.Size.Height
            );

            if (!rect.IntersectsWith(bounds))
                continue;

            if (axisX)
            {
                if (transform.Velocity.X > 0)
                    transform.Position.X = bounds.X - transform.Size.Width;
                else if (transform.Velocity.X < 0)
                    transform.Position.X = bounds.X + bounds.Width;

                transform.Velocity.X *= -0.7f;
                transform.Acceleration.X = 0;
            }
            else
            {
                if (transform.Velocity.Y > 0)
                    transform.Position.Y = bounds.Y - transform.Size.Height;
                else if (transform.Velocity.Y < 0)
                    transform.Position.Y = bounds.Y + bounds.Height;

                transform.Velocity.Y = 0;
                transform.Acceleration.Y = 0;
            }
        }

        // Boundary checking
        if (transform.Position.Y + transform.Size.Height > 500)
        {
            transform.Position.Y = 500 - transform.Size.Height;
            transform.Velocity.Y = 0;
            transform.Acceleration.Y = 0;
        }

        World.SetComponent(entity, transform);
    }
}

public class RenderSystem : GameSystem
{
    private Graphics graphics;
    private Bitmap backBuffer;
    private Bitmap backgroundImage;
    private int playerEntityId;
    private Dictionary<Color, Brush> brushCache = new Dictionary<Color, Brush>();

    public RenderSystem(World world, Graphics g, Bitmap buffer, Bitmap background, int playerId) : base(world)
    {
        graphics = g;
        backBuffer = buffer;
        backgroundImage = background;
        playerEntityId = playerId;
    }

    public override void Update(float deltaTime)
    {
        using (Graphics g = Graphics.FromImage(backBuffer))
        {
            float para = 50;
            var playerTransform = World.GetComponent<Transform>(playerEntityId);
            float xD = ((1 - (playerTransform.Position.X / backBuffer.Width)) * para) - para;
            float yD = ((1 - (playerTransform.Position.Y / backBuffer.Height)) * para) - para;
            g.DrawImage(backgroundImage, xD, yD, backBuffer.Width + para, backBuffer.Height + para);

            var entities = World.GetEntities<Transform, Renderable>();

            foreach (var entity in entities)
            {
                var transform = World.GetComponent<Transform>(entity);
                var renderable = World.GetComponent<Renderable>(entity);

                if (renderable.Sprite != null)
                {
                    g.DrawImage(renderable.Sprite,
                        transform.Position.X,
                        transform.Position.Y,
                        transform.Size.Width,
                        transform.Size.Height);
                }
                else
                {
                    // Get or create brush from cache
                    if (!brushCache.TryGetValue(renderable.Color, out Brush brush))
                    {
                        brush = new SolidBrush(renderable.Color);
                        brushCache[renderable.Color] = brush;
                    }

                    g.FillRectangle(brush,
                        transform.Position.X,
                        transform.Position.Y,
                        transform.Size.Width,
                        transform.Size.Height);
                }
            }
        }

        graphics.DrawImageUnscaled(backBuffer, 0, 0);
    }

    // Clean up brushes when the system is disposed
    public void Dispose()
    {
        foreach (var brush in brushCache.Values)
        {
            brush.Dispose();
        }
        brushCache.Clear();
    }
}

public class World
{
    private int nextEntityId = 0;
    private Dictionary<int, Dictionary<Type, IComponent>> entities = new Dictionary<int, Dictionary<Type, IComponent>>();

    public int CreateEntity()
    {
        int entityId = nextEntityId++;
        entities[entityId] = new Dictionary<Type, IComponent>();
        return entityId;
    }

    public void AddComponent<T>(int entityId, T component) where T : IComponent
    {
        if (entities.ContainsKey(entityId))
        {
            entities[entityId][typeof(T)] = component;
        }
    }

    public T GetComponent<T>(int entityId) where T : IComponent
    {
        if (entities.ContainsKey(entityId) && entities[entityId].ContainsKey(typeof(T)))
        {
            return (T)entities[entityId][typeof(T)];
        }
        return default(T);
    }

    public void SetComponent<T>(int entityId, T component) where T : IComponent
    {
        if (entities.ContainsKey(entityId))
        {
            entities[entityId][typeof(T)] = component;
        }
    }

    public List<int> GetEntities<T1>() where T1 : IComponent
    {
        return entities.Where(e => e.Value.ContainsKey(typeof(T1)))
                      .Select(e => e.Key)
                      .ToList();
    }

    public List<int> GetEntities<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        return entities.Where(e => e.Value.ContainsKey(typeof(T1)) && e.Value.ContainsKey(typeof(T2)))
                      .Select(e => e.Key)
                      .ToList();
    }

    public List<int> GetEntities<T1, T2, T3>() where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        return entities.Where(e => e.Value.ContainsKey(typeof(T1)) && e.Value.ContainsKey(typeof(T2)) && e.Value.ContainsKey(typeof(T3)))
                      .Select(e => e.Key)
                      .ToList();
    }

    public List<int> GetEntities(params Type[] componentTypes)
    {
        return entities.Where(e => componentTypes.All(ct => e.Value.ContainsKey(ct)))
                      .Select(e => e.Key)
                      .ToList();
    }

    public void RemoveEntity(int entityId)
    {
        entities.Remove(entityId);
    }
}