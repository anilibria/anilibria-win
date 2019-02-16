using System.IO;
using Windows.Storage;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Local database path.
	/// </summary>
	public class LiteDbLocalPath : ILocalDatabasePath {

		/// <summary>
		/// Get database path.
		/// </summary>
		/// <returns>Database path.</returns>
		public string GetDatabasePath () => Path.Combine ( ApplicationData.Current.LocalFolder.Path , "mainc.db" );

	}

}
