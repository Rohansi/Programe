using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using FarseerCircleShape = FarseerPhysics.Collision.Shapes.CircleShape;

namespace PhysicsTest
{
    class Program
    {
        private static RenderWindow window;
        private static Texture shipTex;
        private static Texture asteroidTex;
        private static World world;

        static void Main(string[] args)
        {
            window = new RenderWindow(new VideoMode(1280, 720), "", Styles.Close);
            window.SetFramerateLimit(60);
            window.Closed += (sender, eventArgs) => window.Close();

            shipTex = new Texture("Ship.png");
            asteroidTex = new Texture("Asteroid.png");

            var shipSpr = new Sprite(shipTex);
            shipSpr.Origin = new Vector2f(shipTex.Size.X / 2f, shipTex.Size.Y / 2f);

            var asteroidSpr = new Sprite(asteroidTex);
            asteroidSpr.Origin = new Vector2f(asteroidTex.Size.X / 2f, asteroidTex.Size.Y / 2f);

            world = new World(new Vector2(0, 0));

            var debugView = new SFMLDebugView(world);
            debugView.AppendFlags(DebugViewFlags.Shape);

            CreateBounds(20, 11.25f);

            var ship = CreateShip();
            ship.Position = new Vector2(3, 1);
            ship.Rotation = 1.7f;

            var ship2 = CreateShip();
            ship2.Position = new Vector2(3, 1);
            ship2.Rotation = 1.7f;

            var asteroid = CreateAsteroid();
            asteroid.Position = new Vector2(4, 4);

            window.KeyPressed += (sender, eventArgs) =>
            {
                if (eventArgs.Code == Keyboard.Key.Space)
                {
                    CreateBullets(ship);
                }
            };

            while (window.IsOpen())
            {
                window.DispatchEvents();

                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    ship.ApplyForce(ship.GetWorldVector(new Vector2(0.0f, -25.0f)));
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    ship.ApplyForce(ship.GetWorldVector(new Vector2(0.0f, 10.0f)));
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    ship.ApplyTorque(-10);
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    ship.ApplyTorque(10);
                }

                world.Step(1f / 60);

                window.Clear(Color.Black);

                shipSpr.Position = new Vector2f(ship.Position.X * 64, ship.Position.Y * 64);
                shipSpr.Rotation = (ship.Rotation * (180 / (float)Math.PI));
                window.Draw(shipSpr);

                asteroidSpr.Position = new Vector2f(asteroid.Position.X * 64, asteroid.Position.Y * 64);
                asteroidSpr.Rotation = (asteroid.Rotation * (180 / (float)Math.PI));
                window.Draw(asteroidSpr);

                var start = ship.Position;
                var step = (float)(2 * Math.PI) / 200;
                byte col = 0;
                var line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(start.X * 64, start.Y * 64), Color.White);
                for (var dir = 0f; dir <= 2 * Math.PI; dir += step)
                {
                    float min = 100;
                    byte res = 255;
                    var point = start + LengthDir(dir, 20);
                    world.RayCast((f, p, n, fr) =>
                    {
                        if (fr > min)
                            return 1;

                        min = fr;
                        res = (byte)(fr * 255);
                        point = p;
                        return fr;
                    }, start, point);

                    line[0] = new Vertex(new Vector2f(start.X * 64, start.Y * 64), new Color(col, 0, 0));
                    line[1] = new Vertex(new Vector2f(point.X * 64, point.Y * 64), new Color(col, 0, 0));
                    window.Draw(line);
                    col++;
                }

                debugView.Draw(window);

                window.Display();
            }
        }

        private static void CreateBullets(Body ship)
        {
            Func<Vector2, Body> createBullet = pos =>
            {
                var body = new Body(world);
                body.BodyType = BodyType.Dynamic;
                body.IsBullet = true;

                var shape = new FarseerCircleShape(0.15f / 4, 1);
                body.CreateFixture(shape);

                body.Position = ship.Position + ship.GetWorldVector(pos);
                body.Rotation = ship.Rotation;
                body.ApplyForce(body.GetWorldVector(new Vector2(0.0f, -15f)));

                body.OnCollision += (a, b, contact) =>
                {
                    world.RemoveBody(body);
                    return false;
                };

                return body;
            };

            createBullet(new Vector2(-0.575f, -0.20f));
            createBullet(new Vector2(0.575f, -0.20f));
        }

        private static Body CreateShip()
        {
            var body = new Body(world);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.5f;
            body.AngularDamping = 1f;

            // tip
            var rect1 = new PolygonShape(1);
            rect1.SetAsBox(0.25f, 0.5f, new Vector2(0, -0.5f), 0);

            // tail
            var rect2 = new PolygonShape(3);
            rect2.SetAsBox(0.75f, 0.5f, new Vector2(0, 0.5f), 0);

            body.CreateFixture(rect1);
            body.CreateFixture(rect2);

            body.OnCollision += (a, b, contact) =>
            {
                var hit = false;
                var linear = a.Body.LinearVelocity - b.Body.LinearVelocity;
                var angular = a.Body.AngularVelocity - b.Body.AngularVelocity;

                if (Math.Abs(linear.X) > 4 || Math.Abs(linear.Y) > 4)
                {
                    Console.WriteLine("OW linear" + DateTime.Now.Millisecond);
                    hit = true;
                }

                if (!hit && Math.Abs(angular) > 4)
                {
                    Console.WriteLine("OW angular" + DateTime.Now.Millisecond);
                }

                return true;
            };

            return body;
        }

        private static Body CreateAsteroid()
        {
            var body = new Body(world);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.1f;
            body.AngularDamping = 0.1f;

            var shape = new FarseerCircleShape(1, 25);
            body.CreateFixture(shape);
            return body;
        }

        private static void CreateBounds(float width, float height)
        {
            var body = new Body(world);
            body.BodyType = BodyType.Static;

            var bottom = new PolygonShape(5);
            bottom.SetAsBox(width / 2, 0.1f, new Vector2(width / 2, height), 0);

            var top = new PolygonShape(5);
            top.SetAsBox(width / 2, 0.1f, new Vector2(width / 2, 0), 0);

            var left = new PolygonShape(5);
            left.SetAsBox(0.1f, height / 2, new Vector2(0, height / 2), 0);

            var right = new PolygonShape(5);
            right.SetAsBox(0.1f, height / 2, new Vector2(width, height / 2), 0);

            body.CreateFixture(bottom);
            body.CreateFixture(top);
            body.CreateFixture(left);
            body.CreateFixture(right);
        }

        public static Vector2 LengthDir(float dir, float len)
        {
            return new Vector2((float)Math.Cos(dir) * len, (float)-Math.Sin(dir) * len);
        }
    }
}
