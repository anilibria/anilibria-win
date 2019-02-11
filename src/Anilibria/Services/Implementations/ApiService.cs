namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Api service.
	/// </summary>
	public static class ApiService {

		private static AnilibriaApiService m_AnilibriaApiService = new AnilibriaApiService();

		/// <summary>
		/// Get current instance api service.
		/// </summary>
		public static IAnilibriaApiService Current () => m_AnilibriaApiService;

	}

}
