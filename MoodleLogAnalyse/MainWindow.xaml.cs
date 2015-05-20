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
using System.Windows.Threading;

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

        #region combo box fix
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }

        private void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }

        private static void GridColumnFastEdit(DataGridCell cell, RoutedEventArgs e)
        {
            if (cell == null || cell.IsEditing || cell.IsReadOnly)
                return;

            DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
            if (dataGrid == null)
                return;

            if (!cell.IsFocused)
            {
                cell.Focus();
            }

            if (cell.Content is CheckBox)
            {
                if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                {
                    if (!cell.IsSelected)
                        cell.IsSelected = true;
                }
                else
                {
                    DataGridRow row = FindVisualParent<DataGridRow>(cell);
                    if (row != null && !row.IsSelected)
                    {
                        row.IsSelected = true;
                    }
                }
            }
            else
            {
                ComboBox cb = cell.Content as ComboBox;
                if (cb != null)
                {
                    //DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                    dataGrid.BeginEdit(e);
                    cell.Dispatcher.Invoke(
                     DispatcherPriority.Background,
                     new Action(delegate { }));
                    cb.IsDropDownOpen = true;
                }
            }
        }


        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
        #endregion

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear All Data?", "Are You Sure!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                commitEdits();
                Analyse.clearData();
                studentDataGrid.Items.Refresh();
            }
        }

        private void OpenCommand_Executed(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Log"; // Default file name
            dlg.DefaultExt = ".ods"; // Default file extension
            dlg.Filter = "Open Document Spreadsheets (.ods)|*.ods"; // Filter files by extension 

            if (dlg.ShowDialog() == true)
            {
                commitEdits();
                Analyse.getStudentData(dlg.FileName);
                studentDataGrid.Items.Refresh();
            }
        }

        private void ImportLogCommand_Executed(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Log"; // Default file name
            dlg.DefaultExt = ".ods"; // Default file extension
            dlg.Filter = "Open Document Spreadsheets (.ods)|*.ods"; // Filter files by extension 

            if (dlg.ShowDialog() == true)
            {
                commitEdits();
                Analyse.getData(dlg.FileName);
                Analyse.findStudents();
                // This binds the student list to the data grid on the form
                itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
                itemCollectionViewSource.Source = Analyse.studentList;
                itemCollectionViewSource.View.Refresh();
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
                commitEdits();
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
                Analyse.findSections();
                ChartWindow c = new ChartWindow();
                c.ShowDialog();
            }
        }

    }
}
