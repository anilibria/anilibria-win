using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.OnlinePlayer.PresentationClasses;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services;
using Anilibria.Services.Implementations;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Newtonsoft.Json;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI.ViewManagement;

namespace Anilibria.Pages.OnlinePlayer {

	/// <summary>
	/// View model.
	/// </summary>
	public class OnlinePlayerViewModel : ViewModel, INavigation {

		private const string PlayerQualitySettings = "PlayerQuality";

		private const string PlayerVolumeSettings = "PlayerVolume";

		private const string AutoTransitionSettings = "AutoTransition";

		private const string NeedShowReleaseInfoSettings = "NeedShowReleaseInfo";

		private const string ControlPanelOpacitySettings = "ControlPanelOpacity";

		private const string PlaylistButtonPositionSettings = "PlaylistButtonPosition";

		private const string PlaylistSortSettings = "PlaylistSorting";

		private const string JumpMinutesSetting = "JumpMinutesSetting";

		private const string JumpSecondsSetting = "JumpSecondsSetting";

		private double m_Volume;

		private string m_DisplayVolume;

		private bool m_IsMuted;

		private double m_PreviousVolume;

		private string m_DisplayDuration;

		private string m_DisplayPosition;

		private string m_DisplayPositionPercent;

		private TimeSpan m_Duration;

		private Uri m_VideoSource;

		private ReleaseModel m_SelectedRelease;

		private ObservableCollection<OnlineVideoModel> m_OnlineVideos = new ObservableCollection<OnlineVideoModel> ();

		private ObservableCollection<IGrouping<string , OnlineVideoModel>> m_GroupingOnlineVideos = new ObservableCollection<IGrouping<string , OnlineVideoModel>> ();

		private IEnumerable<ReleaseModel> m_Releases;

		private OnlineVideoModel m_SelectedOnlineVideo;

		private double m_DurationSecond;

		private double m_Position;

		private readonly IAnalyticsService m_AnalyticsService;

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IDataContext m_DataContext;

		private readonly IDownloadService m_DownloadService;

		private readonly IReleasesService m_ReleasesService;

		private bool m_IsHD;

		private bool m_IsSD;

		private double m_RestorePosition = 0;

		private bool m_IsMediaOpened;

		private bool m_IsBuffering;

		private PlayerRestoreEntity m_PlayerRestoreEntity;

		private ReleaseVideoStateEntity m_ReleaseVideoStateEntity;

		private double m_PositionPercent;

		private readonly IEntityCollection<PlayerRestoreEntity> m_RestoreCollection;

		private readonly IEntityCollection<ReleaseVideoStateEntity> m_ReleaseStateCollection;

		private DisplayRequest m_DisplayRequest;

		private bool m_IsVideosFlyoutVisible;

		private bool m_IsShowReleaseInfo;

		private bool m_IsExistsFullHD;

		private bool m_IsFullHD;

		private bool m_ShowPlaylistButton = true;

		private bool m_IsAutoTransition;

		private bool m_IsSupportedCompactOverlay;

		private bool m_IsCompactOverlayEnabled;

		private bool m_IsNeedShowReleaseInfo;

		private double m_ControlPanelOpacity;

		private bool m_NotUpdateSelectedRelese = false;

		private ObservableCollection<PlaylistButtonPositionItem> m_PlaylistButtonPositions = new ObservableCollection<PlaylistButtonPositionItem>
		{
			new PlaylistButtonPositionItem
			{
				Position = PlaylistButtonPosition.Top,
				Title = "Сверху"
			},
			new PlaylistButtonPositionItem
			{
				Position = PlaylistButtonPosition.Center,
				Title = "В центре"
			},
			new PlaylistButtonPositionItem
			{
				Position = PlaylistButtonPosition.Bottom,
				Title = "Снизу"
			}
		};

		private ObservableCollection<MinuteItem> m_Minutes = new ObservableCollection<MinuteItem> {
			new MinuteItem {
				Title = "0",
				Value = 0
			},
			new MinuteItem {
				Title = "1",
				Value = 1
			},
			new MinuteItem {
				Title = "2",
				Value = 2
			}
		};

		private MinuteItem m_SelectedMinute;

		private ObservableCollection<SecondItem> m_Seconds = new ObservableCollection<SecondItem> {
			new SecondItem {
				Title = "0",
				Value = 0
			},
			new SecondItem {
				Title = "5",
				Value = 5
			},
			new SecondItem {
				Title = "10",
				Value = 10
			},
			new SecondItem {
				Title = "15",
				Value = 15
			},
			new SecondItem {
				Title = "20",
				Value = 20
			},
			new SecondItem {
				Title = "25",
				Value = 25
			},
			new SecondItem {
				Title = "30",
				Value = 30
			}
		};

		private SecondItem m_SelectedSecond;

		private PlaylistButtonPositionItem m_SelectedPlaylistButtonPosition;

		private int m_JumpMinutes;

		private int m_JumpSeconds;

		private bool m_IsXbox;

		private bool m_IsCinemaHall = false;

		private bool m_IsNormalSpeed = true;

		private bool m_Is025xSpeed;

		private bool m_Is05xSpeed;

		private bool m_Is075xSpeed;

		private bool m_Is125xSpeed;
		
		private bool m_Is175xSpeed;

		private bool m_Is2xSpeed;

		private bool m_Is3xSpeed;

		private bool m_Is4xSpeed;

		private bool m_Is15xSpeed;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="analyticsService">Analytics service.</param>
		/// <param name="dataContext">Data context.</param>
		/// <param name="anilibriaApiService">Anilibria restful service.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public OnlinePlayerViewModel ( IAnalyticsService analyticsService , IDataContext dataContext , IAnilibriaApiService anilibriaApiService , IDownloadService downloadService , IReleasesService releasesService ) {
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
			m_DownloadService = downloadService ?? throw new ArgumentNullException ( nameof ( downloadService ) );
			m_ReleasesService = releasesService ?? throw new ArgumentNullException ( nameof ( releasesService ) );
			m_IsSD = true;
			m_Volume = .8;

			RestoreSettings ();

			UpdateVolumeState ( m_Volume );

			CreateCommands ();

			m_RestoreCollection = m_DataContext.GetCollection<PlayerRestoreEntity> ();
			m_PlayerRestoreEntity = m_RestoreCollection.FirstOrDefault ();
			if ( m_PlayerRestoreEntity == null ) {
				m_PlayerRestoreEntity = new PlayerRestoreEntity {
					ReleaseId = -1 ,
					VideoId = -1 ,
					VideoPosition = 0 ,
					IsCinemaHall = false
				};
				m_RestoreCollection.Add ( m_PlayerRestoreEntity );
			}

