using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.ProtocolsPages {

	/// <summary>
	/// Change api confirm page.
	/// </summary>
	public sealed partial class ChangeApiView : Page {

		public ChangeApiView () {
			InitializeComponent ();

			MainControl.SetNeedCloseApplicationOnCancel ( true );
		}

	}

}
