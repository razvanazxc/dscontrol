using Licenta_Concept;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Text.Json;
using System.IO;

namespace GUI_Control
{
    internal class UsbConnectionStatus
    {
        TextBlock infoStatusBar;
        Dispatcher mainUiDispatcher;

        public UsbConnectionStatus(TextBlock infoStatusBar, Dispatcher mainUiDispatcher)
        {
            this.infoStatusBar = infoStatusBar;
            this.mainUiDispatcher = mainUiDispatcher;
        }

        public void serialPortListener() {
            /*
                EventType's for Win32_DeviceChangeEvent class are:
                Configuration Changed (1)
                Device Arrival (2)
                Device Removal (3)
                Docking (4)
            */
            var watcherNewDeviceConnected = new ManagementEventWatcher();
            var queryNewDeviceConnected = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            watcherNewDeviceConnected.EventArrived += new EventArrivedEventHandler(watcherNewDevice_EventArrived);
            watcherNewDeviceConnected.Query = queryNewDeviceConnected;
            watcherNewDeviceConnected.Start();

            var watcherDeviceDisconnected = new ManagementEventWatcher();
            var queryDeviceDisconnected = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            watcherDeviceDisconnected.EventArrived += new EventArrivedEventHandler(watcherDeviceDisconnect_EventArrived);
            watcherDeviceDisconnected.Query = queryDeviceDisconnected;
            watcherDeviceDisconnected.Start();
        }
        void watcherNewDevice_EventArrived(object sender, EventArrivedEventArgs e) {
            mainUiDispatcher.Invoke((Action)(() =>
            {
                //is settings file com port open?
                string jsonFile = "settings.json";
                try
                {
                    string jsonString = File.ReadAllText(jsonFile);
                    Settings currentSettings = JsonSerializer.Deserialize<Settings>(jsonString);
                    if (TestSerialComunnication(currentSettings.ComPort, currentSettings.BaudRate, currentSettings.Parity, currentSettings.DataBits, currentSettings.StopBits))
                    {
                        infoStatusBar.Text = "Comm "+ currentSettings.ComPort+" connected!";
                    }
                    else
                        throw new InvalidOperationException("Com port is not connected anymore");
                }
                catch (Exception e)
                {
                    infoStatusBar.Text = "Change settings or connect device...";
                }

            }));
        }

        void watcherDeviceDisconnect_EventArrived(object sender, EventArrivedEventArgs e)
        {
            mainUiDispatcher.Invoke((Action)(() =>
            {
                //is settings file com port open?
                string jsonFile = "settings.json";
                try
                {
                    string jsonString = File.ReadAllText(jsonFile);
                    Settings currentSettings = JsonSerializer.Deserialize<Settings>(jsonString);
                    if (!TestSerialComunnication(currentSettings.ComPort, currentSettings.BaudRate, currentSettings.Parity, currentSettings.DataBits, currentSettings.StopBits))
                        throw new InvalidOperationException("Com port is not connected anymore");
                }
                catch (Exception e)
                {
                    infoStatusBar.Text = "Change settings or connect device...";
                }

            }));
        }

        public void InitialDeviceCheck() {
            watcherNewDevice_EventArrived(null, null);
        }

        bool TestSerialComunnication(string portName, int baudRate, int parity, int dataBits, int stopBits)
        {
            SerialPort _serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
            //_serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            try
            {
                _serialPort.Open();
                _serialPort.Close();
                return true;
            }
            catch (IOException e)
            {
                return false;
            }
        }
    }
}
