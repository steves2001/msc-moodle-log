using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MoodleLogAnalyse
{
    class Bar : ChartDataItem
    {
        public Rectangle dataBar = new Rectangle();
        public TextBlock dataLabel;

        public Bar() : base()
        { }

        public Bar(double data, double dataMax, string dataLabel, uint graphicLimit, Point position, Brush lineColour, Color startFillColour, Color EndFillColour) :
            base(data, dataMax, dataLabel, graphicLimit, lineColour, startFillColour, EndFillColour)
        {
            this.dataBar.Fill = base.brush;
            this.dataBar.StrokeThickness = 1;
            this.dataBar.Stroke = base.outlineColour;
            this.dataBar.Width = base.drawLength;
            this.dataBar.Height = 20;
            Canvas.SetLeft(this.dataBar, position.X);
            Canvas.SetTop(this.dataBar, position.Y);
            this.dataLabel = new TextBlock(new Run(base.text));
            
        }

    }
}
