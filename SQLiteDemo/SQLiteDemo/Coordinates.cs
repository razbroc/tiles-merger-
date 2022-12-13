using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class Coordinates
    {


        double X;
        double Y;

        public double GetX()
        {
            return this.X;
        }

        public double GetY()
        {
            return this.Y;
        }

        public Coordinates(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Coordinates()
        {
        }

        public Coordinates findMax(Coordinates coordinates)
        {
            double maxX = Math.Max(this.X, coordinates.X);
            double maxy = Math.Max(this.Y, coordinates.Y);

            return new Coordinates(maxX, maxy);
        }

        public Coordinates findMin(Coordinates coordinates)
        {
            double minX = Math.Min(this.X, coordinates.X);
            double miny = Math.Min(this.Y, coordinates.Y);

            return new Coordinates(minX, miny);
        }
    }
}
