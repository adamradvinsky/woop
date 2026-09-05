using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using System.Text;
using Windows.Storage.Streams;
using System;
using System.Diagnostics;

namespace woop
{
    public class BleClient {
        
        public enum status{
            Scanning,
            Pairing,
            Connected
        }
        
        private DeviceInformation myWoopInformation;
        
        public event Action<List<string>>? Pairable_Devices;
        public event Action<string>? Connected;
        public event Action<int> HeartRateUpdated;
        static List<DeviceInformation> device_ids = new List<DeviceInformation>();
        static int devices = 0;

        public BleClient(){

            Console.WriteLine("BLE CLIENT CREATED");
            //myWoopInformation.Id = ;


        }


        public void startDeviceWatcher(){
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),requestedProperties,DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;


            Console.WriteLine("Device Watcher has started");
            deviceWatcher.Start();
            Console.WriteLine("Device Watcher has finished");

        }

        public async Task ScanForDevice(){
            startDeviceWatcher();

            // wait 20s 

            //  bro i couldnt find a woop

        }

        public void testTerminalMessage(){
            Console.WriteLine("skib test skib");
        }

        public async Task ConnectDevice(int device_number){

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

                Console.WriteLine("Choose a service");
                String input = Console.ReadLine();
                int val = int.Parse(input);

                var chosen_service = services[val];

                GattCharacteristicsResult char_properties = await chosen_service.GetCharacteristicsAsync();


                if (char_properties.Status == GattCommunicationStatus.Success)
                {
                    // get characteristics to this service
                    var characteristics = char_properties.Characteristics;
                    Console.WriteLine("characteristics: " + characteristics);

                    foreach (var characteristic in characteristics)
                    {
                        Console.WriteLine("characteristic uuid: "+ characteristic.Uuid);
                    }


                    // choose a characteristic
                    Console.WriteLine("Choose a characteristic");
                    input = Console.ReadLine();
                    val = int.Parse(input);                
                    
                    var chosen_characteristic = characteristics[val];


                    GattCharacteristicProperties properties = chosen_characteristic.CharacteristicProperties;
                    

                    if(properties.HasFlag(GattCharacteristicProperties.Notify))
                    {
                        GattCommunicationStatus status = await chosen_characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        // subscribe to the event that the value changed
                        if(status == GattCommunicationStatus.Success){
                            Console.WriteLine("bleh");
                            chosen_characteristic.ValueChanged += characteristic_ValueChanged;
                        }
                    }
                //await Task.Delay(10000);
                input = Console.ReadLine(); 


                }
            }


            return;
        }

        private void characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args){
            Console.WriteLine("the value changed twin !");

            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);

            byte flags = data[0];
            bool is16Bit = (flags & 0x1) == 1;
            int offset = 1;

            //Console.WriteLine(string.Join(", ", data));
            ushort heartRate;

            if (is16Bit)
            {
                Console.WriteLine("16 bit hr");
                // uses 2 bytes for HR
                byte a = data[1];
                byte b = data[2];
                heartRate = (UInt16)((a << 8) | b);

                offset += 2;
            } else {
                // uses 1 byte for HR 
                Console.WriteLine("8 bit hr");

                heartRate = data[offset];
                offset++;
            }

            Console.WriteLine("HR: " + heartRate);
            HeartRateUpdated?.Invoke(heartRate);
        }

        List<string> device_list = new List<string>{"phone"};

        private void DeviceWatcher_Added(DeviceWatcher deviceWatcher, DeviceInformation deviceInformation){

          
            Console.WriteLine("number: " + devices + "  id: " + deviceInformation.Id + " name: " + deviceInformation.Name);
            device_ids.Add(deviceInformation);
            device_list.Add(deviceInformation.Name);
            Pairable_Devices?.Invoke(device_list);
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
