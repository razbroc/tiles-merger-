using DbTests;
using Moq;
using SQLiteDemo;
using System.Data.SQLite;

namespace DbQueriesTests
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

        public void TestAxisEdgeCoordinate(List<TileContent> contentToAdd, Coordinate expected, int xCol, int yCol, Coordinate.CompareFunc compareValue)
        {
            // ARRANGE
            string path = InMemoryDBTemplate.GetGpkgPath();
            SQLiteConnection fakeConnection = new SQLiteConnection($"Data Source={path}");
            fakeConnection.Open();
            InMemoryDBTemplate.Create(path);
            InMemoryDBTemplate.InsertDefaultIntoTileMatrix(fakeConnection);
            InMemoryDBTemplate.InsertIntoGpkgContents(fakeConnection, contentToAdd);

            // ACT
            Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates(compareValue, xCol, yCol, fakeConnection);

            // ASSERT
            Assert.AreEqual(actualCoordinate, expected);
        }

        [TestMethod]
        public void AxisEdgeCoordinategFindingMaxWhenMixedInMultipleRows()
        {
            // ARRANGE
            List<TileContent> contentToAdd = new List<TileContent>();
            contentToAdd.Add(new TileContent("test", "tiles", 34.2663935002085, 31.1786148130457, 31.3258795317408, 33.23180570002));
            contentToAdd.Add(new TileContent("test1", "tiles", 34.2663935002085, 31.1786148130457, 34.3258795317408, 31.23180570002));
            contentToAdd.Add(new TileContent("test2", "tiles", 34.2663935002085, 31.1786148130457, 32.3258795317408, 32.23180570002));
            Coordinate expected = new Coordinate(34.3258795317408, 33.23180570002);

            // Act and assert
            TestAxisEdgeCoordinate(contentToAdd, expected, X_MAX_COL, Y_MAX_COL, Math.Max);
        }



        [TestMethod]
        public void AxisEdgeCoordinategFindingMinWhenMixedInMultipleRows()
        {
            // ARRANGE
            List<TileContent> contentToAdd = new List<TileContent>();
            contentToAdd.Add(new TileContent("test", "tiles", 31.2663935002085, 32.1786148130457, 31.3258795317408, 33.23180570002));
            contentToAdd.Add(new TileContent("test1", "tiles", 32.2663935002085, 31.1786148130457, 34.3258795317408, 31.23180570002));
            contentToAdd.Add(new TileContent("test2", "tiles", 34.2663935002085, 33.1786148130457, 32.3258795317408, 32.23180570002));
            Coordinate expected = new Coordinate(31.2663935002085, 31.1786148130457);

            // Act and assert
            TestAxisEdgeCoordinate(contentToAdd, expected, X_MIN_COL, Y_MIN_COL, Math.Min);
        }
    }
}