			m_ReleaseStateCollection = m_DataContext.GetCollection<ReleaseVideoStateEntity> ();
			m_IsSupportedCompactOverlay = ApplicationView.GetForCurrentView ().IsViewModeSupported ( ApplicationViewMode.CompactOverlay );
		}

		private void RestoreSettings () {
			m_JumpMinutes = 0;
			m_JumpSeconds = 5;
			m_ControlPanelOpacity = 1;
			m_SelectedPlaylistButtonPosition = PlaylistButtonPositions.First ( a => a.Position == PlaylistButtonPosition.Center );
			var values = ApplicationData.Current.RoamingSettings.Values;
			if ( values.ContainsKey ( PlayerQualitySettings ) ) {
				var isHD = (bool) values[PlayerQualitySettings];
				m_IsSD = !isHD;
				m_IsHD = isHD;
			}
			if ( values.ContainsKey ( PlayerVolumeSettings ) ) m_Volume = (double) values[PlayerVolumeSettings];
			if ( values.ContainsKey ( AutoTransitionSettings ) ) m_IsAutoTransition = (bool) values[AutoTransitionSettings];
			if ( values.ContainsKey ( NeedShowReleaseInfoSettings ) ) m_IsNeedShowReleaseInfo = (bool) values[NeedShowReleaseInfoSettings];
			if ( values.ContainsKey ( ControlPanelOpacitySettings ) ) m_ControlPanelOpacity = (double) values[ControlPanelOpacitySettings];
			if ( values.ContainsKey ( PlaylistButtonPositionSettings ) ) {
				var indexButtonPosition = (int) values[PlaylistButtonPositionSettings];
				var position = (PlaylistButtonPosition) indexButtonPosition;

				m_SelectedPlaylistButtonPosition = PlaylistButtonPositions.FirstOrDefault ( a => a.Position == position ) ?? PlaylistButtonPositions.First ( a => a.Position == PlaylistButtonPosition.Center );
			}
			if ( values.ContainsKey ( JumpMinutesSetting ) ) m_JumpMinutes = (int) values[JumpMinutesSetting];
			if ( values.ContainsKey ( JumpSecondsSetting ) ) m_JumpSeconds = (int) values[JumpSecondsSetting];
			SelectedMinute = Minutes.FirstOrDefault ( a => a.Value == m_JumpMinutes );
			SelectedSecond = Seconds.FirstOrDefault ( a => a.Value == m_JumpSeconds );
		}

		private async Task SaveReleaseWatchTimestamp ( long releaseId ) {
			var release = m_ReleasesService.GetReleaseById ( releaseId );

			release.LastWatchTimestamp = (long) ( DateTime.UtcNow.Subtract ( new DateTime ( 1970 , 1 , 1 ) ) ).TotalSeconds;

			await m_ReleasesService.SaveReleases ();

			var lastThreeWatchReleases = m_ReleasesService.GetReleases ()
				.Where ( a => a.LastWatchTimestamp > 0 )
				.OrderByDescending ( a => a.LastWatchTimestamp )
				.Take ( 3 )
				.ToList ();
			if ( !lastThreeWatchReleases.Any () ) return;

			var jumpService = new JumpListService ();
			var dictionary = new Dictionary<long , string> ();
			foreach ( var watchRelease in lastThreeWatchReleases ) dictionary.Add ( watchRelease.Id , watchRelease.Title );
			await jumpService.ChangeWatchHistoryItems ( dictionary );
		}

		/// <summary>
		/// Create commands.
		/// </summary>
		private void CreateCommands () {
			ChangeVolumeCommand = CreateCommand<double> ( ChangeVolume );
			MuteCommand = CreateCommand ( Mute );
			ShowSidebarCommand = CreateCommand ( ShowSidebarFromPage );
			ToggleFullScreenCommand = CreateCommand ( ToggleFullScreen );
			ShowPlaylistCommand = CreateCommand ( ShowPlaylist );
			NextTrackCommand = CreateCommand ( NextTrack );
			PreviousTrackCommand = CreateCommand ( PreviousTrack );
			EnableCompactModeCommand = CreateCommand ( EnableCompactMode );
			LeaveCompactModeCommand = CreateCommand ( LeaveCompactMode );
			ToggleSeenMarkCommand = CreateCommand<OnlineVideoModel> ( ToggleSeenMark );
		}

		private void ToggleSeenMark ( OnlineVideoModel onlineVideo ) {
			if ( onlineVideo == null ) return;
			if ( Releases == null ) return;

			var videoRelease = Releases.FirstOrDefault ( a => a.Id == onlineVideo.ReleaseId );
			if ( videoRelease == null ) return;

			var oldSelectedRelease = m_SelectedRelease;
			m_SelectedRelease = videoRelease;
			FillReleaseVideoState ();
			m_SelectedRelease = oldSelectedRelease;

			var videoState = m_ReleaseVideoStateEntity.VideoStates.FirstOrDefault ( a => a.Id == onlineVideo.Order );
			if ( videoState == null ) {
				m_ReleaseVideoStateEntity.VideoStates.Add (
					new VideoStateEntity {
						Id = onlineVideo.Order ,
						IsSeen = true
					}
				);
				onlineVideo.IsSeen = true;
			} else {
				onlineVideo.IsSeen = !onlineVideo.IsSeen;
				videoState.IsSeen = onlineVideo.IsSeen;
			}

			m_ReleaseStateCollection.Update ( m_ReleaseVideoStateEntity );
		}

		private async void LeaveCompactMode () {
			await ApplicationView.GetForCurrentView ().TryEnterViewModeAsync ( ApplicationViewMode.Default );
			IsCompactOverlayEnabled = false;
		}

		private async void EnableCompactMode () {
			var enabled = await ApplicationView.GetForCurrentView ().TryEnterViewModeAsync ( ApplicationViewMode.CompactOverlay );
			IsCompactOverlayEnabled = enabled;
			if ( enabled ) ShowPlaylistButton = true;
		}

