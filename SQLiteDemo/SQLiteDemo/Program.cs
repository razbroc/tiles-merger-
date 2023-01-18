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
DbQueries.InsertTileMatrix(DbQueries.ReadTileMatrixData(sourcePath,sourceConn),basePath, baseConn);
DbQueries.InsertTileData(DbQueries.ReadTileData(sourcePath, sourceConn), basePath, baseConn);
DbQueries.UpdateExtent(baseConn, sourceConn);
Console.WriteLine("your request completed succssefully");

baseConn.Close();
sourceConn.Close();


