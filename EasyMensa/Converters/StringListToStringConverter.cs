using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace EasyMensa.Converters
{
	public class StringListToStringConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (targetType != typeof(string))
				throw new InvalidOperationException("The target must be a String");

			List<String> list = (List<String>)value;
			return string.Join(", ", list);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}