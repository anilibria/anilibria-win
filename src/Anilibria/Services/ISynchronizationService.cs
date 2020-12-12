using System.Threading.Tasks;

namespace Anilibria.Services {

	/// <summary>
	/// Synchronization service.
	/// </summary>
	public interface ISynchronizationService {

		/// <summary>
		/// Synchronize favorites.
		/// </summary>
		Task SynchronizeFavorites ();

		/// <summary>
		/// Synchronize releases.
		/// </summary>
		Task SynchronizeReleases ();

		/// <summary>
		/// Synchronize releases from json content.
		/// </summary>
		/// <param name="content">Json content.</param>
		Task SynchronizeReleasesFromContent ( string content );

		/// <summary>
		/// Synchronize youtubes.
		/// </summary>
		Task SynchronizeYoutubes ();

	}

}
