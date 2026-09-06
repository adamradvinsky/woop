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
using woop;

namespace woop_app
{
    public partial class MainWindow : Window
    {
        public event Action<int> ui_Connect_Device;
        private BleClient bleClient;

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;

            Application currentApp = Application.Current;

            App myApp = (App)currentApp;

            bleClient = myApp.BleClient;

        }

        List<string> test = new List<string>{ "Apple", "Banana", "Cherry", "Date" };

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("bleClient is null: " + (bleClient == null));
            ConnectButton.IsEnabled = false;

            bleClient.Pairable_Devices_Add += updatePairableDevices;
            

            bleClient.ScanForDevice();

            StatusText.Foreground = Brushes.Yellow;

        }

        public void changeStatusText(string new_text){
            Console.WriteLine("trying to change it to: " + new_text);
            StatusText.Text = new_text;
        }

        private void HeartRateCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
           
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

        private void updatePairableDevices(DeviceInformation newDevice){
           
            if (!Pairable_Devices.Dispatcher.CheckAccess()) {
                Pairable_Devices.Dispatcher.Invoke(() => updatePairableDevices(newDevice));
            } 

            Pairable_Devices.Items.Add(newDevice);
            Console.WriteLine("added: " + newDevice.Name);
            
        }

        private void printServices(){

        }


        private async void Pairable_Devices_SelectionChanged(object sender, SelectionChangedEventArgs e){
            Console.WriteLine("the selected item is: " + ((DeviceInformation)Pairable_Devices.SelectedItem).Name);
            
            // // say that you are connecting
            // // disable list so cant click
            
            Pairable_Devices.IsEnabled = false;

            try
            {
                bool isConnected = await bleClient.ConnectDevice((DeviceInformation)Pairable_Devices.SelectedItem);

                if(isConnected){
                    StatusText.Text = "Connected";
                    Console.WriteLine("we connected baby");
                } else {
                    StatusText.Text = "Failed";
                    Console.WriteLine("we couldnt connect");

                }

                
            }
            catch (System.Exception)
            {
                
                throw;
            } 
            finally {
                // enable everything
                // say couldnt connect/
                Pairable_Devices.IsEnabled = true;
                //StatusText.Text = "connection failed";
            }


        }

        

        private async void MainWindow_Closed(object sender, object args)
        {
        
            // TODO: Save application state and stop any background BLE operations here
            Console.WriteLine("unpairing ");
            DeviceUnpairingResult result = await bleClient.connectedDevice.Pairing.UnpairAsync();
            if (bleClient.connectedDevice != null)
            {
                bleClient.connectedDevice = null;
            }
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