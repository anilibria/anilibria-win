using Anilibria.Services.Implementations;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.CinemaHall {

	/// <summary>
	/// Cinema hall page view.
	/// </summary>
	public sealed partial class CinemaHallView : Page {

		public CinemaHallView () {
			InitializeComponent ();

			DataContext = new CinemaHallViewModel ( StorageService.Current () , new AnalyticsService () );
		}

	}

}
