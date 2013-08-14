using System;
using System.Collections.Generic;
using Programe.Machine;

namespace Assembler
{
    public class AssemblerInstruction
    {
        public readonly Opcode Opcode;
        public readonly Operand Left;
        public readonly Operand Right;
        public readonly int Line;

        public AssemblerInstruction()
        {
            Line = -1;
            Opcode = Opcode.None;
            Left = null;
            Right = null;
        }

        public AssemblerInstruction(int line, Opcode opcode, Operand left, Operand right)
        {
            Line = line;
            Opcode = opcode;
            Left = left;
            Right = right;
        }

        public virtual short[] Assemble()
        {
            var opcode = new byte[4];
            opcode[1] = (byte)Opcode;

            if (Left != null)
            {
                opcode[3] = (byte)Left.TypeId;

                if (Left.Pointer)
                    opcode[3] |= 0x80;

                if (Left.Offset)
                {
                    opcode[3] |= 0x40;
                    opcode[0] |= (byte)((int)Left.OffsetRegister << 4);
                }
            }

            if (Right != null)
            {
                opcode[2] = (byte)Right.TypeId;

                if (Right.Pointer)
                    opcode[2] |= 0x80;

                if (Right.Offset)
                {
                    opcode[2] |= 0x40;
                    opcode[0] |= (byte)((int)Right.OffsetRegister & 15);
                }
            }

            var shorts = new List<short>
            {
                BitConverter.ToInt16(opcode, 0),
                BitConverter.ToInt16(opcode, 2)
            };

            if (Left != null && Left.Payload.HasValue)
                shorts.Add(Left.Payload.Value);

            if (Right != null && Right.Payload.HasValue)
                shorts.Add(Right.Payload.Value);

            return shorts.ToArray();
        }
    }

    public class DataInstruction : AssemblerInstruction
    {
        private List<short> data = new List<short>();
        
        public void Add(short value)
        {
            data.Add(value);
        }

        public override short[] Assemble()
        {
            return data.ToArray();
        }
    }
}
