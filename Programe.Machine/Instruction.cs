using System;
using System.Collections.Generic;
using System.Text;

namespace Programe.Machine
{
    public enum Opcode : byte
    {
        Mov, Add, Sub, Mul, Div, Mod, Inc, Dec, Not, And, Or,
        Xor, Shl, Shr, Push, Pop, Jmp, Call, Ret, Cmp, Jz, Jnz,
        Je, Jne, Ja, Jae, Jb, Jbe, Rand, Int, Iret, Ivt, None
    }

    public class Instruction
    {
        public Opcode Opcode;
        public Operand Left;
        public Operand Right;

        private VirtualMachine machine;

        public Instruction(VirtualMachine machine)
        {
            this.machine = machine;

            Left = new Operand(machine);
            Right = new Operand(machine);
        }

        public void Decode()
        {
            var b1 = machine.Memory[machine.IP++];
            var b2 = machine.Memory[machine.IP++];

            var opcode = (Opcode)(b1 >> 8);

            var left = b2 >> 8;
            var leftType = left & 0x3F;
            var leftPtr = (left & 0x80) != 0;
            var leftOffset = (left & 0x40) != 0;
            var leftOffsetReg = (b1 >> 4) & 0x0F;
            var leftPayload = leftType == 0x12 ? machine.Memory[machine.IP++] : (short)0;

            var right = b2 & 0xFF;
            var rightType = right & 0x3F;
            var rightPtr = (right & 0x80) != 0;
            var rightOffset = (right & 0x40) != 0;
            var rightOffsetReg = b1 & 0x0F;
            var rightPayload = rightType == 0x12 ? machine.Memory[machine.IP++] : (short)0;

            Opcode = opcode;
            Left.Change(leftType, leftPtr, leftOffset, leftOffsetReg, leftPayload);
            Right.Change(rightType, rightPtr, rightOffset, rightOffsetReg, rightPayload);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Opcode.ToString().ToUpper());
            sb.Append(' ');
            sb.Append(Left);
            sb.Append(", ");
            sb.Append(Right);

            return sb.ToString();
        }

        public static readonly Dictionary<Opcode, int> OperandCounts = new Dictionary<Opcode, int>
        {
            { Opcode.Mov,     2 },
            { Opcode.Add,     2 },
            { Opcode.Sub,     2 },
            { Opcode.Mul,     2 },
            { Opcode.Div,     2 },
            { Opcode.Mod,     2 },
            { Opcode.Inc,     1 },
            { Opcode.Dec,     1 },
            { Opcode.Not,     1 },
            { Opcode.And,     2 },
            { Opcode.Or,      2 },
            { Opcode.Xor,     2 },
            { Opcode.Shl,     2 },
            { Opcode.Shr,     2 },
            { Opcode.Push,    1 },
            { Opcode.Pop,     1 },
            { Opcode.Jmp,     1 },
            { Opcode.Call,    1 },
            { Opcode.Ret,     0 },
            { Opcode.Cmp,     2 },
            { Opcode.Jz,      1 },
            { Opcode.Jnz,     1 },
            { Opcode.Je,      1 },
            { Opcode.Jne,     1 },
            { Opcode.Ja,      1 },
            { Opcode.Jae,     1 },
            { Opcode.Jb,      1 },
            { Opcode.Jbe,     1 },
            { Opcode.Rand,    1 },
            { Opcode.Int,     1 },
            { Opcode.Iret,    0 },
            { Opcode.Ivt,     1 },
        };
    }
}
