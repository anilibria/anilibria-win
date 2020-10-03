using Anilibria.Storage.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anilibria.Services {

	/// <summary>
	/// Release service
	/// </summary>
	public interface IReleasesService {

		/// <summary>
		/// Get all releases.
		/// </summary>
		/// <returns>Collection classes <see cref="ReleaseEntity"/>.</returns>
		IEnumerable<ReleaseEntity> GetReleases ();

		/// <summary>
		/// Get release by identifier.
		/// </summary>
		/// <param name="id">Identifier.</param>
		ReleaseEntity GetReleaseById (long id);

		/// <summary>
		/// Set releases.
		/// </summary>
		/// <param name="releases">Releases.</param>
		void SetReleases ( IEnumerable<ReleaseEntity> releases );

		/// <summary>
		/// Save releases.
		/// </summary>
		/// <returns></returns>
		Task SaveReleases ();

	}

}
