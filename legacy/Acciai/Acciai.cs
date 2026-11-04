using System;
using System.Windows.Data;
namespace Acciai
{
    public class ClassiAcciaioArmaturaToString : IValueConverter
    {

        #region IValueConverter Membri di

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (TipoAcciai)Enum.Parse(typeof(TipoAcciai), value.ToString(), true);
        }

        #endregion
    }
}
