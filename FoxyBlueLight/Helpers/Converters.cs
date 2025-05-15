using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FoxyBlueLight.Helpers
{
    // Convertisseur Bool vers String (ON/OFF)
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "ON";
        public string FalseValue { get; set; } = "OFF";
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueValue : FalseValue;
            }
            
            return FalseValue;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == TrueValue;
        }
    }
    
    // Convertisseur Bool vers SolidColorBrush (pour interrupteur ON/OFF)
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return new SolidColorBrush(boolValue ? Color.FromRgb(58, 160, 255) : Color.FromRgb(80, 80, 80));
            }
            
            return new SolidColorBrush(Color.FromRgb(80, 80, 80));
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    
    // Convertisseur Bool vers HorizontalAlignment (pour interrupteur ON/OFF)
    public class BoolToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            }
            
            return HorizontalAlignment.Left;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is HorizontalAlignment alignment && alignment == HorizontalAlignment.Right;
        }
    }
    
    // Ajoutez ce convertisseur s'il manque
    public class BoolToLeftPosConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string paramString)
            {
                if (double.TryParse(paramString, out double position))
                {
                    return boolValue ? position : 0;
                }
            }
            return 0;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    
    // Et aussi celui-ci s'il est référencé
    public class MultiplyByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue && parameter is string paramString)
            {
                if (double.TryParse(paramString, out double multiplier))
                {
                    return (byte)(doubleValue * multiplier);
                }
            }
            return 0;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}