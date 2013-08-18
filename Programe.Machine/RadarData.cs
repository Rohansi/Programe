using System;

namespace Programe.Machine
{
    /// <summary>
    /// Stores information for the radar.
    /// </summary>
    public class RadarData
    {
        /// <summary>
        /// Type to report on the radar
        /// </summary>
        public readonly byte Type;

        /// <summary>
        /// Additional user data (Ship, etc)
        /// </summary>
        public readonly object Data;

        public RadarData(byte type, object data = null)
        {
            Type = type;
            Data = data;
        }
    }
}
