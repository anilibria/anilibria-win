using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Anilibria.Pages.OnlinePlayer {

	/// <summary>
	/// Online player.
	/// </summary>
	public sealed partial class OnlinePlayerView : Page {

		private int m_TapCount = 0;

		private OnlinePlayerViewModel m_ViewModel;

		private DispatcherTimer m_DispatherTimer;

		private bool m_MediaOpened = false;

		private TimeSpan m_Duration = new TimeSpan ();

		public OnlinePlayerView () {
			InitializeComponent ();
			m_ViewModel = new OnlinePlayerViewModel {
				ChangeVolumeHandler = ChangeVolumeHandler ,
				ChangePlayback = ChangePlaybackHandler
			};
			DataContext = m_ViewModel;
			OnlinePlayer.Source = MediaSource.CreateFromUri ( new Uri ( "https://x.anilibria.tv/videos/ts/8052/0001/playlist.m3u8" ) );
			OnlinePlayer.MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
			OnlinePlayer.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;

			RunTimer ();

			Loaded += OnlinePlayerView_Loaded;
			Unloaded += OnlinePlayerView_Unloaded;
		}

		private void OnlinePlayerView_Unloaded ( object sender , RoutedEventArgs e ) {
			Unloaded -= OnlinePlayerView_Unloaded;

			StopTimer ();
		}

		private void OnlinePlayerView_Loaded ( object sender , RoutedEventArgs e ) {
			Loaded -= OnlinePlayerView_Loaded;

			//var assets = await Package.Current.InstalledLocation.GetFolderAsync ( "Assets" );
			//var testVideo = await assets.GetFileAsync ( "test.mkv" );
			//OnlinePlayer.Source = MediaSource.CreateFromStorageFile ( testVideo );

			RunTimer ();
		}

		private void RunTimer () {
			m_DispatherTimer = new DispatcherTimer ();
			m_DispatherTimer.Tick += TimerTick;
			m_DispatherTimer.Interval = new TimeSpan ( 200 );
			m_DispatherTimer.Start ();
		}

		private void TimerTick ( object sender , object e ) {
			if ( m_MediaOpened && m_Duration.TotalMilliseconds == 0 && OnlinePlayer.MediaPlayer.PlaybackSession.NaturalDuration.TotalMilliseconds > 0 ) {
				m_Duration = OnlinePlayer.MediaPlayer.PlaybackSession.NaturalDuration;
				m_ViewModel.MediaOpened ( true , m_Duration );
			}
			if ( m_MediaOpened ) {
				m_ViewModel.RefreshPosition ( OnlinePlayer.MediaPlayer.PlaybackSession.Position );
			}
		}

		private void StopTimer () {
			if ( m_DispatherTimer.IsEnabled ) m_DispatherTimer.Stop ();
		}

		private void ChangePlaybackHandler ( PlaybackState state ) {
			switch ( state ) {
				case PlaybackState.Stop:
					if ( OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing ) OnlinePlayer.MediaPlayer.Pause ();
					OnlinePlayer.MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds ( 0 );
					break;
				case PlaybackState.Pause:
					if ( OnlinePlayer.MediaPlayer.PlaybackSession.CanPause ) OnlinePlayer.MediaPlayer.Pause ();
					break;
				case PlaybackState.Play:
					OnlinePlayer.MediaPlayer.Play ();
					break;
				default: throw new NotSupportedException ( $"State {state} not supporting." );
			}
		}

		private void MediaPlayer_MediaFailed ( MediaPlayer sender , MediaPlayerFailedEventArgs args ) => m_MediaOpened = false;
		private void MediaPlayer_MediaOpened ( MediaPlayer sender , object args ) => m_MediaOpened = true;
		private void ChangeVolumeHandler ( double value ) => OnlinePlayer.MediaPlayer.Volume = value;

		private async void OnlinePlayer_Tapped ( object sender , TappedRoutedEventArgs e ) {
			m_TapCount = 1;

			await Task.Delay ( 300 );

			if ( m_TapCount > 1 ) return;

			switch ( OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState ) {
				case MediaPlaybackState.Playing:
					var storyboard = Resources["ShowPause"] as Storyboard;
					storyboard.Begin ();
					OnlinePlayer.MediaPlayer.Pause ();
					break;
				case MediaPlaybackState.Paused:
					var hideStoryboard = Resources["HidePause"] as Storyboard;
					hideStoryboard.Begin ();
					OnlinePlayer.MediaPlayer.Play ();
					break;
			}
		}

		private void OnlinePlayer_DoubleTapped ( object sender , DoubleTappedRoutedEventArgs e ) {
			m_TapCount++;

			var view = ApplicationView.GetForCurrentView ();
			if ( view.IsFullScreenMode ) {
				view.ExitFullScreenMode ();
			}
			else {
				view.TryEnterFullScreenMode ();
			}
		}

		private void StackPanel_PointerEntered ( object sender , PointerRoutedEventArgs e ) => VolumeArea.Opacity = 1;
		private void StackPanel_PointerExited ( object sender , PointerRoutedEventArgs e ) => VolumeArea.Opacity = 0;

	}

}
