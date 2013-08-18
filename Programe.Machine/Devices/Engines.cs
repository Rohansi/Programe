using System;

namespace Programe.Machine.Devices
{
    public class Engines : Device
    {
        public override byte Id
        {
            get { return 3; }
        }

        public override bool InterruptRequest
        {
            get { return false; }
        }

        public float Thruster { get; private set; }
        public float AngularThruster { get; private set; }

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
            switch (machine.Registers[0xA])
            {
                case 0: // set thruster speed
                    Thruster = Util.Clamp(machine.Registers[0xB] / 100f, -1, 1) * -1;
                    break;

                case 1: // set angular thrusters
                    AngularThruster = Util.Clamp(machine.Registers[0xB] / 100f, -1, 1);
                    break;
            }
        }
    }
}
