using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services;

namespace Anilibria.Pages.OnlinePlayer {

	/// <summary>
	/// View model.
	/// </summary>
	public class OnlinePlayerViewModel : ViewModel, INavigation {

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

		private IAnalyticsService m_AnalyticsService;

		private bool m_IsHD;

		private bool m_IsSD;

		private double m_RestorePosition = 0;

		private bool m_IsMediaOpened;

		private bool m_IsBuffering;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="analyticsService">Analytics service.</param>
		public OnlinePlayerViewModel ( IAnalyticsService analyticsService ) {
			m_AnalyticsService = analyticsService;
			m_IsHD = false;

			CreateCommands ();
			Volume = .8;
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

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void NavigateTo ( object parameter ) {
			if ( parameter == null ) {
				if ( VideoSource != null ) ChangePlayback ( PlaybackState.Play , false );
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

				SetPercentDisplayVolume ( value );

				ChangeVolumeHandler?.Invoke ( value );
			}
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

				if ( m_SelectedOnlineVideo != null) ChangeVideoSource ();
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
