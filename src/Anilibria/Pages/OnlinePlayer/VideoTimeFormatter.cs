using System;
using System.Linq;

namespace Anilibria.Pages.OnlinePlayer {

	/// <summary>
	/// Video time formatter.
	/// </summary>
	public static class VideoTimeFormatter {

		private static string FormattingMinutes ( int value ) {
			if ( value == 0 ) return "";

			var lastDigit = Convert.ToInt32 ( value.ToString ().Last ().ToString() );

			if ( lastDigit == 1 ) return $"{value} минута";
			if ( lastDigit >= 2 && lastDigit <= 4 ) return $"{value} минуты";

			return $"{value} минут";
		}

		private static string FormattingSeconds ( int value ) {
			if ( value == 0 ) return "";

			var lastDigit = Convert.ToInt32 ( value.ToString ().Last ().ToString () );

			if ( lastDigit == 1 ) return $"{value} секунда";
			if ( lastDigit >= 2 && lastDigit <= 4 ) return $"{value} секунды";

			return $"{value} секунд";
		}

		private static string FormattingHours ( int value ) {
			if ( value == 0 ) return "";

			var lastDigit = Convert.ToInt32 ( value.ToString ().Last ().ToString () );

			if ( lastDigit == 1 ) return $"{value} час";
			if ( lastDigit >= 2 && lastDigit <= 4 ) return $"{value} часа";

			return $"{value} часов";
		}

		public static string ConvertTimeSpanToText ( TimeSpan time ) {
			if ( time.Hours > 0 ) {
				return $"{FormattingHours ( time.Hours )} {FormattingMinutes ( time.Minutes )}";
			}
			else {
				return $"{FormattingMinutes ( time.Minutes )} {FormattingSeconds ( time.Seconds )}";
			}
		}

	}

}
