using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day_11a
{
    class Program
    {
        static void Main(string[] args)
        {
            long[] code = File.ReadAllText(@"day11a-input.txt").Split(",").Select(long.Parse).ToArray();

            Bot bot = new Bot(code);
        }
    }

    class Bot
    {
        private Dictionary<(long x, long y), long> _panels = new Dictionary<(long, long), long>();
        private (long x, long y) _position = (0,0);
        private Direction _direction = Direction.Down;
        private Intcode _processor;

        public Bot(long[] code)
        {
            _processor = new Intcode(code);
            _panels.Add(_position, 1);

            (bool, long) result;
            do
            {
                if (!_panels.ContainsKey(_position)) _panels.Add(_position, 0);

                result = _processor.Process(new List<long> { _panels[_position] });
                _panels[_position] = result.Item2;
                if (result.Item1) break;

                result = _processor.Process(new List<long> { _panels[_position] });
                if (result.Item2 == 0) TurnRight();
                else if (result.Item2 == 1) TurnLeft();
                else throw new Exception($"Invalid return value received from brain: {result.Item2}");

                Move();
            } while (!result.Item1);

            Console.WriteLine($"Finished processing. Panels touched: {_panels.Count}");
            Console.WriteLine($"Min x:{_panels.OrderBy(x => x.Key.x).First().Key.x} Max x:{_panels.OrderByDescending(x => x.Key.x).First().Key.x}");
            Console.WriteLine($"Min y:{_panels.OrderBy(x => x.Key.y).First().Key.y} Max y:{_panels.OrderByDescending(x => x.Key.y).First().Key.y}");

            string[] identifier = new string[8];
            for (int i = 0; i < identifier.Length; i++) identifier[i] = "".PadLeft(50, ' ');

            foreach (var panel in _panels)
            {
                StringBuilder builder = new StringBuilder(identifier[panel.Key.y]);
                builder[(int)panel.Key.x] = panel.Value == 1 ? '8' : ' ';
                identifier[panel.Key.y] = builder.ToString();
            }

            foreach (string str in identifier) Console.WriteLine(str);
        }

        private void TurnRight() => _direction = (int)++_direction > 3 ? 0 : _direction;

        private void TurnLeft() => _direction = --_direction < 0 ? Direction.Left : _direction;

        private void Move()
        {
            _position = _direction switch
            {
                Direction.Up => (_position.x, _position.y + 1),
                Direction.Right => (_position.x + 1, _position.y),
                Direction.Down => (_position.x, _position.y - 1),
                Direction.Left => (_position.x - 1, _position.y),
                _ => throw new NotImplementedException()
            };
        }

        public enum Direction { Up, Right, Down, Left }
    }
}
