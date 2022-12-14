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

        public delegate Coordinate FindValue(Coordinate coordinate1, Coordinate coordinate2);

        public static Coordinate FindMax(Coordinate coordinate1, Coordinate coordinate2)
        {
            double maxX = Math.Max(coordinate2.X, coordinate1.X);
            double maxy = Math.Max(coordinate2.Y, coordinate1.Y);

            return new Coordinate(maxX, maxy);
        }

        public static Coordinate FindMin(Coordinate coordinate1, Coordinate coordinate2)
        {
            double minX = Math.Min(coordinate2.X, coordinate1.X);
            double miny = Math.Min(coordinate2.Y, coordinate1.Y);

            return new Coordinate(minX, miny);
        }

 
    }
}