		private void PreviousTrack () {
			if ( !( SelectedRelease != null && SelectedRelease.OnlineVideos != null && SelectedRelease.OnlineVideos.Any () ) ) return;
			if ( SelectedOnlineVideo == null ) return;

			if ( !IsCinemaHall ) {
				if ( SelectedOnlineVideo.Order == 1 ) return;

				var previousTrack = SelectedRelease.OnlineVideos.FirstOrDefault ( a => a.Order == SelectedOnlineVideo.Order - 1 );
				if ( previousTrack != null ) {
					PositionPercent = 0;
					SelectedOnlineVideo = previousTrack;
				}
			} else {
				SetPreviousVideoInCinemaHall ();
			}
		}

		private void NextTrack () {
			if ( !( SelectedRelease != null && SelectedRelease.OnlineVideos != null && SelectedRelease.OnlineVideos.Any () ) ) return;
			if ( SelectedOnlineVideo == null ) return;

			if ( !IsCinemaHall ) {
				if ( SelectedOnlineVideo.Order == SelectedRelease.OnlineVideos.Count () ) return;

				var nextTrack = SelectedRelease.OnlineVideos.FirstOrDefault ( a => a.Order == SelectedOnlineVideo.Order + 1 );
				if ( nextTrack != null ) {
					PositionPercent = 0;
					SelectedOnlineVideo = nextTrack;
				}
			} else {
				SetNextVideoInCinemaHall ();
			}
		}

		private void ShowPlaylist () {
			ShowPlaylistButton = false;
		}

		private void ToggleFullScreen () {
			var view = ApplicationView.GetForCurrentView ();
			view.FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
			if ( view.IsFullScreenMode ) {
				view.ExitFullScreenMode ();
			} else {
				view.TryEnterFullScreenMode ();
			}
		}

		/// <summary>
		/// Show sidebar from player page.
		/// </summary>
		private void ShowSidebarFromPage () {
			ShowSidebar?.Invoke ();
		}

		/// <summary>
		/// Mute.
		/// </summary>
		private void Mute () {
			if ( IsMuted ) {
				IsMuted = false;
				Volume = m_PreviousVolume;
			} else {
				m_PreviousVolume = Volume;
				IsMuted = true;
				Volume = 0;
				DisplayVolume = "Mute";
			}
		}

		/// <summary>
		/// Change volume.
		/// </summary>
		/// <param name="value">Value.</param>
		private void ChangeVolume ( double value ) {
			var newVolume = Volume + ( value > 0 ? .1 : -.1 );

			if ( newVolume < 0 ) newVolume = 0;
			if ( newVolume > 1 ) newVolume = 1;

			Volume = newVolume;
		}

		private void SetPercentDisplayVolume ( double value ) => DisplayVolume = ( (int) ( value * 100 ) ).ToString () + "%";

		private void ChangeVideoSource () {
			if ( m_SelectedOnlineVideo == null ) return;

			if ( IsFullHD ) {
				VideoSource = m_SelectedOnlineVideo.FullHDQuality;
				return;
			}
			VideoSource = IsHD ? m_SelectedOnlineVideo.HDQuality : m_SelectedOnlineVideo.SDQuality;
		}

		/// <summary>
		/// Refresh video position.
		/// </summary>
		/// <param name="timeSpan">Time span.</param>
		public void RefreshPosition ( TimeSpan timeSpan ) {
			DisplayPosition = VideoTimeFormatter.ConvertTimeSpanToText ( timeSpan );

			Position = timeSpan.TotalSeconds;

			PositionPercent = Math.Round ( timeSpan.TotalMilliseconds / m_Duration.TotalMilliseconds * 100 );
			DisplayPositionPercent = $"({PositionPercent}%)";
		}

		/// <summary>
		/// Buffering start.
		/// </summary>
		public void BufferingStart () {
			IsBuffering = true;
		}

		/// <summary>
		/// Buffering end.
		/// </summary>
		public void BufferingEnd () {
			IsBuffering = false;
		}

		/// <summary>
		/// Media ended handler.
		/// </summary>
		public void MediaEnded () {
			IsMediaOpened = false;
			var order = SelectedOnlineVideo?.Order ?? -1;

			if ( order > -1 && m_IsAutoTransition ) {
				if ( !IsCinemaHall ) {
					var newSeria = SelectedRelease.OnlineVideos.FirstOrDefault ( a => a.Order == order + 1 );
					if ( newSeria != null ) {
						PositionPercent = 0;
						SelectedOnlineVideo = newSeria;
					}
				} else {
					SetNextVideoInCinemaHall ();
				}
			}
		}

		private void SetNextVideoInCinemaHall () {
			var allVideos = Releases.SelectMany ( a => a.OnlineVideos ).ToList ();
			var indexVideo = allVideos.IndexOf ( SelectedOnlineVideo );
			if ( indexVideo < allVideos.Count - 1 ) {
				var newSeria = allVideos.Skip ( indexVideo + 1 ).FirstOrDefault ( a => !a.IsSeen );
				if ( newSeria != null ) {
					PositionPercent = 0;
					SelectedOnlineVideo = newSeria;
				}
			}
		}

		private void SetPreviousVideoInCinemaHall () {
			var allVideos = Releases.SelectMany ( a => a.OnlineVideos ).ToList ();
			var indexVideo = allVideos.IndexOf ( SelectedOnlineVideo );
			if ( indexVideo > 0 ) {
				var newSeria = allVideos.Take ( indexVideo ).Reverse ().FirstOrDefault ( a => !a.IsSeen );
				if ( newSeria != null ) {
					PositionPercent = 0;
					SelectedOnlineVideo = newSeria;
				}
			}
		}

		public void MediaStateChanged ( MediaPlaybackState playbackState ) {
			switch ( playbackState ) {
				case MediaPlaybackState.None:
					break;
				case MediaPlaybackState.Opening:
					break;
				case MediaPlaybackState.Buffering:
					break;
				case MediaPlaybackState.Playing:
					//WORKAROUND: reactive value changed only after real value changed.
					if ( !IsVideosFlyoutVisible ) IsVideosFlyoutVisible = true;
					IsVideosFlyoutVisible = false;
					break;
				case MediaPlaybackState.Paused:

					break;
				default:
					break;
			}
			IsShowReleaseInfo = playbackState == MediaPlaybackState.Paused;
		}

		/// <summary>
		/// Media closed handler.
		/// </summary>
		public void MediaClosed () => IsMediaOpened = false;

