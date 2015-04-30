using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace MoodleLogAnalyse
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        List<Bar> dataBars = new List<Bar>();
        Rectangle minimumLimit = new Rectangle();
        Rectangle aboveLimit = new Rectangle();

        double barLimit = 400;

        public ChartWindow()
        {
            InitializeComponent();
            ModuleSelector.ItemsSource = Analyse.moduleTypeList.Values; // Add the module types to the list box
            ModuleSelector.SelectAll(); // Start with everything selected
            drawBarChart();

        }

        private void setupChart()
        {


        }

        private bool notSelectedListItem(uint currentId)
        {
            foreach (ModuleType item in ModuleSelector.SelectedItems)
            {
                if (currentId == item.id)
                    return false;
            }
            return true;
        }



        private void drawBarChart()
        {
            uint maxModule = 0; // Maximum number of accesses for the selected modules
            uint barCount = 0;  // How many bars


            Bar accBar;  // Variable to hold the bar data for module accesses
            Bar stuBar;  // Variable to hold the bar data for unique module accesses

            int yPos = 0;
            int barSpacing = 30;  // distance between bars

            double barXStart = 490;  // Where to start drawing the bars from
            double labelWidth = 480; // Size of the label

            string statistics = "";  // Number of unique students accessed a module
            string modDetails = "";  // Module type and decription for use in the label

            // Colours for bar showing total accesses
            Color accBarBtm = (Color)ColorConverter.ConvertFromString("#FFE1A900"); //ARGB
            Color accBarTop = (Color)ColorConverter.ConvertFromString("#FFFFFF99");

            // Colours for bar showing total unique student accesses
            Color stuBarBtm = (Color)ColorConverter.ConvertFromString("#FF47A15F"); //ARGB
            Color stuBarTop = (Color)ColorConverter.ConvertFromString("#FF63E686");

            // Line colour for the bars
            Brush lineColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF000000"));

            // Find the module with the highest access count
            foreach (Module m in Analyse.moduleList.Values)
            {

                if (notSelectedListItem(m.type)) continue;  // Skip this module if it is not selected in the list view
                if (m.totalAccesses > maxModule) maxModule = m.totalAccesses;
            }

            ChartCanvas.Children.Clear();

            // Label headings
            TextBlock labelHeading = new TextBlock(new Run("TYPE : DESCRIPTION"));
            Canvas.SetLeft(labelHeading, 10);
            Canvas.SetTop(labelHeading, 10);
            labelHeading.Width = 200;
            labelHeading.Height = 20;
            ChartCanvas.Children.Add(labelHeading);
            
            TextBlock statHeading = new TextBlock(new Run("UNIQUE ACCESSES"));
            Canvas.SetLeft(statHeading, barXStart - 210);
            Canvas.SetTop(statHeading, 10);
            statHeading.Width = 200;
            statHeading.Height = 20;
            statHeading.TextAlignment = TextAlignment.Right;
            ChartCanvas.Children.Add(statHeading);
            
            foreach (Module m in Analyse.moduleList.Values)
            {
                if (notSelectedListItem(m.type)) continue;
                
                yPos += barSpacing;  
                barCount++;

                statistics = string.Format("{0:000} ", m.uniqueAccesses);
                modDetails = Analyse.moduleTypeList[m.type].name.Substring(0, 3).ToUpper() + "  : " + m.name;

                //Total module access bar
                accBar = new Bar(m.totalAccesses, maxModule, modDetails, labelWidth, 20, barLimit, new Point(barXStart, yPos), lineColour, accBarTop, accBarBtm);
                dataBars.Add(accBar);
                Canvas.SetZIndex(accBar.dataBar, 1);
                Canvas.SetZIndex(accBar.dataLabel, 2);
                ChartCanvas.Children.Add(accBar.dataBar);
                ChartCanvas.Children.Add(accBar.dataLabel);

                // Unique student access count bar
                stuBar = new Bar(m.uniqueAccesses, maxModule, statistics, 30, 14, barLimit, new Point(barXStart, yPos + 3), lineColour, stuBarTop, stuBarBtm);
                dataBars.Add(stuBar);
                Canvas.SetZIndex(stuBar.dataBar, 2);
                ChartCanvas.Children.Add(stuBar.dataBar);
                Canvas.SetZIndex(stuBar.dataLabel, 2);
                ChartCanvas.Children.Add(stuBar.dataLabel);
            }

            ChartCanvas.Height = (barCount + 1) * 30;

            if (barCount > 0)
            {
                double accessPercentage = (double)Analyse.selectedStudentCount / (double)maxModule;
                //Draw the minimum expected accesses
                minimumLimit.StrokeThickness = 1;
                minimumLimit.Stroke = lineColour;
                minimumLimit.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#55CC0000"));
                Canvas.SetLeft(minimumLimit, barXStart);
                Canvas.SetTop(minimumLimit, 30);
                Canvas.SetZIndex(minimumLimit, -1);
                minimumLimit.Width = barLimit * accessPercentage;
                minimumLimit.Height = yPos - 10;
                ChartCanvas.Children.Add(minimumLimit);

                //Draw the above expected accesses
                aboveLimit.StrokeThickness = 1;
                aboveLimit.Stroke = lineColour;
                aboveLimit.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5500CC00"));
                Canvas.SetLeft(aboveLimit, barXStart  + barLimit * accessPercentage);
                Canvas.SetTop(aboveLimit, 30);
                Canvas.SetZIndex(aboveLimit, -1);
                aboveLimit.Width = barLimit * (1.0 - accessPercentage);
                aboveLimit.Height = yPos - 10;
                ChartCanvas.Children.Add(aboveLimit);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            drawBarChart();

        }
    }
}
