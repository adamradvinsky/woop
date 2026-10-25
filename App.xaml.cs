using System.Windows;
//using ble;
//sing strap;

namespace woop_app
{
    public partial class App : Application
    {
        BleClient bleClient;
        Database db;
        public event Action<int> HeartRateUpdated;   
        StrapClient strapClient;

        
        public BleClient BleClient{

            get{
                if (bleClient == null)
                {
                    bleClient = new BleClient();
                    Console.WriteLine("bleclient was null");
                } 
                return bleClient;
            }
        }

        public Database Database{
            get{
                if (db == null)
                {
                    db = new Database();
                    Console.WriteLine("databse was null");
                } 
                return db;
            }
        }

        public StrapClient StrapClient{

            get{
                if (strapClient == null)
                {
                    strapClient = new StrapClient(bleClient, db);
                    Console.WriteLine("strapclient was null");
                } 
                return strapClient;
            }
        }
    }
}