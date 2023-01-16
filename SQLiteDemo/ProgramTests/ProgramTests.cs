using Moq;
using SQLiteDemo;
using System.Data;
using System.Data.Common;
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
        private Mock<ISqliteCommand> ISQLiteCommandMock;
        private Mock<SqliteCommandWrapper> sqliteCommandWrapper;
        private Mock<ISqlDataReader> ISQLiteDataReaderMock;
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
            SQLiteConnection fakeConnection = new SQLiteConnection($"Data Source={GetGpkgPath()}");
            fakeConnection.Open();
            var commanandOnFakeDB = new SQLiteCommand("", fakeConnection);
            ISQLiteDataReaderMock = new Mock<ISqlDataReader>(MockBehavior.Loose);
            ISQLiteCommandMock = new Mock<ISqliteCommand>();
            ISQLiteDataReaderMock.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false) ;
            SQLiteDataReader fakeReader = commanandOnFakeDB.ExecuteReader();
            DbDataReader ds = new DbDataReader();
            ISQLiteCommandMock.Setup(x => x.ExecuteReader()).Returns(ds);
            ISQLiteDataReaderMock.SetupSequence(x => x.GetDouble(It.IsAny<int>())).Returns(new Queue<double>(new[] { 1.35, 1.123, 34.3258795317408, 31.23180570002 }).Dequeue);

            SQLiteConnection stubConn = new SQLiteConnection();
            Coordinate Expected = new Coordinate(34.3258795317408, 31.23180570002);

            // ACT
            Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates("stub", Math.Max, 0, 0, stubConn);

            // ASSERT
            Assert.AreEqual(actualCoordinate, Expected);
        }
    }
}