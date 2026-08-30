using woop;


namespace woop
{
    class Program {
        static void Main(string[] args){

            Console.WriteLine("Hello, World!");

            BleClient bleClient = new BleClient();

            bleClient.startDeviceWatcher();
            
            Console.WriteLine("Press 'Enter' to exit the program.");
            Console.ReadLine();
        }
    }
}