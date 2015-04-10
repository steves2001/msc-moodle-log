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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace MoodleLogAnalyse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Analyse Analyser; // The log file analyser

        public MainWindow()
        {
            InitializeComponent();
            Analyser = new Analyse();
        }

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Log"; // Default file name
            dlg.DefaultExt = ".ods"; // Default file extension
            dlg.Filter = "Open Document Spreadsheets (.ods)|*.ods"; // Filter files by extension 

            if (dlg.ShowDialog() == true)
                Analyse.getData(dlg.FileName);
        }
    }
}
