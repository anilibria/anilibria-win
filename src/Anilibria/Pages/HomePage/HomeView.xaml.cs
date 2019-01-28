using System;
using Anilibria.Pages.HomePage;
using Anilibria.Pages.Releases;
using Windows.UI.Xaml.Controls;

namespace Anilibria {
	
	/// <summary>
	/// Home page view.
	/// </summary>
	public sealed partial class HomeView : Page {

		public HomeView () {
			InitializeComponent ();

			var viewmodel = new HomeViewModel ();
			DataContext = viewmodel;
			var releasesViewModel = Releases.DataContext as ReleasesViewModel;
			releasesViewModel.ShowSidebar = ShowSidebar;
		}

		private void ShowSidebar () {
			Sidebar.IsPaneOpen = true;
		}

	}

}
