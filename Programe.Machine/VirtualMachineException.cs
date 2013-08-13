using System;

namespace Programe.Machine
{
    public class VirtualMachineException : Exception
    {
        public VirtualMachineException(short ip, string message)
            : base(string.Format("{0:X4}: {1}", ip, message))
        {
            
        }

        public VirtualMachineException(short ip, string message, Exception innerException)
            : base(string.Format("{0:X4}: {1}", ip, message), innerException)
        {

        }
    }
}
