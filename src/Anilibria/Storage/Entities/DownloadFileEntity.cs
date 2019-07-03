using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Download file entity.
	/// </summary>
	public class DownloadFileEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public long Id
		{
			get;
			set;
		}

		/// <summary>
		/// Downloading releases.
		/// </summary>
		public IEnumerable<DownloadReleaseEntity> DownloadingReleases
		{
			get;
			set;
		}

	}

}
