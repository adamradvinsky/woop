using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using woop;

namespace woop_app
{
    public partial class MainWindow : Window
    {

        
        // Standard Bluetooth SIG Heart Rate service/characteristic UUIDs, used as a
        // fallback. WHOOP almost certainly exposes its own proprietary GATT service
        // instead of this standard profile - swap these once you've pulled the real
        // UUIDs out of your reverse-engineering work.
        // private static readonly Guid HeartRateServiceUuid = GattServiceUuids.HeartRate;
        // private static readonly Guid HeartRateMeasurementCharUuid = GattCharacteristicUuids.HeartRateMeasurement;

        // private BluetoothLEDevice _device;
        // private GattCharacteristic _heartRateCharacteristic;

        // private readonly List<double> _heartRateHistory = new();
        // private const int MaxHistoryPoints = 60;

        // // Fakes incoming BPM data so the UI can be exercised without a strap connected.
        // private readonly DispatcherTimer _simulationTimer = new();
        // private readonly Random _rng = new();


        private BleClient bleClient;

        public MainWindow()
        {
            InitializeComponent();
            
            Application currentApp = Application.Current;

            App myApp = (App)currentApp;

            bleClient = myApp.BleClient;

        }

        List<string> test = new List<string>{ "Apple", "Banana", "Cherry", "Date" };

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("bleClient is null: " + (bleClient == null));
            ConnectButton.IsEnabled = false;

            bleClient.Pairable_Devices += updatePairableDevices;
            

            bleClient.ScanForDevice();
            //bleClient.testTerminalMessage();
            changeStatusText("broski");

            StatusText.Foreground = Brushes.Yellow;
            //StatusText.Text = "bruh...";
            updatePairableDevices(test);

        }

        public void changeStatusText(string new_text){
            Console.WriteLine("trying to change it to: " + new_text);
            StatusText.Text = new_text;
        }

        private void HeartRateCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // Standard HR measurement payload: byte 0 = flags, then either a uint8 or
            // uint16 BPM value depending on flag bit 0.
            // DataReader reader = DataReader.FromBuffer(args.CharacteristicValue);
            // byte flags = reader.ReadByte();
            // bool isUInt16 = (flags & 0x01) != 0;
            // double bpm = isUInt16 ? reader.ReadUInt16() : reader.ReadByte();
            double bpm = 5;
            Dispatcher.Invoke(() => UpdateHeartRate(bpm));
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            //double bpm = 60 + _rng.NextDouble() * 40;
            int bpm = 6;
            UpdateHeartRate(bpm);
        }

        private void UpdateHeartRate(double bpm)
        {
    
        }

        private void updatePairableDevices(List<string> items){
            //List<string> items = new List<string> { "Apple", "Banana", "Cherry", "Date" };

            if (!Pairable_Devices.Dispatcher.CheckAccess()) {
                Pairable_Devices.Dispatcher.Invoke(() => updatePairableDevices(items));
            } 

            Console.WriteLine("list bruh");
            // 2. Create the UI ListBox control
            ListBox myListBox = new ListBox();

            // 3. Give the items to the ListBox
            myListBox.ItemsSource = items;

            // 4. Add the ListBox into your XAML Grid dynamically
            Pairable_Devices.Children.Add(myListBox);
            
        }


      

        
    }
}