		/// <summary>
		/// Media opened.
		/// </summary>
		/// <param name="success">Success media opened.</param>
		/// <param name="duration">Duration.</param>
		public void MediaOpened ( TimeSpan? duration = default ( TimeSpan? ) ) {
			DisplayDuration = VideoTimeFormatter.ConvertTimeSpanToText ( duration.Value );
			m_Duration = duration.Value;
			DurationSecond = duration.Value.TotalSeconds;

			if ( m_RestorePosition > 0 ) {
				var lastPosition = TimeSpan.FromSeconds ( m_RestorePosition );
				m_RestorePosition = 0;
				ChangePosition ( lastPosition );
			}
			IsMediaOpened = true;

			if ( Is2xSpeed ) ChangePlaybackRate ( 2 );
			if ( Is15xSpeed ) ChangePlaybackRate ( 1.5 );
			if ( Is3xSpeed ) ChangePlaybackRate ( 3 );
			if ( Is025xSpeed ) ChangePlaybackRate ( 0.25 );
			if ( Is05xSpeed ) ChangePlaybackRate ( 0.5 );
			if ( Is075xSpeed ) ChangePlaybackRate ( 0.75 );
			if ( Is175xSpeed ) ChangePlaybackRate ( 1.75 );
		}

		private ReleaseModel MapToReleaseModel ( ReleaseEntity releaseEntity ) {
			return new ReleaseModel {
				Id = releaseEntity.Id ,
				Title = releaseEntity.Title ,
				CountVideoOnline = releaseEntity.Playlist?.Count () ?? 0 ,
				Poster = m_AnilibriaApiService.GetUrl ( releaseEntity.Poster ) ,
				OnlineVideos = releaseEntity.Playlist?
					.Select (
						a => new OnlineVideoModel {
							HDQuality = a.HD ,
							Order = a.Id ,
							SDQuality = a.SD ,
							FullHDQuality = a.FullHD ,
							Title = a.Title ,
							ReleaseName = releaseEntity.Title ,
							ReleaseId = releaseEntity.Id
						}
					)
					.OrderBy ( a => a.Order )
					.ToList ()
			};
		}

		/// <summary>
		/// Save player restore state.
		/// </summary>
		public void SavePlayerRestoreState () {
			if ( SelectedOnlineVideo == null || SelectedRelease == null ) return;

			var isNotNeedUpdatePosition = m_PlayerRestoreEntity?.ReleaseId == SelectedRelease?.Id && m_PlayerRestoreEntity?.VideoPosition > 0 && Position == 0;
			m_PlayerRestoreEntity.ReleaseId = SelectedRelease.Id;
			m_PlayerRestoreEntity.VideoId = SelectedOnlineVideo.Order;
			m_PlayerRestoreEntity.IsCinemaHall = IsCinemaHall;
			if ( !isNotNeedUpdatePosition ) m_PlayerRestoreEntity.VideoPosition = Position;
			m_RestoreCollection.Update ( m_PlayerRestoreEntity );

			FillReleaseVideoState ();

			var videoState = m_ReleaseVideoStateEntity.VideoStates.FirstOrDefault ( a => a.Id == SelectedOnlineVideo.Order );
			if ( videoState == null ) {
				m_ReleaseVideoStateEntity.VideoStates.Add (
					new VideoStateEntity {
						Id = SelectedOnlineVideo.Order ,
						LastPosition = Position
					}
				);
			} else {
				videoState.LastPosition = Position == 0 && videoState.LastPosition > 0 ? videoState.LastPosition : Position;

				if ( !videoState.IsSeen && PositionPercent >= 90 && PositionPercent <= 100 ) {
					videoState.IsSeen = true;
					SelectedOnlineVideo.IsSeen = true;
				}
			}

			m_ReleaseStateCollection.Update ( m_ReleaseVideoStateEntity );
		}

		private void FillReleaseVideoState () {
			if ( m_ReleaseVideoStateEntity == null || m_ReleaseVideoStateEntity.ReleaseId != SelectedRelease.Id ) {
				m_ReleaseVideoStateEntity = m_ReleaseStateCollection.FirstOrDefault ( a => a.ReleaseId == SelectedRelease.Id );
				if ( m_ReleaseVideoStateEntity == null ) {
					m_ReleaseVideoStateEntity = new ReleaseVideoStateEntity {
						ReleaseId = SelectedRelease.Id ,
						VideoStates = new List<VideoStateEntity> ()
					};
					m_ReleaseStateCollection.Add ( m_ReleaseVideoStateEntity );
				}
			}

			if ( m_ReleaseVideoStateEntity.VideoStates == null ) m_ReleaseVideoStateEntity.VideoStates = new List<VideoStateEntity> ();
		}

