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
using Microsoft.UI.Xaml;
using Windows.Storage.Streams;
using ble;
using strap;

namespace woop_app
{
    public partial class MainWindow : Window
    {
        public event Action<int> ui_Connect_Device;

        private BleClient bleClient;
        private StrapClient strapClient;

        public MainWindow()
        {
            InitializeComponent();

            this.Closed += MainWindow_Closed;
            AppDomain.CurrentDomain.ProcessExit += MainWindow_Closed;
            AppDomain.CurrentDomain.UnhandledException += MainWindow_Closed;

            Application currentApp = Application.Current;
            App myApp = (App)currentApp;

            bleClient = myApp.BleClient;
            strapClient = myApp.StrapClient;
        }

        // ---------------------------------------------------------
        // TEST DATA
        // ---------------------------------------------------------

        List<string> test = new List<string>
        {
            "Apple",
            "Banana",
            "Cherry",
            "Date"
        };

        // ---------------------------------------------------------
        // CONNECT BUTTON
        // ---------------------------------------------------------

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("bleClient is null: " + (bleClient == null));

            ConnectButton.IsEnabled = false;

            bleClient.Pairable_Devices_Add += updatePairableDevices;

            bleClient.ScanForDevice();

            StatusText.Foreground = Brushes.Yellow;
            StatusText.Text = "Scanning...";
        }

        // ---------------------------------------------------------
        // SEND HELLO BUTTON
        // ---------------------------------------------------------

        private void SendHelloButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Send Hello button clicked");

            StatusText.Text = "Sending Hello...";
            StatusText.Foreground = Brushes.Yellow;

            // TODO:
            // Add BLE command for "Hello" here.
            strapClient.Send_Hello();

            // Temporary test
            Console.WriteLine("HELLO");

            StatusText.Text = "Hello sent";
            StatusText.Foreground = Brushes.LightGreen;
        }

        // ---------------------------------------------------------
        // SEND COMMAND BUTTON
        // ---------------------------------------------------------

        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Send Command button clicked");

            StatusText.Text = "Sending command...";
            StatusText.Foreground = Brushes.Yellow;

            // TODO:
            strapClient.Send_Buzz_Command_To_Strap();
            // Add actual BLE command here.

            // Temporary test
            Console.WriteLine("COMMAND");

            StatusText.Text = "Command sent";
            StatusText.Foreground = Brushes.LightGreen;
        }

        // ---------------------------------------------------------
        // STATUS TEXT
        // ---------------------------------------------------------

        public void changeStatusText(string new_text)
        {
            Console.WriteLine("trying to change it to: " + new_text);

            StatusText.Text = new_text;
        }

        // ---------------------------------------------------------
        // HEART RATE
        // ---------------------------------------------------------

        private void HeartRateCharacteristic_ValueChanged(
            GattCharacteristic sender,
            GattValueChangedEventArgs args)
        {
            double bpm = 5;

            Dispatcher.Invoke(() => UpdateHeartRate(bpm));
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            int bpm = 6;

            UpdateHeartRate(bpm);
        }

        private void UpdateHeartRate(double bpm)
        {
            // TODO:
            // Update heart rate UI here.
        }

        // ---------------------------------------------------------
        // BLE DEVICE DISCOVERY
        // ---------------------------------------------------------

        private void updatePairableDevices(DeviceInformation newDevice)
        {
            if (!Pairable_Devices.Dispatcher.CheckAccess())
            {
                Pairable_Devices.Dispatcher.Invoke(
                    () => updatePairableDevices(newDevice)
                );

                return;
            }

            Pairable_Devices.Items.Add(newDevice);

            //Console.WriteLine("added: " + newDevice.Name);
        }

        // ---------------------------------------------------------
        // SERVICES
        // ---------------------------------------------------------

        private void printServices()
        {
            // TODO:
            // Print discovered GATT services here.
        }

        // ---------------------------------------------------------
        // DEVICE SELECTION / CONNECTION
        // ---------------------------------------------------------

        private async void Pairable_Devices_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (Pairable_Devices.SelectedItem == null)
                return;

            DeviceInformation selectedDevice =
                (DeviceInformation)Pairable_Devices.SelectedItem;

            Console.WriteLine(
                "the selected item is: " + selectedDevice.Name
            );

            Pairable_Devices.IsEnabled = false;

            StatusText.Text = "Connecting...";
            StatusText.Foreground = Brushes.Yellow;

            try
            {
                bool isConnected =
                    await bleClient.ConnectDevice(selectedDevice);

                if (isConnected)
                {
                    StatusText.Text = "Connected";
                    StatusText.Foreground = Brushes.LightGreen;

                    Console.WriteLine("we connected baby");
                }
                else
                {
                    StatusText.Text = "Failed";
                    StatusText.Foreground = Brushes.Red;

                    Console.WriteLine("we couldnt connect");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(
                    "Connection error: " + ex.Message
                );

                StatusText.Text = "Connection error";
                StatusText.Foreground = Brushes.Red;
            }
            finally
            {
                Pairable_Devices.IsEnabled = true;
            }
        }

        // ---------------------------------------------------------
        // WINDOW CLOSED
        // ---------------------------------------------------------

        private async void MainWindow_Closed(
            object sender,
            object args)
        {
            // TODO:
            // Save application state and stop any background
            // BLE operations here.

        }
    }
}
/*

        
<!-- <Grid Grid.Row="1">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <Border Grid.Column="0" BorderBrush="White" BorderThickness="2" Margin="5">
                <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
                    <TextBlock Text="REST" Foreground="Gray" FontSize="16" HorizontalAlignment="Center"/>
                    <TextBlock x:Name="RestValueText" Text="--%" Foreground="LightGreen" FontSize="40" FontWeight="Bold" HorizontalAlignment="Center"/>
                </StackPanel>
            </Border>

            <Border Grid.Column="1" BorderBrush="White" BorderThickness="2" Margin="5">
                <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
                    <TextBlock Text="CHARGE" Foreground="Gray" FontSize="16" HorizontalAlignment="Center"/>
                    <TextBlock x:Name="ChargeValueText" Text="--%" Foreground="Cyan" FontSize="40" FontWeight="Bold" HorizontalAlignment="Center"/>
                </StackPanel>
            </Border>

            <Border Grid.Column="2" BorderBrush="White" BorderThickness="2" Margin="5">
                <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
                    <TextBlock Text="EFFORT" Foreground="Gray" FontSize="16" HorizontalAlignment="Center"/>
                    <TextBlock x:Name="EffortValueText" Text="--" Foreground="Orange" FontSize="40" FontWeight="Bold" HorizontalAlignment="Center"/>
                </StackPanel>
            </Border>
        </Grid> -->
      
      
        <Grid Grid.Row="2">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <Border Grid.Column="0" BorderBrush="White" BorderThickness="2" Margin="5">
                <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
                    <TextBlock Text="HEART RATE" Foreground="Gray" FontSize="14" HorizontalAlignment="Center"/>
                    <TextBlock x:Name="HeartRateText" Text="-- bpm" Foreground="Red" FontSize="32" FontWeight="Bold" HorizontalAlignment="Center"/>
                </StackPanel>
            </Border>

            <Border Grid.Column="1" BorderBrush="White" BorderThickness="2" Margin="5">
                <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
                    <TextBlock Text="HRV" Foreground="Gray" FontSize="14" HorizontalAlignment="Center"/>
                    <TextBlock x:Name="HrvText" Text="-- ms" Foreground="Yellow" FontSize="32" FontWeight="Bold" HorizontalAlignment="Center"/>
                </StackPanel>
            </Border>
        </Grid>-->
        */