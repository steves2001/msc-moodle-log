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
        CollectionViewSource itemCollectionViewSource;
        public static String gradeFilter = "P,M,D";
        public static String GradeFilters
        {
            get { return gradeFilter; }
            set { gradeFilter = value; }
        }
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

                itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
                itemCollectionViewSource.Source = Analyse.studentList;
            }
        }

        private void SaveCommand_Executed(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Log"; // Default file name
            dlg.DefaultExt = ".ods"; // Default file extension
            dlg.Filter = "Open Document Spreadsheets (.ods)|*.ods"; // Filter files by extension 
            if (dlg.ShowDialog() == true)
            {
                Analyse.storeStudentData(dlg.FileName);
            }
        }

        private void SelectAllCommand_Executed(object sender, RoutedEventArgs e)
        {
            commitEdits();
            Analyse.selectAllStudents();
            studentDataGrid.Items.Refresh();
        }

        private void SelectNoneCommand_Executed(object sender, RoutedEventArgs e)
        {
            commitEdits();
            Analyse.clearAllStudents();
            studentDataGrid.Items.Refresh();
            
        }

        private void SelectInvertCommand_Executed(object sender, RoutedEventArgs e)
        {
            commitEdits();
            Analyse.invertAllStudents();
            studentDataGrid.Items.Refresh();
        }


        private void SelectFilterCommand_Executed(object sender, RoutedEventArgs e)
        {
            commitEdits();

            Analyse.selectStudentsOnGrade(gradeFilter);
            studentDataGrid.Items.Refresh();
        }

        private void commitEdits()
        {
            studentDataGrid.CommitEdit();
            studentDataGrid.CommitEdit();
        }

        private void ChartCommand_Executed(object sender, RoutedEventArgs e)
        {
            commitEdits();

            if (Analyse.selectedStudentCount > 0)
            {
                Analyse.findExcludedStudents();
                Analyse.findModules();
                ChartWindow c = new ChartWindow();
                c.ShowDialog();
            }
        }

    }
}
