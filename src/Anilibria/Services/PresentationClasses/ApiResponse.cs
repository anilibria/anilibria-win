namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Api response.
	/// </summary>
	public class ApiResponse<T> {

		public bool Status
		{
			get;
			set;
		}

		/// <summary>
		/// Response payload.
		/// </summary>
		public T Data
		{
			get;
			set;
		}

		/// <summary>
		/// Error description.
		/// </summary>
		public ApiResponseError Error
		{
			get;
			set;
		}

	}

}
