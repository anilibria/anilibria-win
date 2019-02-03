using System;
using Windows.Media.Core;
using Windows.UI.Xaml.Data;

namespace Anilibria.Converters {

	/// <summary>
	/// Convert Uri to Media source.
	/// </summary>
	public class MediaSourceConverter : IValueConverter {

		/// <summary>
		/// Convert from <see cref="Uri"/> to <see cref="MediaSource"/>.
		/// </summary>
		public object Convert ( object value , Type targetType , object parameter , string language ) {
			var uri = value as Uri;
			if ( uri == null ) return null;

			return MediaSource.CreateFromUri ( uri );
		}

		/// <summary>
		/// Convert back not supported.
		/// </summary>
		public object ConvertBack ( object value , Type targetType , object parameter , string language ) {
			throw new NotImplementedException ();
		}

	}

}
