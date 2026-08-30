using woop;


namespace woop
{
    class Program {
        static async Task Main(string[] args){

            Console.WriteLine("Hello, World!");

            //BleClient bleClient = new BleClient();

            BleClient.startDeviceWatcher();

            String input = "1";

            input = Console.ReadLine();
                
            BleClient.ConnectDevice(int.Parse(input)).Wait();

            
            
            
        }
    }
}