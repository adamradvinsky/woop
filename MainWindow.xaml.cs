using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

using Microsoft.UI.Xaml;

// using ble;
// using strap;

namespace woop_app
{
    public partial class MainWindow : Window
    {
        public event Action<int> ui_Connect_Device;

        private BleClient bleClient;
        private StrapClient strapClient;

        // WHOOP instance
        private Woop woop;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

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

            /*
             * IMPORTANT:
             *
             * This assumes your StrapClient contains a Woop instance
             * called "woop".
             *
             * If your variable/property has a different name,
             * change this line.
             */
            woop = strapClient.woop;

            UpdateWoopInfo();
            PopulateCommandButtons();
        }


        // =========================================================
        // TEST DATA
        // =========================================================

        List<string> test = new List<string>
        {
            "Apple",
            "Banana",
            "Cherry",
            "Date"
        };


        // =========================================================
        // CONNECT BUTTON
        // =========================================================

        private async void ConnectButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Console.WriteLine(
                "bleClient is null: " +
                (bleClient == null)
            );

            ConnectButton.IsEnabled = false;

            bleClient.Pairable_Devices_Add += updatePairableDevices;

            bleClient.ScanForDevice();

            StatusText.Foreground = Brushes.Yellow;
            StatusText.Text = "Scanning...";
        }


        // =========================================================
        // SEND HELLO BUTTON
        // =========================================================

        private void SendHelloButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Console.WriteLine(
                "Send Hello button clicked"
            );

            StatusText.Text = "Sending Hello...";
            StatusText.Foreground = Brushes.Yellow;

            // Send Hello command
            strapClient.Send_Hello();

            Console.WriteLine("HELLO");

            StatusText.Text = "Hello sent";
            StatusText.Foreground = Brushes.LightGreen;

            UpdateWoopInfo();
        }


        // =========================================================
        // WHOOP COMMANDS (auto-generated from WhoopCommands enum)
        // =========================================================

        /*
         * Builds one button per value in the WhoopCommands enum
         * (defined in strap / StrapClient.cs) and drops them into
         * the CommandsPanel WrapPanel in the XAML.
         *
         * To add a new command: add a value to the WhoopCommands
         * enum in StrapClient.cs and rebuild. A matching button
         * shows up here automatically - no other changes needed.
         */
        private void PopulateCommandButtons()
        {
            if (CommandsPanel == null)
                return;

            CommandsPanel.Children.Clear();

            foreach (WhoopCommands cmd in Enum.GetValues(typeof(WhoopCommands)))
            {
                Button btn = new Button
                {
                    Content = cmd.ToString(),
                    Tag = cmd,
                    Margin = new Thickness(4),
                    Padding = new Thickness(10, 5, 10, 5),
                    Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22)),
                    Foreground = Brushes.White
                };

                btn.Click += CommandButton_Click;

                CommandsPanel.Children.Add(btn);
            }
        }


        /*
         * Every auto-generated command button routes here. The
         * WhoopCommands value it represents is stashed in Tag,
         * so this one handler works for every command, present
         * and future.
         */
        private async void CommandButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            WhoopCommands cmd = (WhoopCommands)btn.Tag;

            Console.WriteLine(
                "Sending command: " + cmd + " (" + (int)cmd + ")"
            );

            StatusText.Text = "Sending " + cmd + "...";
            StatusText.Foreground = Brushes.Yellow;

            try
            {
                // Requires a Send_Command(WhoopCommands) method on
                // StrapClient - see the snippet provided alongside
                // this file.
                bool success = await strapClient.Send_Command(cmd);

                StatusText.Text = success ? cmd + " sent" : cmd + " failed";
                StatusText.Foreground = success ? Brushes.LightGreen : Brushes.Red;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(
                    "Error sending command " + cmd + ": " + ex.Message
                );

                StatusText.Text = "Command error";
                StatusText.Foreground = Brushes.Red;
            }

            UpdateWoopInfo();
        }


        // =========================================================
        // REFRESH WOOP BUTTON
        // =========================================================

        private void RefreshWoopButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Console.WriteLine(
                "Refreshing Woop information..."
            );

            UpdateWoopInfo();
        }


        // =========================================================
        // STATUS TEXT
        // =========================================================

        public void changeStatusText(string new_text)
        {
            Console.WriteLine(
                "trying to change it to: " + new_text
            );

            StatusText.Text = new_text;
        }


        // =========================================================
        // WOOP INFORMATION
        // =========================================================

        private void UpdateWoopInfo()
        {
            if (WoopInfoText == null)
                return;

            if (woop == null)
            {
                WoopInfoText.Text =
                    "========== WOOP ==========\n\n" +
                    "Woop instance is NULL.\n\n" +
                    "Make sure StrapClient is creating a Woop instance.";
                
                return;
            }


            // -----------------------------------------------------
            // CMD TO STRAP
            // -----------------------------------------------------

            string cmdToStrapInfo;

            if (woop.CMD_TO_STRAP == null)
            {
                cmdToStrapInfo = "NULL";
            }
            else
            {
                cmdToStrapInfo =
                    $"UUID: {woop.CMD_TO_STRAP.Uuid}\n" +
                    $"Properties: {woop.CMD_TO_STRAP.CharacteristicProperties}\n" +
                    $"Handle: {woop.CMD_TO_STRAP.AttributeHandle}";
            }


            // -----------------------------------------------------
            // DEVICE
            // -----------------------------------------------------

            string deviceInfo;

            if (woop.device == null)
            {
                deviceInfo = "NULL";
            }
            else
            {
                deviceInfo =
                    $"Name: {woop.device.Name}\n" +
                    $"Connection Status: {woop.device.ConnectionStatus}\n" +
                    $"Bluetooth Address: {woop.device.BluetoothAddress:X}";
            }


            // -----------------------------------------------------
            // WOOP INFORMATION
            // -----------------------------------------------------

            string woopInformation;

            if (woop.woop_information == null)
            {
                woopInformation = "NULL";
            }
            else
            {
                woopInformation =
                    $"Name: {woop.woop_information.Name}\n" +
                    $"ID: {woop.woop_information.Id}\n" +
                    $"Kind: {woop.woop_information.Kind}\n" +
                    $"Is Enabled: {woop.woop_information.IsEnabled}";
            }


            // -----------------------------------------------------
            // CUSTOM SERVICE
            // -----------------------------------------------------

            string customServiceInfo;

            if (woop.custom_service == null)
            {
                customServiceInfo = "NULL";
            }
            else
            {
                customServiceInfo =
                    $"UUID: {woop.custom_service.Uuid}\n" +
                    $"Attribute Handle: {woop.custom_service.AttributeHandle}";
            }


            // -----------------------------------------------------
            // SERVICES
            // -----------------------------------------------------

            string servicesInfo;

            if (woop.services == null)
            {
                servicesInfo = "NULL";
            }
            else
            {
                servicesInfo =
                    $"Status: {woop.services.Status}\n" +
                    $"Services Found: {woop.services.Services.Count}";
            }


            // -----------------------------------------------------
            // COMBINE EVERYTHING
            // -----------------------------------------------------

            WoopInfoText.Text =
                "==================================================\n" +
                "                    WOOP INSTANCE\n" +
                "==================================================\n\n" +

                "CMD_TO_STRAP\n" +
                "--------------------------------------------------\n" +
                cmdToStrapInfo +
                "\n\n" +

                "DEVICE\n" +
                "--------------------------------------------------\n" +
                deviceInfo +
                "\n\n" +

                "WOOP_INFORMATION\n" +
                "--------------------------------------------------\n" +
                woopInformation +
                "\n\n" +

                "CUSTOM_SERVICE\n" +
                "--------------------------------------------------\n" +
                customServiceInfo +
                "\n\n" +

                "SERVICES\n" +
                "--------------------------------------------------\n" +
                servicesInfo +
                "\n";
        }


        // =========================================================
        // HEART RATE
        // =========================================================

        private void HeartRateCharacteristic_ValueChanged(
            GattCharacteristic sender,
            GattValueChangedEventArgs args)
        {
            double bpm = 5;

            Dispatcher.Invoke(
                () => UpdateHeartRate(bpm)
            );
        }


        private void SimulationTimer_Tick(
            object sender,
            EventArgs e)
        {
            int bpm = 6;

            UpdateHeartRate(bpm);
        }


        private void UpdateHeartRate(double bpm)
        {
            // TODO:
            // Update heart rate UI here.
        }


        // =========================================================
        // BLE DEVICE DISCOVERY
        // =========================================================

        private void updatePairableDevices(
            DeviceInformation newDevice)
        {
            if (!Pairable_Devices.Dispatcher.CheckAccess())
            {
                Pairable_Devices.Dispatcher.Invoke(
                    () => updatePairableDevices(newDevice)
                );

                return;
            }

            Pairable_Devices.Items.Add(newDevice);
        }


        // =========================================================
        // SERVICES
        // =========================================================

        private void printServices()
        {
            UpdateWoopInfo();
        }


        // =========================================================
        // DEVICE SELECTION / CONNECTION
        // =========================================================

        private async void Pairable_Devices_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (Pairable_Devices.SelectedItem == null)
                return;


            DeviceInformation selectedDevice =
                (DeviceInformation)Pairable_Devices.SelectedItem;


            Console.WriteLine(
                "the selected item is: " +
                selectedDevice.Name
            );


            Pairable_Devices.IsEnabled = false;

            StatusText.Text = "Connecting...";
            StatusText.Foreground = Brushes.Yellow;


            try
            {
                bool isConnected =
                    await bleClient.ConnectDevice(
                        selectedDevice
                    );


                if (isConnected)
                {
                    StatusText.Text = "Connected";
                    StatusText.Foreground =
                        Brushes.LightGreen;

                    Console.WriteLine(
                        "we connected baby"
                    );

                    /*
                     * Refresh the Woop instance after connection.
                     *
                     * This is useful if StrapClient creates/populates
                     * the Woop object during ConnectDevice().
                     */

                    woop = strapClient.woop;

                    UpdateWoopInfo();
                }
                else
                {
                    StatusText.Text = "Failed";
                    StatusText.Foreground =
                        Brushes.Red;

                    Console.WriteLine(
                        "we couldnt connect"
                    );

                    UpdateWoopInfo();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(
                    "Connection error: " +
                    ex.Message
                );

                StatusText.Text =
                    "Connection error";

                StatusText.Foreground =
                    Brushes.Red;

                WoopInfoText.Text =
                    "Connection error:\n\n" +
                    ex.Message;
            }
            finally
            {
                Pairable_Devices.IsEnabled = true;
            }
        }


        // =========================================================
        // WINDOW CLOSED
        // =========================================================

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