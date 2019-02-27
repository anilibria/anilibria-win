namespace Anilibria.Services {

	/// <summary>
	/// Analytics service.
	/// </summary>
	public interface IAnalyticsService {

		/// <summary>
		/// Track event.
		/// </summary>
		/// <param name="name">Event name</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="eventValue">Event value.</param>
		void TrackEvent ( string name , string eventName , string eventValue );

	}

}
