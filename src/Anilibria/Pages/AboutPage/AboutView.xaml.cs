using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.AboutPage {

	/// <summary>
	/// About page.
	/// </summary>
	public sealed partial class AboutView : UserControl {

		public AboutView () {
			InitializeComponent ();

			DataContext = new AboutViewModel ();
		}

	}

}
