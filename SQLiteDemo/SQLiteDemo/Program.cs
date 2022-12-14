using SQLiteDemo;
using System.Data.SQLite;
using System.Collections;

Console.WriteLine("Please enter the path of the GKPG to expand:");
String path1 = Console.ReadLine();
Console.WriteLine("Please enter the path of the GKPG to pull data:");
String path2 = Console.ReadLine();
InsertData( ReadData(path2), path1);
UpdateAxises(path1, path2);
Console.WriteLine("finished");

static Coordinate MinAxisCoordinates(String path)
{
    SQLiteConnection connArea = new SQLiteConnection("Data Source=" + path);
    connArea.Open();

    String query = "SELECT * FROM gpkg_contents";
    SQLiteCommand commandArea = new SQLiteCommand(query, connArea);
    SQLiteDataReader dataReader = commandArea.ExecuteReader();
    Coordinate areaMinCoordinates = new Coordinate();

    while (dataReader.Read())
    {
        areaMinCoordinates = new Coordinate(dataReader.GetDouble(5), dataReader.GetDouble(6));
    }

    return areaMinCoordinates;
}

static Coordinate MaxAxisCoordinates(String path)
{
    SQLiteConnection connArea = new SQLiteConnection("Data Source=" + path);
    connArea.Open();

    String query = "SELECT * FROM gpkg_contents";
    SQLiteCommand commandArea = new SQLiteCommand(query, connArea);
    SQLiteDataReader dataReader = commandArea.ExecuteReader();
    Coordinate areaMaxCoordinates = new Coordinate();

    while (dataReader.Read())
    {
        areaMaxCoordinates = new Coordinate(dataReader.GetDouble(7), dataReader.GetDouble(8));
    }

    return areaMaxCoordinates;
}

static void UpdateAxises(String path1, String path2)
{
    SQLiteConnection connArea1 = new SQLiteConnection("Data Source=" + path1);
    connArea1.Open();

    Coordinate area1MinCoordinates = MinAxisCoordinates(path1);
    Coordinate area2MinCoordinates = MinAxisCoordinates(path2);
    Coordinate area1MaxCoordinates = MaxAxisCoordinates(path1);
    Coordinate area2MaxCoordinates = MaxAxisCoordinates(path2);

    //Coordinate maxCoordinates = area1MaxCoordinates.FindMax(area2MaxCoordinates);
    //Coordinate minCoordinates = area1MinCoordinates.FindMin(area2MinCoordinates);

    Coordinate.FindValue findMaxCoordinates = Coordinate.FindMax; 
    Coordinate.FindValue findMinCoordinates = Coordinate.FindMin;

    Coordinate minCoordinates = findMinCoordinates(area1MinCoordinates, area2MinCoordinates);
    Coordinate maxCoordinates = findMaxCoordinates(area1MaxCoordinates, area2MaxCoordinates);



    String query = "UPDATE gpkg_contents SET min_x = "+ minCoordinates.X+
        ", min_y = "+ minCoordinates.Y+
        ", max_x = " + maxCoordinates.X+
        ", max_y = " + maxCoordinates.Y+ ";";
    SQLiteCommand commandArea1 = new SQLiteCommand(query, connArea1);

    commandArea1.ExecuteNonQuery();
    connArea1.Close();

}

static void InsertData( ArrayList data, String PathOfGkpgToUpdate)
{
    SQLiteConnection conn = new SQLiteConnection("Data Source="+ PathOfGkpgToUpdate);
    conn.Open();

    SQLiteCommand command;
    SQLiteParameter parm;

    for (int index = 0; index < data.Count; index++)
    {
        int zoomLevel = ((GeoPackage) data[index]).GetZoomLevel(),
            tileColumn = ((GeoPackage)data[index]).GetTileColumn(), 
            tileRow = ((GeoPackage)data[index]).GetTileRow();
        byte[] tileData = ((GeoPackage)data[index]).GetTileData();
        String query = "INSERT or REPLACE INTO O_arzi_mz_w84geo_Apr19_gpkg_18_0(zoom_level, tile_column, tile_row, tile_data)" + " VALUES(" + zoomLevel + "," + tileColumn + "," + tileRow + ", @tileData)";
        command = new SQLiteCommand(query, conn);
        command.Parameters.AddWithValue("@tileData", tileData);
        command.ExecuteNonQuery();
    }
    conn.Close();
}

static ArrayList ReadData(String PathOfGkpgToRead)
{
    SQLiteConnection conn = new SQLiteConnection("Data Source=" + PathOfGkpgToRead);
    conn.Open();

 
    SQLiteDataReader dataReader;
    String query = "SELECT * FROM O_arzi_mz_w84geo_Apr19_gpkg_18_0";
    SQLiteCommand command = new SQLiteCommand(query, conn);
    ArrayList geoPackages = new ArrayList();
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