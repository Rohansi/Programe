using System;

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
            var leftOffsetReg = b1 >> 4;
            var leftPayload = leftType == 0x12 ? machine.Memory[machine.IP++] : (short)0;

            var right = b2 & 0xFF;
            var rightType = right & 0x3F;
            var rightPtr = (right & 0x80) != 0;
            var rightOffset = (right & 0x40) != 0;
            var rightOffsetReg = b1 >> 4;
            var rightPayload = rightType == 0x12 ? machine.Memory[machine.IP++] : (short)0;

            Opcode = opcode;
            Left.Change(leftType, leftPtr, leftOffset, leftOffsetReg, leftPayload);
            Right.Change(rightType, rightPtr, rightOffset, rightOffsetReg, rightPayload);
        }
    }
}
