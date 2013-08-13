using System;
using System.Collections.Generic;

namespace Assembler
{
    class Assembler
    {
        private TokenList<Token> tokens;
        private int pos;
        private List<Instruction> instructions;
        private Dictionary<string, Label> labels;
        private byte[] binary;

        public byte[] Binary
        {
            get
            {
                if (binary == null)
                    Build();
                return binary;
            }
        }

        public Assembler(string source)
        {
            var tokenizer = new AssemblyTokenizer(source);
            tokenizer.Scan();

            tokens = tokenizer.Tokens;
            instructions = new List<Instruction>();
            labels = new Dictionary<string, Label>();

            Parse();
        }

        public void Locate(short address)
        {
            var offset = 0;
            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                if (offset == address)
                {
                    throw new ArgumentException(string.Format("Address refers to instruction on line {0}", instruction.Line));
                }

                if (offset > address)
                {
                    throw new ArgumentException(string.Format("Address refers to something on line {0} (wrong file?)", instruction.Line));
                }

                var shorts = instruction.Assemble();
                offset += shorts.Length;
            }

            throw new ArgumentException("Address not found (wrong file?)");
        }

        private void Build()
        {
            var offset = 0;

            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                foreach (var label in labels.Values)
                {
                    if (label.Index == i)
                        label.Address = offset;
                }

                var shorts = instruction.Assemble();
                offset += shorts.Length;
            }

            if (offset > short.MaxValue)
                throw new AssemblerException(string.Format("Programs must be less than 32767 words, currently using {0} words", offset));

            foreach (Label label in labels.Values)
            {
                if (label.Address == 0 && label.Index >= instructions.Count)
                    label.Address = offset;
            }

            var assembled = new List<short>();
            foreach (var instruction in instructions)
            {
                Label label;

                if (instruction.Left != null && instruction.Left.Type == OperandType.Label)
                {
                    if (!labels.TryGetValue(instruction.Left.Label, out label))
                        throw new AssemblerException(string.Format("Unresolved label '{0}' on line {1}.", instruction.Left.Label,
                                                                    instruction.Left.Line));

                    instruction.Left.Payload = (short)label.Address;
                }

                if (instruction.Right != null && instruction.Right.Type == OperandType.Label)
                {
                    if (!labels.TryGetValue(instruction.Right.Label, out label))
                        throw new AssemblerException(string.Format("Unresolved label '{0}' on line {1}.", instruction.Right.Label, instruction.Right.Line));

                    instruction.Right.Payload = (short)label.Address;
                }

                assembled.AddRange(instruction.Assemble());
            }

            var assembledArray = assembled.ToArray();
            binary = new byte[assembled.Count * 2];
            Buffer.BlockCopy(assembledArray, 0, Binary, 0, Binary.Length);
        }

        private void Parse()
        {
            var t = tokens[pos];

            while (t.Type != TokenType.EndOfFile)
            {
                if (t.Type == TokenType.Label)
                {
                    if (labels.ContainsKey(t.Value))
                        throw new AssemblerException(string.Format("Duplicate label '{0}' on line {1}.", t.Value, t.Line));

                    labels.Add(t.Value, new Label(t.Value, instructions.Count));
                    pos++;
                }
                else if (t.Type == TokenType.Keyword)
                {
                    instructions.Add(ParseInstruction());
                }
                else if (t.Type == TokenType.Word && t.Value.ToLower() == "dat")
                {
                    t = tokens[++pos];

                    var data = new DataInstruction();
                    while (t.Type == TokenType.Number)
                    {
                        short value;
                        if (!short.TryParse(t.Value, out value))
                            throw new AssemblerException(string.Format("Value out of range (-32768 to 32767) on line {0}.", t.Line));

                        data.Add(value);

                        pos++;

                        if (tokens[pos].Type == TokenType.Comma)
                        {
                            t = tokens[++pos];
                            continue;
                        }

                        break;
                    }

                    instructions.Add(data);
                }
                else
                {
                    throw new AssemblerException(string.Format("Unexpected {0} on line {1}.", t.Type, t.Line));
                }

                t = tokens[pos];
            }
        }

        #region Operand Counts
        private static Dictionary<Opcode, int> operandCounts = new Dictionary<Opcode, int>
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
        #endregion

        private Instruction ParseInstruction()
        {
            var t = tokens[pos++];

            Opcode opcode;
            if (!Enum.TryParse(t.Value, true, out opcode))
                throw new AssemblerException(string.Format("Expected opcode on line {0}.", t.Line));

            var operandCount = operandCounts[opcode];

            if (operandCount == 0)
                return new Instruction(t.Line, opcode, null, null);

            var left = ParseOperand();

            if (operandCount == 1)
                return new Instruction(t.Line, opcode, left, null);

            Require(TokenType.Comma);
            var right = ParseOperand();
            return new Instruction(t.Line, opcode, left, right);
        }

        private Operand ParseOperand()
        {
            var t = tokens[pos];
            var pointer = false;

            if (t.Type == TokenType.OpenBracket)
            {
                pointer = true;
                pos++;
            }

            try
            {
                var offset = false;
                var offsetRegister = Register.R0;

                if (tokens[pos].Type == TokenType.Word && tokens[pos + 1].Type == TokenType.Plus)
                {
                    if (!Enum.TryParse(tokens[pos].Value, true, out offsetRegister))
                    {
                        throw new AssemblerException(string.Format("Expected register on line {0}", tokens[pos].Line));
                    }
                    else
                    {
                        if (!pointer)
                            throw new AssemblerException(string.Format("Offset should only be used for pointers on line {0}", tokens[pos].Line));
                        if (offsetRegister > Register.RF)
                            throw new AssemblerException(string.Format("IP or SP used in offset on line {0}", tokens[pos].Line));
                        offset = true;
                        pos += 2;
                    }
                }

                t = tokens[pos++];

                if (t.Type == TokenType.Word)
                {
                    Register register;
                    if (Enum.TryParse(t.Value, true, out register))
                    {
                        return Operand.FromRegister(register, pointer, offset, offsetRegister);
                    }
                    else
                    {
                        return Operand.FromLabel(t.Value, t.Line, pointer, offset, offsetRegister);
                    }
                }

                if (t.Type == TokenType.Number)
                {
                    return Operand.FromNumber(short.Parse(t.Value), pointer, offset, offsetRegister);
                }

                throw new AssemblerException(string.Format("Expected operand on line {0}.", t.Line));
            }
            finally
            {
                if (pointer)
                {
                    Require(TokenType.CloseBracket);
                }
            }
        }

        private void Require(TokenType tokenType)
        {
            if (tokens[pos++].Type != tokenType)
                throw new AssemblerException(string.Format("Expected {0} on line {1}", tokenType, tokens[pos - 1].Line));
        }
    }
}
