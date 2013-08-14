using System;
using Programe.Machine;

namespace Programe.Server.Devices
{
    class Propulsion : Device
    {
        public override byte Id
        {
            get { return 1; }
        }

        public override bool InterruptRequest
        {
            get { return false; }
        }

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
            
        }
    }
}
