using System;
using System.Collections.Generic;
using System.Linq;
using Programe.Machine;
using Programe.Network;
using Programe.Server.Devices;

namespace Programe.Server
{
    class Ship
    {
        public readonly string Name;
        public bool Dead { get; private set; }

        private readonly VirtualMachine machine;

        public Ship(string name, short[] program)
        {
            Name = name;

            machine = new VirtualMachine();
            for (var i = 0; i < Math.Min(program.Length, short.MaxValue); i++)
            {
                machine.Memory[i] = program[i];
            }

            machine.Attach(new Timer());
            machine.Attach(new Propulsion());
        }

        public void Update()
        {
            const int steps = (int)(Constants.SecondsPerUpdate * Constants.InstructionsPerSecond);

            try
            {
                for (var i = 0; i < steps; i++)
                {
                    machine.Step();
                }
            }
            catch (VirtualMachineException e)
            {
                // TODO
                Dead = true;
            }
        }
    }
}
