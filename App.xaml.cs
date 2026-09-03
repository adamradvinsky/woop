using System.Windows;
using woop;

namespace woop_app
{
    public partial class App : Application
    {
        BleClient bleClient;
        
        public BleClient BleClient{

            get{
                if (bleClient == null)
                {
                    BleClient bleClient = new BleClient();
                } 
                return bleClient;
            }
        }
    }
}