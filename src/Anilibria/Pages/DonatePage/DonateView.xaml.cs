using System;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.DonatePage {

	/// <summary>
	/// Donate page.
	/// </summary>
	public sealed partial class DonateView : UserControl {

		public DonateView () {
			InitializeComponent ();

			DataContext = new DonateViewModel ();
		}

		private async void Image_Tapped ( object sender , Windows.UI.Xaml.Input.TappedRoutedEventArgs e ) {
			await Launcher.LaunchUriAsync ( new Uri ( "https://www.youtube.com/watch?v=xGYsezJddlw" ) );
		}
	}

}
