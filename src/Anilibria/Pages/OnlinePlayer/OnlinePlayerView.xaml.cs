using System;
using System.Threading.Tasks;
using Anilibria.Services.Implementations;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Media.Casting;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

		private bool m_BlockedTrackSlider = false;

		private double m_MouseX = 0;

		private double m_MouseY = 0;

		private double m_PreviousX = 0;

		private double m_PreviousY = 0;

		private int m_LastActivityTime = 0;

		CastingDevicePicker castingPicker;

		public OnlinePlayerView () {
			InitializeComponent ();
			m_ViewModel = new OnlinePlayerViewModel ( new AnalyticsService () ) {
				ChangeVolumeHandler = ChangeVolumeHandler ,
				ChangePlayback = ChangePlaybackHandler ,
				ChangePosition = ChangePosition
			};
			DataContext = m_ViewModel;
			OnlinePlayer.MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
			OnlinePlayer.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
			OnlinePlayer.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
			OnlinePlayer.MediaPlayer.SourceChanged += MediaPlayer_SourceChanged;
			OnlinePlayer.MediaPlayer.BufferingStarted += MediaPlayer_BufferingStarted;
			OnlinePlayer.MediaPlayer.BufferingEnded += MediaPlayer_BufferingEnded;

			RunTimer ();

			Loaded += OnlinePlayerView_Loaded;
			Unloaded += OnlinePlayerView_Unloaded;

			castingPicker = new CastingDevicePicker ();
			castingPicker.Filter.SupportsVideo = true;
			castingPicker.Filter.SupportedCastingSources.Add ( OnlinePlayer.MediaPlayer.GetAsCastingSource () );
			castingPicker.CastingDeviceSelected += CastingPicker_CastingDeviceSelected;

			Window.Current.CoreWindow.KeyUp += GlobalKeyUpHandler;
		}

		private void GlobalKeyUpHandler ( CoreWindow sender , KeyEventArgs args ) {
			if ( Visibility != Visibility.Visible ) return;

			if ( args.VirtualKey == VirtualKey.Space ) ControlPanel.Visibility = ControlPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		}

		private async void CastingPicker_CastingDeviceSelected ( CastingDevicePicker sender , CastingDeviceSelectedEventArgs args ) {
			await Dispatcher.RunAsync (
				CoreDispatcherPriority.Normal ,
				async () => {
					var connection = args.SelectedCastingDevice.CreateCastingConnection ();

					//Hook up the casting events
					//connection.ErrorOccurred += Connection_ErrorOccurred;
					//connection.StateChanged += Connection_StateChanged;

					var videoSource = OnlinePlayer.MediaPlayer.GetAsCastingSource ();
					await connection.RequestStartCastingAsync ( videoSource );
				}
			);
		}

		private async void MediaPlayer_BufferingEnded ( MediaPlayer sender , object args ) {
			await Dispatcher.RunAsync (
				CoreDispatcherPriority.Normal ,
				() => {
					m_ViewModel.BufferingEnd ();
				}
			);
		}

		private async void MediaPlayer_BufferingStarted ( MediaPlayer sender , object args ) {
			await Dispatcher.RunAsync (
				CoreDispatcherPriority.Normal ,
				() => {
					m_ViewModel.BufferingStart ();
				}
			);
		}

		private async void MediaPlayer_MediaEnded ( MediaPlayer sender , object args ) {
			await Dispatcher.RunAsync (
				CoreDispatcherPriority.Normal ,
				() => {
					m_ViewModel.MediaEnded ();
				}
			);
		}

		private async void MediaPlayer_SourceChanged ( MediaPlayer sender , object args ) {
			m_Duration = TimeSpan.FromSeconds ( 0 );
			m_MediaOpened = false;
			await Dispatcher.RunAsync (
				CoreDispatcherPriority.Normal ,
				() => {
					m_ViewModel.MediaClosed ();
				}
			);
		}

		private void OnlinePlayerView_Unloaded ( object sender , RoutedEventArgs e ) {
			Unloaded -= OnlinePlayerView_Unloaded;

			StopTimer ();
		}

		private void OnlinePlayerView_Loaded ( object sender , RoutedEventArgs e ) {
			Loaded -= OnlinePlayerView_Loaded;

			RunTimer ();
		}

		private void RunTimer () {
			m_DispatherTimer = new DispatcherTimer ();
			m_DispatherTimer.Tick += TimerTick;
			m_DispatherTimer.Interval = new TimeSpan ( 300 );
			m_DispatherTimer.Start ();
		}

		private void Grid_PointerMoved ( object sender , PointerRoutedEventArgs e ) {
			if ( e.Pointer.PointerDeviceType == PointerDeviceType.Mouse ) {
				PointerPoint ptrPt = e.GetCurrentPoint ( this );
				m_MouseX = ptrPt.Position.X;
				m_MouseY = ptrPt.Position.Y;
			}
		}

		private void MouseHidingTracker () {
			if ( OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing && ControlPanel.Visibility == Visibility.Collapsed ) {
				m_LastActivityTime++;
				if ( !( m_PreviousX == m_MouseX && m_PreviousY == m_MouseY ) ) {
					Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Arrow , 0 );
					m_LastActivityTime = 0;
					m_PreviousX = m_MouseX;
					m_PreviousY = m_MouseY;
				}

				if ( m_LastActivityTime == 100 ) {
					m_LastActivityTime = 0;
					Window.Current.CoreWindow.PointerCursor = null;
				}
			}
		}

		private void TimerTick ( object sender , object e ) {
			if ( m_MediaOpened ) {
				m_ViewModel.RefreshPosition ( OnlinePlayer.MediaPlayer.PlaybackSession.Position );
				if ( !m_BlockedTrackSlider ) Slider.Value = OnlinePlayer.MediaPlayer.PlaybackSession.Position.TotalSeconds;

				MouseHidingTracker ();
			}
		}

		private void StopTimer () {
			if ( m_DispatherTimer.IsEnabled ) m_DispatherTimer.Stop ();
		}

		private void ChangePlaybackHandler ( PlaybackState state , bool needAnimation = true ) {
			switch ( state ) {
				case PlaybackState.Stop:
					if ( OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing ) OnlinePlayer.MediaPlayer.Pause ();
					OnlinePlayer.MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds ( 0 );
					break;
				case PlaybackState.Pause:
					if ( OnlinePlayer.MediaPlayer.PlaybackSession.CanPause ) {
						if ( needAnimation ) {
							RunShowPauseAnimation ();
						}
						else {
							PauseIcon.Opacity = .8;
						}
						OnlinePlayer.MediaPlayer.Pause ();
						CurrentReleaseInfo.Visibility = Visibility.Visible;
					}
					break;
				case PlaybackState.Play:
					if ( OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Paused ) {
						if ( needAnimation ) {
							RunHidePauseAnimation ();
						}
						else {
							PauseIcon.Opacity = 0;
						}
					}
					if ( OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing ) OnlinePlayer.MediaPlayer.Play ();
					CurrentReleaseInfo.Visibility = Visibility.Collapsed;
					if ( ControlPanel.Visibility == Visibility.Visible ) ControlPanel.Visibility = Visibility.Collapsed;
					break;
				default: throw new NotSupportedException ( $"State {state} not supporting." );
			}
		}

		/// <summary>
		/// Change position.
		/// </summary>
		/// <param name="position"></param>
		private void ChangePosition ( TimeSpan position ) {
			var playbackState = OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState;
			if ( playbackState == MediaPlaybackState.Playing || playbackState == MediaPlaybackState.Paused ) {
				OnlinePlayer.MediaPlayer.PlaybackSession.Position = position;
			}
		}

		private void MediaPlayer_MediaFailed ( MediaPlayer sender , MediaPlayerFailedEventArgs args ) => m_MediaOpened = false;

		private async void MediaPlayer_MediaOpened ( MediaPlayer sender , object args ) {
			m_MediaOpened = true;
			await Dispatcher.RunAsync (
				CoreDispatcherPriority.Normal ,
				() => {
					m_Duration = OnlinePlayer.MediaPlayer.PlaybackSession.NaturalDuration;
					m_ViewModel.MediaOpened ( m_Duration );
					PauseIcon.Opacity = 0;
				}
			);
		}
		private void ChangeVolumeHandler ( double value ) => OnlinePlayer.MediaPlayer.Volume = value;

		private async void OnlinePlayer_Tapped ( object sender , TappedRoutedEventArgs e ) {
			m_TapCount = 1;

			await Task.Delay ( 300 );

			if ( m_TapCount > 1 ) return;

			switch ( OnlinePlayer.MediaPlayer.PlaybackSession.PlaybackState ) {
				case MediaPlaybackState.Playing:
					if ( ControlPanel.Visibility == Visibility.Visible ) {
						ControlPanel.Visibility = Visibility.Collapsed;
						return;
					}
					ChangePlaybackHandler ( PlaybackState.Pause , needAnimation: true );
					if ( ControlPanel.Visibility != Visibility.Visible ) ControlPanel.Visibility = Visibility.Visible;
					break;
				case MediaPlaybackState.Paused:
					ChangePlaybackHandler ( PlaybackState.Play , needAnimation: true );
					if ( ControlPanel.Visibility == Visibility.Visible ) ControlPanel.Visibility = Visibility.Collapsed;
					break;
			}
		}

		private void RunHidePauseAnimation () {
			var hideStoryboard = Resources["HidePause"] as Storyboard;
			hideStoryboard.Begin ();
		}

		private void RunShowPauseAnimation () {
			var storyboard = Resources["ShowPause"] as Storyboard;
			storyboard.Begin ();
		}

		private void OnlinePlayer_DoubleTapped ( object sender , DoubleTappedRoutedEventArgs e ) {
			m_TapCount++;

			var view = ApplicationView.GetForCurrentView ();
			view.FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
			if ( view.IsFullScreenMode ) {
				view.ExitFullScreenMode ();
			}
			else {
				view.TryEnterFullScreenMode ();
			}
		}

		private async void Slider_ManipulationCompleted ( object sender , ManipulationCompletedRoutedEventArgs e ) {
			await Task.Delay ( 100 );
			ChangePosition ( TimeSpan.FromSeconds ( Slider.Value ) );
			m_ViewModel.RefreshPosition ( TimeSpan.FromSeconds ( Slider.Value ) );
			m_BlockedTrackSlider = false;
		}

		private void Slider_Tapped ( object sender , TappedRoutedEventArgs e ) {
			m_BlockedTrackSlider = true;

			ChangePosition ( TimeSpan.FromSeconds ( Slider.Value ) );
			m_ViewModel.RefreshPosition ( TimeSpan.FromSeconds ( Slider.Value ) );

			m_BlockedTrackSlider = false;
		}

		private void OnlinePlayer_RightTapped ( object sender , RightTappedRoutedEventArgs e ) {
			if ( ControlPanel.Visibility == Visibility.Collapsed ) {
				ControlPanel.Visibility = Visibility.Visible;
			}
			else {
				ControlPanel.Visibility = Visibility.Collapsed;
			}
		}

		private void Slider_ManipulationStarting ( object sender , ManipulationStartingRoutedEventArgs e ) {
			m_BlockedTrackSlider = true;
		}

		private void CastToDevice_Click ( object sender , RoutedEventArgs e ) {
			var transform = CastToDevice.TransformToVisual ( Window.Current.Content as UIElement );
			var pt = transform.TransformPoint ( new Point ( 0 , 0 ) );

			castingPicker.Show ( new Rect ( pt.X , pt.Y , CastToDevice.ActualWidth , CastToDevice.ActualHeight ) , Windows.UI.Popups.Placement.Above );
		}

	}

}
