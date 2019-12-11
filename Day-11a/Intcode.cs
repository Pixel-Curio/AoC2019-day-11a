using System;
using System.Collections.Generic;

namespace Day_11a
{
    class Intcode
    {
        private long _pointer;
        private readonly long[] _code;
        private List<long> _input = new List<long>();
        private long _lastOutput = 0;

        private const string AddCommand = "01";
        private const string MultiplyCommand = "02";
        private const string InputCommand = "03";
        private const string OutputCommand = "04";
        private const string JumpIfTrueCommand = "05";
        private const string JumpIfFalseCommand = "06";
        private const string LessThanCommand = "07";
        private const string EqualsCommand = "08";
        private const string ExitCommand = "99";

        public Intcode(long[] code) => _code = code;

        public (bool, long) Process() => Process(null);

        public (bool, long) Process(List<long> input)
        {
            if (input != null) _input.AddRange(input);

            while (true)
            {
                var command = ConsumeOpCode().ToString();
                var opCode = command.Length < 2 ? "0" + command : command[^2..];
                var parameters = command.Length > 2 ? command[..^2].PadLeft(4, '0') : "0000";

                if (opCode == AddCommand) Add(parameters);
                else if (opCode == MultiplyCommand) Multiply(parameters);
                else if (opCode == InputCommand) Input(parameters);
                else if (opCode == OutputCommand) 
                    return (false, Output(parameters));
                else if (opCode == JumpIfTrueCommand) JumpIfTrue(parameters);
                else if (opCode == JumpIfFalseCommand) JumpIfFalse(parameters);
                else if (opCode == LessThanCommand) LessThan(parameters);
                else if (opCode == EqualsCommand) Equals(parameters);
                else if (opCode == ExitCommand) 
                    return (true, _lastOutput);
            }
        }

        private void Add(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = parameters[^1] == '0' ? _code.ExpandingGet(a) : a;
            b = parameters[^2] == '0' ? _code.ExpandingGet(b) : b;

            _code.ExpandingSet(target, a + b);
        }

        private void Multiply(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = parameters[^1] == '0' ? _code.ExpandingGet(a) : a;
            b = parameters[^2] == '0' ? _code.ExpandingGet(b) : b;

            _code.ExpandingSet(target, a * b);
        }

        private void Input(string parameters)
        {
            long target = ConsumeOpCode();
            long input;

            if (_input.Count > 0)
            {
                input = _input[0];
                _input.RemoveAt(0);
            }
            else
            {
                Console.WriteLine($"Waiting for input at ({target}): ");
                input = Convert.ToInt64(Console.ReadLine());
            }

            _code.ExpandingSet(target, input);
        }

        private long Output(string parameters)
        {
            long target = ConsumeOpCode();

            long value = parameters[^1] == '0' ? _code.ExpandingGet(target) : target;
            _lastOutput = value;

            return value;
        }

        private void JumpIfTrue(string parameters)
        {
            long condition = ConsumeOpCode();
            long target = ConsumeOpCode();

            condition = parameters[^1] == '0' ? _code.ExpandingGet(condition) : condition;
            target = parameters[^2] == '0' ? _code.ExpandingGet(target) : target;

            if (condition != 0) _pointer = target;
        }

        private void JumpIfFalse(string parameters)
        {
            long condition = ConsumeOpCode();
            long target = ConsumeOpCode();

            condition = parameters[^1] == '0' ? _code.ExpandingGet(condition) : condition;
            target = parameters[^2] == '0' ? _code.ExpandingGet(target) : target;

            if (condition == 0) _pointer = target;
        }

        private void LessThan(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = parameters[^1] == '0' ? _code.ExpandingGet(a) : a;
            b = parameters[^2] == '0' ? _code.ExpandingGet(b) : b;

            _code.ExpandingSet(target, a < b ? 1 : 0);
        }

        private void Equals(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = parameters[^1] == '0' ? _code.ExpandingGet(a) : a;
            b = parameters[^2] == '0' ? _code.ExpandingGet(b) : b;

            _code.ExpandingSet(target, a == b ? 1 : 0);
        }

        public long GetValue(long index) => _code[index];

        public Intcode BufferInput(List<long> input)
        {
            _input.AddRange(input);
            return this;
        }

        public long GetLastOutput() => _lastOutput;

        private long ConsumeOpCode() => _pointer < _code.Length ? _code[_pointer++] : 99;
    }

    public static class ArrayExtensions
    {
        public static void ExpandingSet(this long[] source, long index, long value)
        {
            if (index > source.Length) Array.Resize(ref source, (int)index + 1);
            source[index] = value;
        }
        public static long ExpandingGet(this long[] source, long index)
        {
            if (index > source.Length) Array.Resize(ref source, (int)index + 1);
            return source[index];
        }
    }
}