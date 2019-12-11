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
        private List<(int x, int y, int color)> _panels = new List<(int x, int y, int color)>();
        private Direction _currentDirection = Direction.Up;
        private Intcode _processor;

        public Bot(long[] code)
        {
            _processor = new Intcode(code);
        }

        private void TurnRight() => _currentDirection = (int)(++_currentDirection) > 3 ? 0 : _currentDirection;
        private void TurnLeft() => _currentDirection = (int)(--_currentDirection) < 0 ? Direction.Left : _currentDirection;

        public enum Direction { Up, Right, Down, Left }
    }
}
