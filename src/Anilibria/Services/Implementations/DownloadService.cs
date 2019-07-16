using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Windows.Storage;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Download service.
	/// </summary>
	public class DownloadService : IDownloadService {

		private readonly int BufferSize = 1024 * 20;

		private readonly long BufferFlush = 204800;

		private readonly IDataContext m_DataContext;

		private readonly IEntityCollection<DownloadFileEntity> m_Collection;

		private DownloadFileEntity m_Entity;

		private HttpClient m_HttpClient = new HttpClient ();

		private bool m_DownloadingProcessed = false;

		private Action<long , int , int> m_DownloadProgressHandler;

		private Action<DownloadReleaseEntity , int> m_DownloadFinishedHandler;

		public DownloadService ( IDataContext dataContext ) {
			m_DataContext = dataContext;
			m_Collection = m_DataContext.GetCollection<DownloadFileEntity> ();

			m_Entity = m_Collection.FirstOrDefault ();
			if ( m_Entity == null ) {
				m_Entity = new DownloadFileEntity {
					DownloadingReleases = new List<DownloadReleaseEntity> ()
				};
				m_Collection.Add ( m_Entity );
			}
		}

		/// <summary>
		/// Add download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="videoIds">Video identifier.</param>
		/// <param name="quality">Quality.</param>
		public void AddDownloadFile ( long releaseId , IEnumerable<OnlineVideoModel> videoIds , VideoQuality quality ) {
			foreach ( var videoId in videoIds ) AddDownloadFile ( releaseId , videoId , quality );
		}

		/// <summary>
		/// Set download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="videoId">Video identifier.</param>
		/// <param name="quality">Quality.</param>
		public void AddDownloadFile ( long releaseId , OnlineVideoModel videoInfo , VideoQuality quality ) {
			var releaseItem = m_Entity.DownloadingReleases.FirstOrDefault ( a => a.ReleaseId == releaseId );
			if ( releaseItem == null ) {
				releaseItem = new DownloadReleaseEntity {
					ReleaseId = releaseId ,
					Active = true ,
					Videos = new List<DownloadReleaseVideoEntity> {
						new DownloadReleaseVideoEntity {
							IsDownloaded = false,
							Quality = quality,
							DownloadUrl = quality == VideoQuality.HD ? videoInfo.DownloadableHD.ToString() : videoInfo.DownloadableSD.ToString(),
							DownloadedSize = 0,
							Id = videoInfo.Order
						}
					}
				};
				var list = m_Entity.DownloadingReleases.ToList ();
				list.Add ( releaseItem );
				m_Entity.DownloadingReleases = list;
			}
			else {
				var video = releaseItem.Videos.Where ( a => a.Id == videoInfo.Order && a.Quality == quality ).FirstOrDefault ();
				if ( video == null ) {
					video = new DownloadReleaseVideoEntity {
						IsDownloaded = false ,
						Quality = quality ,
						DownloadUrl = quality == VideoQuality.HD ? videoInfo.DownloadableHD.ToString () : videoInfo.DownloadableSD.ToString () ,
						DownloadedSize = 0 ,
					};
					var videos = releaseItem.Videos.ToList ();
					videos.Add ( video );
					releaseItem.Videos = videos;
				}
				releaseItem.Active = true;
			}

			m_Collection.Update ( m_Entity );
		}

		/// <summary>
		/// Get downloads.
		/// </summary>
		/// <param name="downloadItemsMode">Download items mode.</param>
		/// <returns></returns>
		public IEnumerable<DownloadReleaseEntity> GetDownloads ( DownloadItemsMode downloadItemsMode ) {

			switch ( downloadItemsMode ) {
				case DownloadItemsMode.All:
					return m_Entity.DownloadingReleases
						.OrderBy ( a => a.Order )
						.ToList ();
				case DownloadItemsMode.Downloaded:
					return m_Entity.DownloadingReleases
						.Where ( a => a.Videos.All ( b => b.IsDownloaded ) )
						.OrderBy ( a => a.Order )
						.ToList ();
				case DownloadItemsMode.Downloading:
					return m_Entity.DownloadingReleases
						.Where ( a => a.Videos.Any ( b => b.IsDownloaded ) && !a.Videos.All ( b => b.IsDownloaded ) )
						.OrderBy ( a => a.Order )
						.ToList ();
				case DownloadItemsMode.NotDownloaded:
					return m_Entity.DownloadingReleases
						.Where ( a => !a.Videos.All ( b => b.IsDownloaded ) )
						.OrderBy ( a => a.Order )
						.ToList ();
				default: throw new NotSupportedException ( $"Download item mode {downloadItemsMode} not supported." );
			}

		}

		/// <summary>
		/// Remove download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="videoId">Video identifier.</param>
		public async Task RemoveDownloadFile ( long releaseId , int videoId ) {
			var releaseItem = m_Entity.DownloadingReleases.FirstOrDefault ( a => a.ReleaseId == releaseId );
			if ( releaseItem == null ) return;

			var files = releaseItem.Videos.Where ( a => a.Id == videoId ).ToList ();
			releaseItem.Videos = releaseItem.Videos.Where ( a => a.Id != videoId ).ToList ();

			foreach ( var file in files ) {
				if ( file.IsDownloaded ) {
					var storageFile = await StorageFile.GetFileFromPathAsync ( file.DownloadedPath );
					await storageFile.DeleteAsync ();
				}
			}
			m_Collection.Update ( m_Entity );
		}

		/// <summary>
		/// Remove download release.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		public async Task RemoveDownloadRelease ( long releaseId ) {
			var releaseItem = m_Entity.DownloadingReleases.FirstOrDefault ( a => a.ReleaseId == releaseId );
			if ( releaseItem == null ) return;

			foreach ( var file in releaseItem.Videos ) {
				if ( file.IsDownloaded ) {
					var storageFile = await StorageFile.GetFileFromPathAsync ( file.DownloadedPath );
					await storageFile.DeleteAsync ();
				}
			}

			var list = m_Entity.DownloadingReleases.ToList ();
			list.Remove ( releaseItem );
			m_Entity.DownloadingReleases = list;

			m_Collection.Update ( m_Entity );
		}

		private async Task<StorageFile> DownloadFile ( string url , long offset , long releaseId , int videoId ) {
			long contentLength = 0;
			long downloadedSize = offset;
			byte[] buffer = new byte[BufferSize];
			using ( var response = await m_HttpClient.GetAsync ( url , HttpCompletionOption.ResponseHeadersRead ) ) {
				if ( !response.Content.Headers.ContentLength.HasValue ) throw new NotSupportedException ( "Files without content lenght not supported" );

				contentLength = response.Content.Headers.ContentLength.Value;

				var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync ( "temp_downloading" , CreationCollisionOption.GenerateUniqueName );

				using ( var fileStream = await file.OpenStreamForWriteAsync () )
				using ( var stream = await response.Content.ReadAsStreamAsync () ) {
					while ( true ) {
						var readed = await stream.ReadAsync ( buffer , 0 , BufferSize );
						if ( readed == 0 ) break;

						await fileStream.WriteAsync ( buffer , 0 , readed );

						downloadedSize += readed;

						var percent = Math.Round ( ( (double) downloadedSize / (double) contentLength ) * 100 );

						m_DownloadProgressHandler?.Invoke ( releaseId , videoId , (int) percent );

						if ( downloadedSize % BufferFlush‬ == 0 ) await fileStream.FlushAsync ();
					}
					await fileStream.FlushAsync ();
					fileStream.Close ();
				}

				return file;
			}
		}

		/// <summary>
		/// Start download process.
		/// </summary>
		/// <returns></returns>
		public async Task StartDownloadProcess () {
			if ( m_DownloadingProcessed ) return; // prevent processes from running twice

			var activeReleases = m_Entity.DownloadingReleases.Where ( a => a.Active ).OrderBy ( a => a.Order ).ToList ();

			m_DownloadingProcessed = true;

			foreach ( var activeRelease in activeReleases ) {
				var videos = activeRelease.Videos.Where ( a => !a.IsDownloaded ).ToList ();
				foreach ( var videoFile in videos ) {
					var downloadedFile = await DownloadFile ( videoFile.DownloadUrl , 0 , activeRelease.ReleaseId , videoFile.Id );
					videoFile.IsDownloaded = true;
					videoFile.DownloadedPath = downloadedFile.Path;

					m_DownloadFinishedHandler?.Invoke ( activeRelease , videoFile.Id );
				}
				activeRelease.Active = activeRelease.Videos.Any ( a => a.IsDownloaded );
				m_Collection.Update ( m_Entity );
			}

			m_DownloadingProcessed = false;
		}

		/// <summary>
		/// Set download progress.
		/// </summary>
		/// <param name="progressHandler">Progress handler.</param>
		public void SetDownloadProgress ( Action<long , int , int> progressHandler ) => m_DownloadProgressHandler = progressHandler;

		/// <summary>
		/// Set download finished.
		/// </summary>
		/// <param name="finishHandler">Finish handler.</param>
		public void SetDownloadFinished ( Action<DownloadReleaseEntity , int> finishHandler ) => m_DownloadFinishedHandler = finishHandler;

		/// <summary>
		/// Get download release.
		/// </summary>
		public DownloadReleaseEntity GetDownloadRelease ( long releaseId ) {
			return m_Entity.DownloadingReleases
				.FirstOrDefault ( a => a.ReleaseId == releaseId );
		}

	}

}
