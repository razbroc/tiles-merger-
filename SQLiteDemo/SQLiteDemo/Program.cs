using SQLiteDemo;
using System.Data.SQLite;
using System.Collections;

InputOutput.Print("GKPG to expand:");
String path1 = InputOutput.GetPath();
InputOutput.Print("GKPG to pull data:");
String path2 = InputOutput.GetPath();
InsertData( ReadData(path2), path1);
UpdateAxises(path1, path2);
Console.WriteLine("finished");

static Coordinates MinAxisCoordinates(String path)
{
    SQLiteConnection connArea = new SQLiteConnection("Data Source=" + path);
    connArea.Open();

    String query = "SELECT * FROM gpkg_contents";
    SQLiteCommand commandArea = new SQLiteCommand(query, connArea);
    SQLiteDataReader dataReader = commandArea.ExecuteReader();
    Coordinates areaMinCoordinates = new Coordinates();

    while (dataReader.Read())
    {
        areaMinCoordinates = new Coordinates(dataReader.GetDouble(5), dataReader.GetDouble(6));
    }

    return areaMinCoordinates;
}

static Coordinates MaxAxisCoordinates(String path)
{
    SQLiteConnection connArea = new SQLiteConnection("Data Source=" + path);
    connArea.Open();

    String query = "SELECT * FROM gpkg_contents";
    SQLiteCommand commandArea = new SQLiteCommand(query, connArea);
    SQLiteDataReader dataReader = commandArea.ExecuteReader();
    Coordinates areaMaxCoordinates = new Coordinates();

    while (dataReader.Read())
    {
        areaMaxCoordinates = new Coordinates(dataReader.GetDouble(7), dataReader.GetDouble(8));
    }

    return areaMaxCoordinates;
}

static void UpdateAxises(String path1, String path2)
{
    SQLiteConnection connArea1 = new SQLiteConnection("Data Source=" + path1);
    connArea1.Open();

    Coordinates area1MinCoordinates = MinAxisCoordinates(path1);
    Coordinates area2MinCoordinates = MinAxisCoordinates(path2);
    Coordinates area1MaxCoordinates = MaxAxisCoordinates(path1);
    Coordinates area2MaxCoordinates = MaxAxisCoordinates(path2);

    Coordinates maxCoordinates = area1MaxCoordinates.findMax(area2MaxCoordinates);
    Coordinates minCoordinates = area1MinCoordinates.findMin(area2MinCoordinates);
    String query = "UPDATE gpkg_contents SET min_x = "+ minCoordinates.GetX()+ ", min_y = "+minCoordinates.GetY()+", max_x = " + maxCoordinates.GetX() + ", max_y = " + maxCoordinates.GetY()+ ";";
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