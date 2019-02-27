using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Analitics service for tracking information about application.
	/// </summary>
	public class AnalyticsService : IAnalyticsService {

		public const string TrackKey = "b5d6d7ce-3983-46f3-8e94-39eda3d39091";

		public void TrackEvent ( string eventName , string parameterName , string parameterValue ) {
#if !DEBUG
			try {
				Analytics.TrackEvent ( eventName , new Dictionary<string , string> { { parameterName , parameterValue } } );
			}
			catch {
				//This is one way operation.If error is cause crash we don't care.
			}
#endif
		}

		/// <summary>
		/// Track event with single parameter.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="parameterName">Parameter name.</param>
		/// <param name="parameterValue">Parameter value.</param>
		public static void TrackEventWithSingleParameter ( string eventName , string parameterName , string parameterValue ) {
#if !DEBUG
			try {
				Analytics.TrackEvent ( eventName , new Dictionary<string , string> { { parameterName , parameterValue } } );
			}
			catch {
				//This is one way operation.If error is cause crash we don't care.
			}
#endif
		}

	}

}
