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
        private static readonly Random Random = new Random();

        public static ShipQueue SpawnQueue;
        public static World World;

        private static float width;
        private static float height;
        private static int sceneTimer;

        private static int maxShips;
        private static List<Ship> ships;

        public static void Start(float w, float h, int max)
        {
            width = w;
            height = h;

            // TODO: investivate fix for farseer quadreee: http://farseerphysics.codeplex.com/workitem/30555
            World = new World(new Vector2(0, 0));

            maxShips = max;
            ships = new List<Ship>();

            SpawnQueue = new ShipQueue(32);

            CreateBounds(width, height);

            var asteroidCount = (width * height) / 100;
            for (var i = 0; i < (int)asteroidCount; i++)
            {
                var asteroid = CreateAsteroid();
                asteroid.Position = FindOpenSpace(new Vector2(1.5f, 1.5f));
                asteroid.Rotation = (float)(Random.NextDouble() * (Math.PI * 2));
            }
        }

        public static void Update()
        {
            foreach (var ship in ships.Where(s => s.Dead).ToList())
            {
                World.RemoveBody(ship.Body);
                ships.Remove(ship);
            }

            // TODO: shouldn't allow multiple ships from one person
            while (ships.Count < maxShips && SpawnQueue.Count > 0)
            {
                Spawn();
            }

            foreach (var ship in ships)
            {
                ship.Update();
            }

            World.Step((float)Constants.SecondsPerUpdate);

            sceneTimer++;
            if (sceneTimer >= 1) // TODO: increase this when the client lerps pls
            {
                var scene = new Scene();

                foreach (var body in World.BodyList)
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

        private static void Spawn()
        {
            lock (SpawnQueue)
            {
                var ship = SpawnQueue.Dequeue();
                var body = CreateShip();
                body.UserData = new RadarData(RadarType.Ship, new NetShip(ship));

                body.Position = FindOpenSpace(new Vector2(2.5f, 2.5f));
                body.Rotation = (float)(Random.NextDouble() * (Math.PI * 2));

                ship.Spawn(World, body);
                ships.Add(ship);
            }
        }

        private static Vector2 FindOpenSpace(Vector2 size)
        {
            Vector2 position;
            bool empty;
            do
            {
                position = new Vector2((float)Random.NextDouble() * width, (float)Random.NextDouble() * height);
                empty = true;

                var aabb = new AABB(position, size.X, size.Y);
                World.QueryAABB(f =>
                {
                    empty = false;
                    return true;
                }, ref aabb);
            } while (!empty); // TODO: don't loop forever if no space

            return position;
        }

        #region Physics Objects
        private static Body CreateShip()
        {
            var body = new Body(World);
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
            var body = new Body(World);
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
            var body = new Body(World);
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
