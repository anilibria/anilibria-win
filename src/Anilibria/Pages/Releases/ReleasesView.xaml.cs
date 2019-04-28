using System;
using System.Linq;
using System.Net;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services.Implementations;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Anilibria.Pages.Releases {

	/// <summary>
	/// Releases view.
	/// </summary>
	public sealed partial class ReleasesView : UserControl {

		private Uri m_CurrentUri;

		private ReleasesViewModel m_ViewModel;

		public ReleasesView () {
			InitializeComponent ();

			m_ViewModel = new ReleasesViewModel ( ApiService.Current () , StorageService.Current () , SyncService.Current () , new AnalyticsService () );
			DataContext = m_ViewModel;

			Window.Current.CoreWindow.KeyUp += GlobalKeyUpHandler;

			m_ViewModel.SetCommentsUrl = SetCommentsUrl;
		}

		private void GlobalKeyUpHandler ( CoreWindow sender , KeyEventArgs args ) {
			if ( Visibility != Visibility.Visible ) return;

			if ( args.VirtualKey == VirtualKey.Escape ) Rectangle_Tapped ( null , null );
		}

		private void UserControl_Loaded ( object sender , RoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			dataContext.Initialize ();
		}

		private void Rectangle_Tapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			dataContext.HideReleaseCardCommand.Execute ( null );
		}

		private void PointingGridControl_Tapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			dataContext.AddCardFavoriteCommand.Execute ( null );

		}

		private void RemoveFavoriteButton_Tapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			dataContext.RemoveCardFavoriteCommand.Execute ( null );
		}

		private void ShowComments_Tapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			dataContext.ShowCommentsCommand.Execute ( null );
		}

		private void NewReleasesNotificationTapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			if ( dataContext.SelectedSection?.Type != SectionType.NewReleases ) dataContext.SelectedSection = dataContext.Sections.FirstOrDefault ( a => a.Type == SectionType.NewReleases );
		}

		private void NewOnlineSeriesNotificationTapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			if ( dataContext.SelectedSection?.Type != SectionType.NewOnlineSeries ) dataContext.SelectedSection = dataContext.Sections.FirstOrDefault ( a => a.Type == SectionType.NewOnlineSeries );
		}

		private void NewTorrentsNotificationTapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			if ( dataContext.SelectedSection?.Type != SectionType.NewTorrentSeries ) dataContext.SelectedSection = dataContext.Sections.FirstOrDefault ( a => a.Type == SectionType.NewTorrentSeries );
		}

		private void SingoutTapped ( object sender , TappedRoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			dataContext.Signout ();
		}

		private async void WebView_NewWindowRequested ( WebView sender , WebViewNewWindowRequestedEventArgs args ) {
			args.Handled = true;

			var uri = args.Uri.ToString ();

			if ( !uri.StartsWith ( "https://oauth.vk.com/authorize" ) && !uri.StartsWith ( "https://vk.com/widget_comments.php" ) && !uri.StartsWith ( "https://vk.com/id" ) ) {
				await Launcher.LaunchUriAsync ( args.Uri );
				return;
			}

			if ( uri.StartsWith ( "https://oauth.vk.com/authorize" ) && m_CurrentUri != null ) {
				uri = uri.Replace ( "close.html" , WebUtility.UrlEncode ( m_CurrentUri.ToString () ) );
			}

			if ( uri.StartsWith ( "https://vk.com/id" ) ) {
				uri = uri.Replace ( "vk.com" , "m.vk.com" );
			}

			CommentsWebView.Navigate ( new Uri ( uri ) );
		}

		private void SetCommentsUrl ( Uri newUrl ) {
			CommentsWebView.Navigate ( newUrl );
		}

		private void CommentsWebView_NavigationCompleted ( WebView sender , WebViewNavigationCompletedEventArgs args ) {
			m_CurrentUri = args.Uri;
		}

		private void Button_RightTapped ( object sender , RightTappedRoutedEventArgs e ) {
			FlyoutBase.ShowAttachedFlyout ( sender as FrameworkElement );
			e.Handled = true;
		}

		private void PrefferedOnlineVideoSelector_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
			if ( PrefferedOnlineVideoSelector.SelectedItem == null ) return;

			m_ViewModel.OpenOnlineVideoCommand.Execute ( null );
		}

		private void Button_Tapped ( object sender , TappedRoutedEventArgs e ) {
			switch ( m_ViewModel.SelectedOpenVideoMode.Mode ) {
				case OpenVideoMode.SelectOnlineVideo:
					FlyoutBase.ShowAttachedFlyout ( sender as FrameworkElement );
					break;
				case OpenVideoMode.ImmediatlyOpenVideoPlayer:
				default:
					m_ViewModel.OpenOnlineVideoCommand.Execute ( null );
					break;
			}
		}

		private void StatusLabel_RightTapped ( object sender , RightTappedRoutedEventArgs e ) {
			FlyoutBase.ShowAttachedFlyout ( sender as FrameworkElement );
			e.Handled = true;
		}

		private void GenresFiltersMenuTextBlock_RightTapped ( object sender , RightTappedRoutedEventArgs e ) {
			GenresFiltersMenu.Items.Clear ();

			var allOption = new MenuFlyoutItem ();
			allOption.Text = "Фильтровать по всем";
			allOption.Style = Application.Current.Resources["AnilibriaMenuFlyoutItem"] as Style;
			allOption.Command = m_ViewModel.AddGenreToFilterCommand;
			allOption.CommandParameter = m_ViewModel.OpenedRelease.Genres;
			GenresFiltersMenu.Items.Add ( allOption );

			var genres = m_ViewModel.OpenedRelease.Genres.Split ( ", " );
			foreach ( var genre in genres ) {
				var option = new MenuFlyoutItem ();
				option.Text = "Фильтровать по " + genre;
				option.Style = Application.Current.Resources["AnilibriaMenuFlyoutItem"] as Style;
				option.Command = m_ViewModel.AddGenreToFilterCommand;
				option.CommandParameter = genre;
				GenresFiltersMenu.Items.Add ( option );
			}

			FlyoutBase.ShowAttachedFlyout ( sender as FrameworkElement );
			e.Handled = true;
		}

		private void VoicesFiltersMenuTextBlock_RightTapped ( object sender , RightTappedRoutedEventArgs e ) {
			VoicesFiltersMenu.Items.Clear ();

			var allOption = new MenuFlyoutItem ();
			allOption.Text = "Фильтровать по всем";
			allOption.Style = Application.Current.Resources["AnilibriaMenuFlyoutItem"] as Style;
			allOption.Command = m_ViewModel.AddVoicesToFilterCommand;
			allOption.CommandParameter = m_ViewModel.OpenedRelease.Voices;
			VoicesFiltersMenu.Items.Add ( allOption );

			var voices = m_ViewModel.OpenedRelease.Voices.Split ( ", " );
			foreach ( var voice in voices ) {
				var option = new MenuFlyoutItem ();
				option.Text = "Фильтровать по " + voice;
				option.Style = Application.Current.Resources["AnilibriaMenuFlyoutItem"] as Style;
				option.Command = m_ViewModel.AddVoicesToFilterCommand;
				option.CommandParameter = voice;
				VoicesFiltersMenu.Items.Add ( option );
			}

			FlyoutBase.ShowAttachedFlyout ( sender as FrameworkElement );
			e.Handled = true;
		}

	}

}
