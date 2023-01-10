using SQLiteDemo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TileMergererTests
{
    [TestClass]
    public class CoordinateTests
    {
        [TestMethod]
        public void MaxEdgeBothValuesLowerThanOtherCoordinate()
        {
            double x1 = 2.1;
            double y1 = 2.2;
            double x2 = 3.3;
            double y2 = 4.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate predicted = new Coordinate(x2, y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, predicted, Math.Max);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeOneValueIsLowerThanOtherCoordinateValues()
        {
            double x1 = 2.1;
            double y1 = 2.2;
            double x2 = 3.3;
            double y2 = 1.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x2,y1);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Max);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeBothValuesAreLowerThanOtherCoordinateValues()
        {
            double x2 = 2.1;
            double y2 = 2.2;
            double x1 = 3.3;
            double y1 = 1.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y1);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Max);

            Assert.AreEqual(predicted, actual);
        }
        /*
        public void MaxEdgeBothValuesLowerThanOtherCoordinate()
        {
            double x1 = 2.1;
            double y1 = 2.2;
            double x2 = 1.3;
            double y2 = 1.4;

            Coordinate predicted = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c2, predicted, Math.Max);

            Assert.AreEqual(predicted, actual);
        }
        */
        public void MaxEdgeEqualXValuesYIsLower()
        {
            double x1= 2.1;
            double y2 = 2.2;
            double x2 = 2.1;
            double y1 = 1.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Max);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeEqualXValuesYIsBigger()
        {
            double x1= 2.1;
            double y2 = 2.2;
            double x2 = 2.1;
            double y1 = 3.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y1);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Max);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeEqualYValuesXIsLower()
        {
            double x1= 1.1;
            double y2 = 2.2;
            double x2 = 2.1;
            double y1 = 2.2;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x2,y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Max);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeEqualYValuesXIsBigger()
        {
            double x1= 2.1;
            double y2 = 2.2;
            double x2 = 1.1;
            double y1 = 2.2;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Max);

            Assert.AreEqual(predicted, actual);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MinEdgeBothValuesLowerThanOtherCoordinate()
        {
            double x1 = 2.1;
            double y1 = 2.2;
            double x2 = 3.3;
            double y2 = 4.4;

            Coordinate predicted = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c2, predicted, Math.Min);

            Assert.AreEqual(predicted, actual);
        }

        public void MinEdgeOneValueIsLowerThanOtherCoordinateValues()
        {
            double x1 = 2.1;
            double y1 = 2.2;
            double x2 = 3.3;
            double y2 = 1.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Min);

            Assert.AreEqual(predicted, actual);
        }
        /*
        public void MaxEdgeBothValuesAreLowerThanOtherCoordinateValues()
        {
            double x2 = 2.1;
            double y2 = 2.2;
            double x1 = 3.3;
            double y1 = 1.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x2,y1);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Min);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeBothValuesLowerThanOtherCoordinate()
        {
            double x1 = 2.1;
            double y1 = 2.2;
            double x2 = 1.3;
            double y2 = 1.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate predicted = new Coordinate(x2, y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, predicted, Math.Min);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeEqualXValuesYIsLower()
        {
            double x1= 2.1;
            double y2 = 2.2;
            double x2 = 2.1;
            double y1 = 1.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y1);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Min);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeEqualXValuesYIsBigger()
        {
            double x1= 2.1;
            double y2 = 2.2;
            double x2 = 2.1;
            double y1 = 3.4;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Min);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeEqualYValuesXIsLower()
        {
            double x1= 1.1;
            double y2 = 2.2;
            double x2 = 2.1;
            double y1 = 2.2;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x1,y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Min);

            Assert.AreEqual(predicted, actual);
        }

        public void MaxEdgeEqualYValuesXIsBigger()
        {
            double x1= 2.1;
            double y2 = 2.2;
            double x2 = 1.1;
            double y1 = 2.2;

            Coordinate c1 = new Coordinate(x1, y1);
            Coordinate c2 = new Coordinate(x2, y2);
            
            Coordinate predicted = new Coordinate(x2,y2);
            Coordinate actual = Coordinate.EdgeCoordinatesByEdgeSide(c1, c2, Math.Main);

            Assert.AreEqual(predicted, actual);
        }
        */
    }
}