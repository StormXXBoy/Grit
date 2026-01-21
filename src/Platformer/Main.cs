using GritNetworking;
using MoonSharp.Interpreter;
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
    public partial class Main : Form
    {
        public Netwerkr.Netwerkr net = new Netwerkr.Netwerkr();
        public NetwerkrClient client;

        Chat chatForm = new Chat();

        Size gameBounds = new Size(500, 500);
        Graphics area;
        Bitmap backgroundImage = Properties.Resources.background;
        Bitmap backBuffer;

        LuaEngine luaEngine = new LuaEngine();

        Color background = Color.White;

        InputHandler input;

        PlayerEntity player;
        List<Entity> entities = new List<Entity>();

        List<PlayerEntity> playerEntities = new List<PlayerEntity>();
        List<PhysicsEntity> physicsEntities = new List<PhysicsEntity>();
        List<ICollidable> collisionEntities = new List<ICollidable>();

        SoundMachine soundMachine = new SoundMachine();

        public Main()
        {
            InitializeComponent();

            this.Load += (s, e) => Start();
            this.Resize += (s, e) => onResize();
            //gameScreen.MouseDown += (s, e) => HandleClick(s, e);

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

        //Point? bufferPoint = null;
        //void HandleClick(object s, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        //if (bufferPoint == null)
        //        //{
        //        //    bufferPoint = e.Location;
        //        //}
        //        //else
        //        //{
        //        //    Point start = bufferPoint.Value;
        //        //    Point end = e.Location;

        //        //    int x = Math.Min(start.X, end.X);
        //        //    int y = Math.Min(start.Y, end.Y);
        //        //    int w = Math.Abs(end.X - start.X);
        //        //    int h = Math.Abs(end.Y - start.Y);

        //        //    addEntity(new PlatformEntity(x, y, w, h));
        //        //    bufferPoint = null;
        //        //}
        //    }
        //    else
        //    {
        //        //Vector shootDirection = new Vector(e.Location.X, e.Location.Y) - player.position;

        //        //PhysicsEntity newBullet = new PhysicsEntity();

        //        //newBullet.position = new Vector(player.center) - new Vector(0, 10);
        //        //newBullet.acceleration = new Vector(player.acceleration);
        //        //newBullet.velocity = new Vector(player.velocity) + (shootDirection.normalize() * 50f);
        //        ////player.velocity += (shootDirection.normalize() * 50f);

        //        //newBullet.size = new Vector(10, 10);

        //        //addEntity(newBullet);
        //    }
        //}

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
            if (entity is PlayerEntity plrEnt)
            {
                playerEntities.Add(plrEnt);
            }
            return entity;
        }

        T removeEntity<T>(T entity) where T : Entity
        {
            entities.Remove(entity);
            if (entity is PhysicsEntity physEnt)
            {
                physicsEntities.Remove(physEnt);
            }
            if (entity is ICollidable colEnt)
            {
                collisionEntities.Remove(colEnt);
            }
            if (entity is PlayerEntity plrEnt)
            {
                playerEntities.Remove(plrEnt);
            }
            return entity;
        }

        void initLua()
        {
            //luaEngine.RegisterObject("entities", entities);
            luaEngine.RegisterObject("player", player);
            luaEngine.RegisterObject("input", input);

            var entitiesTable = DynValue.NewTable(luaEngine.script);
            entitiesTable.Table.Set("all", luaEngine.createFunction((Func<List<Entity>>)(() => entities)));
            entitiesTable.Table.Set("physics", luaEngine.createFunction((Func<List<PhysicsEntity>>)(() => physicsEntities)));
            entitiesTable.Table.Set("collision", luaEngine.createFunction((Func<List<ICollidable>>)(() => collisionEntities)));
            entitiesTable.Table.Set("players", luaEngine.createFunction((Func<List<PlayerEntity>>)(() => playerEntities)));
            luaEngine.script.Globals["Entities"] = entitiesTable;

            luaEngine.RegisterFunction("addEntity", (Func<Entity, Entity>)((ent) => addEntity(ent)));
            luaEngine.RegisterFunction("removeEntity", (Func<Entity, Entity>)((ent) => removeEntity(ent)));
            luaEngine.RegisterFunction("entityJump", (Action<PhysicsEntity>)((ent) => entityJump(ent)));

            luaEngine.RunFile("scripts/main.lua");

            luaEngine?.Call("init");

            input.subscribeInputEvent((info) =>
            {
                luaEngine?.Call("onInput", info);
            });
        }

        void Start()
        {
            area = gameScreen.CreateGraphics();
            backBuffer = new Bitmap(gameBounds.Width, gameBounds.Height);

            input = new InputHandler(this, gameScreen);
            input.subscribeKeyDown(Keys.Escape, () => {
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

            initLua();

            StartLoop();
        }

        DateTime lastUpdate;
        float currentFPS = 60;
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        void StartLoop()
        {
            lastUpdate = DateTime.Now;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += TimerTick;
            timer.Start();
        }

        void TimerTick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            float dt = (float)(now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            currentFPS = Lerp(currentFPS, (1f / dt), 0.1f);
            FPScounter.Text = currentFPS.ToString("0") + " FPS";

            Loop(dt);
        }

        void Loop(float dt)
        {
            HandleInput(dt);
            PhysicsLoop(dt);
            UpdateNetwork();
            DrawLoop();

            luaEngine?.Call("update", dt);
        }

        bool isGrounded(Entity entity)
        {
            RectangleF feet = new RectangleF(entity.position.X, entity.position.Y + entity.size.Y, entity.size.X, 1);

            foreach (var p in collisionEntities)
            {
                if (feet.IntersectsWith(p.bounds))
                    return true;
            }

            return entity.position.Y + entity.size.Y >= gameBounds.Height;
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
            if (input.isKeyDown(Keys.Right) || input.isKeyDown(Keys.D))
            {
                player.MoveHorizontal(dt, 1);
            }
            if (input.isKeyDown(Keys.Left) || input.isKeyDown(Keys.Q))
            {
                player.MoveHorizontal(dt, -1);
            }
            if (input.isKeyDown(Keys.Up) || input.isKeyDown(Keys.Space))
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
                    if (!isGrounded(entity)) entity.acceleration.Y += gravity * dt;

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
            RectangleF entRect = new RectangleF(
                entity.position.X,
                entity.position.Y,
                entity.size.X,
                entity.size.Y
            );

            foreach (var col in collisionEntities)
            {
                if (col == entity) continue;
                RectangleF colRect = col.bounds;

                if (!entRect.IntersectsWith(colRect))
                    continue;

                if (entity.position.Y + entity.size.Y < 0) continue;

                float overlapLeft = (entRect.Right - colRect.Left);
                float overlapRight = (colRect.Right - entRect.Left);
                float overlapTop = (entRect.Bottom - colRect.Top);
                float overlapBottom = (colRect.Bottom - entRect.Top);

                float moveX = (overlapLeft < overlapRight) ? -overlapLeft : overlapRight;
                float moveY = (overlapTop < overlapBottom) ? -overlapTop : overlapBottom;

                if (axisX)
                {
                    if (Math.Abs(moveY) < 0.1) continue;

                    entity.position.X += moveX;
                    entity.velocity.X *= -0.7f;
                    entity.acceleration.X = 0;
                }
                else
                {
                    entity.position.Y += moveY;
                    entity.velocity.Y += moveY;
                    entity.acceleration.Y = 0;
                }

                entRect.X = entity.position.X;
                entRect.Y = entity.position.Y;
            }

            if (entity.position.Y + entity.size.Y > gameBounds.Height)
            {
                entity.position.Y = gameBounds.Height - entity.size.Y;
                entity.velocity.Y = 0;
                entity.acceleration.Y = 0;
            }

            if (entity.position.X + entity.size.X > gameBounds.Width)
            {
                entity.position.X = gameBounds.Width - entity.size.X;
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


        //void ResolveAxisCollision(PhysicsEntity entity, bool axisX)
        //{
        //    RectangleF rect = new RectangleF(
        //        entity.position.X,
        //        entity.position.Y,
        //        entity.size.X,
        //        entity.size.Y
        //    );

        //    foreach (var collisionEnt in collisionEntities)
        //    {
        //        if (collisionEnt == entity) continue;

        //        if (!rect.IntersectsWith(collisionEnt.bounds)) continue;

        //        if (axisX)
        //        {
        //            if (entity.velocity.X > 0)
        //                entity.position.X = collisionEnt.bounds.X - entity.size.X;
        //            else if (entity.velocity.X < 0)
        //                entity.position.X = collisionEnt.bounds.X + collisionEnt.bounds.Width;

        //            entity.velocity.X *= -0.7f;
        //            entity.acceleration.X = 0;
        //        }
        //        else
        //        {
        //            if (entity.velocity.Y > 0)
        //                entity.position.Y = collisionEnt.bounds.Y - entity.size.Y;
        //            else if (entity.velocity.Y < 0)
        //                entity.position.Y = collisionEnt.bounds.Y + collisionEnt.bounds.Height;

        //            entity.velocity.Y = 0;
        //            entity.acceleration.Y = 0;
        //        }
        //    }

        //    if (entity.position.Y + entity.size.Y > gameBounds.Height)
        //    {
        //        entity.position.Y = gameBounds.Height - entity.size.Y;
        //        entity.velocity.Y = 0;
        //        entity.acceleration.Y = 0;
        //    }

        //    if (entity.position.X + entity.size.X > gameBounds.Width)
        //    {
        //        entity.position.X = gameBounds.Width - entity.size.X;
        //        entity.velocity.X *= -2;
        //        entity.acceleration.X = 0;
        //    }

        //    if (entity.position.X < 0)
        //    {
        //        entity.position.X = 0;
        //        entity.velocity.X *= -2;
        //        entity.acceleration.X = 0;
        //    }
        //}

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
            NetEntity netPlayer = new NetEntity(new NetVector(player.position.X, player.position.Y), new NetVector(player.velocity.X, player.velocity.Y));
            client.fire("update", netPlayer.ToString());
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            client = net.startClient(ipInput.Text);
            luaEngine?.Call("serverConnected", new LuaClient(client));

            void handleData(string data)
            {
                if (string.IsNullOrWhiteSpace(data)) return;

                string[] clientsData = data.Split('|');

                foreach (var entry in clientsData)
                {
                    if (string.IsNullOrWhiteSpace(entry)) continue;

                    NetEntity netEntity = new NetEntity();
                    netEntity.UpdateFromString(entry, true);

                    PlayerEntity ent = playerEntities.FirstOrDefault(a => a.id == netEntity.clientId);

                    if (ent == null)
                    {
                        ent = new PlayerEntity();
                        ent.id = netEntity.clientId;
                        ent.sprite.image = Properties.Resources.Player;
                        addEntity(ent);
                    }

                    ent.position.X = netEntity.position.X;
                    ent.position.Y = netEntity.position.Y;
                    ent.velocity.X = netEntity.velocity.X;
                    ent.velocity.Y = netEntity.velocity.Y;
                }
            }

            client.listen("connect", handleData);

            client.listen("update", handleData);

            chatForm = new Chat();
            chatForm.Show();
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
