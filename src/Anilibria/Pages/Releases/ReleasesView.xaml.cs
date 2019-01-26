using Anilibria.Services.Implementations;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.Releases {

	/// <summary>
	/// Releases view.
	/// </summary>
	public sealed partial class ReleasesView : UserControl {

		public ReleasesView () {
			InitializeComponent ();

			DataContext = new ReleasesViewModel ( new AnilibriaApiService () );
		}

		private void UserControl_Loaded ( object sender , Windows.UI.Xaml.RoutedEventArgs e ) {
			var dataContext = (ReleasesViewModel) DataContext;
			dataContext.Initialize ();
		}

	}

}
