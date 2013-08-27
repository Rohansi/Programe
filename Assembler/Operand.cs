using System;

namespace Assembler
{
    public enum Register
    {
        R0, R1, R2, R3, R4, R5, R6, R7, R8, R9, RA, RB, RC, RD, RE, RF, IP, SP
    }

    public enum OperandType
    {
        Register, Number, Label
    }

    public class Operand
    {
        public OperandType Type;

        public Register Register;
        public bool Pointer;
        public bool Offset;
        public Register OffsetRegister;
        public short? Payload;

        public string Label;
        public int Line;

        public int TypeId
        {
            get
            {
                if (Type == OperandType.Register)
                    return (int)Register;
                if (Type == OperandType.Number || Type == OperandType.Label)
                    return 0x12;

                throw new NotImplementedException();
            }
        }

        public static Operand FromNumber(short number, bool pointer, bool offset, Register offsetRegister)
        {
            return new Operand
            {
                Type = OperandType.Number,
                Payload = number,
                Pointer = pointer,
                Offset = offset,
                OffsetRegister = offsetRegister
            };
        }

        public static Operand FromRegister(Register register, bool pointer, bool offset, Register offsetRegister)
        {
            return new Operand
            {
                Type = OperandType.Register,
                Register = register,
                Payload = null,
                Pointer = pointer,
                Offset = offset,
                OffsetRegister = offsetRegister
            };
        }

        public static Operand FromLabel(string label, int line, bool pointer, bool offset, Register offsetRegister)
        {
            return new Operand
            {
                Type = OperandType.Label,
                Label = label,
                Line = line,
                Payload = 0,
                Pointer = pointer,
                Offset = offset,
                OffsetRegister = offsetRegister
            };
        }
    }
}
