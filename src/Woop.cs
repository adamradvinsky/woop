using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using System.Text;
using Windows.Storage.Streams;
using System;
using System.Diagnostics;
// using woop_app;
// using strap;

namespace woop_app{



    public class Woop{
 
        public Woop(){

        }
        
        public BluetoothLEDevice device;
        public DeviceInformation woop_information;


        public GattDeviceServicesResult services;

        public string test = "skibidi";

        public GattDeviceService custom_service;
        public int counter = 0;

        public GattCharacteristic CMD_TO_STRAP;
        private GattCharacteristic CMD_FROM_STRAP;
        private GattCharacteristic EVENTS_FROM_STRAP;
        private GattCharacteristic DATA_FROM_STRAP;
        private GattCharacteristic MEMFAULT;

    
        public const byte COMMAND_BYTE = 35;

        // battery
        // AA-01-10-00-01-00-20-81-24-17-1A-00-01-15-00-00-00-00-00-00-76-55-EF-2E

        // clock
        // AA-01-14-00-01-00-21-B1-24-1B-0B-00-01-4C-73-AF-6A-CC-0C-00-00-00-00-00-E5-43-DB-A6

        // 67
        // AA-01-20-00-01-00-2F-81-24-1E-43-00-00-04-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-6D-2B-66-A6

        // GET_ADVERTISING_NAME = 76
        // AA-01-0C-00-01-00-27-11-24-34-4C-00-03-00-00-00-7D-57-1D-2E
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
              fd4b0001-cce1-4033-93ce-002d5875f58a
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

}
}


/*

    after being in pairing mode and pairing for first time 
    and then sending hello 

    
fd4b0007-cce1-4033-93ce-002d5875f58a08-
42-71-59-51-58-66-45-48-74-43-6D-77-31-30-54-4F-38-59-61-48-4F-63-4E-51-54-5A-34-78-4C-6D-5A-61-31-00-A7-02-02-03-01-0A-68-6D-61-76-65-72-69-63-6B-09-69-35-30-2E-33-35-2E-33-2E-30-06-68-57-47-35-30-5F-72-35-32-01-1A-6A-A8-AC-54-04-A5-01-19-10-01-02-1A-00-05-87-B2-03-1A-00-05-88-89-04-01-05-00-39-A2

sender uuid: fd4b0004-cce1-4033-93ce-002d5875f58aAA
01-10-00-01-00-20-81-30-60-09-00-86-AC-A8-6A-CC-6C-00-00-4B-E7-F9-8D

Send Hello button clicked
fd4b0003-cce1-4033-93ce-002d5875f58aA
01-74-00-01-00-3F-B1-24-00-91-01-02-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-0B-B4-F7-F8
fd4b0003-cce1-4033-93ce-002d5875f58aAA
01-74-00-01-00-3F-B1-24-01-91-01-01-01-50-02-00-00-00-8A-AC-A8-6A-5C-2F-00-00-35-42-30-30-32-33-39-31-31-33-00-31-62-30-36-36-35-35-63-30-31-61-30-37-65-31-65-39-32-34-62-62-63-66-31-64-37-66-61-65-63-37-36-36-64-61-35-63-37-38-38-63-61-34-30-62-65-37-35-61-39-34-34-66-38-0D-00-00-00-00-00-00-00-52-00-00-00-32-23-03-00-00-00-00-0A-03-20-01-01-00-00-00-00-E8-F7-CE-4F



*/