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
        public Woop woop = new Woop();

        int counter = 74;


        public StrapClient(BleClient bleClient){
           Console.WriteLine("created the strap client");
           this.bleClient = bleClient;

           
            Console.WriteLine(woop.test);

           bleClient.Connected += ActivateStrap;
        }

        public enum WhoopCommands
        {
            broski,
            cowabunga,
            skibidi,
            Buzz = 68,
            Get_Clock = 11
        }


        public async void ActivateStrap(DeviceInformation deviceinfo){
           
            woop.woop_information = deviceinfo;
            Console.WriteLine("IsPaired at activate time: " + woop.woop_information.Pairing.IsPaired);    

            Console.WriteLine("strap activated");
            woop.device = await BluetoothLEDevice.FromIdAsync(woop.woop_information.Id);
            await SetStrapServices();

            // subscribe to everything and shi
            Console.WriteLine("custome service " + woop.custom_service.Uuid);
            await SubscribeToChars(woop.custom_service);
        }

        private async Task SubscribeToChars(GattDeviceService service){


            Console.WriteLine("IsPaired at subscribe time: " + woop.woop_information.Pairing.IsPaired);    
            var characteristics = await service.GetCharacteristicsAsync();
            foreach(var characteristic in characteristics.Characteristics){
                
                if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                    continue;


                characteristic.ValueChanged += Characteristic_ValueChanged;
                Console.WriteLine("the char has a flag of: " + characteristic.CharacteristicProperties + " uuid: " + characteristic.Uuid + " and i have subscribed");
                var result = await characteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (result.Status != GattCommunicationStatus.Success)
                {
                    Console.WriteLine("light switch status: " + result.ProtocolError);
                    Console.WriteLine("unable to subscribed to notifications ");
                    return;
                }
            }

            Console.WriteLine("subscribed to notifications ");
        }

        public async Task Send_Hello(){
            Console.WriteLine("sending hello yo ");

             byte[] rawBytes = new byte[] 
            { 
            0xAA, 0x01, 0x08, 0x00, 0x00, 0x01, 0xE6, 0x71, 
            0x23, 0x01, 0x91, 0x01, 0x36, 0x3E, 0x5C, 0x8D 
            };

            // 2. Convert the byte array into an IBuffer
            IBuffer data = rawBytes.AsBuffer();

            try
            {
                Console.WriteLine("asdsada");
                GattCommunicationStatus result = await woop.CMD_TO_STRAP.WriteValueAsync(data, GattWriteOption.WriteWithoutResponse);
                Console.WriteLine("sent data and the result is: " + result);
            }
            catch (System.Exception)
            {
                
                throw;
            }

            
        }
        
        

        public async void Send_Buzz_Command_To_Strap(){
            Send_Command_To_Strap(WhoopCommands.Get_Clock);
        }



        private async void Send_Command_To_Strap(WhoopCommands command, byte[] payload = null){
            
            Console.WriteLine("sending a command");
 
            byte[] data = new byte[] {Woop.COMMAND_BYTE, (byte)command, (byte)counter};

            IBuffer buffer = data.AsBuffer();
            Console.WriteLine(string.Join(", ", data));

            
            Console.WriteLine(woop.CMD_TO_STRAP.Uuid);
            GattCommunicationStatus result = await woop.CMD_TO_STRAP.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse);
            
            if(result == GattCommunicationStatus.Success){
                Console.WriteLine("it sent over");
                counter++;
            }

        }


        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args){
            Console.WriteLine("the value changed twin !");

            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);

            Console.WriteLine("sender uuid: " + sender.Uuid + BitConverter.ToString(data));
        }




        
        public async Task SetStrapServices(){
            
            // get services
            GattDeviceServicesResult result = await woop.device.GetGattServicesAsync();
        
            // if got services 
            if(result.Status == GattCommunicationStatus.Success){
                
                woop.services = result;
                Console.WriteLine("I have set the services");

                // print out services
                var services = result.Services;
                int i = 0;
                foreach (var service in services)
                {
                    Console.WriteLine(" uuid: " + service.Uuid);   
                    if (service.Uuid.ToString() == "fd4b0001-cce1-4033-93ce-002d5875f58a")
                    {
                        woop.custom_service = service;
                    }
                }
            }
            SetStrapCharacteristics();
        }

        
        public void SetStrapCharacteristics(){
            GattDeviceService service = woop.custom_service;

            foreach (var characteristic in service.GetAllCharacteristics())
            {
                if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)){
                    woop.CMD_TO_STRAP = characteristic;
                    Console.WriteLine("set the command to strap command");
                }
            }
        }


    }
}





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