		private void SetDownloadedPaths ( long releaseId , IEnumerable<OnlineVideoModel> onlineVideos ) {
			var downloads = m_DownloadService.GetDownloads ( DownloadItemsMode.All );
			var releaseDownload = downloads.Where ( a => a.ReleaseId == releaseId ).FirstOrDefault ();
			if ( releaseDownload == null ) return;

			foreach ( var onlineVideo in onlineVideos ) {
				var downloadedVideos = releaseDownload.Videos
					.Where ( a =>
						a.Id == onlineVideo.Order &&
						a.IsDownloaded
					)
					.ToList ();
				var hdVideo = downloadedVideos.FirstOrDefault ( a => a.Quality == VideoQuality.HD );
				var sdVideo = downloadedVideos.FirstOrDefault ( a => a.Quality == VideoQuality.SD );

				if ( hdVideo != null ) onlineVideo.HDQuality = new Uri ( hdVideo.DownloadedPath );
				if ( sdVideo != null ) onlineVideo.SDQuality = new Uri ( sdVideo.DownloadedPath );
			}
		}

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public async void NavigateTo ( object parameter ) {
			try {
				if ( m_DisplayRequest == null ) m_DisplayRequest = new DisplayRequest ();
				m_DisplayRequest.RequestActive ();
			} catch {
			}

			UpdateVolumeState ( m_Volume );

			if ( parameter == null ) {
				if ( VideoSource != null ) {
					ChangePlayback ( PlaybackState.Play , false );
				} else {
					IsCinemaHall = false;

					if ( m_PlayerRestoreEntity != null && m_PlayerRestoreEntity.ReleaseId > 0 ) {
						IsCinemaHall = m_PlayerRestoreEntity.IsCinemaHall;
						if ( IsCinemaHall ) {
							var cinemaHallEntity = m_DataContext.GetCollection<CinemaHallReleaseEntity> ().FirstOrDefault ();
							if ( cinemaHallEntity == null ) return;

							var cinemaHallReleasesIds = cinemaHallEntity.Releases.ToList ();
							var releases = m_ReleasesService.GetReleases ()
								.Where ( a => cinemaHallEntity.Releases.Contains ( a.Id ) )
								.OrderBy ( a => cinemaHallReleasesIds.IndexOf ( a.Id ) )
								.ToList ();
							if ( releases.Any () ) {
								m_RestorePosition = m_PlayerRestoreEntity.VideoPosition;
								Releases = releases
									.Select ( a => MapToReleaseModel ( a ) )
									.ToList ();
								SelectedRelease = Releases.FirstOrDefault ( a => a.Id == m_PlayerRestoreEntity.ReleaseId );
								var onlineVideos = Releases.SelectMany ( a => a.OnlineVideos ).ToList ();
								OnlineVideos = new ObservableCollection<OnlineVideoModel> ( onlineVideos );
								SetDownloadPathsAllReleases ();
								m_NotUpdateSelectedRelese = true;
								GroupingOnlineVideos = new ObservableCollection<IGrouping<string , OnlineVideoModel>> ( onlineVideos.GroupBy ( a => a.ReleaseName ) );
								m_NotUpdateSelectedRelese = false;
							}
						} else {
							var release = m_ReleasesService.GetReleaseById ( m_PlayerRestoreEntity.ReleaseId );
							if ( release != null ) {
								m_RestorePosition = m_PlayerRestoreEntity.VideoPosition;
								SelectedRelease = MapToReleaseModel ( release );
								Releases = new List<ReleaseModel> { SelectedRelease };
								SetDownloadedPaths ( SelectedRelease.Id , SelectedRelease.OnlineVideos );
								OnlineVideos = new ObservableCollection<OnlineVideoModel> ( SelectedRelease.OnlineVideos );
								SelectedOnlineVideo = SelectedRelease.OnlineVideos?.FirstOrDefault ( a => a.Order == m_PlayerRestoreEntity.VideoId );
								ChangePlayback ( PlaybackState.Play , false );
								if ( SelectedRelease != null ) await SaveReleaseWatchTimestamp ( SelectedRelease.Id );
							}
						}
					}
				}
			} else {
				IsCinemaHall = false;

				var releaseLink = parameter as ReleaseLinkModel;
				var cinemHallLink = parameter as CinemaHallLinkModel;

				if ( releaseLink != null ) {
					var releaseLinkEntity = m_ReleasesService.GetReleases ().FirstOrDefault ( a => a.Id == releaseLink.ReleaseId );
					if ( releaseLinkEntity != null ) Releases = new List<ReleaseModel> { MapToReleaseModel ( releaseLinkEntity ) };
				} else if ( cinemHallLink != null ) {
					var releasesIds = cinemHallLink.Releases.ToList ();
					var cinemaHallLinkEntity = m_ReleasesService.GetReleases()
						.Where ( a => cinemHallLink.Releases.Contains ( a.Id ) )
						.ToList ();
					if ( cinemaHallLinkEntity != null ) {
						Releases = cinemaHallLinkEntity
							.Select ( a => MapToReleaseModel ( a ) )
							.OrderBy ( a => releasesIds.IndexOf ( a.Id ) )
							.ToList ();
					}
					IsCinemaHall = true;
				} else {
					Releases = parameter as IEnumerable<ReleaseModel>;
				}
				var release = Releases.First ();
				m_ReleaseVideoStateEntity = m_ReleaseStateCollection?.FirstOrDefault ( a => a.ReleaseId == release.Id );

				m_Position = 0;
				m_RestorePosition = 0;
				RaisePropertyChanged ( () => Position );

				int onlineVideoIndex = release.PrefferedOpenedVideo == null ? -1 : release.PrefferedOpenedVideo.Order;
				if ( !IsCinemaHall && onlineVideoIndex == -1 && m_ReleaseVideoStateEntity != null && m_ReleaseVideoStateEntity.VideoStates != null && m_ReleaseVideoStateEntity.VideoStates.Any () ) {
					onlineVideoIndex = m_ReleaseVideoStateEntity.VideoStates.Max ( a => a.Id );
					var lastVideo = m_ReleaseVideoStateEntity.VideoStates.First ( a => a.Id == onlineVideoIndex );
					m_RestorePosition = lastVideo.LastPosition;
				}

				if ( release != null ) {
					SetDownloadedPaths ( release.Id , release.OnlineVideos );

					release.OnlineVideos = release.OnlineVideos?.OrderBy ( a => a.Order ).ToList () ?? Enumerable.Empty<OnlineVideoModel> ();
					OnlineVideos = new ObservableCollection<OnlineVideoModel> ( release.OnlineVideos );
					m_NotUpdateSelectedRelese = true;
					GroupingOnlineVideos = new ObservableCollection<IGrouping<string , OnlineVideoModel>> ( Releases.SelectMany ( a => a.OnlineVideos ).GroupBy ( a => a.ReleaseName ) );
					m_NotUpdateSelectedRelese = false;
				}
				SelectedRelease = release;
				if ( !IsCinemaHall ) {
					SelectedOnlineVideo = null;
					SelectedOnlineVideo = onlineVideoIndex == -1 ? SelectedRelease?.OnlineVideos?.FirstOrDefault () : SelectedRelease?.OnlineVideos?.FirstOrDefault ( a => a.Order == onlineVideoIndex );

					if ( SelectedOnlineVideo != null ) ChangePlayback ( PlaybackState.Play , false );

					if ( release != null ) {
						release.PrefferedOpenedVideo = null;
						await SaveReleaseWatchTimestamp ( release.Id );
					}
				}
			}

			ShowPlaylistButton = true;

			m_AnalyticsService.TrackEvent ( "OnlinePlayer" , "Opened" , parameter == null ? "Parameter is null" : "Parameter is populated" );

			if ( SelectedOnlineVideo != null ) ScrollToSelectedPlaylist ();

			if ( SelectedRelease != null && !IsCinemaHall ) {
				FillReleaseVideoState ();

				var states = m_ReleaseVideoStateEntity.VideoStates?.ToList ();
				foreach ( var onlinevideo in SelectedRelease.OnlineVideos ) {
					onlinevideo.IsSeen = states?.FirstOrDefault ( a => a.Id == onlinevideo.Order )?.IsSeen ?? false;
				}
			}

			if ( SelectedRelease != null && IsCinemaHall ) {
				SetDownloadPathsAllReleases ();
				SelectedOnlineVideo = null;
				if ( parameter == null && m_PlayerRestoreEntity != null ) {
					SelectedOnlineVideo = SelectedRelease.OnlineVideos.FirstOrDefault ( a => a.Order == m_PlayerRestoreEntity.VideoId );
				} else {
					PositionPercent = 0;
					SelectedOnlineVideo = Releases.SelectMany ( a => a.OnlineVideos ).FirstOrDefault ( a => !a.IsSeen );
				}

				if ( SelectedOnlineVideo != null ) {
					ChangePlayback ( PlaybackState.Play , false );
				} else {
					VideoSource = null;
				}
				await SaveReleaseWatchTimestamp ( SelectedRelease.Id );
			}
		}

