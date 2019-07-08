using System.Collections.Generic;
using System.Threading.Tasks;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services.PresentationClasses;
using Anilibria.Storage.Entities;

namespace Anilibria.Services {

	/// <summary>
	/// Download service.
	/// </summary>
	public interface IDownloadService {

		/// <summary>
		/// Get pending downloads.
		/// </summary>
		/// <returns></returns>
		IEnumerable<DownloadItem> GetPendingDownloads ();

		/// <summary>
		/// Start download process.
		/// </summary>
		/// <returns></returns>
		Task StartDownloadProcess ();

		/// <summary>
		/// Set download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="video">Video information.</param>
		/// <param name="quality">Quality.</param>
		void AddDownloadFile ( long releaseId , OnlineVideoModel videoId , VideoQuality quality );

		/// <summary>
		/// Set download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="videos">Videos.</param>
		/// <param name="quality">Quality.</param>
		void AddDownloadFile ( long releaseId , IEnumerable<OnlineVideoModel> videos , VideoQuality quality );

		/// <summary>
		/// Remove download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="videoId">Video identifier.</param>
		Task RemoveDownloadFile ( long releaseId , int videoId );

		/// <summary>
		/// Remove download release.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		Task RemoveDownloadRelease ( long releaseId );

		/// <summary>
		/// Downliading processed.
		/// </summary>
		bool DownloadingProcessed
		{
			get;
		}

	}

}
