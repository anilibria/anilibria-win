using System;
using Windows.UI.Xaml.Data;

namespace Anilibria.Converters {

	/// <summary>
	/// Is null value converter.
	/// </summary>
	public class IsNullValueConverter : IValueConverter {

		public object Convert ( object value , Type targetType , object parameter , string language ) {
			return value == null;
		}

		public object ConvertBack ( object value , Type targetType , object parameter , string language ) {
			throw new NotImplementedException ();
		}

	}

}
