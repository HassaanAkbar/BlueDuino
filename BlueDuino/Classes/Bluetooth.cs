using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace BlueDuino.Classes
{
    class Bluetooth
    {
        public static Bluetooth Instance;
        public Action OnDisconnected;
        public Action OnConnected;
        public StreamSocket Stream;
        private DataReader _btReader;
        private DataWriter _btWriter;
        public bool IsConnected;


        public Bluetooth()
        {
            Instance = this;
        }


        public async Task<DeviceInformationCollection> FindPairedDevicesAsync()
        {
            var aqsDevices = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(aqsDevices);
            return devices;
        }

        public async Task<bool> ConnectAsync(string id)
        {
            RfcommDeviceService rfcommService = await RfcommDeviceService.FromIdAsync(id);
            if (rfcommService == null) return false;
            Stream = new StreamSocket();
            try
            {
                await Stream.ConnectAsync(rfcommService.ConnectionHostName, rfcommService.ConnectionServiceName);
            }
            catch
            {
                return false;
            }

            IsConnected = true;
            _btReader = new DataReader(Stream.InputStream);
            _btWriter = new DataWriter(Stream.OutputStream);
            _btWriter.UnicodeEncoding = UnicodeEncoding.Utf8;
            _btWriter.WriteString("Ready");
            await _btWriter.StoreAsync();
            OnConnected?.Invoke();
            return true;
        }

        public async void WriteAsync(string str)
        {
            _btWriter.WriteString(str);
            await _btWriter.StoreAsync();
        }

        public async void WriteAsync(byte[] bytes)
        {
            _btWriter.WriteBytes(bytes);
            await _btWriter.StoreAsync();
        }

        public async Task StartListeningAsync()
        {
            while (true)
            {
                var size = await _btReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    IsConnected = false;
                    Debug.WriteLine("Disconnected");
                    OnDisconnected?.Invoke();
                    return;
                }
            }
        }

        public async Task ListenAsync()
        {
            var size = await _btReader.LoadAsync(sizeof(uint));
            if (size < sizeof(uint))
            {
                IsConnected = false;
                Debug.WriteLine("Disconnected");
                OnDisconnected?.Invoke();
            }
        }

        public string Read()
        {

            return _btReader.ReadString(_btReader.UnconsumedBufferLength);

            //try
            //{
            //    return _btReader.ReadString(n);
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine(e.ToString());
            //    IsConnected = false;
            //    return "";
            //}
        }
    }
}
