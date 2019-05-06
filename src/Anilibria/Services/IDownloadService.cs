using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;

namespace Anilibria.Services {

	/// <summary>
	/// Download service.
	/// </summary>
	public interface IDownloadService {

		/// <summary>
		/// Download file.
		/// </summary>
		/// <param name="uri">Uri file.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="percentProgress">Percent progress.</param>
		Task DownloadFile ( Uri uri , long id, Action<double> percentProgress );

		/// <summary>
		/// Get pending downloads.
		/// </summary>
		/// <returns></returns>
		IEnumerable<DownloadItem> GetPendingDownloads ();

		/// <summary>
		/// Set download file.
		/// </summary>
		/// <param name="release">Release name.</param>
		/// <param name="seria">Seria.</param>
		/// <param name="uri">Uri.</param>
		void AddDownloadFile ( DownloadItem downloadItem );

		/// <summary>
		/// Remove download file.
		/// </summary>
		/// <param name="id">Identifier.</param>
		void RemoveDownloadFile ( long id );

	}

}
