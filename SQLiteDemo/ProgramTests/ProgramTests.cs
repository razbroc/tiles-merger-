using Moq;
using SQLiteDemo;
using System.Data.SQLite;

namespace ProgramTests
{
    [TestClass]
    public class DBTests
    {/*
        #region consts

        const int X_MIN_COL = 5;
        const int Y_MIN_COL = 6;
        const int X_MAX_COL = 7;
        const int Y_MAX_COL = 8;

        #endregion
        */

        #region mocks

        private Mock<SQLiteConnection> SQLiteConnectionMock;
        private Mock<SQLiteCommand> SQLiteCommandMock;
        private Mock<SQLiteDataReader> SQLiteDataReaderMock;
        /*
         * Random r = new Random();
            int range = 9;
            byte[] byteArr = new byte[] { (byte)r.Next(1, range), (byte)r.Next(1, range), (byte)r.Next(1, range), };
         *  SQLiteDataReaderMock.Setup(x => x.GetInt32(It.IsAny<int>())).Returns(r.Next(1, range));
            SQLiteDataReaderMock.Setup(x => x.GetValue(It.IsAny<int>())).Returns(byteArr);
         */


        #endregion

        [TestMethod]
        public void AxisEdgeCoordinategffgs()
        {
            // ARRANGE
            SQLiteDataReaderMock = new Mock<SQLiteDataReader>(MockBehavior.Loose);
            SQLiteDataReaderMock.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false) ;
            SQLiteDataReaderMock.SetupSequence(x => x.GetDouble(It.IsAny<int>())).Returns(new Queue<double>(new[] { 1.35, 1.123, 34.3258795317408, 31.23180570002 }).Dequeue);

            SQLiteConnection stubConn = new SQLiteConnection();
            Coordinate Expected = new Coordinate(34.3258795317408, 31.23180570002);

            // ACT
            Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates("stub", Math.Max, 0, 0, stubConn);

            // ASSERT

            Assert.AreEqual(actualCoordinate, Expected);
        }
    }
}