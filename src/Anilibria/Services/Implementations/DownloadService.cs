using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Windows.Storage;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Download service.
	/// </summary>
	public class DownloadService : IDownloadService {

		private readonly int BufferSize = 1024 * 3;

		private readonly IDataContext m_DataContext;

		private readonly IEntityCollection<DownloadFileEntity> m_Collection;

		private DownloadFileEntity m_Entity;

		private HttpClient m_HttpClient = new HttpClient ();

		private bool m_DownloadingProcessed = false;

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
		/// Set download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="videoId">Video identifier.</param>
		/// <param name="quality">Quality.</param>
		public void AddDownloadFile ( long releaseId , int videoId , VideoQuality quality ) {
			var releaseItem = m_Entity.DownloadingReleases.FirstOrDefault ( a => a.ReleaseId == releaseId );
			if ( releaseItem == null ) {
				releaseItem = new DownloadReleaseEntity {
					ReleaseId = releaseId ,
					Active = false ,
					Videos = new List<DownloadReleaseVideoEntity> {
						new DownloadReleaseVideoEntity {
							IsDownloaded = false,
							Quality = quality,
							DownloadedPath = "",
							DownloadedSize = 0,
							Id = videoId
						}
					}
				};
			}
			else {
				var video = releaseItem.Videos.Where ( a => a.Id == videoId && a.Quality == quality ).FirstOrDefault ();
				if ( video == null ) {
					video = new DownloadReleaseVideoEntity {
						IsDownloaded = false ,
						Quality = quality ,
						DownloadedPath = "" ,
						DownloadedSize = 0 ,
					};
					var videos = releaseItem.Videos.ToList ();
					videos.Add ( video );
					releaseItem.Videos = videos;
				}
			}

			m_Collection.Update ( m_Entity );
		}

		/// <summary>
		/// Get panding downloads.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DownloadItem> GetPendingDownloads () {
			return null;
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
			m_Collection.Update ( m_Entity );
		}

		private async Task DownloadFile ( string url , long offset ) {
			long contentLength = 0;
			byte[] buffer = new byte[BufferSize];
			using ( var response = await m_HttpClient.GetAsync ( url , HttpCompletionOption.ResponseHeadersRead ) ) {
				if ( !response.Content.Headers.ContentLength.HasValue ) throw new NotSupportedException ( "Files without content lenght not supported" );

				contentLength = response.Content.Headers.ContentLength.Value;

				var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync ( "temp_downloading" , CreationCollisionOption.GenerateUniqueName );

				using ( var fileStream = await file.OpenStreamForWriteAsync () )
				using ( var stream = await response.Content.ReadAsStreamAsync () ) {
					await stream.ReadAsync ( buffer , 0 , BufferSize );
					await fileStream.WriteAsync ( buffer , 0 , BufferSize );
				}
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
				foreach ( var videoFile in videos ) await DownloadFile ( videoFile.DownloadUrl , 0 );
			}

			m_DownloadingProcessed = false;
		}

	}

}
