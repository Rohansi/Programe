
using System.Collections.Generic;

namespace Programe.Network
{
    public static class Constants
    {
        public const string ApplicationIdentifier = "Programe";
        public const int Port = 17394;

        public const double SecondsPerUpdate = 1.0 / 60.0;
        public const int InstructionsPerSecond = 50000;

        public const float PixelsPerMeter = 64;

        public static readonly List<float> AsteroidRadiuses = new List<float> { 0.25f, 0.35f, 0.5f, 1.0f };
    }
}
