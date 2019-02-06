using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.Releases.PresentationClasses;

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

		/// <summary>
		/// Constructor injection.
		/// </summary>
		public OnlinePlayerViewModel () {

			CreateCommands ();
			Volume = 1;
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

		/// <summary>
		/// Set display volume.
		/// </summary>
		/// <param name="value">Value.</param>
		private void SetPercentDisplayVolume ( double value ) => DisplayVolume = ( (int) ( value * 100 ) ).ToString () + "%";

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
		/// Media opened.
		/// </summary>
		/// <param name="success">Success media opened.</param>
		/// <param name="duration">Duration.</param>
		public void MediaOpened ( bool success , TimeSpan? duration = default ( TimeSpan? ) ) {
			if ( success ) {
				DisplayDuration = VideoTimeFormatter.ConvertTimeSpanToText ( duration.Value );
				m_Duration = duration.Value;
				DurationSecond = duration.Value.TotalSeconds;
			}
		}

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void NavigateTo ( object parameter ) {
			if ( parameter == null ) {
				if ( VideoSource != null ) ChangePlayback ( PlaybackState.Play );
			}
			else {
				Releases = parameter as IEnumerable<ReleaseModel>;
				SelectedRelease = Releases.First ();
				SelectedOnlineVideo = SelectedRelease.OnlineVideos.First ();
			}

		}

		/// <summary>
		/// End navigate to page.
		/// </summary>
		public void NavigateFrom () {
			if ( VideoSource != null ) ChangePlayback ( PlaybackState.Pause );
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

				VideoSource = m_SelectedOnlineVideo.HDQuality != null ? m_SelectedOnlineVideo.HDQuality : m_SelectedOnlineVideo.SDQuality;
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
		public Action<PlaybackState> ChangePlayback
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
