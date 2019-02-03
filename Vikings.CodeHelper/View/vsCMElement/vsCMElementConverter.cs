﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Vikings.CodeHelper.View.vsCMElement
{
    public class VsCMElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"vsCMElement/{value}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
