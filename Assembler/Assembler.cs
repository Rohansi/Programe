using System;
using System.Collections.Generic;
using Programe.Machine;

namespace Assembler
{
    class Assembler
    {
        private string source;
        private TokenList<Token> tokens;
        private int pos;
        private List<AssemblerInstruction> instructions;
        private Dictionary<string, Label> labels;
        private string parentLabel;

        public byte[] Binary;
        public DebugInfo Debug;

        public Assembler(string sourceText)
        {
            source = sourceText;

            instructions = new List<AssemblerInstruction>();
            labels = new Dictionary<string, Label>();
            Debug = new DebugInfo();
        }

        public void Assemble()
        {
            var tokenizer = new AssemblyTokenizer(source);
            tokenizer.Scan();
            tokens = tokenizer.Tokens;

            Parse();
            Build();
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
                    {
                        label.Address = offset;
                        Debug.AddSymbol(label.Name, label.Address);
                    }
                }

                Debug.AddLineAddress(instruction.Line, offset);
                var shorts = instruction.Assemble();
                offset += shorts.Length;
            }

            foreach (Label label in labels.Values)
            {
                if (label.Address == 0 && label.Index >= instructions.Count)
                    label.Address = offset;
            }

            if (offset > short.MaxValue)
                throw new AssemblerException(string.Format("Programs must be less than 32767 words, currently using {0} words", offset));

            var assembled = new List<short>();
            foreach (var instruction in instructions)
            {
                Label label;

                if (instruction.Left != null && instruction.Left.Type == OperandType.Label)
                {
                    if (!labels.TryGetValue(instruction.Left.Label, out label))
                        throw new AssemblerException(string.Format("Unresolved label '{0}' on line {1}", instruction.Left.Label,
                                                                    instruction.Left.Line));

                    instruction.Left.Payload = (short)label.Address;
                }

                if (instruction.Right != null && instruction.Right.Type == OperandType.Label)
                {
                    if (!labels.TryGetValue(instruction.Right.Label, out label))
                        throw new AssemblerException(string.Format("Unresolved label '{0}' on line {1}", instruction.Right.Label, instruction.Right.Line));

                    instruction.Right.Payload = (short)label.Address;
                }

                assembled.AddRange(instruction.Assemble());
            }

            var assembledArray = assembled.ToArray();
            Binary = new byte[assembled.Count * 2];
            Buffer.BlockCopy(assembledArray, 0, Binary, 0, Binary.Length);
        }

        private void Parse()
        {
            var t = tokens[pos];
            while (t.Type != TokenType.EndOfFile)
            {
                if (t.Type == TokenType.Label)
                {
                    var prefix = "";
                    
                    if (t.Value.StartsWith("."))
                    {
                        if (parentLabel == null)
                            throw new AssemblerException(string.Format("Local label with no parent on line {0}", t.Line));

                        prefix = parentLabel;
                    }
                    else
                    {
                        if (t.Value.Contains("."))
                            throw new AssemblerException(string.Format("Period used in global label declaration on line {0}", t.Line));

                        parentLabel = t.Value;
                    }

                    var label = prefix + t.Value;
                    if (labels.ContainsKey(label))
                        throw new AssemblerException(string.Format("Duplicate label '{0}' on line {1}", label, t.Line));

                    labels.Add(label, new Label(label, instructions.Count));
                    pos++;
                }
                else if (t.Type == TokenType.Opcode)
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
                        short.TryParse(t.Value, out value);

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
                else if (t.Type == TokenType.Word && t.Value.ToLower() == "resv")
                {
                    t = tokens[++pos];

                    if (t.Type != TokenType.Number)
                        throw new AssemblerException(string.Format("Expected Number on line {0}", t.Line));

                    var data = new DataInstruction();
                    var count = short.Parse(t.Value);

                    for (var i = 0; i < count; i++)
                    {
                        data.Add(0);
                    }

                    pos++;
                    instructions.Add(data);
                }
                else
                {
                    throw new AssemblerException(string.Format("Unexpected {0} on line {1}", t.Type, t.Line));
                }

                t = tokens[pos];
            }
        }

        private AssemblerInstruction ParseInstruction()
        {
            var t = tokens[pos++];

            Opcode opcode;
            if (!Enum.TryParse(t.Value, true, out opcode))
                throw new AssemblerException(string.Format("Expected opcode on line {0}", t.Line));

            var operandCount = Instruction.OperandCounts[opcode];

            if (operandCount == 0)
                return new AssemblerInstruction(t.Line, opcode, null, null);

            var left = ParseOperand();

            if (operandCount == 1)
                return new AssemblerInstruction(t.Line, opcode, left, null);

            Require(TokenType.Comma);
            var right = ParseOperand();
            return new AssemblerInstruction(t.Line, opcode, left, right);
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

                if (tokens[pos].Type == TokenType.Register && tokens[pos + 1].Type == TokenType.Plus)
                {
                    Enum.TryParse(tokens[pos].Value, true, out offsetRegister);

                    if (!pointer)
                        throw new AssemblerException(string.Format("Offset should only be used for pointers on line {0}", tokens[pos].Line));

                    if (offsetRegister > Register.RF)
                        throw new AssemblerException(string.Format("IP or SP used in offset on line {0}", tokens[pos].Line));

                    offset = true;
                    pos += 2;
                }

                t = tokens[pos++];

                if (t.Type == TokenType.Register)
                {
                    Register register;
                    Enum.TryParse(t.Value, true, out register);
                    return Operand.FromRegister(register, pointer, offset, offsetRegister);
                }

                if (t.Type == TokenType.Word)
                {
                    var prefix = "";
                    if (t.Value.StartsWith("."))
                        prefix = parentLabel;

                    return Operand.FromLabel(prefix + t.Value, t.Line, pointer, offset, offsetRegister);
                }

                if (t.Type == TokenType.Number)
                {
                    return Operand.FromNumber(short.Parse(t.Value), pointer, offset, offsetRegister);
                }

                throw new AssemblerException(string.Format("Expected operand on line {0}", t.Line));
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
