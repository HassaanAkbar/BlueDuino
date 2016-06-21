using BlueDuino.Classes;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Store;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BlueDuino.Views
{

    public sealed partial class ConnectionPage : Page
    {
        private DeviceInformationCollection _devices;
        private List<string> _deviceNames;
        private int _selectedIndex;

        public ConnectionPage()
        {
            _deviceNames = new List<string>();
            this.InitializeComponent();
            Bluetooth.Instance.OnDisconnected += OnDisconnected;
            Bluetooth.Instance.OnConnected += OnConnected;
        }

        private void OnConnected()
        {
            textblock.Text = "Connected";
        }

        private void OnDisconnected()
        {
            textblock.Text = "Connection lost";
        }

        private async void comboBox_SelectionChanged(object sender, object e)
        {
            _selectedIndex = comboBox.SelectedIndex;
            if (comboBox.SelectedItem == null) return;
            
            try
            {
                if (await Bluetooth.Instance.ConnectAsync(_devices[comboBox.SelectedIndex].Id))
                {
                    //textblock.Text = "Connected";
                   // await Bluetooth.Instance.StartListeningAsync();
                }
            }
            catch (Exception ex)
            {
                textblock.Text = ex.Message;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Bluetooth.Instance.IsConnected)
            {
                textblock.Text = "Connected";
                comboBox.ItemsSource = _deviceNames;
                comboBox.SelectedIndex = _selectedIndex;
            }
            else
            {
                textblock.Text = "Not connected";
            }
            
                
        }

        private async void comboBox_DropDownOpened(object sender, object e)
        {
            _devices = await Bluetooth.Instance.FindPairedDevicesAsync();
            
            foreach (var device in _devices)
            {
                _deviceNames.Add(device.Name);
            }
  
            comboBox.ItemsSource = _deviceNames;
        }
    }
}
