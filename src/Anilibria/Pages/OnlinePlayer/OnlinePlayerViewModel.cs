using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
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

		private IEnumerable<ReleaseModel> m_Releases;

		private OnlineVideoModel m_SelectedOnlineVideo;

		private double m_DurationSecond;

		private double m_Position;

		private readonly IAnalyticsService m_AnalyticsService;

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IDataContext m_DataContext;

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

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="analyticsService">Analytics service.</param>
		/// <param name="dataContext">Data context.</param>
		/// <param name="anilibriaApiService">Anilibria restful service.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public OnlinePlayerViewModel ( IAnalyticsService analyticsService , IDataContext dataContext , IAnilibriaApiService anilibriaApiService ) {
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
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
					VideoPosition = 0
				};
				m_RestoreCollection.Add ( m_PlayerRestoreEntity );
			}

			m_ReleaseStateCollection = m_DataContext.GetCollection<ReleaseVideoStateEntity> ();
			m_IsSupportedCompactOverlay = ApplicationView.GetForCurrentView ().IsViewModeSupported ( ApplicationViewMode.CompactOverlay );
		}

		private void RestoreSettings () {
			var values = ApplicationData.Current.RoamingSettings.Values;
			if ( values.ContainsKey ( PlayerQualitySettings ) ) {
				var isHD = (bool) values[PlayerQualitySettings];
				m_IsSD = !isHD;
				m_IsHD = isHD;
			}
			if ( values.ContainsKey ( PlayerVolumeSettings ) ) m_Volume = (double) values[PlayerVolumeSettings];
			if ( values.ContainsKey ( AutoTransitionSettings ) ) m_IsAutoTransition = (bool) values[AutoTransitionSettings];
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
		}

		private async void LeaveCompactMode () {
			await ApplicationView.GetForCurrentView ().TryEnterViewModeAsync ( ApplicationViewMode.Default );
			IsCompactOverlayEnabled = false;
		}

		private async void EnableCompactMode () {
			var enabled = await ApplicationView.GetForCurrentView ().TryEnterViewModeAsync ( ApplicationViewMode.CompactOverlay );
			IsCompactOverlayEnabled = enabled;
		}

		private void PreviousTrack () {
			if ( !( SelectedRelease != null && SelectedRelease.OnlineVideos != null && SelectedRelease.OnlineVideos.Any () ) ) return;
			if ( SelectedOnlineVideo == null ) return;
			if ( SelectedOnlineVideo.Order == 1 ) return;

			var previousTrack = SelectedRelease.OnlineVideos.FirstOrDefault ( a => a.Order == SelectedOnlineVideo.Order - 1 );
			if ( previousTrack != null ) SelectedOnlineVideo = previousTrack;
		}

		private void NextTrack () {
			if ( !( SelectedRelease != null && SelectedRelease.OnlineVideos != null && SelectedRelease.OnlineVideos.Any () ) ) return;
			if ( SelectedOnlineVideo == null ) return;
			if ( SelectedOnlineVideo.Order == SelectedRelease.OnlineVideos.Count () ) return;

			var nextTrack = SelectedRelease.OnlineVideos.FirstOrDefault ( a => a.Order == SelectedOnlineVideo.Order + 1 );
			if ( nextTrack != null ) SelectedOnlineVideo = nextTrack;
		}

		private void ShowPlaylist () {
			ShowPlaylistButton = false;
		}

		private void ToggleFullScreen () {
			var view = ApplicationView.GetForCurrentView ();
			view.FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
			if ( view.IsFullScreenMode ) {
				view.ExitFullScreenMode ();
			}
			else {
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
			}
			else {
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
			var currentIndex = SelectedRelease.OnlineVideos.ToList ().IndexOf ( SelectedOnlineVideo );

			if ( currentIndex > 0 && m_IsAutoTransition ) SelectedOnlineVideo = SelectedRelease.OnlineVideos.ElementAt ( currentIndex - 1 );
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
							Title = a.Title
						}
					)
					.ToList ()
			};
		}

		/// <summary>
		/// Save player restore state.
		/// </summary>
		public void SavePlayerRestoreState () {
			if ( SelectedOnlineVideo == null ) return;

			var isNotNeedUpdatePosition = m_PlayerRestoreEntity?.ReleaseId == SelectedRelease?.Id && m_PlayerRestoreEntity?.VideoPosition > 0 && Position == 0;
			m_PlayerRestoreEntity.ReleaseId = SelectedRelease.Id;
			m_PlayerRestoreEntity.VideoId = SelectedOnlineVideo.Order;
			if ( !isNotNeedUpdatePosition ) m_PlayerRestoreEntity.VideoPosition = Position;
			m_RestoreCollection.Update ( m_PlayerRestoreEntity );

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

			var videoState = m_ReleaseVideoStateEntity.VideoStates.FirstOrDefault ( a => a.Id == SelectedOnlineVideo.Order );
			if ( videoState == null ) {
				m_ReleaseVideoStateEntity.VideoStates.Add (
					new VideoStateEntity {
						Id = SelectedOnlineVideo.Order ,
						LastPosition = Position
					}
				);
			}
			else {
				videoState.LastPosition = Position == 0 && videoState.LastPosition > 0 ? videoState.LastPosition : Position;

				if ( !videoState.IsSeen && PositionPercent >= 90 && PositionPercent <= 100 ) videoState.IsSeen = true;
			}

			m_ReleaseStateCollection.Update ( m_ReleaseVideoStateEntity );
		}

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void NavigateTo ( object parameter ) {
			try {
				if ( m_DisplayRequest == null ) m_DisplayRequest = new DisplayRequest ();
				m_DisplayRequest.RequestActive ();
			}
			catch {
			}

			UpdateVolumeState ( m_Volume );

			if ( parameter == null ) {
				if ( VideoSource != null ) {
					ChangePlayback ( PlaybackState.Play , false );
				}
				else {
					if ( m_PlayerRestoreEntity != null && m_PlayerRestoreEntity.ReleaseId > 0 ) {
						var release = m_DataContext.GetCollection<ReleaseEntity> ().FirstOrDefault ( a => a.Id == m_PlayerRestoreEntity.ReleaseId );
						if ( release != null ) {
							m_RestorePosition = m_PlayerRestoreEntity.VideoPosition;
							SelectedRelease = MapToReleaseModel ( release );
							SelectedOnlineVideo = SelectedRelease.OnlineVideos?.FirstOrDefault ( a => a.Order == m_PlayerRestoreEntity.VideoId );

							ChangePlayback ( PlaybackState.Play , false );
						}
					}
				}
			}
			else {
				Releases = parameter as IEnumerable<ReleaseModel>;
				var release = Releases.First ();
				m_ReleaseVideoStateEntity = m_ReleaseStateCollection?.FirstOrDefault ( a => a.ReleaseId == release.Id );
				int onlineVideoIndex = release.PrefferedOpenedVideo == null ? -1 : release.PrefferedOpenedVideo.Order;
				if ( onlineVideoIndex == -1 && m_ReleaseVideoStateEntity != null && m_ReleaseVideoStateEntity.VideoStates != null && m_ReleaseVideoStateEntity.VideoStates.Any () ) {
					onlineVideoIndex = m_ReleaseVideoStateEntity.VideoStates.Max ( a => a.Id );
					var lastVideo = m_ReleaseVideoStateEntity.VideoStates.First ( a => a.Id == onlineVideoIndex );
					m_RestorePosition = lastVideo.LastPosition;
				}

				SelectedRelease = release;
				SelectedOnlineVideo = onlineVideoIndex == -1 ? SelectedRelease?.OnlineVideos?.LastOrDefault () : SelectedRelease?.OnlineVideos?.FirstOrDefault ( a => a.Order == onlineVideoIndex );

				if ( SelectedOnlineVideo != null ) ChangePlayback ( PlaybackState.Play , false );

				if ( release != null ) release.PrefferedOpenedVideo = null;
			}

			ShowPlaylistButton = true;

			m_AnalyticsService.TrackEvent ( "OnlinePlayer" , "Opened" , parameter == null ? "Parameter is null" : "Parameter is populated" );

			if ( SelectedOnlineVideo != null ) ScrollToSelectedPlaylist ();
		}

		/// <summary>
		/// End navigate to page.
		/// </summary>
		public void NavigateFrom () {
			try {
				m_DisplayRequest.RequestRelease ();
			}
			catch {
			}
			ShowPlaylistButton = true;

			if ( VideoSource != null ) ChangePlayback ( PlaybackState.Pause , false );

			var view = ApplicationView.GetForCurrentView ();
			if ( view.IsFullScreenMode ) view.ExitFullScreenMode ();
		}

		/// <summary>
		/// Is videos flyout visible.
		/// </summary>
		public bool IsVideosFlyoutVisible
		{
			get => m_IsVideosFlyoutVisible;
			set => Set ( ref m_IsVideosFlyoutVisible , value );
		}

		/// <summary>
		/// Media opened.
		/// </summary>
		public bool IsMediaOpened
		{
			get => m_IsMediaOpened;
			set => Set ( ref m_IsMediaOpened , value );
		}

		/// <summary>
		/// Volume.
		/// </summary>
		public double Volume
		{
			get => m_Volume;
			set
			{
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
		public bool IsBuffering
		{
			get => m_IsBuffering;
			set => Set ( ref m_IsBuffering , value );
		}

		/// <summary>
		/// Display duration.
		/// </summary>
		public string DisplayDuration
		{
			get => m_DisplayDuration;
			set => Set ( ref m_DisplayDuration , value );
		}

		/// <summary>
		/// Display position.
		/// </summary>
		public string DisplayPosition
		{
			get => m_DisplayPosition;
			set => Set ( ref m_DisplayPosition , value );
		}

		/// <summary>
		/// Position.
		/// </summary>
		public double Position
		{
			get => m_Position;
			set => Set ( ref m_Position , value );
		}

		/// <summary>
		/// Is muted.
		/// </summary>
		public bool IsMuted
		{
			get => m_IsMuted;
			set => Set ( ref m_IsMuted , value );
		}

		/// <summary>
		/// Is HD quality.
		/// </summary>
		public bool IsHD
		{
			get => m_IsHD;
			set
			{
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
		public bool IsSD
		{
			get => m_IsSD;
			set
			{
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
		public bool IsFullHD
		{
			get => m_IsFullHD;
			set
			{
				if ( !Set ( ref m_IsFullHD , value ) ) return;

				m_IsHD = !value;
				m_IsSD = !value;
				RaisePropertyChanged ( () => IsHD );
				RaisePropertyChanged ( () => IsSD );

				m_RestorePosition = Position;
				ChangeVideoSource ();
			}
		}

		/// <summary>
		/// is exists fullHD quality.
		/// </summary>
		public bool IsExistsFullHD
		{
			get => m_IsExistsFullHD;
			set => Set ( ref m_IsExistsFullHD , value );
		}

		/// <summary>
		/// Display volume.
		/// </summary>
		public string DisplayVolume
		{
			get => m_DisplayVolume;
			set => Set ( ref m_DisplayVolume , value );
		}

		/// <summary>
		/// Display position percent.
		/// </summary>
		public double PositionPercent
		{
			get => m_PositionPercent;
			set => Set ( ref m_PositionPercent , value );
		}

		/// <summary>
		/// Display position percent.
		/// </summary>
		public string DisplayPositionPercent
		{
			get => m_DisplayPositionPercent;
			set => Set ( ref m_DisplayPositionPercent , value );
		}

		/// <summary>
		/// Video source.
		/// </summary>
		public Uri VideoSource
		{
			get => m_VideoSource;
			set => Set ( ref m_VideoSource , value );
		}

		/// <summary>
		/// Duration.
		/// </summary>
		public double DurationSecond
		{
			get => m_DurationSecond;
			set => Set ( ref m_DurationSecond , value );
		}

		/// <summary>
		/// Selected video.
		/// </summary>
		public OnlineVideoModel SelectedOnlineVideo
		{
			get => m_SelectedOnlineVideo;
			set
			{
				if ( !Set ( ref m_SelectedOnlineVideo , value ) ) return;

				if ( m_SelectedOnlineVideo != null ) {
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
		public Action<double> ChangeVolumeHandler
		{
			get;
			set;
		}

		/// <summary>
		/// Change playback state (pause, play or stop).
		/// </summary>
		public Action<PlaybackState , bool> ChangePlayback
		{
			get;
			set;
		}

		/// <summary>
		/// Show release info.
		/// </summary>
		public bool IsShowReleaseInfo
		{
			get => m_IsShowReleaseInfo;
			set => Set ( ref m_IsShowReleaseInfo , value );
		}

		/// <summary>
		/// Releases.
		/// </summary>
		public IEnumerable<ReleaseModel> Releases
		{
			get => m_Releases;
			set => Set ( ref m_Releases , value );
		}

		/// <summary>
		/// Selected release.
		/// </summary>
		public ReleaseModel SelectedRelease
		{
			get => m_SelectedRelease;
			set => Set ( ref m_SelectedRelease , value );
		}

		/// <summary>
		/// Show playlist button.
		/// </summary>
		public bool ShowPlaylistButton
		{
			get => m_ShowPlaylistButton;
			set => Set ( ref m_ShowPlaylistButton , value );
		}

		/// <summary>
		/// Enable/disable auto transition beetween videos.
		/// </summary>
		public bool IsAutoTransition
		{
			get => m_IsAutoTransition;
			set
			{
				if ( !Set ( ref m_IsAutoTransition , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[AutoTransitionSettings] = value;
			}
		}

		/// <summary>
		/// Is supported compact overlay.
		/// </summary>
		public bool IsCompactOverlayEnabled
		{
			get => m_IsCompactOverlayEnabled;
			set
			{
				if ( !Set ( ref m_IsCompactOverlayEnabled , value ) ) return;

				SetVisiblePlaybackButtons ( !value );
			}
		}

		/// <summary>
		/// Is supported compact overlay.
		/// </summary>
		public bool IsSupportedCompactOverlay
		{
			get => m_IsSupportedCompactOverlay;
			set => Set ( ref m_IsSupportedCompactOverlay , value );
		}

		/// <summary>
		/// Change page handler.
		/// </summary>
		public Action<string , object> ChangePage
		{
			get;
			set;
		}

		/// <summary>
		/// Show sidebar.
		/// </summary>
		public Action ShowSidebar
		{
			get;
			set;
		}

		/// <summary>
		/// Change position.
		/// </summary>
		public Action<TimeSpan> ChangePosition
		{
			get;
			set;
		}

		/// <summary>
		/// Scroll to selected item in playlist.
		/// </summary>
		public Action ScrollToSelectedPlaylist
		{
			get;
			set;
		}

		/// <summary>
		/// Set visible playback buttons.
		/// </summary>
		public Action<bool> SetVisiblePlaybackButtons
		{
			get;
			set;
		}

		/// <summary>
		/// Change volume.
		/// </summary>
		public ICommand ChangeVolumeCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Mute command.
		/// </summary>
		public ICommand MuteCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Show sidebar command.
		/// </summary>
		public ICommand ShowSidebarCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Toggle full screen command.
		/// </summary>
		public ICommand ToggleFullScreenCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Show playlist command.
		/// </summary>
		public ICommand ShowPlaylistCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Open next track in playlist command.
		/// </summary>
		public ICommand NextTrackCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Open previous track in playlist command.
		/// </summary>
		public ICommand PreviousTrackCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Enable compact mode command.
		/// </summary>
		public ICommand EnableCompactModeCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Leave compact mode command.
		/// </summary>
		public ICommand LeaveCompactModeCommand
		{
			get;
			set;
		}

	}

}
