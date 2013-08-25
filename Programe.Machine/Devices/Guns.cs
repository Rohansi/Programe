using System;
using Programe.Network;

namespace Programe.Machine.Devices
{
    public class Guns : Device
    {
        private const double MaxAmmo = 100;
        private const double AmmoPerSecond = 1;
        private const double Cooldown = 0.25f;

        public override byte Id
        {
            get { return 4; }
        }

        public override bool InterruptRequest
        {
            get { return false; }
        }

        private double ammo = MaxAmmo;
        private double timer;
        private bool shooting;

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
            switch (machine.Registers[0xA])
            {
                case 0: // ammo level
                    machine.Registers[0xA] = (short)ammo;
                    break;
                case 1: // set trigger
                    shooting = machine.Registers[0xB] != 0;
                    break;
            }
        }

        public bool Update()
        {
            ammo = Math.Min(ammo + (AmmoPerSecond * Constants.SecondsPerUpdate), MaxAmmo);
            timer = Math.Max(timer - Constants.SecondsPerUpdate, 0);

            var shoot = shooting && timer <= 0 && ammo >= 1;
            if (shoot)
            {
                ammo -= 1;
                timer = Cooldown;
            }

            return shoot;
        }
    }
}
