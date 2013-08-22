using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Programe.Machine;
using Programe.Network;
using Programe.Network.Packets;
using Programe.Server.NetObjects;

namespace Programe.Server
{
    public static class Game
    {
        private static Random random = new Random();

        private static float width;
        private static float height;
        private static int sceneTimer;
        private static World world;
        private static List<Ship> ships;
        private static Queue<Ship> spawnQueue; 

        public static void Start(float w, float h)
        {
            width = w;
            height = h;

            // TODO: investivate fix for farseer quadreee: http://farseerphysics.codeplex.com/workitem/30555
            world = new World(new Vector2(0, 0));
            ships = new List<Ship>();
            spawnQueue = new Queue<Ship>();

            CreateBounds(width, height);

            var asteroidCount = (width * height) / 100;
            for (var i = 0; i < (int)asteroidCount; i++)
            {
                var asteroid = CreateAsteroid();
                asteroid.Position = FindOpenSpace(new Vector2(1.5f, 1.5f));
                asteroid.Rotation = (float)(random.NextDouble() * (Math.PI * 2));
            }
        }

        public static void Update()
        {
            foreach (var ship in ships.Where(s => s.Dead).ToList())
            {
                world.RemoveBody(ship.Body);
                ships.Remove(ship);
            }

            while (spawnQueue.Count > 0)
            {
                Spawn();
            }

            foreach (var ship in ships)
            {
                ship.Update();
            }

            world.Step((float)Constants.SecondsPerUpdate);

            sceneTimer++;
            if (sceneTimer >= 3)
            {
                var scene = new Scene();

                foreach (var body in world.BodyList)
                {
                    var data = (RadarData)body.UserData;

                    if (data.UserData == null)
                        continue;

                    var netObj = (NetObject)data.UserData;
                    scene.Objects.Add(netObj);
                }

                Server.Broadcast(scene, NetDeliveryMethod.UnreliableSequenced, 0);
                sceneTimer = 0;
            }
        }

        public static void Queue(Ship ship)
        {
            lock (spawnQueue)
            {
                spawnQueue.Enqueue(ship);
            }
        }

        private static void Spawn()
        {
            lock (spawnQueue)
            {
                var ship = spawnQueue.Dequeue();
                var body = CreateShip();
                body.UserData = new RadarData(RadarType.Ship, new NetShip(ship));

                body.Position = FindOpenSpace(new Vector2(2.5f, 2.5f));
                body.Rotation = (float)(random.NextDouble() * (Math.PI * 2));

                ship.Spawn(world, body);
                ships.Add(ship);
            }
        }

        private static Vector2 FindOpenSpace(Vector2 size)
        {
            Vector2 position;
            bool empty;
            do
            {
                position = new Vector2((float)random.NextDouble() * width, (float)random.NextDouble() * height);
                empty = true;

                var aabb = new AABB(position, size.X, size.Y);
                world.QueryAABB(f =>
                {
                    empty = false;
                    return true;
                }, ref aabb);
            } while (!empty); // TODO: don't loop forever if no space

            return position;
        }

        #region Physics Objects
        private static void CreateBullets(Body ship)
        {
            Func<Vector2, Body> createBullet = pos =>
            {
                var body = new Body(world);
                body.BodyType = BodyType.Dynamic;
                body.IsBullet = true;

                var shape = new CircleShape(0.15f / 4, 1);
                body.CreateFixture(shape);

                body.Position = ship.Position + ship.GetWorldVector(pos);
                body.Rotation = ship.Rotation;
                body.ApplyForce(body.GetWorldVector(new Vector2(0.0f, -15.0f)));

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

            return body;
        }

        private static Body CreateAsteroid()
        {
            var body = new Body(world);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 1f;
            body.AngularDamping = 1f;
            body.UserData = new RadarData(RadarType.Asteroid, new NetAsteroid(body));

            var shape = new CircleShape(1, 10);
            body.CreateFixture(shape);
            return body;
        }

        private static void CreateBounds(float width, float height)
        {
            var body = new Body(world);
            body.BodyType = BodyType.Static;
            body.UserData = new RadarData(RadarType.Wall);

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
        #endregion
    }
}
