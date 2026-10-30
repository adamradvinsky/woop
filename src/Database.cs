using Microsoft.Data.Sqlite;
using System.IO;


namespace woop_app {
    
    // time | heartrate | acceleration | raw bytes
    public class Database {

        private readonly string _connString;
        public Database(){

            Console.WriteLine("created the databse ");
            
            
            Console.WriteLine("appcontext" + AppContext.BaseDirectory);
            

            string folder = Path.Combine(AppContext.BaseDirectory, "data");
            Console.WriteLine("folder" + folder);

            // creates data folder
            Directory.CreateDirectory(folder);
            
            string path = Path.Combine(folder, "woop.db");

            Console.WriteLine("data base file path: " + path);

            _connString = $"Data Source={path}";

            using var conn = new SqliteConnection(_connString);
            conn.Open();

            using var create_table_cmd = conn.CreateCommand();
            create_table_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS woop (
                                    time INTEGER PRIMARY KEY,
                                    hr   INTEGER NOT NULL,
                                    ax REAL, 
                                    ay REAL, 
                                    az REAL,
                                    raw  BLOB NOT NULL
                                    );";

            // execute create command
            create_table_cmd.ExecuteNonQuery();
        }



        // hallowoeeen
        public void insert(Row s){
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            using var insert_cmd = conn.CreateCommand();
            insert_cmd.CommandText = @"INSERT OR IGNORE INTO woop (time, hr, ax, ay, az, raw)
                              VALUES ($time, $hr, $ax, $ay, $az, $raw)";

            insert_cmd.Parameters.AddWithValue("$time", s.time);

            insert_cmd.Parameters.AddWithValue("$hr", s.hr);
            
            insert_cmd.Parameters.AddWithValue("$ax", s.ax);
            insert_cmd.Parameters.AddWithValue("$ax", s.ay);
            insert_cmd.Parameters.AddWithValue("$ax", s.az);
            
            // scoirear
            // asdsad
            insert_cmd.Parameters.AddWithValue("$raw", s.bytes);

            
            insert_cmd.ExecuteNonQuery();
        }
    }
}