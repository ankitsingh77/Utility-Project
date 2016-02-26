using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Search_Files
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            drives = drives.Where(o => o.DriveType.Equals(DriveType.Fixed | DriveType.Removable)).ToArray();
            this.cbDrive.ItemsSource = drives;
            this.cbDrive.SelectedItem = drives[0];
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lblchkValidation.Visibility = System.Windows.Visibility.Hidden;
            if (chkFiles.IsChecked == false && chkDirectory.IsChecked == false)
            {
                lblchkValidation.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                FileorDirecotry searchType = FileorDirecotry.Both;
                if (!(chkFiles.IsChecked == true && chkDirectory.IsChecked == true))
                {
                    if (chkFiles.IsChecked == true)
                    {
                        searchType = FileorDirecotry.File;
                    }
                    else 
                    {
                        searchType = FileorDirecotry.Directory;
                    }
                }
                SearchResult resultWindow = new SearchResult(txtSearch.Text,this.cbDrive.SelectedItem.ToString()[0],searchType);
                resultWindow.Show();
                this.Close();
            }
        }
    }
}
