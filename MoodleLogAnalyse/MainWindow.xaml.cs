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
        public MainWindow()
        {
            InitializeComponent();

        }

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, RoutedEventArgs e)
        {
        }

        private void OpenCommand_Executed(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Log"; // Default file name
            dlg.DefaultExt = ".ods"; // Default file extension
            dlg.Filter = "Open Document Spreadsheets (.ods)|*.ods"; // Filter files by extension 

            if (dlg.ShowDialog() == true)
            {
                Analyse.getData(dlg.FileName);
                Analyse.findStudents();
                StudentSelector.ItemsSource = Analyse.studentList.Values; // Add the module types to the list box
                //StudentSelector.SelectAll(); // Start with everything selected

            }
        }

        private void SaveCommand_Executed(object sender, RoutedEventArgs e)
        {
        }
        private void ChartCommand_Executed(object sender, RoutedEventArgs e)
        {
            if (StudentSelector.Items.Count == StudentSelector.SelectedItems.Count) return;

            Analyse.excludedStudents.Clear();

            foreach (Student student in StudentSelector.SelectedItems)
            {
                Analyse.excludedStudents.Add(student.id);
            }

            Analyse.findModules();
            ChartWindow c = new ChartWindow();
            c.ShowDialog();

        }

    }
}
