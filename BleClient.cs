using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace woop
{
    public class BleClient {
    
        public void startDeviceWatcher(){
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };


            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),requestedProperties,DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;


            Console.WriteLine("Device Watcher has started");
            deviceWatcher.Start();
            Console.WriteLine("Device Watcher has done started");

        }


        void DeviceWatcher_Added(DeviceWatcher deviceWatcher, DeviceInformation deviceInformation){
            
            Console.WriteLine("added");
            Console.WriteLine("id: " + deviceInformation.Id + " name: " + deviceInformation.Name);
        }

        void DeviceWatcher_Updated(DeviceWatcher deviceWatcher, DeviceInformationUpdate deviceInformation){
    
            Console.WriteLine("updatead");
        }

        void DeviceWatcher_Removed(DeviceWatcher deviceWatcher, DeviceInformationUpdate deviceInformation){
            Console.WriteLine("removed");
        }


        async Task ConnectDevice(DeviceInformation deviceInfo){
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
        }
        
    }
}
