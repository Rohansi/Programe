using System;

namespace Programe.Network
{
    public static class NetUtil
    {
        private const float MaxRadians = (float)Math.PI * 2;

        public static ushort ToNetworkRotation(this float value)
        {
            var rotation = value % MaxRadians;
            if (rotation < 0)
                rotation += MaxRadians;
            rotation /= MaxRadians;
            return (ushort)(rotation * ushort.MaxValue);
        }

        public static float FromNetworkRotation(this ushort value)
        {
            return ((float)value / ushort.MaxValue) * MaxRadians;
        }
    }
}
