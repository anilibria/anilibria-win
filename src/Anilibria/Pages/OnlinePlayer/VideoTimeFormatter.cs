using System;

namespace Anilibria.Pages.OnlinePlayer {

	/// <summary>
	/// Video time formatter.
	/// </summary>
	public static class VideoTimeFormatter {

		private static string FormattingMinutes ( int value ) {
			if ( value == 0 ) return "";
			if ( value == 1 ) return $"{value} минута";
			if ( value >= 2 && value <= 4 ) return $"{value} минуты";

			return $"{value} минут";
		}

		private static string FormattingSeconds ( int value ) {
			if ( value == 0 ) return "";
			if ( value == 1 ) return $"{value} секунда";
			if ( value >= 2 && value <= 4 ) return $"{value} секунды";

			return $"{value} секунд";
		}

		private static string FormattingHours ( int value ) {
			if ( value == 0 ) return "";
			if ( value == 1 ) return $"{value} час";
			if ( value >= 2 && value <= 4 ) return $"{value} часа";

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
