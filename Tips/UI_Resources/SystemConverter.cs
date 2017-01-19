using System;
using System.Windows.Data;


namespace Tips.UI_Resources
{
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
        {
            if ((int)value > 0)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
        {
            if ((bool)value ==false)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return true;
            }
            return false;
        }
    }
}
