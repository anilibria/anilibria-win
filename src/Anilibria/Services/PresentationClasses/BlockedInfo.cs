namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Blocked info.
	/// </summary>
	public sealed class BlockedInfo {

		/// <summary>
		/// Sign of blocking.
		/// </summary>
		public bool Blocked
		{
			get;
			set;
		}

		/// <summary>
		/// Reason.
		/// </summary>
		public string Reason
		{
			get;
			set;
		}

	}

}