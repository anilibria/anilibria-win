using Anilibria.Services.Implementations;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.AuthorizePage {

	/// <summary>
	/// Page contains form for authorize user.
	/// </summary>
	public sealed partial class AuthorizeView : UserControl {

		public AuthorizeView () {
			InitializeComponent ();

			DataContext = new AuthorizeViewModel ( ApiService.Current () , new AnalyticsService () );
		}

	}

}
