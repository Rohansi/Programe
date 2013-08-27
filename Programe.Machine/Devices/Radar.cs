using System;
using FarseerPhysics.Dynamics;
using Programe.Network;

namespace Programe.Machine.Devices
{
    public class Radar : Device
    {
        public const int Rays = 200;
        private const float MaxDistance = 20;
        private const int UpdateEvery = Constants.InstructionsPerSecond / 2; 

        private int timer;

        private World world;
        private Body body;
        private short radarPointer;
        private short[] radarData;

        public override byte Id
        {
            get { return 2; }
        }

        public override bool InterruptRequest
        {
            get
            {
                if (radarPointer == 0)
                    return false;

                timer++;
                return timer >= UpdateEvery;
            }
        }

        public Radar(World world, Body body)
        {
            radarPointer = 0;
            radarData = new short[Rays];

            this.world = world;
            this.body = body;
        }

        public override void HandleInterruptRequest(VirtualMachine machine)
        {
            timer -= UpdateEvery;

            RayCast();

            for (var i = 0; i < radarData.Length; i++)
            {
                machine.Memory[radarPointer + i] = radarData[i];
            }
        }

        public override void HandleInterrupt(VirtualMachine machine)
        {
            radarPointer = machine.Registers[0xA];
        }

        private void RayCast()
        {
            const float step = (float)(2 * Math.PI) / Rays;

            var start = body.Position;
            var i = 0;
            for (var dir = 0f; dir <= 2 * Math.PI; dir += step, i++)
            {
                byte type = 255;
                byte distance = 255;

                float min = 100;
                var point = start + Util.RadarLengthDir(dir, MaxDistance);

                world.RayCast((f, p, n, fr) =>
                {
                    if (fr > min)
                        return 1;

                    min = fr;

                    var data = (RadarData)f.Body.UserData;
                    type = (byte)data.Type;
                    distance = (byte)(fr * 255);
                    return fr;
                }, start, point);

                radarData[i] = (short)(type << 8 | distance);
            }
        }
    }
}
