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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using NLog;
using System.IO;
using GUI_Control;
using System.Text.Json;
using System.Configuration;

namespace Licenta_Concept
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        AppDomain currentDomain = AppDomain.CurrentDomain;
        int flagSideMenu = 0;
        static UsbConnectionStatus usbStat;
        SettingWindow settingWindow;
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            InitializeStatusBar();
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(programCrashedHandeler);
            logger.Info("Program started successfully");
        }

        private void InitializeStatusBar()
        {
            usbStat = new UsbConnectionStatus(infoStatusBar, this.Dispatcher);
            usbStat.serialPortListener();
            usbStat.InitialDeviceCheck();
        }

        private async void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            if(settingWindow!=null)
                settingWindow.Close();
            if (settingsButton.Opacity == 0)
                return;
            settingWindow = new SettingWindow();
            settingWindow.Show();
            settingWindow.chechCommStatusOnExit += settings_chechCommStatusOnExit;
        }

        static void settings_chechCommStatusOnExit(object sender, EventArgs e) {
            usbStat.InitialDeviceCheck();
        }

        private async void buttonDoor_Click(object sender, RoutedEventArgs e)
        {
            Task rez = doorButtonAnimation();
            await rez;
            tabCurentWindow.SelectedIndex = 1;

        }

        private async Task doorButtonAnimation()
        {
            await Task.Delay(125);
            buttonDoor.Content = FindResource("door_two");
            await Task.Delay(125);//miliseconds
            buttonDoor.Content = FindResource("door_three");
            await Task.Delay(125);
            buttonDoor.Content = FindResource("door_one");
            await Task.Delay(125);
        }

        private void sideMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (flagSideMenu == 0)
            {
                Storyboard sideMenuStoryBoard = (Storyboard)Resources["SideMenuInAnimation"];
                sideMenuStoryBoard.Begin();
                flagSideMenu = 1;
            }
            else
            {
                Storyboard sideMenuStoryBoard = (Storyboard)Resources["SideMenuOutAnimation"];
                sideMenuStoryBoard.Begin();
                flagSideMenu = 0;
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e) {
            if(settingWindow != null)
            settingWindow.Close();
        }

        private void doorWindowUp_Click(object sender, RoutedEventArgs e)
        {
            usbStat.sendSerialMessage(0x31);
            winButtonsImg.Source = GetBitmapImage("/UpWinBtn.png");
            Storyboard winUpStoryBoard = (Storyboard)Resources["WindowUpAnim"];
            winUpStoryBoard.Begin();
        }

        private void doorWindowUp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x21);
            winButtonsImg.Source = GetBitmapImage("/IdleWinBtn.png");
            Storyboard winUpStoryBoard = (Storyboard)Resources["WindowUpAnim"];
            winUpStoryBoard.Stop();
        }

        private void goHomeBtn_Click(object sender, RoutedEventArgs e)
        {
            tabCurentWindow.SelectedIndex = 0;
        }

        BitmapImage GetBitmapImage(string relativePath) {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(relativePath, UriKind.Relative);
            bi.EndInit();
            return bi;
        }

        private void doorWindowDown_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x32);
            winButtonsImg.Source = GetBitmapImage("/DownWinBtn.png");
            Storyboard winDownStoryBoard = (Storyboard)Resources["WindowDownAnim"];
            winDownStoryBoard.Begin();
        }

        private void doorWindowDown_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x22);
            winButtonsImg.Source = GetBitmapImage("/IdleWinBtn.png");
            Storyboard winDownStoryBoard = (Storyboard)Resources["WindowDownAnim"];
            winDownStoryBoard.Stop();
        }

        private async void buttonSeat_Click(object sender, RoutedEventArgs e)
        {
            Task rezSeatAnim = seatButtonAnimation();
            await rezSeatAnim;
            tabCurentWindow.SelectedIndex = 2;

        }

        private async Task seatButtonAnimation()
        {
            await Task.Delay(125);
            buttonSeat.Content = FindResource("seat_one");
            await Task.Delay(125);//miliseconds
            buttonSeat.Content = FindResource("seat_two");
            await Task.Delay(125);
            buttonSeat.Content = FindResource("seat_one");
            await Task.Delay(125);
        }

        private void positionForwardBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x41);
            Storyboard seatForwardStoryBoard = (Storyboard)Resources["SeatPositionForwardAnim"];
            seatForwardStoryBoard.Begin();
        }

        private void positionForwardBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x11);
            Storyboard seatForwardStoryBoard = (Storyboard)Resources["SeatPositionForwardAnim"];
            seatForwardStoryBoard.Stop();
        }

        private void positionBackwardBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x42);
            Storyboard seatBackwardStoryBoard = (Storyboard)Resources["SeatPositionBackwardAnim"];
            seatBackwardStoryBoard.Begin();
        }

        private void positionBackwardBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x12);
            Storyboard seatBackwardStoryBoard = (Storyboard)Resources["SeatPositionBackwardAnim"];
            seatBackwardStoryBoard.Stop();
        }

        private void seatUpBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x43);
            Storyboard seatUpStoryboard = (Storyboard)Resources["SeatHeightUpAnim"];
            seatUpStoryboard.Begin();
        }

        private void seatUpBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x13);
            Storyboard seatUpStoryboard = (Storyboard)Resources["SeatHeightUpAnim"];
            seatUpStoryboard.Stop();
        }

        private void seatDownBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x44);
            Storyboard seatDownStoryboard = (Storyboard)Resources["SeatHeightDownAnim"];
            seatDownStoryboard.Begin();
        }

        private void seatDownBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x14);
            Storyboard seatDownStoryboard = (Storyboard)Resources["SeatHeightDownAnim"];
            seatDownStoryboard.Stop();
        }

        private void seatBackRestForwardBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x45);
            Storyboard backRestForwardStoryboard = (Storyboard)Resources["SeatBackreastForwardAnim"];
            backRestForwardStoryboard.Begin();

        }

        private void seatBackRestForwardBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x15);
            Storyboard backRestForwardStoryboard = (Storyboard)Resources["SeatBackreastForwardAnim"];
            backRestForwardStoryboard.Stop();
        }

        private void seatBackRestBackBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x46);
            Storyboard backRestBackStoryboard = (Storyboard)Resources["SeatBackreastBackwardAnim"];
            backRestBackStoryboard.Begin();
        }

        private void seatBackRestBackBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x16);
            Storyboard backRestBackStoryboard = (Storyboard)Resources["SeatBackreastBackwardAnim"];
            backRestBackStoryboard.Stop();
        }

        private void seatHeadRestUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x47);
            Storyboard headRestUpStoryboard = (Storyboard)Resources["SeatHeadRestUpAnim"];
            headRestUpStoryboard.Begin();
        }

        private void seatHeadRestUp_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x17);
            Storyboard headRestUpStoryboard = (Storyboard)Resources["SeatHeadRestUpAnim"];
            headRestUpStoryboard.Stop();
        }

        private void seatHeadRestDown_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x48);
            Storyboard headRestDownStoryboard = (Storyboard)Resources["SeatHeadRestDownAnim"];
            headRestDownStoryboard.Begin();

        }

        private void seatHeadRestDown_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            usbStat.sendSerialMessage(0x18);
            Storyboard headRestDownStoryboard = (Storyboard)Resources["SeatHeadRestDownAnim"];
            headRestDownStoryboard.Stop();
        }

        private void seatHeatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (seatHeatBtnLogo.Foreground.ToString() == "#FFFFFFFF")
            {
                usbStat.sendSerialMessage(0x49);
                seatHeatBtnLogo.Foreground = new SolidColorBrush(Colors.DarkOrange);
                seatBackRestImg.Source = GetBitmapImage(@"/seatHeatedBackRest.png");
                seatButtRestImg.Source= GetBitmapImage(@"/seatHeatedButtRest.png");
            }
            else {
                usbStat.sendSerialMessage(0x19);
                seatHeatBtnLogo.Foreground = new SolidColorBrush(Colors.White);
                seatBackRestImg.Source = GetBitmapImage(@"/seatBackRest.png");
                seatButtRestImg.Source = GetBitmapImage(@"/seatButtRest.png");
            }
        }

        static void programCrashedHandeler(object sender, UnhandledExceptionEventArgs args) {
            Exception e = (Exception)args.ExceptionObject;
            logger.Error("MyHandler caught : " + e.Message);
            logger.Error("Runtime terminating: {0}", args.IsTerminating);
        }
    }
}
