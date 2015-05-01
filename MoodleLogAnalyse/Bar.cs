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
        public Rectangle dataBar = new Rectangle(); // The rectangle that repsresents the bar
        public TextBlock dataLabel;  // The text label for the bar

        public Bar() : base()
        { }
        /// <summary>
        /// Basic structure for a data bar on a bar chart
        /// </summary>
        /// <param name="data">Value to be represent ed by the bar</param>
        /// <param name="dataMax">The maximum bar value on the graph</param>
        /// <param name="dataLabel">The text to display on the left of the bar</param>
        /// <param name="labelWidth">Number of pixels width for the label</param>
        /// <param name="barHeight">The thickness of the bar</param>
        /// <param name="graphicLimit">The width of the bar chart dispay area</param>
        /// <param name="position">The location to start drawing the top left corner of the bar</param>
        /// <param name="z">The z-index of the bar</param>
        /// <param name="lineColour">The colour of the line around the bar</param>
        /// <param name="startFillColour">The top colour of the gradient fill for the bar</param>
        /// <param name="EndFillColour">The bottom colour of the gradient fill for the bar</param>
        public Bar(double data, double dataMax, string dataLabel, double labelWidth, double barHeight, double graphicLimit, Point position,int z, string lineColour, string startFillColour, string EndFillColour) :
            base(data, dataMax, dataLabel, graphicLimit, lineColour, startFillColour, EndFillColour)
        {
            this.dataBar.Fill = base.brush;
            this.dataBar.StrokeThickness = 1;
            this.dataBar.Stroke = base.outlineColour;
            this.dataBar.Width = base.drawLength;
            this.dataBar.Height = barHeight;
            Canvas.SetLeft(this.dataBar, position.X );
            Canvas.SetTop(this.dataBar, position.Y);
            Canvas.SetZIndex(this.dataBar, z);
            
            this.dataLabel = new TextBlock(new Run(base.text));
            Canvas.SetLeft(this.dataLabel, position.X - labelWidth);
            Canvas.SetTop(this.dataLabel, position.Y);
            Canvas.SetZIndex(this.dataLabel, z);
            this.dataLabel.Width = labelWidth;
            this.dataLabel.Height = barHeight;
            this.dataLabel.VerticalAlignment = VerticalAlignment.Center;
        }

    }
}
