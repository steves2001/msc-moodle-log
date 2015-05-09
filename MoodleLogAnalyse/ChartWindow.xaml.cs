using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //List<Bar> dataBars = new List<Bar>();
        Rectangle minimumLimit = new Rectangle();
        Rectangle aboveLimit = new Rectangle();

        // Colours for bar showing total accesses
        string accBarBtm = "#FFE1A900"; //ARGB
        string accBarTop = "#FFFFFF99";

        // Colours for bar showing total unique student accesses
        string stuBarBtm = "#FF47A15F"; //ARGB
        string stuBarTop = "#FF63E686";

        // Line colour for the bars
        string lineColour = "#FF000000";
        public ObservableCollection<chartKeyItem> chartKeys = new ObservableCollection<chartKeyItem>();

        double barLimit = 400;

        public ChartWindow()
        {
            InitializeComponent();
            ModuleSelector.ItemsSource = Analyse.moduleTypeList.Values; // Add the module types to the list box
            ModuleSelector.SelectAll(); // Start with everything selected
            drawBarChart();

            chartKeys.Add(new chartKeyItem("Unique Students", stuBarTop, stuBarBtm,lineColour));
            chartKeys.Add(new chartKeyItem("Total Accesses", accBarTop, accBarBtm, lineColour));
            chartKey.ItemsSource = chartKeys;
        }

        private TextBlock createLabel (double left, double top, double width, double height, int z, String text, TextAlignment align , VerticalAlignment valign )
        {
            TextBlock label = new TextBlock(new Run(text));
            Canvas.SetLeft(label, left);
            Canvas.SetTop(label, top);
            label.Width = width;
            label.Height = height;
            Canvas.SetZIndex(label, z);
            label.TextAlignment = align;
            label.VerticalAlignment = valign;
            return label;
        }

        private Rectangle createRectangle(double left, double top, double width, double height, int z, string lineColour, string fillColour )
        {
            Rectangle rect = new Rectangle();

            rect.StrokeThickness = 1;
            rect.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(lineColour));;
            rect.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(fillColour));
            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);
            rect.Width = width;
            rect.Height = height;
            Canvas.SetZIndex(rect, z);

            return rect;

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
            double labelWidth = 440; // Size of the label

            string statistics = "";  // Number of unique students accessed a module
            string modDetails = "";  // Module description for use in the label
            string modType = "";     // Module type  for use in the label



            // Find the module with the highest access count
            foreach (Module m in Analyse.moduleList.Values)
            {

                if (notSelectedListItem(m.type)) continue;  // Skip this module if it is not selected in the list view
                if (m.totalAccesses > maxModule) maxModule = m.totalAccesses;
            }

            ChartCanvas.Children.Clear();

            // Label headings

            ChartCanvas.Children.Add(createLabel(barXStart - 210,10,200,20, 0, "STUDENT COUNT", TextAlignment.Right, VerticalAlignment.Center ));
            ChartCanvas.Children.Add(createLabel( 10, 10, 30, 20, 0, "TYPE", TextAlignment.Right,VerticalAlignment.Center));
            ChartCanvas.Children.Add(createLabel(50, 10, 200, 20, 0, "DESCRIPTION", TextAlignment.Left,VerticalAlignment.Center));
            
            foreach (Module m in Analyse.moduleList.Values)
            {
                if (notSelectedListItem(m.type)) continue;
                
                yPos += barSpacing;  
                barCount++;

                statistics = string.Format("{0:000} ", m.uniqueAccesses);
                modDetails =  m.name;
                modType = Analyse.moduleTypeList[m.type].name.Substring(0, 3).ToUpper();
                // Module type label
                ChartCanvas.Children.Add(createLabel(10, yPos, 30, 20, 0, modType, TextAlignment.Right, VerticalAlignment.Center));
                
                //Total module access bar
                accBar = new Bar(m.totalAccesses, maxModule, modDetails, labelWidth, 24, barLimit, new Point(barXStart, yPos),1, lineColour, accBarTop, accBarBtm);
                ChartCanvas.Children.Add(accBar.dataBar);
                ChartCanvas.Children.Add(accBar.dataLabel);

                // Unique student access count bar
                stuBar = new Bar(m.uniqueAccesses, maxModule, statistics, 30, 18, barLimit, new Point(barXStart, yPos + 3),2, lineColour, stuBarTop, stuBarBtm);
                ChartCanvas.Children.Add(stuBar.dataBar);
                ChartCanvas.Children.Add(stuBar.dataLabel);
            }

            ChartCanvas.Height = (barCount + 1) * 30;

            if (barCount > 0)
            {
                double accessPercentage = (double)Analyse.selectedStudentCount / (double)maxModule;
                //Draw the minimum expected accesses
                ChartCanvas.Children.Add(createRectangle(barXStart, 30, barLimit * accessPercentage, yPos - 6, -1, "#FF000000", "#55CC0000"));
                //Draw the above expected accesses
                ChartCanvas.Children.Add(createRectangle(barXStart + barLimit * accessPercentage, 30, barLimit * (1.0 - accessPercentage), yPos - 6, -1, "#FF000000", "#5500CC00"));

                //ChartCanvas.Children.Add(createLabel(barXStart - 8, 10, 16, 20, 0, "0", TextAlignment.Center, VerticalAlignment.Center));
                ChartCanvas.Children.Add(createLabel(barXStart + barLimit * accessPercentage - 25, 10, 50, 20, 0, Analyse.selectedStudentCount.ToString(), TextAlignment.Center, VerticalAlignment.Center));
                ChartCanvas.Children.Add(createLabel(barXStart + barLimit - 50, 10, 50, 20, 0, maxModule.ToString(), TextAlignment.Right, VerticalAlignment.Center));

            }
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            drawBarChart();

        }
    }

    public class chartKeyItem
    {
        public double width;
        public double height;
        public string Title { get; set; }
        public string brushStart { get; set; }
        public string brushEnd { get; set; }
        public string outline { get; set; }

        public chartKeyItem(string keyTitle, string startFillColour, string endFillColour, string lineColour)
        {
            brushStart = startFillColour;
            brushEnd = endFillColour;
            outline = lineColour;
            width = 10;
            height = 10;
            Title = keyTitle;
        }

    }
}
