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
            Console.WriteLine("creted a ble client");


        }


        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectButton.IsEnabled = false;
            StatusText.Text = "bruh...";
            StatusText.Foreground = Brushes.Yellow;

            // try
            // {
            //     string selector = BluetoothLEDevice.GetDeviceSelectorFromDeviceName("WHOOP");
            //     DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

            //     if (devices.Count == 0)
            //     {
            //         StatusText.Text = "No WHOOP device found - showing simulated data";
            //         StatusText.Foreground = Brushes.Orange;
            //         _simulationTimer.Start();
            //         return;
            //     }

            //     _device = await BluetoothLEDevice.FromIdAsync(devices[0].Id);
            //     var servicesResult = await _device.GetGattServicesForUuidAsync(HeartRateServiceUuid);

            //     if (servicesResult.Status != GattCommunicationStatus.Success || servicesResult.Services.Count == 0)
            //     {
            //         StatusText.Text = "Connected, but HR service not found (likely proprietary - update the UUIDs)";
            //         StatusText.Foreground = Brushes.Orange;
            //         return;
            //     }

            //     var service = servicesResult.Services[0];
            //     var charResult = await service.GetCharacteristicsForUuidAsync(HeartRateMeasurementCharUuid);

            //     if (charResult.Status != GattCommunicationStatus.Success || charResult.Characteristics.Count == 0)
            //     {
            //         StatusText.Text = "Service found, but no matching characteristic";
            //         StatusText.Foreground = Brushes.Orange;
            //         return;
            //     }

            //     _heartRateCharacteristic = charResult.Characteristics[0];
            //     _heartRateCharacteristic.ValueChanged += HeartRateCharacteristic_ValueChanged;
            //     await _heartRateCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
            //         GattClientCharacteristicConfigurationDescriptorValue.Notify);

            //     StatusText.Text = "Connected";
            //     StatusText.Foreground = Brushes.LightGreen;
            // }
            // catch (Exception ex)
            // {
            //     StatusText.Text = $"Connection failed: {ex.Message} - showing simulated data";
            //     StatusText.Foreground = Brushes.Red;
            //     _simulationTimer.Start();
            // }
            // finally
            // {
            //     ConnectButton.IsEnabled = true;
            // }
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
            // HeartRateText.Text = $"{bpm:0} bpm";

            // _heartRateHistory.Add(bpm);
            // if (_heartRateHistory.Count > MaxHistoryPoints)
            //     _heartRateHistory.RemoveAt(0);

            // // HRV isn't part of the standard HR profile - placeholder until you pull
            // // it from WHOOP's own characteristic.
            // HrvText.Text = "-- ms";

            // double avg = _heartRateHistory.Average();
            // RestValueText.Text = $"{Math.Clamp(100 - (avg - 60), 0, 100):0}%";
            // ChargeValueText.Text = $"{Math.Clamp(avg, 0, 100):0}%";
            // EffortValueText.Text = $"{avg / 10:0.0}";

            // DrawGraph();
        }

        private void GraphCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawGraph();
        }

        private void DrawGraph()
        {
            // GraphCanvas.Children.Clear();

            // if (_heartRateHistory.Count < 2)
            //     return;

            // double width = GraphCanvas.ActualWidth;
            // double height = GraphCanvas.ActualHeight;
            // if (width <= 0 || height <= 0)
            //     return;

            // double min = _heartRateHistory.Min();
            // double max = _heartRateHistory.Max();
            // if (Math.Abs(max - min) < 1) max = min + 1;

            // double xStep = width / (MaxHistoryPoints - 1);

            // var linePoints = new PointCollection();
            // for (int i = 0; i < _heartRateHistory.Count; i++)
            // {
            //     double x = i * xStep;
            //     double y = height - ((_heartRateHistory[i] - min) / (max - min) * height);
            //     linePoints.Add(new Point(x, y));
            // }

            // var polyline = new Polyline
            // {
            //     Points = linePoints,
            //     Stroke = Brushes.Red,
            //     StrokeThickness = 2
            // };
            // GraphCanvas.Children.Add(polyline);

            // double avg = _heartRateHistory.Average();
            // double avgY = height - ((avg - min) / (max - min) * height);
            // var avgLine = new Line
            // {
            //     X1 = 0,
            //     Y1 = avgY,
            //     X2 = width,
            //     Y2 = avgY,
            //     Stroke = Brushes.Cyan,
            //     StrokeThickness = 1,
            //     StrokeDashArray = new DoubleCollection { 4, 2 }
            // };
            // GraphCanvas.Children.Add(avgLine);

            // var avgLabel = new TextBlock
            // {
            //     Text = $"avg: {avg:0.0}",
            //     Foreground = Brushes.Cyan,
            //     FontSize = 12
            // };
            // Canvas.SetLeft(avgLabel, 5);
            // Canvas.SetTop(avgLabel, Math.Max(0, avgY - 15));
            // GraphCanvas.Children.Add(avgLabel);
        }
    }
}