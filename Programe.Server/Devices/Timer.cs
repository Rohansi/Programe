using System;
using Programe.Machine;
using Programe.Network;

namespace Programe.Server.Devices
{
    public class Timer : Device
    {
        private const double TickEvery = 0.1;

        private double timer;
        private bool interruptRequest;

        public override byte Id
        {
            get { return 0; }
        }

        public override bool InterruptRequest
        {
            get { return interruptRequest; }
        }

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            timer -= TickEvery;
            interruptRequest = timer >= TickEvery;
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
        
        }

        public void Update()
        {
            timer += Constants.SecondsPerUpdate;
            interruptRequest = timer >= TickEvery;
        }
    }
}
