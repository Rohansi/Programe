using System;

namespace Programe.Machine
{
    public enum RadarType : byte
    {
        Wall, Ship, Asteroid
    }
    
    /// <summary>
    /// Stores information for the radar.
    /// </summary>
    public class RadarData
    {
        /// <summary>
        /// Type to report on the radar
        /// </summary>
        public readonly RadarType Type;

        /// <summary>
        /// Additional user data (Ship, etc)
        /// </summary>
        public readonly object UserData;

        public RadarData(RadarType type, object userData = null)
        {
            Type = type;
            UserData = userData;
        }
    }
}
