using System;
using Programe.Network;

namespace Programe.Machine.Devices
{
    public class Timer : Device
    {
        private const int TickEvery = Constants.InstructionsPerSecond / 10;

        private int timer;

        public override byte Id
        {
            get { return 0; }
        }

        public override bool InterruptRequest
        {
            get
            {
                timer++;
                return timer >= TickEvery;
            }
        }

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            timer -= TickEvery;
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
        
        }
    }
}
