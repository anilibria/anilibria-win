namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Synchronize service global entry point.
	/// </summary>
	public static class SyncService {

		private static readonly object m_LockObject = new object ();

		private static SynchronizeService m_SynchronizeService;

		/// <summary>
		/// Get current instance synchronize service.
		/// </summary>
		public static ISynchronizationService Current () {
			lock ( m_LockObject ) {
				if ( m_SynchronizeService == null ) m_SynchronizeService = new SynchronizeService ( ApiService.Current () , StorageService.Current (), ReleaseSingletonService.Current() );
			}

			return m_SynchronizeService;
		}

	}

}
