using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.Ports;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Animation;

namespace Licenta_Concept
{
    public class Settings
    {
        public String ComPort { get; set; }
        public int BaudRate { get; set; }
        public int Parity { get; set; }
        public int DataBits { get; set; }
        public int StopBits { get; set; }
    }
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        SerialPort _serialPort=new SerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
        public SettingWindow()
        {
            //TestSerialComunnication();
            InitializeComponent();
            DynamicComPorts();
            DynamicParity();
            DynamicStopBits();
            AnimateWindowEntry();
        }

        void AnimateWindowEntry() 
        {
            Storyboard settingsSpawnStoryboard = (Storyboard)Resources["SettingsSpawnAnim"];
            settingsSpawnStoryboard.Begin();
        }

        private void TestSerialComunnication()
        {
            _serialPort = new SerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            _serialPort.Open();
        }
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            string data = _serialPort.ReadLine();
            // Invokes the delegate on the UI thread, and sends the data that was received to the invoked method.  
            // ---- The "si_DataReceived" method will be executed on the UI thread which allows populating of the textbox.  
            MessageBox.Show(data);
        }
        void DynamicComPorts() {
            foreach(string comItemName in SerialPort.GetPortNames())
            comPortSelector.Items.Add(new TextBox().Text=comItemName);
        }
        void DynamicParity() {
            var parityValues = Enum.GetValues(typeof(Parity));
            foreach (int parityValue in parityValues)
            {
                String name = Enum.GetName(typeof(Parity), parityValue);
                parityPicked.Items.Add(new TextBox().Text = name);
            }
        }
        void DynamicStopBits() {
            var stopBitsValues = Enum.GetValues(typeof(StopBits));
            foreach (int stopBitValue in stopBitsValues)
            {
                String name = Enum.GetName(typeof(StopBits), stopBitValue);
                stopBitsPicked.Items.Add(new TextBox().Text = name);
            }
        }

        private void settingsSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings currentSettings = new Settings() {
                ComPort = comPortSelector.SelectedItem.ToString(),
                BaudRate = Convert.ToInt32(baudRatePicked.Text),
                Parity =  (int)Enum.Parse(typeof(Parity), parityPicked.SelectedItem.ToString()),
                DataBits= Convert.ToInt32(dataBitsPicked.Text),
                StopBits= (int)Enum.Parse(typeof(StopBits), stopBitsPicked.SelectedItem.ToString())
            };
            //write json settings file
                string jsonFile = "settings.json";
                string source = JsonSerializer.Serialize(currentSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonFile, source);
                Close();
        }

        private void settingsCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
