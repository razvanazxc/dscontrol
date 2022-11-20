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

namespace Licenta_Concept
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int flagSideMenu = 0;
        SettingWindow settingWindow;
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            if(settingWindow!=null)
                settingWindow.Close();
            if (settingsButton.Opacity == 0)
                return;
            settingWindow = new SettingWindow();
            settingWindow.Show();
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
            Storyboard winUpStoryBoard = (Storyboard)Resources["WindowUpAnim"];
            winUpStoryBoard.Begin();
        }

        private void doorWindowUp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Storyboard winUpStoryBoard = (Storyboard)Resources["WindowUpAnim"];
            winUpStoryBoard.Stop();
        }
    }
}
