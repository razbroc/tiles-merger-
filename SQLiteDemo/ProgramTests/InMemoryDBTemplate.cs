using SQLiteDemo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTests
{
    public abstract class InMemoryDBTemplate
    {
        public const int SRID = 4326;

        public static string GetGpkgPath()
        {
            //create "path" that manipulates sqlite connection string to use shared in memory db.
            //guid (uuid)  is used to make the db unique per test - this is required when running multiple tests in parallel
            return $"{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        }

        private static void SetPragma(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA application_id = 1196444487; " // gpkg v1.2 +
                                                                             //command.CommandText = "PRAGMA application_id = 1196437808; " // gpkg v1.0 or 1.1
                                      + "PRAGMA user_version = 10201; "; // gpkg version number in the form MMmmPP (MM = major version, mm = minor version, PP = patch). aka 10000 is 1.0.0
                // + "PRAGMA page_size = 1024; "; //set sqlite page size, must be power of 2. current default is 4096 - changing the default requires vacuum
                command.ExecuteNonQuery();
            }
        }
        private static void CreateSpatialRefTable(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE \"gpkg_spatial_ref_sys\" (" +
                                      "\"srs_name\" TEXT NOT NULL," +
                                      "\"srs_id\" INTEGER NOT NULL," +
                                      "\"organization\" TEXT NOT NULL," +
                                      "\"organization_coordsys_id\" INTEGER NOT NULL," +
                                      "\"definition\" TEXT NOT NULL," +
                                      "\"description\" TEXT," +
                                      "PRIMARY KEY(\"srs_id\"));";
                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO \"gpkg_spatial_ref_sys\" VALUES " +
                                      "('Undefined cartesian SRS',-1,'NONE',-1,'undefined','undefined cartesian coordinate reference system')," +
                                      "('Undefined geographic SRS',0,'NONE',0,'undefined','undefined geographic coordinate reference system')," +
                                      "('WGS 84 geodetic',4326,'EPSG',4326,'GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]]," +
                                      "AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]]," +
                                      "AUTHORITY[\"EPSG\",\"4326\"]]','longitude/latitude coordinates in decimal degrees on the WGS 84 spheroid');";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateContentsTable(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE \"gpkg_contents\" (" +
                                      "\"table_name\" TEXT NOT NULL," +
                                      "\"data_type\" TEXT NOT NULL," +
                                      "\"identifier\" TEXT UNIQUE," +
                                      "\"description\" TEXT DEFAULT ''," +
                                      "\"last_change\" DATETIME NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))," +
                                      "\"min_x\" DOUBLE," +
                                      "\"min_y\" DOUBLE," +
                                      "\"max_x\" DOUBLE," +
                                      "\"max_y\" DOUBLE," +
                                      "\"srs_id\"	INTEGER," +
                                      "CONSTRAINT \"fk_gc_r_srs_id\" FOREIGN KEY(\"srs_id\") REFERENCES \"gpkg_spatial_ref_sys\"(\"srs_id\")," +
                                      "PRIMARY KEY(\"table_name\"));";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateGeometryColumnsTable(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE \"gpkg_geometry_columns\" (" +
                                      "\"table_name\" TEXT NOT NULL," +
                                      "\"column_name\" TEXT NOT NULL," +
                                      "\"geometry_type_name\" TEXT NOT NULL," +
                                      "\"srs_id\" INTEGER NOT NULL," +
                                      "\"z\" TINYINT NOT NULL," +
                                      "\"m\" TINYINT NOT NULL," +
                                      "CONSTRAINT \"pk_geom_cols\" PRIMARY KEY(\"table_name\",\"column_name\")," +
                                      "CONSTRAINT \"fk_gc_srs\" FOREIGN KEY(\"srs_id\") REFERENCES \"gpkg_spatial_ref_sys\"(\"srs_id\")," +
                                      "CONSTRAINT \"fk_gc_tn\" FOREIGN KEY(\"table_name\") REFERENCES \"gpkg_contents\"(\"table_name\"));";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTileMatrixSetTable(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE \"gpkg_tile_matrix_set\" (" +
                                      "\"table_name\" TEXT NOT NULL," +
                                      "\"srs_id\" INTEGER NOT NULL," +
                                      "\"min_x\" DOUBLE NOT NULL," +
                                      "\"min_y\" DOUBLE NOT NULL," +
                                      "\"max_x\" DOUBLE NOT NULL," +
                                      "\"max_y\" DOUBLE NOT NULL," +
                                      "PRIMARY KEY(\"table_name\")," +
                                      "CONSTRAINT \"fk_gtms_srs\" FOREIGN KEY(\"srs_id\") REFERENCES \"gpkg_spatial_ref_sys\"(\"srs_id\")," +
                                      "CONSTRAINT \"fk_gtms_table_name\" FOREIGN KEY(\"table_name\") REFERENCES \"gpkg_contents\"(\"table_name\"));";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTileMatrixTable(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE \"gpkg_tile_matrix\" (" +
                                      "\"table_name\" TEXT NOT NULL," +
                                      "\"zoom_level\" INTEGER NOT NULL," +
                                      "\"matrix_width\" INTEGER NOT NULL," +
                                      "\"matrix_height\" INTEGER NOT NULL," +
                                      "\"tile_width\" INTEGER NOT NULL," +
                                      "\"tile_height\" INTEGER NOT NULL," +
                                      "\"pixel_x_size\" DOUBLE NOT NULL," +
                                      "\"pixel_y_size\" DOUBLE NOT NULL," +
                                      "CONSTRAINT \"pk_ttm\" PRIMARY KEY(\"table_name\",\"zoom_level\")," +
                                      "CONSTRAINT \"fk_tmm_table_name\" FOREIGN KEY(\"table_name\") REFERENCES \"gpkg_contents\"(\"table_name\"));";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateExtentionTable(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE \"gpkg_extensions\" (" +
                                      "\"table_name\" TEXT," +
                                      "\"column_name\" TEXT," +
                                      "\"extension_name\" TEXT NOT NULL," +
                                      "\"definition\"TEXT NOT NULL," +
                                      "\"scope\" TEXT NOT NULL," +
                                      "CONSTRAINT \"ge_tce\" UNIQUE(\"table_name\",\"column_name\",\"extension_name\"));";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTileTable(SQLiteConnection connection, string tileCache)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"CREATE TABLE \"{tileCache}\" (" +
                                      "\"id\" INTEGER," +
                                      "\"zoom_level\" INTEGER NOT NULL," +
                                      "\"tile_column\" INTEGER NOT NULL," +
                                      "\"tile_row\" INTEGER NOT NULL," +
                                      "\"tile_data\" BLOB NOT NULL," +
                                      "UNIQUE(\"zoom_level\",\"tile_column\",\"tile_row\")," +
                                      "PRIMARY KEY(\"id\" AUTOINCREMENT));";
                command.ExecuteNonQuery();
            }
        }

        private static void Add2X1MatrixSet(SQLiteConnection connection, string tileCache)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO \"gpkg_tile_matrix_set\" VALUES " +
                                      $"('{tileCache}',{SRID},-180,-90,180,90);";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTileMatrixValidationTriggers(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "CREATE TRIGGER 'gpkg_tile_matrix_zoom_level_insert' BEFORE INSERT ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table ''gpkg_tile_matrix'' violates constraint: zoom_level cannot be less than 0') WHERE (NEW.zoom_level < 0); END;" +
                    "CREATE TRIGGER 'gpkg_tile_matrix_zoom_level_update' BEFORE UPDATE of zoom_level ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table ''gpkg_tile_matrix'' violates constraint: zoom_level cannot be less than 0') WHERE(NEW.zoom_level < 0); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_matrix_width_insert' BEFORE INSERT ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table ''gpkg_tile_matrix'' violates constraint: matrix_width cannot be less than 1') WHERE(NEW.matrix_width < 1); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_matrix_width_update' BEFORE UPDATE OF matrix_width ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table ''gpkg_tile_matrix'' violates constraint: matrix_width cannot be less than 1') WHERE(NEW.matrix_width < 1); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_matrix_height_insert' BEFORE INSERT ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table ''gpkg_tile_matrix'' violates constraint: matrix_height cannot be less than 1') WHERE(NEW.matrix_height < 1); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_matrix_height_update' BEFORE UPDATE OF matrix_height ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table ''gpkg_tile_matrix'' violates constraint: matrix_height cannot be less than 1') WHERE(NEW.matrix_height < 1); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_pixel_x_size_insert' BEFORE INSERT ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table ''gpkg_tile_matrix'' violates constraint: pixel_x_size must be greater than 0') WHERE NOT(NEW.pixel_x_size > 0); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_pixel_x_size_update' BEFORE UPDATE OF pixel_x_size ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table ''gpkg_tile_matrix'' violates constraint: pixel_x_size must be greater than 0') WHERE NOT(NEW.pixel_x_size > 0); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_pixel_y_size_insert' BEFORE INSERT ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table ''gpkg_tile_matrix'' violates constraint: pixel_y_size must be greater than 0') WHERE NOT(NEW.pixel_y_size > 0); END; " +
                    "CREATE TRIGGER 'gpkg_tile_matrix_pixel_y_size_update' BEFORE UPDATE OF pixel_y_size ON 'gpkg_tile_matrix' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table ''gpkg_tile_matrix'' violates constraint: pixel_y_size must be greater than 0') WHERE NOT(NEW.pixel_y_size > 0); END; ";
                command.ExecuteNonQuery();
            }
        }

        private static string InternalGetTileCache(string path)
        {
            string tileCache = "";

            using (var connection = new SQLiteConnection($"Data Source={path}"))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT table_name FROM gpkg_contents";

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        tileCache = reader.GetString(0);
                    }
                }
            }

            return tileCache;
        }

        public static void InsertIntoGpkgContents(SQLiteConnection sqliteConnection, List<TileContent> contentArray)
        {
            SQLiteCommand command;
            foreach (var row in contentArray)
            {
                string query = "INSERT INTO \"gpkg_contents\" (\"table_name\",\"data_type\",\"min_x\",\"min_y\",\"max_x\",\"max_y\") " +
                                      $"VALUES ('{row.tableName}','{row.dataType}',{row.minX},{row.minY},{row.maxX},{row.maxY});";
                command = new SQLiteCommand(query, sqliteConnection);
                command.ExecuteNonQuery();
            }
        }
        public static void InsertDefaultIntoTileMatrix(SQLiteConnection sqliteConnection)
        {
           string query = "INSERT INTO \"gpkg_tile_matrix_set\" VALUES('test',4326,-180,-90,180,90);";
           SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
           command.ExecuteNonQuery();
            
        }

        public static void Create(string path)
        {
            String cache = "mytable";
            SQLiteConnection.CreateFile("fakeDb");
            using (var connection = new SQLiteConnection($"Data Source={path}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SetPragma(connection);
                    CreateSpatialRefTable(connection);
                    CreateContentsTable(connection);
                    CreateGeometryColumnsTable(connection);
                    CreateTileMatrixSetTable(connection);
                    CreateTileMatrixTable(connection);
                    CreateExtentionTable(connection);
                    CreateTileTable(connection, cache);
                    Add2X1MatrixSet(connection, cache);
                    CreateTileMatrixValidationTriggers(connection);
                    transaction.Commit();
                }
            }
        }
    }
}
