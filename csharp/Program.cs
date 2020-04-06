using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace csharp
{
    public enum Token
    {
        IncrementPointer,
        DecrementPointer,
        IncrementValue,
        DecrementValue,
        Output,
        Input,
        JumpLeft,
        JumpRight,
        Invalid
    }

    public static class BrainFuck
    {
        /// <summary>
        /// トークン化します。
        /// </summary>
        public static Token[] Tokenize(string code) => 
            code.ToCharArray()
                .Select(ToToken)
                .Where(t => t != Token.Invalid)
                .ToArray();

        public static Token ToToken(char token) => token switch
        {
            '>' => Token.IncrementPointer,
            '<' => Token.DecrementPointer,
            '+' => Token.IncrementValue,
            '-' => Token.DecrementValue,
            '.' => Token.Output,
            ',' => Token.Input,
            '[' => Token.JumpLeft,
            ']' => Token.JumpRight,
             _  => Token.Invalid,
        };

        public static char ToChar(Token token) => token switch
        {
            Token.IncrementPointer => '>',
            Token.DecrementPointer => '<',
            Token.IncrementValue => '+',
            Token.DecrementValue => '-',
            Token.Output => '.',
            Token.Input => ',',
            Token.JumpLeft => '[',
            Token.JumpRight => ']',
            _ => throw new ArgumentException(nameof(token)),
        };

        public static void Run(params Token[] tokens)
        {
            var pc = 0;
            var mem = new byte[256];
            var ptr = 0;
            using var input = new BinaryReader(Console.OpenStandardInput());
            using var output = new BinaryWriter(Console.OpenStandardOutput());
            
            while (pc < tokens.Length)
            {
                Process(tokens, ref pc, mem, ref ptr, input, output);
                pc++;
            }
        }

        private static void Process(Token[] tokens, ref int pc, byte[] mem, ref int ptr, BinaryReader input, BinaryWriter output)
        {
            switch (tokens[pc])
            {
                case Token.DecrementPointer:
                    ptr--;
                    if (ptr < 0)
                    {
                        throw new Exception("ポインタが範囲外を指しました");
                    }
                    break;
                case Token.IncrementPointer:
                    ptr++;
                    break;
                case Token.DecrementValue:
                    mem[ptr]--;
                    break;
                case Token.IncrementValue:
                    mem[ptr]++;
                    break;
                case Token.Output:
                    output.Write(mem[ptr]);
                    break;
                case Token.Input:
                    mem[ptr] = input.ReadByte();
                    break;
                case Token.JumpLeft:
                    if (mem[ptr] == 0)
                    {
                        var nextPc = Array.IndexOf(tokens, Token.JumpRight, pc);
                        if (nextPc < 0)
                        {
                            throw new Exception("対となる ] がありません");
                        }
                        pc = nextPc;
                    }
                    break;
                case Token.JumpRight:
                    {
                        var nextPc = Array.LastIndexOf(tokens, Token.JumpLeft, pc);
                        if (nextPc < 0)
                        {
                            throw new Exception("対となる [ がありません");
                        }
                        pc = nextPc - 1;
                    }
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Brainfuck コードを書いてください");
                Console.Write("> ");
                var code = Console.ReadLine();
                var token = BrainFuck.Tokenize(code);
                BrainFuck.Run(token);
            }
        }
    }
}
