using System.Windows;
using ble;
using strap;

namespace woop_app
{
    public partial class App : Application
    {
        BleClient bleClient;
        public event Action<int> HeartRateUpdated;   
        StrapClient strapClient;

        
        public BleClient BleClient{

            get{
                if (bleClient == null)
                {
                    bleClient = new BleClient();
                    Console.WriteLine("it was null");
                } 
                return bleClient;
            }
        }

        public StrapClient StrapClient{

            get{
                if (strapClient == null)
                {
                    strapClient = new StrapClient(bleClient);
                    Console.WriteLine("it was null");
                } 
                return strapClient;
            }
        }
    }
}