using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using System.Text;
using Windows.Storage.Streams;
using System;
using System.Diagnostics;
using woop_app;
using ble;
using System.Runtime.InteropServices.WindowsRuntime;

namespace strap
{
    public class StrapClient{

        private BleClient bleClient;

        public StrapClient(BleClient bleClient){
           Console.WriteLine("created the strap client");
           this.bleClient = bleClient;

           bleClient.Got_Services += setServices;
        }

        

        /*
        services
         uuid: 00001800-0000-1000-8000-00805f9b34fb
        uuid: 00001801-0000-1000-8000-00805f9b34fb
 
        heartrate
        uuid: 0000180d-0000-1000-8000-00805f9b34fb

        device information
        uuid: 0000180a-0000-1000-8000-00805f9b34fb

        battery information
        uuid: 0000180f-0000-1000-8000-00805f9b34fb

        custom service !!
        uuid: fd4b0001-cce1-4033-93ce-002d5875f58a
        */

        private GattDeviceServicesResult services;
        private GattCharacteristic CMD_TO_STRAP;
        private GattCharacteristic CMD_FROM_STRAP;
        private GattCharacteristic EVENTS_FROM_STRAP;
        private GattCharacteristic DATA_FROM_STRAP;
        private GattCharacteristic MEMFAULT;

        private GattDeviceService custom_service;

        public GattDeviceServicesResult getServices(){
            return services;
        }

        public void setServices(GattDeviceServicesResult services){
            this.services = services;
            
            if (this.services != null)
                ActivateStrap();
            
        }

        public void ActivateStrap(){


            // subscribe to everything and shi
            Console.WriteLine("strap ready n shi");

            foreach(var service in services.Services){
                Console.WriteLine("my services: " + service.Uuid);

                if (service.Uuid.ToString() == "fd4b0001-cce1-4033-93ce-002d5875f58a"){
                    custom_service = service;
                    Console.WriteLine("custom service uuid: " + custom_service.Uuid);
                }
            }

            //Console.WriteLine("notify char: " + notifyChar.Uuid);
            SubscribeToChars(custom_service);
            //ReadFromChar(notifyChar);

        }

        private async void SubscribeToChars(GattDeviceService service){

            var characteristics = await service.GetCharacteristicsAsync();
            foreach(var characteristic in characteristics.Characteristics){

                if(string.Equals(characteristic.Uuid.ToString(), "fd4b0002-cce1-4033-93ce-002d5875f58a", StringComparison.OrdinalIgnoreCase)){
                    Console.WriteLine("sAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                    CMD_TO_STRAP = characteristic;
                }
                
                Console.WriteLine("the char has a flag of: " + characteristic.CharacteristicProperties + " uuid: " + characteristic.Uuid);
                
                if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                    continue;


                characteristic.ValueChanged += Characteristic_ValueChanged;
                var result = await characteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }

            Console.WriteLine("subscribed to notifications ");
            await SendHello();
        }

        private async Task SendHello(){
            Console.WriteLine("sending hello yo ");

             byte[] rawBytes = new byte[] 
        { 
            0xAA, 0x01, 0x08, 0x00, 0x00, 0x01, 0xE6, 0x71, 
            0x23, 0x01, 0x91, 0x01, 0x36, 0x3E, 0x5C, 0x8D 
        };

        // 2. Convert the byte array into an IBuffer
            IBuffer data = rawBytes.AsBuffer();

            GattCommunicationStatus result = await CMD_TO_STRAP.WriteValueAsync(data, GattWriteOption.WriteWithResponse);
            
            Console.WriteLine("sent data and the result is: " + result);

        }
        
        

        private async void SendBuzzCommand(){
            

        }

        private async void ReadFromChar(GattCharacteristic characteristic){
            GattReadResult result = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
            
            Console.WriteLine(result.Status);

            if (result.Status == GattCommunicationStatus.Success)
            {
                byte[] bytes = result.Value.ToArray();
                Console.WriteLine(bytes);
            }
        }

        private void WriteToChar(GattCharacteristic characteristic){

        }


        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args){
            Console.WriteLine("the value changed twin !");

            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);

            Console.WriteLine(data);

            // byte flags = data[0];
            // bool is16Bit = (flags & 0x1) == 1;
            // int offset = 1;

            // //Console.WriteLine(string.Join(", ", data));
            // ushort heartRate;

            // if (is16Bit)
            // {
            //     Console.WriteLine("16 bit hr");
            //     // uses 2 bytes for HR
            //     byte a = data[1];
            //     byte b = data[2];
            //     heartRate = (UInt16)((a << 8) | b);

            //     offset += 2;
            // } else {
            //     // uses 1 byte for HR 
            //     Console.WriteLine("8 bit hr");

            //     heartRate = data[offset];
            //     offset++;
            // }

            // Console.WriteLine("HR: " + heartRate);
            // //HeartRateUpdated?.Invoke(heartRate);
        }


    }
}