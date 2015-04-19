﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace MoodleLogAnalyse
{
    class ChartDataItem
    {
        #region global font settings

        public static FontFamily  font = new FontFamily("Century Gothic");
        public static double fontSize = 12;
        #endregion

        #region non graphical properties

        public double valueLimit; // Maximum length 100% data item
        public double value; // data 
        public string text; // The text for the data item
        public double percentage; // The percentage relationship value to maximum value.
        #endregion

        #region graphical properties

        public uint maxLength; // Max size of the data item
        public uint drawLength; // Length of to draw
        public LinearGradientBrush brush;
        public Brush outlineColour;
        #endregion

        public ChartDataItem()
        {
            maxLength = 100;
            valueLimit = 100;
            value = 0;
            text = "No Data";
            percentage = value / valueLimit;

            drawLength = Convert.ToUInt32((double)maxLength * percentage);
            brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);
            brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            brush.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));
            outlineColour = Brushes.Black;
        }

        public ChartDataItem(double data, double dataMax, string dataLabel, uint graphicLimit, Brush lineColour, Color startFillColour, Color EndFillColour)
        {
            maxLength = graphicLimit;
            valueLimit = dataMax;
            value = data;
            text = dataLabel;
            percentage = value / valueLimit;

            drawLength = Convert.ToUInt32((double)maxLength * percentage);
            brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);
            brush.GradientStops.Add(new GradientStop(startFillColour, 0.0));
            brush.GradientStops.Add(new GradientStop(EndFillColour, 1.0));
            outlineColour = lineColour;
        }


    }
}
