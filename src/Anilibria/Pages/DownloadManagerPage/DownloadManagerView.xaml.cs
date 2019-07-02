using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.DownloadManagerPage {

	/// <summary>
	/// Download manager view.
	/// </summary>
	public sealed partial class DownloadManagerView : UserControl {

		public DownloadManagerView () {
			InitializeComponent ();
			DataContext = new DownloadManagerViewModel ();
		}

	}

}
