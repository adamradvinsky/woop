using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using System.Text;
using Windows.Storage.Streams;
using System;
using System.Diagnostics;
using woop_app;
using strap;

namespace ble
{
    public class BleClient {
        
        public enum whoop_status{
            Scanning,
            Pairing,
            Connected,
            Disconnected
        }


        private Guid whoop_custom_service_uuid = new Guid("fd4b0001-cce1-4033-93ce-002d5875f58a");

        public event Action<BluetoothLEDevice> Connected;
        public event Action UnableToConnect;
        public event Action Disconnect;

        
        public event Action<GattDeviceServicesResult> Got_Services;

        public event Action<DeviceInformation>? Pairable_Devices_Add;
           
        static List<DeviceInformation> device_ids = new List<DeviceInformation>();
        static int devices = 0;

        public BleClient(){

            Console.WriteLine("BLE CLIENT CREATED");

        }



        public void startDeviceWatcher(){
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            
            string aqsSelector = "(System.Devices.Aep.IsPaired:=System.StructuredQueryType.Boolean#True " +
                     "OR System.Devices.Aep.IsPaired:=System.StructuredQueryType.Boolean#False)";

            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher(aqsSelector ,requestedProperties ,DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;


            Console.WriteLine("Device Watcher has started");
            deviceWatcher.Start();
            Console.WriteLine("Device Watcher has finished");

        }


        public async Task ScanForDevice(){
            startDeviceWatcher();
        }

    
        public async Task<bool> ConnectDevice(DeviceInformation device){
            Console.WriteLine("going to try and connect with device: " + device.Id);
            Console.WriteLine("with name: " + device.Name);

            if (!device.Pairing.IsPaired)
            {
                Console.WriteLine("new device hasnt been paired before: TRYING TO PAIR WITH DEVICE");
                
                DevicePairingResult result = await device.Pairing.PairAsync(DevicePairingProtectionLevel.EncryptionAndAuthentication);
                // is pairing successful?
                if (result.Status == DevicePairingResultStatus.Paired){
                    Console.WriteLine("DEVICE IS NOW PAIRED");

                    string savedDeviceId = device.Id;
                    
                    BluetoothLEDevice encrypted_device = await BluetoothLEDevice.FromIdAsync(savedDeviceId);


                    Connected?.Invoke(encrypted_device);
                    return true;

                } else {
                    Console.WriteLine("COULDNT PAIR TO DEVICE because ");
                    UnableToConnect?.Invoke();
                    return false;
                }

            } else {
                Console.WriteLine("device is saved so has been paired before");
                BluetoothLEDevice pairedDevice = await BluetoothLEDevice.FromIdAsync(device.Id);

                if (pairedDevice == null){
                    Console.WriteLine("Failed to load paired device instance.");
                    UnableToConnect?.Invoke();
                    return false;
                }

                
                Connected?.Invoke(pairedDevice);
                return true;

            }

            return false;
        }
    



        private void DeviceWatcher_Added(DeviceWatcher deviceWatcher, DeviceInformation deviceInformation){

          
            //Console.WriteLine("number: " + devices + "  id: " + deviceInformation.Id + " name: " + deviceInformation.Name);
            //device_ids.Add(deviceInformation);
            //device_list.Add(deviceInformation.Name);
            if (deviceInformation.Name != "")
            {
                Pairable_Devices_Add?.Invoke(deviceInformation);
            }
            //devices++;
            


        }

        static void DeviceWatcher_Updated(DeviceWatcher deviceWatcher, DeviceInformationUpdate deviceInformation){
    
            //Console.WriteLine("updatead");
        }

        static void DeviceWatcher_Removed(DeviceWatcher deviceWatcher, DeviceInformationUpdate deviceInformation){
            //Console.WriteLine("removed: " + deviceInformation.Id.Name);
            devices--;
        }
     
    }
}
