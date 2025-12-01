using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Netwerkr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    public partial class Form1 : Form
    {
        public Netwerkr.Netwerkr net = new Netwerkr.Netwerkr();
        public NetwerkrClient client;

        Size gameBounds = new Size(500, 500);
        Graphics area;
        Bitmap backgroundImage = Properties.Resources.background;
        Bitmap backBuffer;

        Color background = Color.White;

        InputHandler input;

        PlayerEntity player;
        List<Entity> entities = new List<Entity>();

        List<PlayerEntity> playerEntities = new List<PlayerEntity>();
        List<PhysicsEntity> physicsEntities = new List<PhysicsEntity>();
        List<ICollidable> collisionEntities = new List<ICollidable>();

        SoundMachine soundMachine = new SoundMachine();

        public Form1()
        {
            InitializeComponent();

            this.Load += (s, e) => Start();
            this.Resize += (s, e) => onResize();
            gameScreen.MouseDown += (s, e) => HandleClick(s, e);

            onResize();
        }

        void onResize() {
            void centerControl(Control control)
            {
                control.Left = (this.ClientSize.Width - control.Width) / 2;
                control.Top = (this.ClientSize.Height - control.Height) / 2;
            }

            centerControl(gameScreen);
            centerControl(menu);
        }

        Point? bufferPoint = null;
        void HandleClick(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (bufferPoint == null)
                {
                    bufferPoint = e.Location;
                }
                else
                {
                    Point start = bufferPoint.Value;
                    Point end = e.Location;

                    int x = Math.Min(start.X, end.X);
                    int y = Math.Min(start.Y, end.Y);
                    int w = Math.Abs(end.X - start.X);
                    int h = Math.Abs(end.Y - start.Y);

                    addEntity(new PlatformEntity(x, y, w, h));
                    bufferPoint = null;
                }
            }
            else
            {
                Vector shootDirection = new Vector(e.Location.X, e.Location.Y) - player.position;

                PhysicsEntity newBullet = new PhysicsEntity();

                newBullet.position = new Vector(player.position) - new Vector(0, 10);
                newBullet.acceleration = new Vector(player.acceleration);
                newBullet.velocity = new Vector(player.velocity) + (shootDirection.normalize() * 50f);
                //player.velocity += (shootDirection.normalize() * 50f);

                newBullet.size = new Size(10, 10);

                addEntity(newBullet);
            }
        }

        T addEntity<T>(T entity) where T : Entity
        {
            entities.Add(entity);
            if (entity is PhysicsEntity physEnt)
            {
                physicsEntities.Add(physEnt);
            }
            if (entity is ICollidable colEnt)
            {
                collisionEntities.Add(colEnt);
            }
            return entity;
        }

        void Start()
        {
            area = gameScreen.CreateGraphics();
            backBuffer = new Bitmap(gameBounds.Width, gameBounds.Height);

            input = new InputHandler(this);
            input.SubscribeKeyDown(Keys.Escape, () => {
                this.ActiveControl = null;
                menu.Visible = !menu.Visible;
                menu.Enabled = menu.Visible;
            });

            player = new PlayerEntity();
            player.sprite.image = Properties.Resources.Player;
            addEntity(player);

            soundMachine.LoadSound("jump", "sounds/jump.mp3");

            addEntity(new PlatformEntity(100, gameBounds.Height - 30, 200, 20));
            addEntity(new PlatformEntity(350, 200, 200, 20));

            StartLoop();
        }

        DateTime lastUpdate;
        float currentFPS = 0;
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        void StartLoop()
        {
            lastUpdate = DateTime.Now;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 16;
            timer.Tick += TimerTick;
            timer.Start();
        }

        void TimerTick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            float dt = (float)(now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            FPScounter.Text = (Lerp((1f / dt), dt, 0.01f)).ToString("0") + " FPS";

            Loop(dt);
        }

        void Loop(float dt)
        {
            HandleInput(dt);
            PhysicsLoop(dt);
            UpdateNetwork();
            DrawLoop();
        }

        bool isGrounded(Entity entity)
        {
            RectangleF feet = new RectangleF(entity.position.X, entity.position.Y + entity.size.Height, entity.size.Width, 1);

            foreach (var p in collisionEntities)
            {
                if (feet.IntersectsWith(p.bounds))
                    return true;
            }

            return entity.position.Y + entity.size.Height >= gameBounds.Height;
        }

        TimeSpan jumpCooldown = TimeSpan.FromMilliseconds(200);
        void entityJump(PhysicsEntity entity)
        {
            if (isGrounded(entity) && DateTime.Now - entity.lastJumpTime >= jumpCooldown)
            {
                entity.acceleration.Y -= 2;
                entity.lastJumpTime = DateTime.Now;
                //soundMachine.Play("jump");
            }
        }

        void HandleInput(float dt)
        {
            if (input.IsKeyDown(Keys.Right) || input.IsKeyDown(Keys.D))
            {
                player.MoveHorizontal(dt, 1);
            }
            if (input.IsKeyDown(Keys.Left) || input.IsKeyDown(Keys.Q))
            {
                player.MoveHorizontal(dt, -1);
            }
            if (input.IsKeyDown(Keys.Up) || input.IsKeyDown(Keys.Space))
            {
                entityJump(player);
            }
        }

        float gravity = 0.2f;
        float friction = 0.9f;
        //void PhysicsLoop(float dt)
        //{
        //    foreach (var entity in physicsEntities)
        //    {
        //        if (!isGrounded(entity))
        //            entity.acceleration.Y += gravity;

        //        entity.velocity += entity.acceleration;

        //        entity.acceleration *= friction;
        //        entity.velocity *= friction;

        //        entity.position.X += entity.velocity.X;
        //        ResolveAxisCollision(entity, axisX: true);

        //        entity.position.Y += entity.velocity.Y;
        //        ResolveAxisCollision(entity, axisX: false);
        //    }
        //}
        void PhysicsLoop(float dt)
        {
            dt *= 40f;

            int subSteps = 3;
            dt /= subSteps;

            for (int step = 0; step < subSteps; step++)
            {
                foreach (var entity in physicsEntities)
                {
                    if (!isGrounded(entity))
                        entity.acceleration.Y += gravity * dt;

                    entity.velocity += entity.acceleration * dt;
                    entity.acceleration *= (float)Math.Pow(friction, dt);
                    entity.velocity *= (float)Math.Pow(friction, dt);

                    entity.position.Y += entity.velocity.Y * dt;
                    ResolveAxisCollision(entity, axisX: false);

                    entity.position.X += entity.velocity.X * dt;
                    ResolveAxisCollision(entity, axisX: true);
                }
            }
        }

        void ResolveAxisCollision(PhysicsEntity entity, bool axisX)
        {
            RectangleF rect = new RectangleF(
                entity.position.X,
                entity.position.Y,
                entity.size.Width,
                entity.size.Height
            );

            foreach (var collisionEnt in collisionEntities)
            {
                if (collisionEnt == entity) continue;

                if (!rect.IntersectsWith(collisionEnt.bounds)) continue;

                if (axisX)
                {
                    if (entity.velocity.X > 0)
                        entity.position.X = collisionEnt.bounds.X - entity.size.Width;
                    else if (entity.velocity.X < 0)
                        entity.position.X = collisionEnt.bounds.X + collisionEnt.bounds.Width;

                    entity.velocity.X *= -0.7f;
                    entity.acceleration.X = 0;
                }
                else
                {
                    if (entity.velocity.Y > 0)
                        entity.position.Y = collisionEnt.bounds.Y - entity.size.Height;
                    else if (entity.velocity.Y < 0)
                        entity.position.Y = collisionEnt.bounds.Y + collisionEnt.bounds.Height;

                    entity.velocity.Y = 0;
                    entity.acceleration.Y = 0;
                }

                rect = new RectangleF(
                    entity.position.X,
                    entity.position.Y,
                    entity.size.Width,
                    entity.size.Height
                );
            }

            if (entity.position.Y + entity.size.Height > gameBounds.Height)
            {
                entity.position.Y = gameBounds.Height - entity.size.Height;
                entity.velocity.Y = 0;
                entity.acceleration.Y = 0;
            }

            if (entity.position.X + entity.size.Width > gameBounds.Width)
            {
                entity.position.X = gameBounds.Width - entity.size.Width;
                entity.velocity.X *= -2;
                entity.acceleration.X = 0;
            }

            if (entity.position.X < 0)
            {
                entity.position.X = 0;
                entity.velocity.X *= -2;
                entity.acceleration.X = 0;
            }
        }

        void DrawLoop()
        {
            using (Graphics g = Graphics.FromImage(backBuffer))
            {
                //g.Clear(background);
                float para = 50;
                float xD = ((1 - (player.position.X / backBuffer.Width)) * para) - para;
                float yD = ((1 - (player.position.Y / backBuffer.Height)) * para) - para;
                g.DrawImage(backgroundImage, xD, yD, backBuffer.Width + para, backBuffer.Height + para);

                foreach (var entity in entities)
                    entity.Draw(g);
            }

            area.DrawImageUnscaled(backBuffer, 0, 0);
        }

        void UpdateNetwork()
        {
            if (client == null) return;
            string data = $"{player.position.X}/{player.position.Y}/{player.velocity.X}/{player.velocity.Y}";
            client.fire("update", data);
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            client = net.startClient("127.0.0.1");

            void handleData(string data)
            {
                if (string.IsNullOrWhiteSpace(data)) return;

                string[] clientsData = data.Split('|');

                foreach (var entry in clientsData)
                {
                    if (string.IsNullOrWhiteSpace(entry)) continue;

                    var parts = entry.Split('/');
                    if (parts.Length != 5) continue;

                    string id = parts[0];
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float vx = float.Parse(parts[3]);
                    float vy = float.Parse(parts[4]);

                    PhysicsEntity ent = physicsEntities.FirstOrDefault(a => a.id == id);

                    if (ent == null)
                    {
                        ent = new PhysicsEntity();
                        ent.id = id;
                        addEntity(ent);
                    }

                    ent.position.X = x;
                    ent.position.Y = y;
                    ent.velocity.X = vx;
                    ent.velocity.Y = vy;
                }
            }

            client.listen("connect", handleData);

            client.listen("update", handleData);
        }
        private void HostButton_Click(object sender, EventArgs e)
        {
            Server.Program serverProgram = new Server.Program();
            serverProgram.Start();
            Connect_Click(null, null);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Test button clicked");
            soundMachine.LoadSound("track03", "sounds/track03.mp3");
            soundMachine.Play("track03");
        }
    }
}
