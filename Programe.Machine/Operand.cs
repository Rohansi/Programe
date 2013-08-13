using System;

namespace Programe.Machine
{
    public class Operand
    {
        private VirtualMachine machine;
        private int type;
        private bool pointer;
        private bool offset;
        private int offsetRegister;
        private short payload;

        public Operand(VirtualMachine machine)
        {
            this.machine = machine;
        }
    
        public void Change(int newType, bool newPointer, bool newOffset, int newOffsetRegister, short newPayload)
        {
            type = newType;
            pointer = newPointer;
            offset = newOffset;
            offsetRegister = newOffsetRegister;
            payload = newPayload;
        }

        public short Get(bool resolvePointer = true)
        {
            short value = 0;
            if (type <= 0x0F)
                value = machine.Registers[type];
            if (type == 0x10)
                value = machine.IP;
            if (type == 0x11)
                value = machine.SP;
            if (type == 0x12)
                value = payload;

            if (pointer && offset)
                value += machine.Registers[offsetRegister];

            if (pointer && resolvePointer)
                value = machine.Memory[value];

            return value;
        }

        public void Set(short value)
        {
            if (!pointer)
            {
                if (type <= 0x0F)
                    machine.Registers[type] = value;
                if (type == 0x10)
                    machine.IP = value;
                if (type == 0x11)
                    machine.SP = value;

                return;
            }

            machine.Memory[Get(false)] = value;
        }
    }
}
