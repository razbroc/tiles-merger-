using DbTests;
using Moq;
using SQLiteDemo;
using System.Data.SQLite;

namespace ProgramTests
{
    [TestClass]
    public class DBTests
    {
        #region consts

        const int X_MIN_COL = 5;
        const int Y_MIN_COL = 6;
        const int X_MAX_COL = 7;
        const int Y_MAX_COL = 8;

        #endregion


        #region mocks

        private Mock<DataReaderWrapper> SQLiteDataReaderMock;

        #endregion


        [TestMethod]
        public void AxisEdgeCoordinategFindingMaxWhenMixedInMultipleRows()
        {
            // ARRANGE
            SQLiteDataReaderMock = new Mock<DataReaderWrapper>(MockBehavior.Loose);
            string path = InMemoryDBTemplate.GetGpkgPath();
            SQLiteConnection fakeConnection = new SQLiteConnection($"Data Source={path}");
            fakeConnection.Open();
            InMemoryDBTemplate.Create(path);
            string query = "INSERT INTO \"gpkg_tile_matrix_set\" VALUES " +
                                          $"('test',4326,-180,-90,180,90);";
            SQLiteCommand command = new SQLiteCommand(query, fakeConnection);
            command.ExecuteNonQuery();
            query = "INSERT INTO \"gpkg_contents\" (\"table_name\",\"data_type\",\"min_x\",\"min_y\",\"max_x\",\"max_y\") " +
                                              "VALUES ('test','tiles',34.2663935002085,31.1786148130457,31.3258795317408,33.23180570002)," +
                                              " ('test1','tiles',34.2663935002085,31.1786148130457,34.3258795317408,31.23180570002)," +
                                              " ('test2','tiles',34.2663935002085,31.1786148130457,32.3258795317408,32.23180570002);";
            command = new SQLiteCommand(query, fakeConnection);
            command.ExecuteNonQuery();
            Coordinate Expected = new Coordinate(34.3258795317408, 33.23180570002);

            // ACT
            Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates(path, Math.Max, X_MAX_COL, Y_MAX_COL, fakeConnection);

            // ASSERT
            Assert.AreEqual(actualCoordinate, Expected);
        }

        [TestMethod]
        public void AxisEdgeCoordinategFindingMinWhenMixedInMultipleRows()
        {
            // ARRANGE
            SQLiteDataReaderMock = new Mock<DataReaderWrapper>(MockBehavior.Loose);
            string path = InMemoryDBTemplate.GetGpkgPath();
            SQLiteConnection fakeConnection = new SQLiteConnection($"Data Source={path}");
            fakeConnection.Open();
            InMemoryDBTemplate.Create(path);
            string query = "INSERT INTO \"gpkg_tile_matrix_set\" VALUES " +
                                          $"('test',4326,-180,-90,180,90);";
            SQLiteCommand command = new SQLiteCommand(query, fakeConnection);
            command.ExecuteNonQuery();
            query = "INSERT INTO \"gpkg_contents\" (\"table_name\",\"data_type\",\"min_x\",\"min_y\",\"max_x\",\"max_y\") " +
                                              "VALUES ('test','tiles',31.2663935002085,32.1786148130457,31.3258795317408,33.23180570002)," +
                                              " ('test1','tiles',32.2663935002085,31.1786148130457,34.3258795317408,31.23180570002)," +
                                              " ('test2','tiles',34.2663935002085,33.1786148130457,32.3258795317408,32.23180570002);";
            command = new SQLiteCommand(query, fakeConnection);
            command.ExecuteNonQuery();
            Coordinate Expected = new Coordinate(31.2663935002085, 31.1786148130457);

            // ACT
            Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates(path, Math.Min, X_MIN_COL, Y_MIN_COL, fakeConnection);

            // ASSERT
            Assert.AreEqual(actualCoordinate, Expected);
        }
    }
}