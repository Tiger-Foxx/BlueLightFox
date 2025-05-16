using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FoxyBlueLight.Helpers
{
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEnabled = (bool)value;
            string colors = parameter?.ToString() ?? "#FF6700:#333333";
            string[] parts = colors.Split(':');
            
            string colorStr = isEnabled ? parts[0] : parts.Length > 1 ? parts[1] : "#333333";
            Color color = (Color)ColorConverter.ConvertFromString(colorStr);
            
            return new SolidColorBrush(color);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}