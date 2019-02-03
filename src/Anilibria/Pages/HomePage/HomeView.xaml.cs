using Anilibria.Pages.HomePage;
using Anilibria.Pages.Releases;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Collections.Generic;
using Anilibria.Pages.OnlinePlayer;
using Anilibria.Pages;

namespace Anilibria {

	/// <summary>
	/// Home page view.
	/// </summary>
	public sealed partial class HomeView : Page {

		private string m_PreviousPage = null;

		private Dictionary<string , FrameworkElement> m_Pages = new Dictionary<string , FrameworkElement> ();

		public HomeView () {
			InitializeComponent ();

			m_Pages.Add ( "Releases" , Releases );
			m_Pages.Add ( "Player" , Player );

			CreateViewModels ();
		}

		private void CreateViewModels () {
			var viewmodel = new HomeViewModel ();
			viewmodel.ChangePage = ChangePage;
			DataContext = viewmodel;
			viewmodel.Initialize ();

			CreateReleasesViewModel ();
			CreatePlayerViewModel ();
		}

		private void CreatePlayerViewModel () {
			var viewModel = Player.DataContext as OnlinePlayerViewModel;
			viewModel.ShowSidebar = ShowSidebar;
			viewModel.ChangePage = ChangePage;
		}

		private void CreateReleasesViewModel () {
			var releasesViewModel = Releases.DataContext as ReleasesViewModel;
			releasesViewModel.ShowSidebar = ShowSidebar;
			releasesViewModel.ChangePage = ChangePage;
		}

		private void ShowSidebar () {
			Sidebar.IsPaneOpen = true;
		}

		/// <summary>
		/// Change page.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <param name="parameter">Parameter.</param>
		public void ChangePage ( string page , object parameter ) {
			if ( !string.IsNullOrEmpty ( m_PreviousPage ) ) {
				var previousPage = m_Pages[m_PreviousPage];
				previousPage.Visibility = Visibility.Collapsed;
				var previousNavigation = previousPage.DataContext as INavigation;
				if ( previousNavigation != null ) previousNavigation.NavigateFrom ();
			}

			var currentPage = m_Pages[page];
			currentPage.Visibility = Visibility.Visible;
			var currentNavigation = currentPage.DataContext as INavigation;
			if ( currentNavigation != null ) currentNavigation.NavigateTo ( parameter );

			m_PreviousPage = page;
			//hide sidebar after change page
			Sidebar.IsPaneOpen = false;
		}

	}

}
