using Anilibria.Services.Implementations;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.Youtube {

	/// <summary>
	/// Youtube page view.
	/// </summary>
	public sealed partial class YoutubeView : UserControl {

		public YoutubeView () {
			InitializeComponent ();

			DataContext = new YoutubeViewModel ( ApiService.Current () , new AnalyticsService () );
		}

	}

}
