using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace EasyMensa.Converters
{
	public class DateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, String culture)
		{
			if (targetType != typeof(string))
				throw new InvalidOperationException("The target must be a String");

			DateTime date = (DateTime)value;
			return date.ToString("m");
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
