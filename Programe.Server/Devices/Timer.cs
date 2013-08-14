using System;
using Programe.Machine;

namespace Programe.Server.Devices
{
    class Timer : Device
    {
        private const int ClocksPerTick = 1000;

        private int clocks;

        public override byte Id
        {
            get { return 0; }
        }

        public override bool InterruptRequest
        {
            get
            {
                clocks++;
                return clocks >= ClocksPerTick;
            }
        }

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            clocks -= ClocksPerTick;
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
            
        }
    }
}
