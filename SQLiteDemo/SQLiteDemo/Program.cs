using SQLiteDemo;
using System.Data.SQLite;

 const int X_MIN_COL = 5;
 const int Y_MIN_COL = 6;
 const int X_MAX_COL = 7;
 const int Y_MAX_COL = 8;

Console.WriteLine("Please enter the path of the GKPG to expand:");
String path1 = Console.ReadLine();
Console.WriteLine("Please enter the path of the GKPG to pull data:");
String path2 = Console.ReadLine();
Console.WriteLine("proccessing your request...");
InsertData( ReadData(path2), path1);
UpdateAxises(path1, path2);
Console.WriteLine("your request completed succssefully");

static Coordinate AxisEdgeCoordinates(int xCol, int yCol, String path, Coordinate.CompareFunc edgeSide)
{
    SQLiteConnection connArea = new SQLiteConnection("Data Source=" + path);
    connArea.Open();

    String query = "SELECT * FROM gpkg_contents";
    SQLiteCommand commandArea = new SQLiteCommand(query, connArea);
    SQLiteDataReader dataReader = commandArea.ExecuteReader();
    Coordinate areaCoordinates = null;
    Coordinate edgeCoordinates = null;
    Coordinate comparedCoordinates = null;
    bool firstIteration = true;

    while (dataReader.Read())
    {
        areaCoordinates = new Coordinate(dataReader.GetDouble(xCol), dataReader.GetDouble(yCol));

        if (firstIteration)
        {
            edgeCoordinates = new Coordinate(dataReader.GetDouble(xCol), dataReader.GetDouble(yCol));
            firstIteration = false;
        }

        /*
         * in case  "gpkg_contents" table has more than one row,
         * we would need to find the x-y limits by finding the edges over all the rows.
         * therefore we need to determine whether we are checking for a max/min edge situation - 
         * and find the fitting edge for that situation.
         * */

        if (edgeSide == Math.Max) 
        {
            comparedCoordinates = Coordinate.CompareCoordinates(edgeCoordinates, areaCoordinates, edgeSide);
            if (!Coordinate.Equals(comparedCoordinates, edgeCoordinates))
            {
                edgeCoordinates = comparedCoordinates;
            }
        }

        if (edgeSide == Math.Min) 
        {
            comparedCoordinates = Coordinate.CompareCoordinates(edgeCoordinates, areaCoordinates, edgeSide);
            if (!Coordinate.Equals(comparedCoordinates, edgeCoordinates))
            {
                edgeCoordinates = comparedCoordinates;
            }
        }
    }

    return edgeCoordinates;
}

//think of while logic
static void UpdateAxises(String path1, String path2)
{
    SQLiteConnection connArea1 = new SQLiteConnection("Data Source=" + path1);
    connArea1.Open();

    Coordinate area1MinCoordinates = AxisEdgeCoordinates(X_MIN_COL, Y_MIN_COL, path1, Math.Min);
    Coordinate area2MinCoordinates = AxisEdgeCoordinates(X_MIN_COL, Y_MIN_COL, path2, Math.Min);
    Coordinate area1MaxCoordinates = AxisEdgeCoordinates(X_MAX_COL, Y_MAX_COL, path1, Math.Max);
    Coordinate area2MaxCoordinates = AxisEdgeCoordinates(X_MAX_COL, Y_MAX_COL, path2, Math.Max);

    Coordinate minCoordinates = Coordinate.CompareCoordinates(area1MinCoordinates, area2MinCoordinates, Math.Min);
    Coordinate maxCoordinates = Coordinate.CompareCoordinates(area1MaxCoordinates, area2MaxCoordinates, Math.Max);

    String query = "UPDATE gpkg_contents SET min_x = @param1, min_y = @param2, max_x = @param3, max_y = @param4 ;";
    SQLiteCommand commandArea1 = new SQLiteCommand(query, connArea1);

    commandArea1.Parameters.AddWithValue("@param1", minCoordinates.X);
    commandArea1.Parameters.AddWithValue("@param2", minCoordinates.Y);
    commandArea1.Parameters.AddWithValue("@param3", maxCoordinates.X);
    commandArea1.Parameters.AddWithValue("@param4", maxCoordinates.Y);

    commandArea1.ExecuteNonQuery();
    connArea1.Close();
}

static void InsertData(List<GeoPackage> data, String gpkgPath)
{
    SQLiteConnection conn = new SQLiteConnection("Data Source="+ gpkgPath);
    conn.Open();
    SQLiteCommand command;
    SQLiteParameter parm;

    for (int index = 0; index < data.Count; index++)
    {
        int zoomLevel = ((GeoPackage) data[index]).GetZoomLevel(),
            tileColumn = ((GeoPackage)data[index]).GetTileColumn(), 
            tileRow = ((GeoPackage)data[index]).GetTileRow();
        byte[] tileData = ((GeoPackage)data[index]).GetTileData();
        String query = "INSERT or REPLACE INTO O_arzi_mz_w84geo_Apr19_gpkg_18_0(zoom_level, tile_column, tile_row, tile_data)" + " VALUES( @zoomLevel, @tileColumn, @tileRow, @tileData)";
        command = new SQLiteCommand(query, conn);
        command.Parameters.AddWithValue("@zoomLevel", zoomLevel);
        command.Parameters.AddWithValue("@tileColumn", tileColumn);
        command.Parameters.AddWithValue("@tileRow", tileRow);
        command.Parameters.AddWithValue("@tileData", tileData);
        command.ExecuteNonQuery();
    }
    conn.Close();
}

static List<GeoPackage> ReadData(String gpkgPath)
{
    SQLiteConnection conn = new SQLiteConnection("Data Source=" + gpkgPath);
    conn.Open();

 
    SQLiteDataReader dataReader;
    String query = "SELECT * FROM O_arzi_mz_w84geo_Apr19_gpkg_18_0";
    SQLiteCommand command = new SQLiteCommand(query, conn);
    List<GeoPackage> geoPackages = new List<GeoPackage>();
    GeoPackage geoPackage = new GeoPackage();
    dataReader = command.ExecuteReader();

    while (dataReader.Read())
    {
        geoPackage = new GeoPackage(dataReader.GetInt32(1), dataReader.GetInt32(2), dataReader.GetInt32(3), (byte[]) dataReader.GetValue(4));
        geoPackages.Add(geoPackage);
    }

    conn.Close();

    return geoPackages;
}