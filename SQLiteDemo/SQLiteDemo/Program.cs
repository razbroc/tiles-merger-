using SQLiteDemo;
using System.Data.SQLite;




Console.WriteLine("Please enter the path of base GKPG:");
String basePath = Console.ReadLine();
Console.WriteLine("Please enter the path of source GKPG:");
String sourcePath = Console.ReadLine();

SQLiteConnection baseConn = new SQLiteConnection($"Data Source={basePath}");
baseConn.Open();
SQLiteConnection sourceConn = new SQLiteConnection($"Data Source={sourcePath}");
sourceConn.Open();

Console.WriteLine("proccessing your request...");
List<TileMatrix> sourceTileMatrixData = DbQueries.ReadTileMatrixData(sourceConn);
DbQueries.InsertTileMatrix(sourceTileMatrixData, baseConn);
List<TileInfo> tileInfo = DbQueries.ReadTileData(sourceConn);
DbQueries.InsertTileData(tileInfo, baseConn);
DbQueries.UpdateExtent(baseConn, sourceConn);
Console.WriteLine("your request completed succssefully");

baseConn.Close();
sourceConn.Close();
