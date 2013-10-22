using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
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

            World = new World(new Vector2(0, 0));

            maxShips = max;
            ships = new List<Ship>();

            SpawnQueue = new ShipQueue(32);

            CreateBounds(width, height);

            var asteroidCount = (width * height) / 50;
            for (var i = 0; i < (int)asteroidCount; i++)
            {
                var position = FindOpenSpace(new Vector2(2, 2));
                if (!position.HasValue)
                    throw new Exception("oh my god too many asteroids");

                var asteroid = CreateAsteroid();
                asteroid.Position = position.Value;
                asteroid.Rotation = (float)(Random.NextDouble() * (Math.PI * 2));
            }

            Server.Connected += session =>
            {
                var scene = new Scene();
                scene.Width = width * Constants.PixelsPerMeter;
                scene.Height = height * Constants.PixelsPerMeter;

                foreach (var body in World.BodyList)
                {
                    var data = (RadarData)body.UserData;

                    if (data.UserData == null)
                        continue;

                    var netObj = (NetObject)data.UserData;
                    if (netObj.IsStatic)
                        scene.Items.Add(netObj);
                }

                Server.Send(scene, session, NetDeliveryMethod.ReliableUnordered);
            };
        }

        public static void Update()
        {
            foreach (var ship in ships.Where(s => s.Dead).ToList())
            {
                Despawn(ship);
            }

            SpawnAll();

            foreach (var ship in ships)
            {
                ship.Update();
            }

            World.Step((float)Constants.SecondsPerUpdate);

            sceneTimer++;
            if (sceneTimer >= 2) // TODO: increase this when the client lerps pls
            {
                var objects = new Objects();

                foreach (var body in World.BodyList)
                {
                    var data = (RadarData)body.UserData;

                    if (data.UserData == null)
                        continue;

                    var netObj = (NetObject)data.UserData;
                    if (!netObj.IsStatic)
                        objects.Items.Add(netObj);
                }

                Server.Broadcast(objects, NetDeliveryMethod.UnreliableSequenced, 0);
                sceneTimer = 0;
            }
        }

        private static void SpawnAll()
        {
            lock (SpawnQueue)
            {
                while (ships.Count < maxShips && SpawnQueue.Count > 0)
                {
                    var position = FindOpenSpace(new Vector2(2.5f, 2.5f));
                    if (!position.HasValue)
                        break;

                    var ship = SpawnQueue.Dequeue();
                    ships.Where(s => s.Name == ship.Name).ToList().ForEach(Despawn);

                    var body = CreateShip();
                    body.UserData = new RadarData(RadarType.Ship, new NetShip(ship));

                    body.Position = position.Value;
                    body.Rotation = (float)(Random.NextDouble() * (Math.PI * 2));

                    ship.Spawn(World, body);
                    ships.Add(ship);
                }
            }
        }

        public static void Despawn(Ship ship)
        {
            World.RemoveBody(ship.Body);
            ships.Remove(ship);
        }

        private static Vector2? FindOpenSpace(Vector2 size)
        {
            const int maxRetry = 10;

            int i = 0;
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

                i++;
            } while (i <= maxRetry && !empty);

            return position;
        }

        #region Physics Objects
        private static Body CreateShip()
        {
            var body = new Body(World);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.5f;
            body.AngularDamping = 1f;

            var head = new PolygonShape(PolygonTools.CreateRectangle(0.25f, 0.5f, new Vector2(0, -0.5f), 0), 1);
            var tail = new PolygonShape(PolygonTools.CreateRectangle(0.75f, 0.5f, new Vector2(0, 0.5f), 0), 3);
            
            body.CreateFixture(head);
            body.CreateFixture(tail);

            return body;
        }

        private static Body CreateAsteroid()
        {
            var type = (byte)Random.Next(Constants.AsteroidRadiuses.Count);

            var body = new Body(World);
            body.BodyType = BodyType.Static;
            body.UserData = new RadarData(RadarType.Asteroid, new NetAsteroid(body, type));

            var shape = new CircleShape(Constants.AsteroidRadiuses[type], 10);
            body.CreateFixture(shape);
            return body;
        }

        private static void CreateBounds(float width, float height)
        {
            var body = new Body(World);
            body.BodyType = BodyType.Static;
            body.UserData = new RadarData(RadarType.Wall);

            var bottom = new PolygonShape(PolygonTools.CreateRectangle(width / 2, 0.1f, new Vector2(width / 2, height), 0), 5);
            var top = new PolygonShape(PolygonTools.CreateRectangle(width / 2, 0.1f, new Vector2(width / 2, 0), 0), 5);
            var left = new PolygonShape(PolygonTools.CreateRectangle(0.1f, height / 2, new Vector2(0, height / 2), 0), 5);
            var right = new PolygonShape(PolygonTools.CreateRectangle(0.1f, height / 2, new Vector2(width, height / 2), 0), 5);

            body.CreateFixture(bottom);
            body.CreateFixture(top);
            body.CreateFixture(left);
            body.CreateFixture(right);
        }
        #endregion
    }
}