		private void SetDownloadPathsAllReleases () {
			foreach ( var release in Releases ) {
				var states = m_ReleaseStateCollection?.FirstOrDefault ( a => a.ReleaseId == release.Id )?.VideoStates?.ToList ();
				if ( states == null ) continue;

				foreach ( var video in release.OnlineVideos ) {
					video.IsSeen = states?.FirstOrDefault ( a => a.Id == video.Order )?.IsSeen ?? false;
				}
			}
		}

		/// <summary>
		/// End navigate to page.
		/// </summary>
		public void NavigateFrom () {
			try {
				m_DisplayRequest.RequestRelease ();
			} catch {
			}
			ShowPlaylistButton = true;

			if ( VideoSource != null ) ChangePlayback ( PlaybackState.Pause , false );

			var view = ApplicationView.GetForCurrentView ();
			if ( view.IsFullScreenMode ) view.ExitFullScreenMode ();
		}

		/// <summary>
		/// Is videos flyout visible.
		/// </summary>
		public bool IsVideosFlyoutVisible {
			get => m_IsVideosFlyoutVisible;
			set => Set ( ref m_IsVideosFlyoutVisible , value );
		}

		/// <summary>
		/// Media opened.
		/// </summary>
		public bool IsMediaOpened {
			get => m_IsMediaOpened;
			set => Set ( ref m_IsMediaOpened , value );
		}

