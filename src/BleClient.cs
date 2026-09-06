using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using System.Text;
using Windows.Storage.Streams;
using System;
using System.Diagnostics;
using woop_app;

namespace woop
{
    public class BleClient {
        
        public enum whoop_status{
            Scanning,
            Pairing,
            Connected,
            Disconnected
        }
        
        public DeviceInformation connected_Device;
        private DeviceInformation myWoopInformation;
        
        public event Action<DeviceInformation>? Pairable_Devices_Add;
        public event Action<string>? Connected;
        public event Action<int> HeartRateUpdated;      
        static List<DeviceInformation> device_ids = new List<DeviceInformation>();
        static int devices = 0;

        public BleClient(){

            Console.WriteLine("BLE CLIENT CREATED");


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
        
        public async void DisconnectDevice(){
            Console.WriteLine("unpairing ");
            
            DeviceUnpairingResult result = await connected_Device.Pairing.UnpairAsync();
            
            if (connected_Device != null)
            {
                connected_Device = null;
            } 

        }

        public async Task<bool> ConnectDevice(DeviceInformation device){

            Console.WriteLine("going to try and connect with device: " + device.Id);
            Console.WriteLine("with name: " + device.Name);

            // checks to see if paired or not
            if(!device.Pairing.IsPaired){
                DevicePairingResult result = await device.Pairing.PairAsync(DevicePairingProtectionLevel.EncryptionAndAuthentication);
                connected_Device = device;

                // is pairing successful?
                if (result.Status == DevicePairingResultStatus.Paired){
                    Console.WriteLine("DEVICE IS NOW PAIRED");
                    return true;

                } else {
                    Console.WriteLine("COULDNT PAIR TO DEVICE");
                    return false;
                }

            } else {
                Console.WriteLine("DEVICE IS ALREADY PAIRED");
                return false;
            }


            // BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(device_ids[device_number].Id);
            
            // // get services
            // GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();
            

            // if (result.Status == GattCommunicationStatus.Success)
            // {

            //     // gets services
            //     var services = result.Services;
            //     int i = 0;
            //     foreach (var service in services)
            //     {
            //         Console.WriteLine("service " + i + " uuid: "+ service.Uuid);
            //         i++;
                    
            //     }

            //     Console.WriteLine("Choose a service");
            //     String input = Console.ReadLine();
            //     int val = int.Parse(input);

            //     var chosen_service = services[val];

            //     GattCharacteristicsResult char_properties = await chosen_service.GetCharacteristicsAsync();


            //     if (char_properties.Status == GattCommunicationStatus.Success)
            //     {
            //         // get characteristics to this service
            //         var characteristics = char_properties.Characteristics;
            //         Console.WriteLine("characteristics: " + characteristics);

            //         foreach (var characteristic in characteristics)
            //         {
            //             Console.WriteLine("characteristic uuid: "+ characteristic.Uuid);
            //         }


            //         // choose a characteristic
            //         Console.WriteLine("Choose a characteristic");
            //         input = Console.ReadLine();
            //         val = int.Parse(input);                
                    
            //         var chosen_characteristic = characteristics[val];


            //         GattCharacteristicProperties properties = chosen_characteristic.CharacteristicProperties;
                    

            //         if(properties.HasFlag(GattCharacteristicProperties.Notify))
            //         {
            //             GattCommunicationStatus status = await chosen_characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

            //             // subscribe to the event that the value changed
            //             if(status == GattCommunicationStatus.Success){
            //                 Console.WriteLine("bleh");
            //                 chosen_characteristic.ValueChanged += characteristic_ValueChanged;
            //             }
            //         }
            //     //await Task.Delay(10000);
            //     input = Console.ReadLine(); 


            //     }
            // }


            // return true;
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
    
            Console.WriteLine("updatead");
        }

        static void DeviceWatcher_Removed(DeviceWatcher deviceWatcher, DeviceInformationUpdate deviceInformation){
            //Console.WriteLine("removed: " + deviceInformation.Id.Name);
            devices--;
        }
     
    }
}
