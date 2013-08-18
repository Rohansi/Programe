using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Programe.Machine;
using Programe.Network;
using Programe.Server.Devices;

namespace Programe.Server
{
    class Ship
    {
        public readonly string Name;
        public bool Dead { get; private set; }
        public Body Body { get; private set; }

        private readonly VirtualMachine machine;
        private Timer timer;
        private Navigation navigation;
        private Engines engines;
        private Radar radar;

        public Ship(string name, short[] program)
        {
            Name = name;

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
        }

        public void Update()
        {
            if (Dead)
                return;

            radar.Update();
            timer.Update();

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
                Dead = true;
                return;
            }

            Body.ApplyForce(Body.GetWorldVector(new Vector2(0.0f, engines.Thruster * 25f)));
            Body.ApplyTorque(engines.AngularThruster * 5f);
        }
    }
}
