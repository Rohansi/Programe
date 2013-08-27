using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Programe.Machine;
using Programe.Network;
using Programe.Machine.Devices;
using Programe.Server.NetObjects;

namespace Programe.Server
{
    public class Ship
    {
        public readonly string Name;
        public float Health { get; private set; }
        public bool Dead { get; private set; }
        public Body Body { get; private set; }

        private readonly VirtualMachine machine;
        private Timer timer;
        private Navigation navigation;
        private Engines engines;
        private Radar radar;
        private Guns guns;

        public Ship(string name, short[] program)
        {
            Name = name;
            Health = 100;

            machine = new VirtualMachine();
            for (var i = 0; i < Math.Min(program.Length, short.MaxValue); i++)
            {
                machine.Memory[i] = program[i];
            }
        }

        public void Spawn(World world, Body body)
        {
            Body = body;

            timer = new Timer();
            machine.Attach(timer);

            navigation = new Navigation(body);
            machine.Attach(navigation);

            engines = new Engines();
            machine.Attach(engines);

            radar = new Radar(world, body);
            machine.Attach(radar);

            guns = new Guns();
            machine.Attach(guns);

            body.OnCollision += (a, b, contact) =>
            {
                var other = (RadarData)b.Body.UserData;
                if (other != null && other.Type == RadarType.Bullet)
                {
                    Damage(5);
                    return true;
                }

                var linear = a.Body.LinearVelocity - b.Body.LinearVelocity;
                var angular = a.Body.AngularVelocity - b.Body.AngularVelocity;

                if (Math.Abs(linear.X) > 4 || Math.Abs(linear.Y) > 4 || Math.Abs(angular) > 4)
                {
                    Damage(5);
                }

                return true;
            };
        }

        public void Update()
        {
            if (Health <= 0)
            {
                Dead = true;
                return;
            }

            radar.Update();
            timer.Update();

            if (guns.Update())
                CreateBullets();

            try
            {
                const int steps = (int)(Constants.SecondsPerUpdate * Constants.InstructionsPerSecond);
                for (var i = 0; i < steps; i++)
                {
                    machine.Step();
                }
            }
            catch (VirtualMachineException e)
            {
                // TODO: save error message for the ship's owner
                Console.WriteLine(e);
                Dead = true;
                return;
            }

            Body.ApplyForce(Body.GetWorldVector(new Vector2(0.0f, engines.Thruster * 25)));
            Body.ApplyTorque(engines.AngularThruster * 10);
        }

        public void Damage(float amount)
        {
            Health -= amount;

            if (Health <= 0)
                Dead = true;
        }

        private void CreateBullets()
        {
            Func<Vector2, Body> createBullet = pos =>
            {
                var body = new Body(Game.World);
                body.BodyType = BodyType.Dynamic;
                body.IsBullet = true;
                body.UserData = new RadarData(RadarType.Bullet, new NetBullet(body));

                var shape = new CircleShape(0.15f / 4, 1);
                body.CreateFixture(shape);

                body.Position = Body.Position + Body.GetWorldVector(pos);
                body.Rotation = Body.Rotation;
                body.ApplyForce(body.GetWorldVector(new Vector2(0.0f, -10f)));

                body.OnCollision += (a, b, contact) =>
                {
                    Game.World.RemoveBody(body);
                    return false;
                };

                return body;
            };

            createBullet(new Vector2(-0.575f, -0.20f));
            createBullet(new Vector2(0.575f, -0.20f));
        }
    }
}
