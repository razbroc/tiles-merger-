using SQLiteDemo;
using System.Data.SQLite;

 const int X_MIN_COL = 5;
 const int Y_MIN_COL = 6;
 const int X_MAX_COL = 7;
 const int Y_MAX_COL = 8;



Console.WriteLine("Please enter the path of base GKPG:");
String basePath = Console.ReadLine();
Console.WriteLine("Please enter the path of source GKPG:");
String sourcePath = Console.ReadLine();

SQLiteConnection baseConn = new SQLiteConnection($"Data Source={basePath}");
baseConn.Open();
SQLiteConnection sourceConn = new SQLiteConnection($"Data Source={sourcePath}");
sourceConn.Open();

Console.WriteLine("proccessing your request...");
InsertTileMatrix(ReadTileMatrixData(sourcePath,sourceConn),basePath, baseConn);
InsertTileData( ReadTileData(sourcePath, sourceConn), basePath, baseConn);
UpdateExtent(basePath, sourcePath, baseConn);
Console.WriteLine("your request completed succssefully");

baseConn.Close();
sourceConn.Close();


static String ReadTableName(String gpkgPath, SQLiteConnection conn)
{
    String query = "SELECT name FROM sqlite_sequence";
    SQLiteCommand command = new SQLiteCommand(query, conn);
    SQLiteDataReader dataReader = command.ExecuteReader();
    dataReader.Read();
    String name = dataReader.GetString(0);

    return name;
}

static Coordinate AxisEdgeType(String path, Edge edgeSide, SQLiteConnection conn)
{
    int xCol;
    int yCol;
    Coordinate.CompareFunc compareFunc;

    if (edgeSide == Edge.Max)
    {
        xCol = X_MAX_COL;
        yCol = Y_MAX_COL;
        compareFunc = Math.Max;
    }
    else
    {
        xCol = X_MIN_COL;
        yCol = Y_MIN_COL;
        compareFunc = Math.Min;
    }

    return AxisEdgeCoordinates(path, compareFunc, xCol, yCol, conn);
}

static Coordinate AxisEdgeCoordinates(String path, Coordinate.CompareFunc compareFunc, int xCol, int yCol, SQLiteConnection connection)
{
    String query = "SELECT * FROM gpkg_contents";
    SQLiteCommand commandArea = new SQLiteCommand(query, connection);
    SQLiteDataReader dataReader = commandArea.ExecuteReader();
    Coordinate areaCoordinates = null;
    Coordinate edgeCoordinates = null;
    Coordinate comparedCoordinates = null;
    dataReader.Read();
    edgeCoordinates = new Coordinate(dataReader.GetDouble(xCol), dataReader.GetDouble(yCol));

    while (dataReader.Read())
    {
        areaCoordinates = new Coordinate(dataReader.GetDouble(xCol), dataReader.GetDouble(yCol));

        /*
         * in case  "gpkg_contents" table has more than one row,
         * we would need to find the x-y limits by finding the edges over all the rows.
         * therefore we need to determine whether we are checking for a max/min edge situation - 
         * and find the fitting edge for that situation.
         * */
        comparedCoordinates = Coordinate.CompareCoordinates(edgeCoordinates, areaCoordinates, compareFunc);
        if (!Coordinate.Equals(comparedCoordinates, edgeCoordinates))
            {
                edgeCoordinates = comparedCoordinates;
            }
    }

    return edgeCoordinates;
}

static void UpdateExtent(String basePath, String sourcePath, SQLiteConnection connectionToBaseGKPG)
{

    Coordinate baseMinCoordinates = AxisEdgeType(basePath, Edge.Min, connectionToBaseGKPG);
    Coordinate sourceMinCoordinates = AxisEdgeType(sourcePath, Edge.Min, connectionToBaseGKPG);
    Coordinate baseMaxCoordinates = AxisEdgeType(basePath, Edge.Max, connectionToBaseGKPG);
    Coordinate sourceMaxCoordinates = AxisEdgeType(sourcePath, Edge.Max, connectionToBaseGKPG);

    Coordinate minCoordinates = Coordinate.CompareCoordinates(baseMinCoordinates, sourceMinCoordinates, Math.Min);
    Coordinate maxCoordinates = Coordinate.CompareCoordinates(baseMaxCoordinates, sourceMaxCoordinates, Math.Max);

    String query = "UPDATE gpkg_contents SET min_x = @param1, min_y = @param2, max_x = @param3, max_y = @param4 ;";
    SQLiteCommand commandArea1 = new SQLiteCommand(query, connectionToBaseGKPG);

    commandArea1.Parameters.AddWithValue("@param1", minCoordinates.X);
    commandArea1.Parameters.AddWithValue("@param2", minCoordinates.Y);
    commandArea1.Parameters.AddWithValue("@param3", maxCoordinates.X);
    commandArea1.Parameters.AddWithValue("@param4", maxCoordinates.Y);

    commandArea1.ExecuteNonQuery();
}

