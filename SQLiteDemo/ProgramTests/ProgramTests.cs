using DbTests;
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
        private Mock<DataReaderWrapper> SQLiteDataReaderMock;
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
            SQLiteDataReaderMock= new Mock<DataReaderWrapper>(MockBehavior.Loose);
            string path = InMemoryDBTemplate.GetGpkgPath();
            SQLiteConnection fakeConnection = new SQLiteConnection($"Data Source={path}");
            fakeConnection.Open();
            InMemoryDBTemplate.Create(path);
            string query = "INSERT INTO \"gpkg_tile_matrix_set\" VALUES " +
                                          $"('test',4326,-180,-90,180,90);";
            SQLiteCommand command = new SQLiteCommand(query, fakeConnection);
            command.ExecuteNonQuery();
            //SQLiteDataReaderMock.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false);
            //SQLiteDataReaderMock.SetupSequence(x => x.GetDouble(It.IsAny<int>())).Returns(new Queue<double>(new[] { 1.35, 1.123, 34.3258795317408, 31.23180570002 }).Dequeue);

            // ACT
            //Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates("stub", Math.Max, 0, 0, stubConn);

            // ASSERT
            //Assert.AreEqual(actualCoordinate, Expected);
        }
    }
}