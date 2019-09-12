using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Windows.Storage;
using Windows.UI.Xaml;

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

		private DispatcherTimer m_SpeedTimer = new DispatcherTimer ();

		private bool m_DownloadingProcessed = false;

		private Action<long , int , int , long , VideoQuality , long> m_DownloadProgressHandler;

		private Action<DownloadReleaseEntity , int , long , VideoQuality> m_DownloadFinishedHandler;

		private long m_DownloadedSize = 0;

		private long m_Speed = 0;

		private bool m_PauseDownloading = false;

		private bool m_SkipCurrentDownloadingRelease = false;

		private bool m_SkipCurrentDownloadingVideo = false;

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
			m_SpeedTimer.Interval = TimeSpan.FromSeconds ( 1 );
			m_SpeedTimer.Tick += SpeedTimerTick;
		}

		private void SpeedTimerTick ( object sender , object e ) {
			if ( !m_DownloadingProcessed ) return;

			m_Speed = m_DownloadedSize;
			m_DownloadedSize = 0;
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
						Id = videoInfo.Order
					};
					var videos = releaseItem.Videos.ToList ();
					videos.Add ( video );
					releaseItem.Videos = videos;
					releaseItem.Active = true;
				}
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
				if ( file.IsDownloaded ) await DeleteFile ( file.DownloadedPath );

				if ( file.IsProgress ) {
					m_SkipCurrentDownloadingVideo = true;
					while ( true ) {
						await Task.Delay ( 50 );
						if ( !m_SkipCurrentDownloadingVideo ) {
							await DeleteFile ( file.DownloadedPath );
							break;
						}
					}
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
				if ( file.IsDownloaded ) await DeleteFile ( file.DownloadedPath );

				if ( file.IsProgress ) {
					m_SkipCurrentDownloadingRelease = true;
					while ( true ) {
						await Task.Delay ( 200 );
						if ( !m_SkipCurrentDownloadingRelease ) {
							await DeleteFile ( file.DownloadedPath );
							break;
						}
					}
				}
			}

			var list = m_Entity.DownloadingReleases.ToList ();
			list.Remove ( releaseItem );
			m_Entity.DownloadingReleases = list;

			m_Collection.Update ( m_Entity );
		}

		private async Task<StorageFile> DownloadFile ( string url , long offset , long releaseId , int videoId , VideoQuality videoQuality , Action<ulong , string> changeSizeHandler , string downloadedPath ) {
			long contentLength = 0;
			long downloadedSize = 0;
			var isResumes = false;
			var isNeedAppend = false;
			byte[] buffer = new byte[BufferSize];
			using ( var response = await m_HttpClient.GetAsync ( url , HttpCompletionOption.ResponseHeadersRead ) ) {
				if ( !response.Content.Headers.ContentLength.HasValue ) throw new NotSupportedException ( "Files without content lenght not supported" );

				contentLength = response.Content.Headers.ContentLength.Value;

				StorageFile file = null;
				if ( string.IsNullOrEmpty ( downloadedPath ) ) {
					file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync ( "temp_downloading" , CreationCollisionOption.GenerateUniqueName );
				}
				else {
					isNeedAppend = true;
					file = await StorageFile.GetFileFromPathAsync ( downloadedPath );
				}

				using ( var fileStream = await file.OpenStreamForWriteAsync () )
				using ( var stream = await response.Content.ReadAsStreamAsync () ) {
					if ( isNeedAppend ) fileStream.Position = fileStream.Length;
					while ( true ) {
						if ( m_SkipCurrentDownloadingRelease ) break;
						if ( m_SkipCurrentDownloadingVideo ) break;
						if ( m_PauseDownloading ) {
							await Task.Delay ( 500 );
							continue;
						}

						if ( buffer.Length < BufferSize ) buffer = new byte[BufferSize];

						var readed = await stream.ReadAsync ( buffer , 0 , BufferSize );
						if ( readed == 0 ) break;

						downloadedSize += readed;
						if ( offset > 0 && downloadedSize <= offset ) continue;

						if ( offset > 0 && !isResumes ) {
							var difference = downloadedSize - offset;
							var cuttedBuffer = new byte[difference];
							Buffer.BlockCopy ( buffer , 0 , cuttedBuffer , 0 , (int) difference );
							buffer = cuttedBuffer;
							readed = (int) difference;
							isResumes = true;
						}

						await fileStream.WriteAsync ( buffer , 0 , readed );

						m_DownloadedSize += readed;
						changeSizeHandler?.Invoke ( (ulong) downloadedSize , file.Path );

						var percent = Math.Round ( ( (double) downloadedSize / (double) contentLength ) * 100 );

						m_DownloadProgressHandler?.Invoke ( releaseId , videoId , (int) percent , m_Speed , videoQuality , downloadedSize );

						if ( downloadedSize % BufferFlush‬ == 0 ) await fileStream.FlushAsync ();
					}
					await fileStream.FlushAsync ();
					fileStream.Close ();
				}

				return file;
			}
		}

		private DownloadReleaseVideoEntity GetNextDownloadItem ( DownloadReleaseEntity downloadRelease ) {
			return downloadRelease.Videos
				.Where ( a => !a.IsDownloaded )
				.OrderByDescending ( a => a.IsProgress )
				.ThenBy ( a => a.Id )
				.FirstOrDefault ();
		}

		/// <summary>
		/// Start download process.
		/// </summary>
		/// <returns></returns>
		public async Task StartDownloadProcess () {
			if ( m_DownloadingProcessed ) return; // prevent processes from running twice

			var activeReleases = m_Entity.DownloadingReleases.Where ( a => a.Active ).OrderBy ( a => a.Order ).ToList ();

			m_DownloadingProcessed = true;

			m_SpeedTimer.Start ();

			foreach ( var activeRelease in activeReleases ) {
				while ( true ) {
					if ( m_SkipCurrentDownloadingRelease ) break;
					var videoFile = GetNextDownloadItem ( activeRelease );
					if ( videoFile == null ) break;
					if ( !activeRelease.Active ) break;

					videoFile.IsProgress = true;
					StorageFile downloadedFile = null;
					try {
						downloadedFile = await DownloadFile (
							videoFile.DownloadUrl ,
							(long) videoFile.DownloadedSize ,
							activeRelease.ReleaseId ,
							videoFile.Id ,
							videoFile.Quality ,
							( newDownloadSize , filePath ) => {
								videoFile.DownloadedSize = newDownloadSize;
								if ( string.IsNullOrEmpty ( videoFile.DownloadedPath ) ) videoFile.DownloadedPath = filePath;
								m_Collection.Update ( m_Entity );
							} ,
							videoFile.DownloadedPath
						);
					}
					catch {
						continue;
					}

					if ( m_SkipCurrentDownloadingVideo ) {
						videoFile.IsDownloaded = true;
						m_SkipCurrentDownloadingVideo = false;
						continue;
					}
					if ( m_SkipCurrentDownloadingRelease ) break;

					videoFile.IsProgress = false;
					videoFile.IsDownloaded = true;
					videoFile.DownloadedSize = ( await downloadedFile.GetBasicPropertiesAsync () ).Size;
					videoFile.DownloadedPath = downloadedFile.Path;

					activeRelease.Active = activeRelease.Videos.Any ( a => !a.IsDownloaded );

					m_DownloadFinishedHandler?.Invoke ( activeRelease , videoFile.Id , (long) videoFile.DownloadedSize , videoFile.Quality );

					m_Collection.Update ( m_Entity );
				}
				m_SkipCurrentDownloadingRelease = false;
			}

			m_SpeedTimer.Stop ();

			m_DownloadingProcessed = false;
		}

		/// <summary>
		/// Set download progress.
		/// </summary>
		/// <param name="progressHandler">Progress handler.</param>
		public void SetDownloadProgress ( Action<long , int , int , long , VideoQuality , long> progressHandler ) => m_DownloadProgressHandler = progressHandler;

		/// <summary>
		/// Set download finished.
		/// </summary>
		/// <param name="finishHandler">Finish handler.</param>
		public void SetDownloadFinished ( Action<DownloadReleaseEntity , int , long , VideoQuality> finishHandler ) => m_DownloadFinishedHandler = finishHandler;

		/// <summary>
		/// Get download release.
		/// </summary>
		public DownloadReleaseEntity GetDownloadRelease ( long releaseId ) {
			return m_Entity.DownloadingReleases
				.FirstOrDefault ( a => a.ReleaseId == releaseId );
		}

		private async Task DeleteFile ( string path ) { 
			try {
				var storageFile = await StorageFile.GetFileFromPathAsync ( path );
				await storageFile.DeleteAsync ();
			} catch {
				//TODO: check file exists and another limitations
			}
		}

		/// <summary>
		/// Remove download file.
		/// </summary>
		/// <param name="releaseId">Release identifier.</param>
		/// <param name="videoId">Video identifier.</param>
		/// <param name="videoQuality">Video quality.</param>
		public async Task RemoveDownloadFile ( long releaseId , int videoId , VideoQuality videoQuality ) {
			var releaseItem = m_Entity.DownloadingReleases.FirstOrDefault ( a => a.ReleaseId == releaseId );
			if ( releaseItem == null ) return;

			var videoFile = releaseItem.Videos.Where ( a => a.Id == videoId && a.Quality == videoQuality ).FirstOrDefault ();
			if ( videoFile == null ) return;

			releaseItem.Videos = releaseItem.Videos
				.Where ( a => a != videoFile )
				.ToList ();

			if ( videoFile.IsDownloaded ) await DeleteFile ( videoFile.DownloadedPath );

			if ( videoFile.IsProgress ) {
				m_SkipCurrentDownloadingVideo = true;
				while ( true ) {
					await Task.Delay ( 50 );
					if ( !m_SkipCurrentDownloadingVideo ) {
						await DeleteFile ( videoFile.DownloadedPath );
						break;
					}
				}
			}

			m_Collection.Update ( m_Entity );
		}

		/// <summary>
		/// Pause download.
		/// </summary>
		public bool IsCanPauseDownload () => m_DownloadingProcessed;

		/// <summary>
		/// Pause download.
		/// </summary>
		public void PauseDownload () => m_PauseDownloading = true;

		/// <summary>
		/// Resume download.
		/// </summary>
		public void ResumeDownload () => m_PauseDownloading = false;

	}

}
