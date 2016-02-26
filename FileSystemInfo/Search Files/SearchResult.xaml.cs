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
using System.Windows.Shapes;

namespace Search_Files
{
    /// <summary>
    /// Interaction logic for SearchResult.xaml
    /// </summary>
    ///         
    public enum FileorDirecotry
        {
            File,
            Directory,
            Both
        }
    public partial class SearchResult : Window
    {
        public string searchText { get; private set; }
        public char driveName { get; private set; }
        public FileorDirecotry SearchContentType { get; private set; }

        public SearchResult()
        {
            InitializeComponent();
        }

        public SearchResult(string text, char drive, FileorDirecotry searchContentType):this()
        {
            this.searchText = text;
            this.driveName = drive;
            this.SearchContentType = SearchContentType;            
        }
    }
}
