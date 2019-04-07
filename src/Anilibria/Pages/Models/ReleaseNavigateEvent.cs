namespace Anilibria.Pages.Models {

	/// <summary>
	/// Release navigate event.
	/// </summary>
	public class ReleaseNavigateEvent {

		/// <summary>
		/// Event type.
		/// </summary>
		public ReleaseNavigateEventType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Payload.
		/// </summary>
		public object Payload
		{
			get;
			set;
		}

	}

}
