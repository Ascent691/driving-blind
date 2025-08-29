using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public enum Direction
    {
        None = 0,
        North = 1,
        West = 2,
        South = 3,
        East = 4
    }

    public static class __DirectionExtensions
    {
        private static readonly Random _random = new Random();


        public static Direction CommunicateToDriver(this Direction direction)
        {
            if (_random.Next(0,2) == 0)
            {
                return (Direction)(((int)direction + 2) % 4);
            }

            return direction;
        }
    }


    public record Point(int X, int Y)
    {
        public Point Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return new Point(X, Y - 1);
                case Direction.South: return new Point(X, Y + 1);
                case Direction.West: return new Point(X + 1, Y);
                case Direction.East: return new Point(X - 1, Y);
            }

            return new Point(X, Y);
        }
    }



}
