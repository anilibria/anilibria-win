using System;
using System.Windows.Input;
using Anilibria.MVVM;

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

		private string FormattingMinutes ( int value ) {
			if ( value == 0 ) return "";
			if ( value == 1 ) return $"{value} минута";
			if ( value >= 2 && value <= 4 ) return $"{value} минуты";

			return $"{value} минут";
		}

		private string FormattingSeconds ( int value ) {
			if ( value == 0 ) return "";
			if ( value == 1 ) return $"{value} секунда";
			if ( value >= 2 && value <= 4 ) return $"{value} секунды";

			return $"{value} секунд";
		}

		private string FormattingHours ( int value ) {
			if ( value == 0 ) return "";
			if ( value == 1 ) return $"{value} час";
			if ( value >= 2 && value <= 4 ) return $"{value} часа";

			return $"{value} часов";
		}

		private string ConvertTimeSpanToText ( TimeSpan time ) {
			if ( time.Hours > 0 ) {
				return $"{FormattingHours ( time.Hours )} {FormattingMinutes ( time.Minutes )}";
			}
			else {
				return $"{FormattingMinutes ( time.Minutes )} {FormattingSeconds ( time.Seconds )}";
			}
		}

		/// <summary>
		/// Refresh video position.
		/// </summary>
		/// <param name="timeSpan">Time span.</param>
		public void RefreshPosition ( TimeSpan timeSpan ) {
			DisplayPosition = ConvertTimeSpanToText ( timeSpan );
			DisplayPositionPercent = $"({Math.Round ( timeSpan.TotalMilliseconds / m_Duration.TotalMilliseconds * 100 )}%)";
		}

		/// <summary>
		/// Media opened.
		/// </summary>
		/// <param name="success">Success media opened.</param>
		/// <param name="duration">Duration.</param>
		public void MediaOpened ( bool success , TimeSpan? duration = default ( TimeSpan? ) ) {
			if ( success ) {
				DisplayDuration = ConvertTimeSpanToText ( duration.Value );
				m_Duration = duration.Value;
			}
		}

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void NavigateTo ( object parameter ) {
			if (VideoSource == null) {
				VideoSource = new Uri ( "https://x.anilibria.tv/videos/ts/8052/0001/playlist.m3u8" );
			} else {
				ChangePlayback ( PlaybackState.Play );
			}
		}

		/// <summary>
		/// End navigate to page.
		/// </summary>
		public void NavigateFrom () {
			ChangePlayback ( PlaybackState.Pause );
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
