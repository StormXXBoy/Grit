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
using System.Security.Policy;
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
        Bitmap brickImage = Properties.Resources.brick;
        Bitmap backBuffer;

        Color background = Color.White;

        InputHandler input;
        SoundMachine soundMachine = new SoundMachine();

        private World world = new World();
        private List<GameSystem> systems = new List<GameSystem>();
        private int playerEntityId;

        public Form1()
        {
            InitializeComponent();

            this.Load += (s, e) => Start();
            this.Resize += (s, e) => onResize();
            gameScreen.MouseDown += (s, e) => HandleClick(s, e);

            onResize();
        }

        void onResize()
        {
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

                    //platforms.Add(new Platform(x, y, w, h));

                    int platformEntity = world.CreateEntity();
                    world.AddComponent(platformEntity, new Transform(
                        new Vector(x, y),
                        new Size(w, h)
                    ));
                    world.AddComponent(platformEntity, new Renderable { Sprite = brickImage });
                    world.AddComponent(platformEntity, new Physics { UseGravity = false, Collidable = true});

                    bufferPoint = null;
                }
            }
            else
            {
                var playerTransform = world.GetComponent<Transform>(playerEntityId);
                Vector shootDirection = new Vector(e.Location.X, e.Location.Y) - playerTransform.Position;

                int bulletEntity = world.CreateEntity();
                world.AddComponent(bulletEntity, new Transform(
                    new Vector(playerTransform.Position),
                    new Size(10, 10)
                ));
                world.AddComponent(bulletEntity, new Renderable { Color = Color.SteelBlue });
                world.AddComponent(bulletEntity, new Physics { UseGravity = true });
                world.AddComponent(bulletEntity, new Bullet { Lifetime = 5f, Damage = 10f });

                // Set bullet velocity
                var bulletTransform = world.GetComponent<Transform>(bulletEntity);
                bulletTransform.Velocity = new Vector(playerTransform.Velocity) + (shootDirection.normalize() * 50f);
                world.SetComponent(bulletEntity, bulletTransform);
            }
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

            playerEntityId = world.CreateEntity();
            world.AddComponent(playerEntityId, new Transform(new Vector(0, 0), new Size(10, 20)));
            world.AddComponent(playerEntityId, new Renderable { Color = Color.SteelBlue });
            world.AddComponent(playerEntityId, new Physics
            {
                UseGravity = true,
                Mass = 1f,
                LastJumpTime = DateTime.Now
            });
            world.AddComponent(playerEntityId, new Player
            {
                MoveSpeed = 1f,
                JumpForce = 10f
            });

            soundMachine.LoadSound("jump", "sounds/jump.mp3");

            //platforms.Add(new Platform(100, gameBounds.Height - 30, 200, 20));
            //platforms.Add(new Platform(350, 200, 200, 20));

            systems.Add(new InputSystem(world, input));
            systems.Add(new PhysicsSystem(world));
            systems.Add(new RenderSystem(world, area, backBuffer, backgroundImage, playerEntityId));

            StartLoop();
        }

        DateTime lastUpdate;

        void StartLoop()
        {
            lastUpdate = DateTime.Now;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += TimerTick;
            timer.Start();
        }

        void TimerTick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            float dt = (float)(now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            Loop(dt);
        }

        void Loop(float dt)
        {
            foreach (var system in systems)
            {
                system.Update(dt);
            }
        }

        void UpdateNetwork()
        {
            if (client == null) return;

            var playerTransform = world.GetComponent<Transform>(playerEntityId);
            string data = $"{playerTransform.Position.X}/{playerTransform.Position.Y}/{playerTransform.Velocity.X}/{playerTransform.Velocity.Y}";
            client.fire("update", data);
        }

        private void button1_Click(object sender, EventArgs e)
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

                    // Find or create networked entity
                    var networkedEntities = world.GetEntities<Networked>();
                    int networkEntity = -1;

                    foreach (var entity in networkedEntities)
                    {
                        var networked = world.GetComponent<Networked>(entity);
                        if (networked.NetworkId == id)
                        {
                            networkEntity = entity;
                            break;
                        }
                    }

                    if (networkEntity == -1)
                    {
                        networkEntity = world.CreateEntity();
                        world.AddComponent(networkEntity, new Transform(new Vector(x, y), new Size(20, 20)));
                        world.AddComponent(networkEntity, new Renderable { Color = Color.Blue });
                        world.AddComponent(networkEntity, new Networked { NetworkId = id });
                    }

                    var transform = world.GetComponent<Transform>(networkEntity);
                    transform.Position.X = x;
                    transform.Position.Y = y;
                    transform.Velocity.X = vx;
                    transform.Velocity.Y = vy;
                    world.SetComponent(networkEntity, transform);
                }
            }

            client.listen("connect", handleData);
            client.listen("update", handleData);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Test button clicked");
        }
    }
}