using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Storage.Entities;

namespace Anilibria.Services {

	/// <summary>
	/// Download service.
	/// </summary>
	public interface IDownloadService {

		/// <summary>
		/// Get downloads.
		/// </summary>
		/// <returns></returns>
		IEnumerable<DownloadReleaseEntity> GetDownloads ( DownloadItemsMode downloadItemsMode );

		/// <summary>
		/// Get download release.
		/// </summary>
		DownloadReleaseEntity GetDownloadRelease ( long releaseId );

		/// <summary>
		/// Start download process.
		/// </summary>
		/// <returns></returns>
		Task StartDownloadProcess ();

		/// <summary>
		/// Set download progress.
		/// </summary>
		/// <param name="progressHandler">Progress handler.</param>
		void SetDownloadProgress ( Action<long , int , int , long , VideoQuality , long> progressHandler );

		/// <summary>
		/// Set download finished.
		/// </summary>
		/// <param name="finishHandler">Finish handler.</param>
		void SetDownloadFinished ( Action<DownloadReleaseEntity , int , long , VideoQuality> finishHandler );

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
		/// <param name="videoQuality">Video quality.</param>
		Task RemoveDownloadFile ( long releaseId , int videoId , VideoQuality videoQuality );

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
		/// Pause download.
		/// </summary>
		void PauseDownload ();

		/// <summary>
		/// Resume download.
		/// </summary>
		void ResumeDownload ();

		/// <summary>
		/// Is can download pause.
		/// </summary>
		bool IsCanPauseDownload ();

	}

}
