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

        public enum WhoopCommands
        {
            broski,
            cowabunga,
            skibidi
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
            
            CMD_TO_STRAP = characteristic uuid: fd4b0002-cce1-4033-93ce-002d5875f58a
             = characteristic uuid: fd4b0003-cce1-4033-93ce-002d5875f58a
             = characteristic uuid: fd4b0004-cce1-4033-93ce-002d5875f58a
             = characteristic uuid: fd4b0005-cce1-4033-93ce-002d5875f58a
             = characteristic uuid: fd4b0007-cce1-4033-93ce-002d5875f58a


        */

        /*
        AA-01-74-00-01-00-3F-B1-24-00-91-01-02-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-0B-B4-F7-F8

        AA-01-74-00-01-00-3F-B1-24-01-91-01-01-01-B5-01-00-00-00-AC-BC-A1-6A-F5-48-00-00-35-42-30-30-32-33-39-31-31-33-00-31-62-30-36-36-35-35-63-30-31-61-30-37-65-31-65-39-32-34-62-62-63-66-31-64-37-66-61-65-63-37-36-36-64-61-35-63-37-38-38-63-61-34-30-62-65-37-35-61-39-34-34-66-38-0D-00-00-00-00-00-00-00-52-00-00-00-32-23-03-00-00-00-00-0A-03-20-01-01-00-00-00-00-B7-A9-46-3B
        
        08-42-71-59-51-58-66-45-48-74-43-6D-77-31-30-54-4F-38-59-61-48-4F-63-4E-51-54-5A-34-78-4C-6D-5A-61-31-00-A7-02-02-03-01-0A-68-6D-61-76-65-72-69-63-6B-09-69-35-30-2E-33-35-2E-33-2E-30-06-68-57-47-35-30-5F-72-35-32-01-1A-6A-A1-BC-A7-04-A5-01-19-10-01-02-1A-00-05-87-B2-03-1A-00-05-88-89-04-01-05-00-F2-07 
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

        public async Task ActivateStrap(){


            // subscribe to everything and shi
            Console.WriteLine("strap ready n shi");

            foreach(var service in services.Services){
                Console.WriteLine("my services: " + service.Uuid);

                // if custom service
                if (service.Uuid.ToString() == "fd4b0001-cce1-4033-93ce-002d5875f58a"){
                    custom_service = service;
                    Console.WriteLine("custom service uuid: " + custom_service.Uuid);

                    Guid uuid = new Guid("fd4b0002-cce1-4033-93ce-002d5875f58a");
                    GattCharacteristicsResult result = await service.GetCharacteristicsForUuidAsync(uuid); 
                    var list = result.Characteristics;
                    CMD_TO_STRAP = list[0];

                    Console.WriteLine("CMD_TO_STRAP: " + CMD_TO_STRAP.Uuid);

                }
            }

            //Console.WriteLine("notify char: " + notifyChar.Uuid);
            SubscribeToChars(custom_service);
            //ReadFromChar(notifyChar);

        }

        private async void SubscribeToChars(GattDeviceService service){

            var characteristics = await service.GetCharacteristicsAsync();
            foreach(var characteristic in characteristics.Characteristics){
                
                if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                    continue;


                characteristic.ValueChanged += Characteristic_ValueChanged;
                Console.WriteLine("the char has a flag of: " + characteristic.CharacteristicProperties + " uuid: " + characteristic.Uuid + " and i have subscribed");
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
        
        

        private async void Send_Buzz_Command_To_Strap(){
            

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

        private void Send_Command_To_Strap(){
            
            Console.WriteLine("sending a command");

            // IBuffer buffer = new IBuffer();
            // GattCommunicationStatus result = CMD_TO_STRAP.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse);
            
        }


        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args){
            Console.WriteLine("the value changed twin !");

            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);

            Console.WriteLine("sender uuid: " + sender.Uuid + BitConverter.ToString(data));

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