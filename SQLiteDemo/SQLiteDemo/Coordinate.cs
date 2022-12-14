using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class Coordinate
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Coordinate()
        {
        }

        public delegate double CompareFunc(double value1, double value2);

        public static Coordinate CompareCoordinates(Coordinate coordinate1, Coordinate coordinate2, CompareFunc compareValue)
        {
            double maxX = compareValue(coordinate2.X, coordinate1.X);
            double maxy = compareValue(coordinate2.Y, coordinate1.Y);

            return new Coordinate(maxX, maxy);
        }
    }
}
