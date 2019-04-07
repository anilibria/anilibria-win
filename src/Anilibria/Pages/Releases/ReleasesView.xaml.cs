using System;
using System.Linq;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services.Implementations;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Anilibria.Pages.Releases {

	/// <summary>
	/// Releases view.
	/// </summary>
	public sealed partial class ReleasesView : UserControl {

		public ReleasesView () {
			InitializeComponent ();

			DataContext = new ReleasesViewModel ( ApiService.Current () , StorageService.Current () , SyncService.Current () , new AnalyticsService () );

			Window.Current.CoreWindow.KeyUp += GlobalKeyUpHandler;
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
			if (dataContext.SelectedSection?.Type != SectionType.NewReleases) dataContext.SelectedSection = dataContext.Sections.FirstOrDefault ( a => a.Type == SectionType.NewReleases );
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
	}

}
