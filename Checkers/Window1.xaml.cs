using Checkers.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace CheckersGame
{
    public partial class MainWindow : Window
    {
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new Checkers.MainWindow();
            mainWindow.Show();


            this.Close();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs c)
        {
            var mainWindow = new Checkers.Views.About();
            mainWindow.Show();
        }

        private void StatButton_Click(object sender, RoutedEventArgs c)
        {
            var mainWindow = new Checkers.Views.Statistics();
            mainWindow.Show();
        }
    }
}
