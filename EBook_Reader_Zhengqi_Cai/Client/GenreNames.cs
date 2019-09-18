using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client
{
    public enum GenreNames
    {
        Computer_Science_Information_General_Works,
        Philosophy_Psychology,
        Religion,
        Social_Sciences,
        Language,
        Pure_Science,
        Technology,
        Arts_Recreation,
        Literature,
        History_Geography,
    }

    public enum Mode
    {
        Post,
        Put,
        
    }

    public class MyEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (GenreNames)Enum.Parse(typeof(GenreNames), value.ToString(), true);
        }
    }

}
