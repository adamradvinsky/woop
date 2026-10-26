using Microsoft.Data.Sqlite;

namespace woop_app {
    

    // time | heartrate | acceleration | raw bytes
    public class Database {

        public Database(){
            Console.WriteLine("created the databse ");
            
            private readonly string _connString;
            
            string folder = Path.Combine(AppContext.BaseDirectory, "data");
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

        public void insert(Sample s){


        }



        private string initialize_database = "CREATE TABLE IF NOT EXISTS woop (
                                                time INTEGER PRIMARY KEY,
                                                hr   INTEGER NOT NULL,
                                                ax REAL, ay REAL, az REAL,
                                                raw  BLOB NOT NULL
                                                );";

        // INTEGER PRIMARY KEY is time
        public void connectDataBase(){
            using var conn = new sqliteConnection("Data Source=\\data.db");
            conn.Open();

            // insert a new line command
            using var insert = conn.CreateCommand();
            insert.CommandText = "$hr $time $bruh";
        }


        public void Insert_Packet(){
            insert.Paramaters.AddWithValue("$hr", 80);
            var count = inert.ExecuteNonQuery();

        }

        

    }


}