static void InsertTileData(List<TileInfo> data, String gpkgPath, SQLiteConnection conn)
{
    SQLiteCommand command;

    for (int index = 0; index < data.Count; index++)
    {
        int zoomLevel = data[index].ZoomLevel,
            tileColumn = data[index].TileColumn, 
            tileRow = data[index].TileRow;
        byte[] tileData = data[index].TileData;
        String tableName = ReadTableName(gpkgPath, conn);
        String query = $"INSERT or REPLACE INTO {tableName}( zoom_level, tile_column, tile_row, tile_data) VALUES( @zoomLevel, @tileColumn, @tileRow, @tileData)";
        command = new SQLiteCommand(query, conn);
        command.Parameters.AddWithValue("@zoomLevel", zoomLevel);
        command.Parameters.AddWithValue("@tileColumn", tileColumn);
        command.Parameters.AddWithValue("@tileRow", tileRow);
        command.Parameters.AddWithValue("@tileData", tileData);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine("geopackge is damaged, can't perform request!");
            throw new Exception($"geopackage is empty or damaged. { ex.Message }");
        }
        
    }
}

static void InsertTileMatrix(List<TileMatrix> data, String gpkgPath, SQLiteConnection conn)
{
    SQLiteCommand command;
    String tableName = ReadTableName(gpkgPath, conn);

    for (int index = 0; index < data.Count; index++)
    {
        int zoomLevel = data[index].ZoomLevel;
        string findZoomLevelQuery = $"SELECT zoom_level FROM gpkg_tile_matrix WHERE zoom_level == {zoomLevel}";
        command = new SQLiteCommand(findZoomLevelQuery, conn);
        Boolean foundZoomLevel = command.ExecuteReader().HasRows;

        if (!foundZoomLevel)
        {
            int MatrixWidth = data[index].MatrixWidth,
                MatrixHeight = data[index].MatrixHeight,
                TileWidth = data[index].TileWidth,
                TileHeight = data[index].TileHeight;
            double PixelXSize = data[index].PixelXSize,
                PixelYSize = data[index].PixelYSize;
            String query = "INSERT or REPLACE INTO gpkg_tile_matrix(table_name, zoom_level, matrix_width, matrix_height, tile_width, tile_height, pixel_x_size, pixel_y_size) " +
                $"VALUES( '{tableName}', @zoomLevel, @MatrixWidth, @MatrixHeight, @TileWidth, @TileHeight, @PixelXSize, @PixelYSize)";
            command = new SQLiteCommand(query, conn);
            command.Parameters.AddWithValue("@zoomLevel", zoomLevel);
            command.Parameters.AddWithValue("@MatrixWidth", MatrixWidth);
            command.Parameters.AddWithValue("@MatrixHeight", MatrixHeight);
            command.Parameters.AddWithValue("@TileWidth", TileWidth);
            command.Parameters.AddWithValue("@TileHeight", TileHeight);
            command.Parameters.AddWithValue("@PixelXSize", PixelXSize);
            command.Parameters.AddWithValue("@PixelYSize", PixelYSize);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("geopackge is damaged, can't perform request!");
                throw new Exception($"geopackage is empty or damaged. {ex.Message}");
            }
        }

    }
}

static List<TileInfo> ReadTileData(String gpkgPath, SQLiteConnection conn)
{
    SQLiteDataReader dataReader;
    String tableName = ReadTableName(gpkgPath, conn);
    String query = $"SELECT * FROM {tableName}";
    SQLiteCommand command = new SQLiteCommand(query, conn);
    List<TileInfo> tiles = new List<TileInfo>();
    TileInfo tile = new TileInfo();

    try
    {
        dataReader = command.ExecuteReader();
    }
    catch (Exception ex) 
    {
        Console.WriteLine("can't perform request");
        throw new Exception($"geopackage is empty or damaged. {ex.Message}");
    }

    while (dataReader.Read())
    {
        tile = new TileInfo(dataReader.GetInt32(1), dataReader.GetInt32(2), dataReader.GetInt32(3), (byte[]) dataReader.GetValue(4));
        tiles.Add(tile);
    }


    return tiles;
}

static List<TileMatrix> ReadTileMatrixData(String gpkgPath, SQLiteConnection conn)
{
    SQLiteDataReader dataReader;
    String query = "SELECT * FROM gpkg_tile_matrix";
    SQLiteCommand command = new SQLiteCommand(query, conn);
    List<TileMatrix> tileMatrices = new List<TileMatrix>();
    TileMatrix tileMatrix= null;

    try
    {
        dataReader = command.ExecuteReader();
    }
    catch (Exception ex)
    {
        Console.WriteLine("can't perform request");
        throw new Exception($"geopackage is empty or damaged. {ex.Message}");
    }

    while (dataReader.Read())
    {
        tileMatrix = new TileMatrix(dataReader.GetInt32(1), dataReader.GetInt32(2), dataReader.GetInt32(3),
            dataReader.GetInt32(4), dataReader.GetInt32(5), dataReader.GetDouble(6), dataReader.GetDouble(7));
        tileMatrices.Add(tileMatrix);
    }

    return tileMatrices;
}
enum Edge
{
    Max,
    Min
}
