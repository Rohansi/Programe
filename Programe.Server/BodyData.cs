using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programe.Server
{
    public class BodyData
    {
        public readonly byte Type;
        public readonly object Data;

        public BodyData(byte type, object data = null)
        {
            Type = type;
            Data = data;
        }
    }
}
