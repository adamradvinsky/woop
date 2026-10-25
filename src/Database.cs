using Microsoft.Data.Sqlite;

namespace woop_app {
    

    // time | heartrate | acceleration | raw bytes
    public class database {

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