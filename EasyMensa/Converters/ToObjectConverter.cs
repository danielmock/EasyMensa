using System;
using Windows.UI.Xaml.Data;

namespace EasyMensa.Converters
{
	public class ToObjectConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}
