using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private (long x, long y) _position;
        private Direction _direction = Direction.Up;
        private Intcode _processor;

        public Bot(long[] code)
        {
            _processor = new Intcode(code);

            (bool, long) result;
            do
            {
                if (!_panels.ContainsKey(_position)) _panels.Add(_position, 0);

                result = _processor.Process(new List<long> { _panels[_position] });
                _panels[_position] = result.Item2;

                result = _processor.Process(null);
                if (result.Item2 == 0) TurnLeft();
                else if (result.Item2 == 1) TurnRight();
                else throw new Exception($"Invalid return value received from brain: {result.Item2}");

                Move();
            } while (!result.Item1);

            Console.WriteLine($"Finished processing. Panels touched: {_panels.Count}");
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
