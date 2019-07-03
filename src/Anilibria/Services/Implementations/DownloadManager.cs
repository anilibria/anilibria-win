namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Download manager.
	/// </summary>
	public static class DownloadManager {

		private static readonly object m_LockObject = new object ();

		private static DownloadService m_DownloadService;

		/// <summary>
		/// Get current instance synchronize service.
		/// </summary>
		public static IDownloadService Current () {
			lock ( m_LockObject ) {
				if ( m_DownloadService == null ) m_DownloadService = new DownloadService ( StorageService.Current () );
			}

			return m_DownloadService;
		}

	}

}
