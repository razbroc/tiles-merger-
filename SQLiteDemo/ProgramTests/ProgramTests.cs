using Moq;
using SQLiteDemo;
using System.Data;
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
        private string GetGpkgPath()
        {
            //create "path" that manipulates sqlite connection string to use shared in memory db.
            //guid (uuid)  is used to make the db unique per test - this is required when running multiple tests in parallel
            return $"{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        }

        [TestMethod]
        public void AxisEdgeCoordinategffgs()
        {
            // ARRANGE
            SQLiteConnection conn = new SQLiteConnection($"Data Source={GetGpkgPath()}");
            conn.Open();
            var readerMock = new SQLiteCommand("", conn);
            ISQLiteDataReaderMock = new Mock<ISqlDataReader>(MockBehavior.Loose);
            ISQLiteCommandMock = new Mock<ISqliteCommand>();
            ISQLiteDataReaderMock.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false) ;
            ISQLiteCommandMock.Setup(x => x.ExecuteReader()).Returns(readerMock.ExecuteReader());
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