		/// <summary>
		/// Volume.
		/// </summary>
		public double Volume {
			get => m_Volume;
			set {
				if ( !Set ( ref m_Volume , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[PlayerVolumeSettings] = value;

				UpdateVolumeState ( value );
			}
		}

		private void UpdateVolumeState ( double value ) {
			SetPercentDisplayVolume ( value );

			ChangeVolumeHandler?.Invoke ( value );
		}

		/// <summary>
		/// Is buffering.
		/// </summary>
		public bool IsBuffering {
			get => m_IsBuffering;
			set => Set ( ref m_IsBuffering , value );
		}

		/// <summary>
		/// Display duration.
		/// </summary>
		public string DisplayDuration {
			get => m_DisplayDuration;
			set => Set ( ref m_DisplayDuration , value );
		}

		/// <summary>
		/// Display position.
		/// </summary>
		public string DisplayPosition {
			get => m_DisplayPosition;
			set => Set ( ref m_DisplayPosition , value );
		}

		/// <summary>
		/// Position.
		/// </summary>
		public double Position {
			get => m_Position;
			set => Set ( ref m_Position , value );
		}

		/// <summary>
		/// Is muted.
		/// </summary>
		public bool IsMuted {
			get => m_IsMuted;
			set => Set ( ref m_IsMuted , value );
		}

		/// <summary>
		/// Is HD quality.
		/// </summary>
		public bool IsHD {
			get => m_IsHD;
			set {
				if ( !Set ( ref m_IsHD , value ) ) return;

				m_IsSD = !value;
				m_IsFullHD = !value;
				RaisePropertyChanged ( () => IsSD );
				RaisePropertyChanged ( () => IsFullHD );

				ApplicationData.Current.RoamingSettings.Values[PlayerQualitySettings] = value;

				m_RestorePosition = Position;
				ChangeVideoSource ();
			}
		}

		/// <summary>
		/// Is SD quality.
		/// </summary>
		public bool IsSD {
			get => m_IsSD;
			set {
				if ( !Set ( ref m_IsSD , value ) ) return;

				m_IsHD = !value;
				m_IsFullHD = !value;
				RaisePropertyChanged ( () => IsHD );
				RaisePropertyChanged ( () => IsFullHD );

				ApplicationData.Current.RoamingSettings.Values[PlayerQualitySettings] = IsHD;

				m_RestorePosition = Position;
				ChangeVideoSource ();
			}
		}

		/// <summary>
		/// Is HD quality.
		/// </summary>
		public bool IsFullHD {
			get => m_IsFullHD;
			set {
				if ( !Set ( ref m_IsFullHD , value ) ) return;

				m_IsHD = !value;
				m_IsSD = !value;
				RaisePropertyChanged ( () => IsHD );
				RaisePropertyChanged ( () => IsSD );

				m_RestorePosition = Position;
				ChangeVideoSource ();
			}
		}

		public void ChangeQuality(double value) {
			m_Is025xSpeed = value == 0.25;
			m_Is05xSpeed = value == 0.5;
			m_Is075xSpeed = value == 0.75;
			m_Is125xSpeed = value == 1.25;
			m_Is15xSpeed = value == 1.5;
			m_Is175xSpeed = value == 1.75;
			m_Is2xSpeed = value == 2;
			m_Is3xSpeed = value == 3;
			m_IsNormalSpeed = value == 1;

			RaisePropertyChanged ( () => Is025xSpeed );
			RaisePropertyChanged ( () => Is05xSpeed );
			RaisePropertyChanged ( () => Is075xSpeed );
			RaisePropertyChanged ( () => Is125xSpeed );
			RaisePropertyChanged ( () => Is15xSpeed );
			RaisePropertyChanged ( () => Is175xSpeed );
			RaisePropertyChanged ( () => Is2xSpeed );
			RaisePropertyChanged ( () => Is3xSpeed );
			RaisePropertyChanged ( () => IsNormalSpeed );

			ChangePlaybackRate ( value );
		}

		public bool Is025xSpeed {
			get => m_Is025xSpeed;
			set {
				if ( !Set ( ref m_Is025xSpeed , value ) ) return;

				ChangeQuality ( 0.25 );
			}
		}

		public bool Is05xSpeed {
			get => m_Is05xSpeed;
			set {
				if ( !Set ( ref m_Is05xSpeed , value ) ) return;

				ChangeQuality ( 0.5 );
			}
		}

		public bool Is075xSpeed {
			get => m_Is075xSpeed;
			set {
				if ( !Set ( ref m_Is075xSpeed , value ) ) return;

				ChangeQuality ( 0.75 );
			}
		}

		public bool Is125xSpeed {
			get => m_Is125xSpeed;
			set {
				if ( !Set ( ref m_Is125xSpeed , value ) ) return;

				ChangeQuality ( 1.25 );
			}
		}

		public bool Is175xSpeed {
			get => m_Is175xSpeed;
			set {
				if ( !Set ( ref m_Is175xSpeed , value ) ) return;

				ChangeQuality ( 1.75 );
			}
		}

		public bool IsNormalSpeed {
			get => m_IsNormalSpeed;
			set {
				if ( !Set ( ref m_IsNormalSpeed , value ) ) return;

				ChangeQuality ( 1 );
			}
		}

		/// <summary>
		/// Is 2x speed.
		/// </summary>
		public bool Is2xSpeed {
			get => m_Is2xSpeed;
			set {
				if ( !Set ( ref m_Is2xSpeed , value ) ) return;

				ChangeQuality ( 2 );
			}
		}

		/// <summary>
		/// Is 2x speed.
		/// </summary>
		public bool Is15xSpeed {
			get => m_Is15xSpeed;
			set {
				if ( !Set ( ref m_Is15xSpeed , value ) ) return;

				ChangeQuality ( 1.5 );
			}
		}

		/// <summary>
		/// Is 3x speed.
		/// </summary>
		public bool Is3xSpeed {
			get => m_Is3xSpeed;
			set {
				if ( !Set ( ref m_Is3xSpeed , value ) ) return;

				ChangeQuality ( 3 );
			}
		}

		/// <summary>
		/// is exists fullHD quality.
		/// </summary>
		public bool IsExistsFullHD {
			get => m_IsExistsFullHD;
			set => Set ( ref m_IsExistsFullHD , value );
		}

		/// <summary>
		/// Display volume.
		/// </summary>
		public string DisplayVolume {
			get => m_DisplayVolume;
			set => Set ( ref m_DisplayVolume , value );
		}

		/// <summary>
		/// Display position percent.
		/// </summary>
		public double PositionPercent {
			get => m_PositionPercent;
			set => Set ( ref m_PositionPercent , value );
		}

		/// <summary>
		/// Display position percent.
		/// </summary>
		public string DisplayPositionPercent {
			get => m_DisplayPositionPercent;
			set => Set ( ref m_DisplayPositionPercent , value );
		}

		/// <summary>
		/// Video source.
		/// </summary>
		public Uri VideoSource {
			get => m_VideoSource;
			set => Set ( ref m_VideoSource , value );
		}

		/// <summary>
		/// Duration.
		/// </summary>
		public double DurationSecond {
			get => m_DurationSecond;
			set => Set ( ref m_DurationSecond , value );
		}

		/// <summary>
		/// Selected video.
		/// </summary>
		public OnlineVideoModel SelectedOnlineVideo {
			get => m_SelectedOnlineVideo;
			set {
				if ( !Set ( ref m_SelectedOnlineVideo , value ) ) return;

				if ( m_NotUpdateSelectedRelese ) return;

				if ( m_SelectedOnlineVideo != null ) {
					if ( SelectedRelease != null && m_SelectedOnlineVideo.ReleaseName != SelectedRelease.Title ) SelectedRelease = Releases.First ( a => a.Title == m_SelectedOnlineVideo.ReleaseName );
					//WORKAROUND: reactive value changed only after real value changed.
					if ( !IsVideosFlyoutVisible ) IsVideosFlyoutVisible = true;
					IsExistsFullHD = m_SelectedOnlineVideo.FullHDQuality != null;
					if ( !IsExistsFullHD && IsFullHD ) {
						m_IsFullHD = false;
						m_IsHD = false;
						m_IsSD = true;
						RaisePropertyChanged ( () => IsSD );
						RaisePropertyChanged ( () => IsHD );
						RaisePropertyChanged ( () => IsFullHD );
					}
					IsVideosFlyoutVisible = false;
					ChangeVideoSource ();
					SavePlayerRestoreState ();
				}
			}
		}

		/// <summary>
		/// Change volume handler.
		/// </summary>
		public Action<double> ChangeVolumeHandler {
			get;
			set;
		}

		/// <summary>
		/// Change volume handler.
		/// </summary>
		public Action<double> ChangePlaybackRate {
			get;
			set;
		}

		/// <summary>
		/// Change playback state (pause, play or stop).
		/// </summary>
		public Action<PlaybackState , bool> ChangePlayback {
			get;
			set;
		}

		/// <summary>
		/// Show release info.
		/// </summary>
		public bool IsShowReleaseInfo {
			get => m_IsShowReleaseInfo;
			set => Set ( ref m_IsShowReleaseInfo , value );
		}

		/// <summary>
		/// Is need show release info.
		/// </summary>
		public bool IsNeedShowReleaseInfo {
			get => m_IsNeedShowReleaseInfo;
			set {
				if ( !Set ( ref m_IsNeedShowReleaseInfo , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[NeedShowReleaseInfoSettings] = value;
			}
		}

		/// <summary>
		/// Releases.
		/// </summary>
		public IEnumerable<ReleaseModel> Releases {
			get => m_Releases;
			set => Set ( ref m_Releases , value );
		}

		/// <summary>
		/// Selected release.
		/// </summary>
		public ReleaseModel SelectedRelease {
			get => m_SelectedRelease;
			set => Set ( ref m_SelectedRelease , value );
		}

		/// <summary>
		/// Online videos.
		/// </summary>
		public ObservableCollection<OnlineVideoModel> OnlineVideos {
			get => m_OnlineVideos;
			set => Set ( ref m_OnlineVideos , value );
		}

		/// <summary>
		/// Grouping online videos.
		/// </summary>
		public ObservableCollection<IGrouping<string , OnlineVideoModel>> GroupingOnlineVideos {
			get => m_GroupingOnlineVideos;
			set => Set ( ref m_GroupingOnlineVideos , value );
		}

		/// <summary>
		/// Show playlist button.
		/// </summary>
		public bool ShowPlaylistButton {
			get => m_ShowPlaylistButton;
			set {
				if ( !Set ( ref m_ShowPlaylistButton , value ) ) return;

				if ( SelectedOnlineVideo != null && !value ) ScrollToSelectedPlaylist ();
			}
		}

		/// <summary>
		/// Enable/disable auto transition beetween videos.
		/// </summary>
		public bool IsAutoTransition {
			get => m_IsAutoTransition;
			set {
				if ( !Set ( ref m_IsAutoTransition , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[AutoTransitionSettings] = value;
			}
		}

		/// <summary>
		/// Is supported compact overlay.
		/// </summary>
		public bool IsCompactOverlayEnabled {
			get => m_IsCompactOverlayEnabled;
			set {
				if ( !Set ( ref m_IsCompactOverlayEnabled , value ) ) return;

				SetVisiblePlaybackButtons ( !value );
			}
		}

		/// <summary>
		/// Is supported compact overlay.
		/// </summary>
		public bool IsSupportedCompactOverlay {
			get => m_IsSupportedCompactOverlay;
			set => Set ( ref m_IsSupportedCompactOverlay , value );
		}

		/// <summary>
		/// Control panel opacity.
		/// </summary>
		public double ControlPanelOpacity {
			get => m_ControlPanelOpacity;
			set {
				if ( !Set ( ref m_ControlPanelOpacity , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[ControlPanelOpacitySettings] = value;
			}
		}

		/// <summary>
		/// Playlist buttons positions.
		/// </summary>
		public ObservableCollection<PlaylistButtonPositionItem> PlaylistButtonPositions {
			get => m_PlaylistButtonPositions;
			set => Set ( ref m_PlaylistButtonPositions , value );
		}

		/// <summary>
		/// Selected playlist button position.
		/// </summary>
		public PlaylistButtonPositionItem SelectedPlaylistButtonPosition {
			get => m_SelectedPlaylistButtonPosition;
			set {
				var oldValue = m_SelectedPlaylistButtonPosition;
				if ( !Set ( ref m_SelectedPlaylistButtonPosition , value ) ) return;
				if ( value == null ) return;

				ChangeOpenPlaylistButton?.Invoke ();
				ApplicationData.Current.RoamingSettings.Values[PlaylistButtonPositionSettings] = (int) value.Position;
			}
		}

		/// <summary>
		/// Jump minutes.
		/// </summary>
		public int JumpMinutes {
			get => m_JumpMinutes;
			set => m_JumpMinutes = value;
		}

		/// <summary>
		/// Jump seconds.
		/// </summary>
		public int JumpSeconds {
			get => m_JumpSeconds;
			set => m_JumpSeconds = value;
		}

		/// <summary>
		/// Selected minute.
		/// </summary>
		public MinuteItem SelectedMinute {
			get => m_SelectedMinute;
			set {
				if ( !Set ( ref m_SelectedMinute , value ) ) return;

				if ( m_SelectedMinute == null ) return;

				JumpMinutes = m_SelectedMinute.Value;
				ApplicationData.Current.RoamingSettings.Values[JumpMinutesSetting] = JumpMinutes;
			}
		}

		/// <summary>
		/// Minutes.
		/// </summary>
		public ObservableCollection<MinuteItem> Minutes {
			get => m_Minutes;
			set => Set ( ref m_Minutes , value );
		}

		/// <summary>
		/// Selected minute.
		/// </summary>
		public SecondItem SelectedSecond {
			get => m_SelectedSecond;
			set {
				if ( !Set ( ref m_SelectedSecond , value ) ) return;

				if ( m_SelectedSecond == null ) return;

				JumpSeconds = m_SelectedSecond.Value;
				ApplicationData.Current.RoamingSettings.Values[JumpSecondsSetting] = JumpSeconds;
			}
		}

		/// <summary>
		/// Seconds.
		/// </summary>
		public ObservableCollection<SecondItem> Seconds {
			get => m_Seconds;
			set => Set ( ref m_Seconds , value );
		}

		/// <summary>
		/// Is xbox.
		/// </summary>
		public bool IsXbox {
			get => m_IsXbox;
			set => Set ( ref m_IsXbox , value );
		}

		/// <summary>
		/// Is cinema hall.
		/// </summary>
		public bool IsCinemaHall {
			get => m_IsCinemaHall;
			set => Set ( ref m_IsCinemaHall , value );
		}

		/// <summary>
		/// Change page handler.
		/// </summary>
		public Action<string , object> ChangePage {
			get;
			set;
		}

		/// <summary>
		/// Show sidebar.
		/// </summary>
		public Action ShowSidebar {
			get;
			set;
		}

		/// <summary>
		/// Change position.
		/// </summary>
		public Action<TimeSpan> ChangePosition {
			get;
			set;
		}

		/// <summary>
		/// Scroll to selected item in playlist.
		/// </summary>
		public Action ScrollToSelectedPlaylist {
			get;
			set;
		}

		/// <summary>
		/// Set visible playback buttons.
		/// </summary>
		public Action<bool> SetVisiblePlaybackButtons {
			get;
			set;
		}

		/// <summary>
		/// Change position for playlist button.
		/// </summary>
		public Action ChangeOpenPlaylistButton {
			get;
			set;
		}

		/// <summary>
		/// Change volume.
		/// </summary>
		public ICommand ChangeVolumeCommand {
			get;
			set;
		}

		/// <summary>
		/// Mute command.
		/// </summary>
		public ICommand MuteCommand {
			get;
			set;
		}

		/// <summary>
		/// Show sidebar command.
		/// </summary>
		public ICommand ShowSidebarCommand {
			get;
			set;
		}

		/// <summary>
		/// Toggle full screen command.
		/// </summary>
		public ICommand ToggleFullScreenCommand {
			get;
			set;
		}

		/// <summary>
		/// Show playlist command.
		/// </summary>
		public ICommand ShowPlaylistCommand {
			get;
			set;
		}

		/// <summary>
		/// Open next track in playlist command.
		/// </summary>
		public ICommand NextTrackCommand {
			get;
			set;
		}

		/// <summary>
		/// Open previous track in playlist command.
		/// </summary>
		public ICommand PreviousTrackCommand {
			get;
			set;
		}

		/// <summary>
		/// Enable compact mode command.
		/// </summary>
		public ICommand EnableCompactModeCommand {
			get;
			set;
		}

		/// <summary>
		/// Leave compact mode command.
		/// </summary>
		public ICommand LeaveCompactModeCommand {
			get;
			set;
		}

		/// <summary>
		/// Toggle seen mark command.
		/// </summary>
		public ICommand ToggleSeenMarkCommand {
			get;
			set;
		}

	}

}
