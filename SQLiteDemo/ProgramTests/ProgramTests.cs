using Moq;
using System.Data.SQLite;

namespace ProgramTests
{
    [TestClass]
    public class ProgramTests
    {
        #region mocks

        private Mock<SQLiteConnection> SQLiteConnectionMock;
        private Mock<SQLiteCommand> SQLiteCommandMock;
        private Mock<SQLiteDataReader> SQLiteDataReaderMock;

        #endregion

        [TestMethod]
        public void AxisEdgeCoordinategffgs()
        {
            // ARRANGE
            SQLiteDataReaderMock = new Mock<SQLiteDataReader>(MockBehavior.Loose);
            SQLiteDataReaderMock.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false) ;
            Random r = new Random();
            int range = 9;
            byte[] byteArr = new byte[] { (byte)r.Next(1, range), (byte)r.Next(1, range), (byte)r.Next(1, range), };
            SQLiteDataReaderMock.Setup(x => x.GetDouble(It.IsAny<int>())).Returns(r.NextDouble() * range);
            SQLiteDataReaderMock.Setup(x => x.GetInt32(It.IsAny<int>())).Returns(r.Next(1, range));
            SQLiteDataReaderMock.Setup(x => x.GetValue(It.IsAny<int>())).Returns(byteArr);

            // ACT
            Program.AxisEdgeCoordinates();

            // ASSERT
        }
    }
}