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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            uint maxModule = 0; // (uint)Analyse.findMaxModuleAccessCount();
            uint barCount = 0;
            
            foreach (Module m in Analyse.moduleList.Values)
            {

                if (notSelectedListItem(m.type)) continue;
                if (m.totalAccesses > maxModule) maxModule = m.totalAccesses;
            }
            int yPos = 0;
            Bar b;
            Bar s;
                Color colourBottom = (Color)ColorConverter.ConvertFromString("#FFE1A900"); //ARGB
                Color colourTop = (Color)ColorConverter.ConvertFromString("#FFFFFF99");
                Brush lineColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF000000"));
            ChartCanvas.Children.Clear();

            //ChartCanvas.Height = (Analyse.moduleList.Count  + 1) * 30;
            foreach(Module m in Analyse.moduleList.Values)
            {

                if (notSelectedListItem(m.type)) continue;
                //if (m.totalAccesses > maxModule) maxModule = m.totalAccesses;
                yPos += 30;
                barCount++;

                //Total module access
                b = new Bar(m.totalAccesses, maxModule, Analyse.moduleTypeList[m.type].name.Substring(0,3) + " : " + m.name, barLimit, new Point(10, yPos), lineColour, colourTop, colourBottom);
                dataBars.Add(b);
                Canvas.SetZIndex(b.dataBar, 1);
                Canvas.SetZIndex(b.dataLabel, 2);
                ChartCanvas.Children.Add(b.dataBar);
                ChartCanvas.Children.Add(b.dataLabel);

                // Unique student access count
                s = new Bar(m.uniqueAccesses, maxModule, "", barLimit, new Point(10, yPos), lineColour, colourTop, colourBottom);
                dataBars.Add(s);
                Canvas.SetZIndex(s.dataBar, 2);
                ChartCanvas.Children.Add(s.dataBar);
                
            }
            ChartCanvas.Height = (barCount + 1) * 30;
            if (barCount > 0)
            {
                double accessPercentage = (double)Analyse.studentList.Count / (double)maxModule;
                //Draw the minimum expected accesses
                minimumLimit.StrokeThickness = 1;
                minimumLimit.Stroke = lineColour;
                minimumLimit.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#55CC0000"));
                Canvas.SetLeft(minimumLimit, 360);
                Canvas.SetTop(minimumLimit, 30);
                Canvas.SetZIndex(minimumLimit, -1);
                minimumLimit.Width = barLimit * accessPercentage;
                minimumLimit.Height = yPos - 10;
                ChartCanvas.Children.Add(minimumLimit);

                //Draw the above expected accesses
                aboveLimit.StrokeThickness = 1;
                aboveLimit.Stroke = lineColour;
                aboveLimit.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5500CC00"));
                Canvas.SetLeft(aboveLimit, 360.0 + barLimit * accessPercentage);
                Canvas.SetTop(aboveLimit, 30);
                Canvas.SetZIndex(aboveLimit, -1);
                aboveLimit.Width = barLimit * (1.0 - accessPercentage);
                aboveLimit.Height = yPos - 10;
                ChartCanvas.Children.Add(aboveLimit);
            }
            
        }
    }
}
