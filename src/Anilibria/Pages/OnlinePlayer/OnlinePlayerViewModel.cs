using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Windows.Storage;

namespace Anilibria.Pages.OnlinePlayer {

	/// <summary>
	/// View model.
	/// </summary>
	public class OnlinePlayerViewModel : ViewModel, INavigation {

		private const string PlayerQualitySettings = "PlayerQuality";

		private const string PlayerVolumeSettings = "PlayerVolume";

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

		private IEntityCollection<PlayerRestoreEntity> m_RestoreCollection;

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

		}

		private void RestoreSettings () {
			var values = ApplicationData.Current.RoamingSettings.Values;
			if ( values.ContainsKey ( PlayerQualitySettings ) ) {
				var isHD = (bool) values[PlayerQualitySettings];
				m_IsSD = !isHD;
				m_IsHD = isHD;
			}
			if ( values.ContainsKey ( PlayerVolumeSettings ) ) m_Volume = (double) values[PlayerVolumeSettings];
		}

		/// <summary>
		/// Create commands.
		/// </summary>
		private void CreateCommands () {
			ChangeVolumeCommand = CreateCommand<double> ( ChangeVolume );
			MuteCommand = CreateCommand ( Mute );
			ShowSidebarCommand = CreateCommand ( ShowSidebarFromPage );
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

			VideoSource = IsHD ? m_SelectedOnlineVideo.HDQuality : m_SelectedOnlineVideo.SDQuality;
		}

		/// <summary>
		/// Refresh video position.
		/// </summary>
		/// <param name="timeSpan">Time span.</param>
		public void RefreshPosition ( TimeSpan timeSpan ) {
			DisplayPosition = VideoTimeFormatter.ConvertTimeSpanToText ( timeSpan );

			Position = timeSpan.TotalSeconds;

			DisplayPositionPercent = $"({Math.Round ( timeSpan.TotalMilliseconds / m_Duration.TotalMilliseconds * 100 )}%)";
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

			if ( currentIndex > 0 ) SelectedOnlineVideo = SelectedRelease.OnlineVideos.ElementAt ( currentIndex - 1 );
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
			m_PlayerRestoreEntity.ReleaseId = SelectedRelease.Id;
			m_PlayerRestoreEntity.VideoId = SelectedOnlineVideo.Order;
			m_PlayerRestoreEntity.VideoPosition = Position;
			m_RestoreCollection.Update ( m_PlayerRestoreEntity );
		}

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void NavigateTo ( object parameter ) {
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
				SelectedRelease = Releases.First ();
				SelectedOnlineVideo = SelectedRelease.OnlineVideos.First ();

				ChangePlayback ( PlaybackState.Play , false );
			}

			m_AnalyticsService.TrackEvent ( "OnlinePlayer" , "Opened" , parameter == null ? "Parameter is null" : "Parameter is populated" );
		}

		/// <summary>
		/// End navigate to page.
		/// </summary>
		public void NavigateFrom () {
			if ( VideoSource != null ) ChangePlayback ( PlaybackState.Pause , false );
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

				IsSD = !value;

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

				IsHD = !value;

				ApplicationData.Current.RoamingSettings.Values[PlayerQualitySettings] = IsHD;

				m_RestorePosition = Position;
				ChangeVideoSource ();
			}
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

	}

}
