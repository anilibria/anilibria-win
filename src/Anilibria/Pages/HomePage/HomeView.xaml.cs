using Anilibria.Pages.HomePage;
using Windows.UI.Xaml.Controls;

namespace Anilibria {
	
	/// <summary>
	/// Home page view.
	/// </summary>
	public sealed partial class HomeView : Page {

		public HomeView () {
			InitializeComponent ();

			DataContext = new HomeViewModel ();
		}

	}

}
