using System;
using System.Threading.Tasks;
using Anilibria.Services.Implementations;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Anilibria.Pages.CinemaHall {

	/// <summary>
	/// Cinema hall page view.
	/// </summary>
	public sealed partial class CinemaHallView : Page {

		public CinemaHallView () {
			InitializeComponent ();

			DataContext = new CinemaHallViewModel ( ApiService.Current () , StorageService.Current () , new AnalyticsService (), ReleaseSingletonService.Current() );
		}

		private async void HelpButton_Click ( object sender , RoutedEventArgs e ) => await ShowCinemaHallPdf ();

		private static async Task ShowCinemaHallPdf () {
			var location = Package.Current.InstalledLocation;
			var folder = await location.GetFolderAsync ( "Assets" );
			var file = await folder.GetFileAsync ( "howtousecinemahall.pdf" );

			await Launcher.LaunchFileAsync ( file );
		}

		private async void Hyperlink_Click ( Hyperlink sender , HyperlinkClickEventArgs args ) => await ShowCinemaHallPdf ();

	}

}
