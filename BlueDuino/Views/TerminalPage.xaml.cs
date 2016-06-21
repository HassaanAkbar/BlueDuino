using System;
using Windows.UI.Xaml.Controls;
using System.IO;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Bluetooth;
using Windows.Networking.Sockets;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using System.Windows;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using BlueDuino.Classes;

namespace BlueDuino.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TerminalPage : Page
    {


        private string RecievedText { get; set; }

        public TerminalPage()
        {

            this.InitializeComponent();

            if (Bluetooth.Instance != null)
            {
                Bluetooth.Instance.OnDisconnected += Disconnected;
            }
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            textblock.Text = "";
            
            if (Bluetooth.Instance.IsConnected)
            {
                textbox.IsEnabled = true;
                textbox.Text = "";
                try
                {
                    ReadingLoop();
                }
                catch (Exception exception)
                {

                    Debug.WriteLine("reading " + exception.Message);
                }

            }
            else
            {
                textbox.IsEnabled = false;
                textbox.Text = "Not connected";
            }

        }

        private async void ReadingLoop()
        {
            
            while (Bluetooth.Instance.IsConnected)
            {
                await Bluetooth.Instance.ListenAsync();
                RecievedText = Bluetooth.Instance.Read();
                if (RecievedText.Length > 50)
                {
                    RecievedText = "";
                }
                if (textblock.Text.Length > 300)
                {
                    textblock.Text = RecievedText;
                }
                else
                {
                    textblock.Text += RecievedText;
                }
                
                try
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("scrolling exception" + e.Message);
                }

                //return await new Task<int>(() => 0);
            }
        }

        private void Disconnected()
        {
            textbox.IsEnabled = false;
            textbox.Text = "Connection lost";
        }

        private void Refresh(object state)
        {
            try
            {
                if (Bluetooth.Instance.IsConnected)
                {
                    //await
                    //    dispatcher.RunAsync(CoreDispatcherPriority.Low,
                    //        (() => { textblock.Text += Bluetooth.Instance.Read(); }));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Bluetooth.Instance.IsConnected) return;
            try
            {
                Bluetooth.Instance.WriteAsync(textbox.Text);
            }
            catch
            {
                textblock.Text += "\nFailed to send";
            }
        }
    }

}
