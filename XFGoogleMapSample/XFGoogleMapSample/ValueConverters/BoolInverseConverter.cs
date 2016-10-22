using System;
using System.Globalization;
using Xamarin.Forms;
namespace XFGoogleMapSample.ValueConverters
{
    public class BoolInverseConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return !(bool)value;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
