using System;
using FarseerPhysics.Dynamics;
using Programe.Machine;
using Programe.Network;

namespace Programe.Server.Devices
{
    public class Navigation : Device
    {
        private Body body;

        public override byte Id
        {
            get { return 1; }
        }

        public override bool InterruptRequest
        {
            get { return false; }
        }

        public Navigation(Body body)
        {
            this.body = body;
        }

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
            switch (machine.Registers[0xA])
            {
                case 0: // speed
                    machine.Registers[0xA] = (short)(body.LinearVelocity.X * Constants.PixelsPerMeter);
                    machine.Registers[0xB] = (short)(body.LinearVelocity.Y * Constants.PixelsPerMeter);
                    break;
                case 1: // angular speed
                    machine.Registers[0xA] = Util.ToMachineRotation(body.AngularVelocity);
                    break;
                case 2: // heading
                    machine.Registers[0xA] = Util.ToMachineRotation(body.Rotation - ((float)Math.PI / 2));
                    break;
            }
        }
    }
}
