using Anilibria.Storage;
using Anilibria.Storage.Implementations;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Storage service global entry point.
	/// </summary>
	public static class StorageService {

		private static DataContext m_DataContext = new DataContext ( new LiteDbLocalPath () );

		/// <summary>
		/// Get current instance api service.
		/// </summary>
		public static IDataContext Current () => m_DataContext;

	}

}
