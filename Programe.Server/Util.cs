﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Programe.Server.Devices;

namespace Programe.Server
{
    public static class Util
    {
        public static float Clamp(float value, float min, float max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        /// <summary>
        /// Math used to find end point of rays in radar. Should not be used for coordinates.
        /// </summary>
        public static Vector2 RadarLengthDir(float dir, float len)
        {
            return new Vector2((float)Math.Cos(dir) * len, (float)Math.Sin(dir) * len);
        }

        /// <summary>
        /// Converts radians into the machine's angular measure.
        /// </summary>
        public static short ToMachineRotation(float radians)
        {
            var value = (radians % (2 * Math.PI)) * ((Radar.Rays / 2) / Math.PI);
            if (value < 0)
                value += Radar.Rays;
            return (short)value;
        }
    }
}
