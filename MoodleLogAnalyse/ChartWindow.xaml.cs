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
using System.Windows.Shapes;

namespace MoodleLogAnalyse
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        List<Bar> dataBars = new List<Bar>();

        public ChartWindow()
        {
            InitializeComponent();
            ModuleSelector.ItemsSource = Analyse.moduleTypeList.Values; // Add the module types to the list box

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            uint maxModule = (uint)Analyse.findMaxModuleAccessCount();

            int yPos = 0;
            Bar b;

            ChartCanvas.Children.Clear();

            ChartCanvas.Height = (Analyse.moduleList.Count  + 1) * 30;
            foreach(Module m in Analyse.moduleList.Values)
            {
                if (Analyse.moduleTypeList[m.type].name == "assign") continue;  // If it is an assignment
                yPos += 30;

                b = new Bar(m.totalAccesses, maxModule, Analyse.moduleTypeList[m.type].name.Substring(0,3) + " : " + m.name, 400, new Point(10, yPos), Brushes.Green, Colors.Blue, Colors.Green);
                dataBars.Add(b);
                ChartCanvas.Children.Add(b.dataBar);
                ChartCanvas.Children.Add(b.dataLabel);
            }
             

            
        }
    }
}
