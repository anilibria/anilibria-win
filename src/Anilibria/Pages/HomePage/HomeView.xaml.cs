using Anilibria.Pages.HomePage;
using Anilibria.Pages.Releases;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Collections.Generic;
using Anilibria.Pages.OnlinePlayer;
using Anilibria.Pages;
using System.Linq;
using Anilibria.Services.Implementations;
using Anilibria.Pages.AuthorizePage;
using Windows.UI.Xaml.Input;

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
			m_Pages.Add ( "Authorize" , Authorize );

			CreateViewModels ();
		}

		private void CreateViewModels () {
			var viewmodel = new HomeViewModel ( ApiService.Current () );
			viewmodel.ChangePage = ChangePage;
			DataContext = viewmodel;

			CreateReleasesViewModel ();
			CreatePlayerViewModel ();
			CreateAuthorizeViewModel ( viewmodel );
		}

		private void CreateAuthorizeViewModel ( HomeViewModel homeViewModel) {
			var viewModel = Authorize.DataContext as AuthorizeViewModel;
			viewModel.ChangePage = ChangePage;
			viewModel.ShowSidebar = ShowSidebar;
			var releases = Releases.DataContext as ReleasesViewModel;
			viewModel.RefreshOptions = homeViewModel.RefreshOptions;
			viewModel.ChangeUserSession = homeViewModel.ChangeUserSession;
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

			var viewModel = DataContext as HomeViewModel;
			viewModel.SelectedItem = viewModel.Items.First ( a => a.Page == page );

			m_PreviousPage = page;
			//hide sidebar after change page
			Sidebar.IsPaneOpen = false;
		}

		private async void Page_Loaded ( object sender , RoutedEventArgs e ) {
			var viewModel = DataContext as HomeViewModel;
			await viewModel.Initialize ();
		}

		private void Grid_Tapped ( object sender , TappedRoutedEventArgs e ) {
			var viewModel = DataContext as HomeViewModel;
			viewModel.SignoutCommand.Execute ( null );
		}
	}

}
