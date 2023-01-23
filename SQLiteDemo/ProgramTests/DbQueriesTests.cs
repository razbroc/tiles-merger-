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
        public static IEnumerable<object[]> GetTileParamsMaxTest
        {
            get
            {
                return new[]
                {
                    new object[] { 
                        new List<TileContent> {
                            new TileContent("test", "tiles", 34.2663935002085, 31.1786148130457, 31.3258795317408, 33.23180570002),
                            new TileContent("test1", "tiles", 34.2663935002085, 31.1786148130457, 34.3258795317408, 31.23180570002),
                            new TileContent("test2", "tiles", 34.2663935002085, 31.1786148130457, 32.3258795317408, 32.23180570002)
                        },
                        new Coordinate(34.3258795317408, 33.23180570002)
                    }
                };
            }
        }

        public static IEnumerable<object[]> GetGetTileParamsMinTest
        {
            get
            {
                return new[]
                {
                    new object[] {
                        new List<TileContent> {
                            new TileContent("test", "tiles", 31.2663935002085, 32.1786148130457, 31.3258795317408, 33.23180570002),
                            new TileContent("test1", "tiles", 32.2663935002085, 31.1786148130457, 34.3258795317408, 31.23180570002),
                            new TileContent("test2", "tiles", 34.2663935002085, 33.1786148130457, 32.3258795317408, 32.23180570002)
                        },
                        new Coordinate(31.2663935002085, 31.1786148130457)
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetTileParamsMaxTest))]
        public void TestAxisEdgeCoordinateMax(List<TileContent> contentToAdd, Coordinate expected)
        {
            // ARRANGE
            string path = InMemoryDBTemplate.GetGpkgPath();
            SQLiteConnection fakeConnection = new SQLiteConnection($"Data Source={path}");
            fakeConnection.Open();
            InMemoryDBTemplate.Create(path);
            InMemoryDBTemplate.InsertDefaultIntoTileMatrix(fakeConnection);
            InMemoryDBTemplate.InsertIntoGpkgContents(fakeConnection, contentToAdd);

            // ACT
            Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates(Math.Max, X_MAX_COL, Y_MAX_COL, fakeConnection);

            // ASSERT
            Assert.AreEqual(actualCoordinate, expected);
        }

        [TestMethod]
        [DynamicData(nameof(GetGetTileParamsMinTest))]
        public void TestAxisEdgeCoordinateMin(List<TileContent> contentToAdd, Coordinate expected)
        {
            // ARRANGE
            string path = InMemoryDBTemplate.GetGpkgPath();
            SQLiteConnection fakeConnection = new SQLiteConnection($"Data Source={path}");
            fakeConnection.Open();
            InMemoryDBTemplate.Create(path);
            InMemoryDBTemplate.InsertDefaultIntoTileMatrix(fakeConnection);
            InMemoryDBTemplate.InsertIntoGpkgContents(fakeConnection, contentToAdd);

            // ACT
            Coordinate actualCoordinate = DbQueries.AxisEdgeCoordinates(Math.Min, X_MIN_COL, Y_MIN_COL, fakeConnection);

            // ASSERT
            Assert.AreEqual(actualCoordinate, expected);
        }
    }
}
