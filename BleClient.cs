using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace woop
{
    public static class BleClient {
        
        static List<DeviceInformation> device_ids = new List<DeviceInformation>();
        static int devices = 0;
    


        static public void startDeviceWatcher(){
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),requestedProperties,DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;


            Console.WriteLine("Device Watcher has started");
            deviceWatcher.Start();
            Console.WriteLine("Device Watcher has done started");

        }

        public static async Task ConnectDevice(int device_number){

            Console.WriteLine("going to try and connect with device: " + device_number);
            Console.WriteLine("with name: " + device_ids[device_number].Name);

            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(device_ids[device_number].Id);
            
            GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();
            

            if (result.Status == GattCommunicationStatus.Success)
            {

                // gets services
                var services = result.Services;
                int i = 0;
                foreach (var service in services)
                {
                    Console.WriteLine("service " + i + " uuid: "+ service.Uuid);
                    i++;
                    
                }

                String input = Console.ReadLine();

                GattCharacteristicsResult char_properties = await services[int.Parse(input)].GetCharacteristicsAsync();

                if (char_properties.Status == GattCommunicationStatus.Success)
                {
                    // get characteristics to this service
                    var characteristics = char_properties.Characteristics;
                    Console.WriteLine("characteristics: " + characteristics);

                    foreach (var characteristic in characteristics)
                    {
                        Console.WriteLine("characteristic uuid: "+ characteristic.Uuid);
                    }

                    Console.WriteLine("bleh");

                    GattCharacteristicProperties properties = characteristics[0].CharacteristicProperties;
                    
                    // if(properties.HasFlag(GattCharacteristicProperties.Read))
                    // {
                    //     GattReadResult read_result = await characteristics[0].ReadValueAsync();
                    //     if(read_result.Status == GattCommunicationStatus.Success){
                    //         // var reader = DataReader.FromBuffer(result.Value);
                    //         // byte[] input = new byte[reader.UnconsumedBufferLength];
                    //         // string read = reader.ReadBytes(input);
                    //         var read = read_result.Value;
                    //         Console.WriteLine("i read: " + read);
                    //     }
                    // }

                    if(properties.HasFlag(GattCharacteristicProperties.Notify))
                    {
                        
                    }



                }
            }


            
            return;
        }



        static void DeviceWatcher_Added(DeviceWatcher deviceWatcher, DeviceInformation deviceInformation){
            
            Console.WriteLine("added");
            Console.WriteLine("number: " + devices + "  id: " + deviceInformation.Id + " name: " + deviceInformation.Name);
            device_ids.Add(deviceInformation);
            devices++;

        }

        static void DeviceWatcher_Updated(DeviceWatcher deviceWatcher, DeviceInformationUpdate deviceInformation){
    
            Console.WriteLine("updatead");
        }

        static void DeviceWatcher_Removed(DeviceWatcher deviceWatcher, DeviceInformationUpdate deviceInformation){
            //Console.WriteLine("removed: " + deviceInformation.Id.Name);
            devices--;
        }

        
    }
}
