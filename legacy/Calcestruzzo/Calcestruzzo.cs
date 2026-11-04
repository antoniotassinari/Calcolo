using System;
using System.Windows.Data;
namespace Calcestruzzo
{
    /// <summary>
    /// Converter da classiNormali a stringa e viceversa
    /// </summary>
    public class ClassiCLSToStringConverter : IValueConverter
    {

        #region IValueConverter Membri di

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (ClassiNormale)Enum.Parse(typeof(ClassiNormale), value.ToString(), true);
        }

        #endregion
    }

    /// <summary>
    /// Converter da double a string e viceversa
    /// </summary>
    public class DoubleToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value).ToString("F2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return double.Parse((string)value);
        }
    }
}
