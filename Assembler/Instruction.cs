using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    public enum Opcode : byte
    {
        Mov, Add, Sub, Mul, Div, Mod, Inc, Dec, Not, And, Or,
        Xor, Shl, Shr, Push, Pop, Jmp, Call, Ret, Cmp, Jz, Jnz,
        Je, Jne, Ja, Jae, Jb, Jbe, Rand, Int, Iret, Ivt, None
    }

    public class Instruction
    {
        public readonly Opcode Opcode;
        public readonly Operand Left;
        public readonly Operand Right;
        public readonly int Line;

        public Instruction()
        {
            Line = -1;
            Opcode = Opcode.None;
            Left = null;
            Right = null;
        }

        public Instruction(int line, Opcode opcode, Operand left, Operand right)
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
                opcode[0] |= (byte)((int)Left.Register << 4);
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
                opcode[0] |= (byte)Right.Register;
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

    public class DataInstruction : Instruction
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
