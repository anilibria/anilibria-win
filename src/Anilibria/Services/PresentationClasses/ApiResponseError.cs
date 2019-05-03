namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Api response error model.
	/// </summary>
	public class ApiResponseError {

		/// <summary>
		/// Code.
		/// </summary>
		public int Code
		{
			get;
			set;
		}

		/// <summary>
		/// Error message.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Error message.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

	}

}