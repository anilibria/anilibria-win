using System;
using Windows.UI.Xaml.Data;

namespace Anilibria.Converters {

	/// <summary>
	/// Inverse boolean value.
	/// </summary>
	public class InverseBoolConverter : IValueConverter {

		public object Convert ( object value , Type targetType , object parameter , string language ) {
			return !(bool) value;
		}

		public object ConvertBack ( object value , Type targetType , object parameter , string language ) {
			return !(bool) value;
		}

	}

}
