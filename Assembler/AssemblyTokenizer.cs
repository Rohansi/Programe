using System;
using System.Collections.Generic;
using System.Linq;
using Programe.Machine;

namespace Assembler
{
    public enum TokenType
    {
        EndOfFile, Number, String,
        Word, Opcode, Register, Label, Comma, OpenBracket, CloseBracket, Plus
    }

    public struct Token
    {
        public readonly TokenType Type;
        public readonly int Line;
        public readonly string Value;

        public Token(TokenType type, string value = null, int line = -1)
        {
            Type = type;
            Line = line;
            Value = value;
        }
    }

    public class AssemblyTokenizer
    {
        private readonly string source;
        private readonly List<Token> tokens;
        private bool hasTokenized;

        public AssemblyTokenizer(string input)
        {
            source = input;
            tokens = new List<Token>();
        }

        public TokenList<Token> Tokens
        {
            get
            {
                if (!hasTokenized)
                    throw new InvalidOperationException("Scan() has not been called");
                var end = new Token(TokenType.EndOfFile, line: tokens[tokens.Count - 1].Line);
                return new TokenList<Token>(tokens, end);
            }
        }

        public void Scan()
        {
            if (hasTokenized)
                throw new InvalidOperationException("Scan() has already been called");

            var tokenizer = new Tokenizer(source);
            tokenizer.Scan();

            var t = tokenizer.Tokens;
            for (var i = 0; t[i].Type != BasicTokenType.EndOfFile; i++)
            {
                var tok = t[i];

                switch (tok.Type)
                {
                    case BasicTokenType.Word:
                    {
                        Opcode opcode;
                        Register register;

                        if (Enum.TryParse(tok.Value, true, out opcode) && opcode < Opcode.None)
                        {
                            tokens.Add(new Token(TokenType.Opcode, tok.Value.ToLower(), tok.Line));
                        }
                        else if (Enum.TryParse(tok.Value, true, out register))
                        {
                            tokens.Add(new Token(TokenType.Register, tok.Value.ToLower(), tok.Line));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Word, tok.Value, tok.Line));
                        }

                        break;
                    }

                    case BasicTokenType.Delimiter:
                    {
                        if (tok.Value == ",")
                        {
                            tokens.Add(new Token(TokenType.Comma, tok.Value, tok.Line));
                            break;
                        }

                        if (tok.Value == "[")
                        {
                            tokens.Add(new Token(TokenType.OpenBracket, tok.Value, tok.Line));
                            break;
                        }

                        if (tok.Value == "]")
                        {
                            tokens.Add(new Token(TokenType.CloseBracket, tok.Value, tok.Line));
                            break;
                        }

                        if (tok.Value == ":" && tokens.Count > 0)
                        {
                            var last = tokens[tokens.Count - 1];
                            if (last.Type == TokenType.Word)
                            {
                                var periodCount = last.Value.Count(c => c == '.');

                                if (periodCount > 1)
                                {
                                    throw new AssemblerException(string.Format("Label with more than one period on line {0}", last.Line));
                                }

                                tokens.RemoveAt(tokens.Count - 1);
                                tokens.Add(new Token(TokenType.Label, last.Value, last.Line));
                                break;
                            }
                        }

                        if (tok.Value == "+")
                        {
                            tokens.Add(new Token(TokenType.Plus, tok.Value, tok.Line));
                            break;
                        }

                        throw new AssemblerException(string.Format("Unexpected delimiter '{0}' on line {1}", tok.Value, tok.Line));
                    }

                    case BasicTokenType.Number:
                    {
                        tokens.Add(new Token(TokenType.Number, tok.Value, tok.Line));
                        break;
                    }

                    case BasicTokenType.String:
                    {
                        tokens.Add(new Token(TokenType.String, tok.Value, tok.Line));
                        break;
                    }

                    default:
                        throw new AssemblerException(string.Format("Unhandled BasicToken {0} on line {1}", tok.Type, tok.Line));
                }
            }

            hasTokenized = true;
        }
    }
}
