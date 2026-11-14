using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading;

namespace woop_app {
    
    // time | heartrate | acceleration | raw bytes
    public class Database {

        private readonly string _connString;

        public Database(){

            Console.WriteLine("creating the database ");

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

        private List<Row> buffer = new();

        // // insert into database
        // public void insert(Row s){
        //     using var conn = new SqliteConnection(_connString);
        //     conn.Open();
        //     using var insert_cmd = conn.CreateCommand();
        //     insert_cmd.CommandText = @"INSERT OR IGNORE INTO woop (time, hr, ax, ay, az, raw)
        //                       VALUES ($time, $hr, $ax, $ay, $az, $raw)";

            
        //     // lock buffer
        //     foreach (var s in buffer)
        //     {
        //         insert_cmd.Parameters.AddWithValue("$time", s.time);

        //         insert_cmd.Parameters.AddWithValue("$hr", s.hr);
            
        //         insert_cmd.Parameters.AddWithValue("$ax", s.ax);
        //         insert_cmd.Parameters.AddWithValue("$ax", s.ay);
        //         insert_cmd.Parameters.AddWithValue("$ax", s.az);
                
        //         insert_cmd.Parameters.AddWithValue("$raw", s.bytes);
        //         insert_cmd.ExecuteNonQuery();
        //     }
        // }


        private readonly Lock bufferlock = new();

        private int max_buffer = 100;

        // add to a queue that will later then be inserted
        public void add_to_buffer(Row row){

            lock( bufferlock ) {
                buffer.Add(row);

                if(buffer.Count() > max_buffer) 
                    flush_to_db();

            }
        }




        

        private void flush_to_db()
        {
            lock (bufferlock)
            {

                Console.WriteLine("flush ");
                if (buffer.Count == 0) return;

                try
                {
                    using var conn = new SqliteConnection(_connString);
                    conn.Open();
                    using var tx = conn.BeginTransaction();

                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandText = @"INSERT OR IGNORE INTO woop (time, hr, ax, ay, az, raw)
                                        VALUES ($time, $hr, $ax, $ay, $az, $raw)";

                    var pTime = cmd.Parameters.Add("$time", SqliteType.Integer);
                    var pHr   = cmd.Parameters.Add("$hr",   SqliteType.Integer);
                    var pAx   = cmd.Parameters.Add("$ax",   SqliteType.Real);
                    var pAy   = cmd.Parameters.Add("$ay",   SqliteType.Real);
                    var pAz   = cmd.Parameters.Add("$az",   SqliteType.Real);
                    var pRaw  = cmd.Parameters.Add("$raw",  SqliteType.Blob);

                    foreach (var s in buffer)
                    {
                        pTime.Value = (long)s.time;
                        pHr.Value = s.hr;
                        pAx.Value = s.ax;
                        pAy.Value = s.ay;
                        pAz.Value = s.az;
                        pRaw.Value = s.bytes;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();     // nothing is saved without this line
                    buffer.Clear();  // only reached if the commit worked
                }
                catch (Exception ex)
                {
                    Console.WriteLine("db write failed: " + ex.Message);
                }
            }
        }
    }
}