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
using Anilibria.Pages.Youtube;
using Windows.UI.Xaml.Media.Animation;
using System;
using Anilibria.Pages.AboutPage;
using Windows.UI.Core;
using Anilibria.Pages.DonatePage;
using Anilibria.Pages.DownloadManagerPage;
using Anilibria.Pages.CinemaHall;
using Windows.System;
using Anilibria.Pages.MaintenancePage;

namespace Anilibria {

	/// <summary>
	/// Home page view.
	/// </summary>
	public sealed partial class HomeView : Page {

		private string m_PreviousPage = null;

		private Dictionary<string , FrameworkElement> m_Pages = new Dictionary<string , FrameworkElement> ();

		private DispatcherTimer m_HideMessageTimer = new DispatcherTimer ();

		private HomeViewModel m_ViewModel;

		private SystemNavigationManager m_NavigationManager = SystemNavigationManager.GetForCurrentView ();

		public HomeView () {
			InitializeComponent ();

			m_Pages.Add ( "Releases" , Releases );
			m_Pages.Add ( "Player" , Player );
			m_Pages.Add ( "Authorize" , Authorize );
			m_Pages.Add ( "Youtube" , Youtube );
			m_Pages.Add ( "About" , About );
			m_Pages.Add ( "Donate" , Donate );
			m_Pages.Add ( "DownloadManager" , DownloadManager );
			m_Pages.Add ( "CinemaHall" , CinemaHall );
			m_Pages.Add ( "Maintenance" , Maintenance );

			CreateViewModels ();

			m_HideMessageTimer.Tick += HideMessageTimer_Tick;
			m_HideMessageTimer.Interval = TimeSpan.FromSeconds ( 4 );

			( Resources["HideMessage"] as Storyboard ).Completed += HideMessageAnimationCompleted;

			m_NavigationManager.BackRequested += CurrentView_BackRequested;
		}

		private void HideMessageAnimationCompleted ( object sender , object e ) {
			m_ViewModel.ShowedMessage = false;
		}

		private void HideMessageTimer_Tick ( object sender , object e ) {
			RunHidePauseAnimation ();
			m_HideMessageTimer.Stop ();
		}

		private void CreateViewModels () {
			var viewmodel = new HomeViewModel ( ApiService.Current () );
			viewmodel.ChangePage = ChangePage;
			viewmodel.StartShowMessageAnimation = RunShowPauseAnimation;
			m_ViewModel = viewmodel;
			DataContext = viewmodel;

			CreateReleasesViewModel ( viewmodel );
			CreatePlayerViewModel ();
			CreateAuthorizeViewModel ( viewmodel );
			CreateYoutubeViewModel ();
			CreateAboutViewModel ();
			CreateDonateViewModel ();
			CreateDownloadManagerViewModel ();
			CreateCinemaHallViewModel ();
			CreateMaintenanceViewModel ();
		}

		private void CreateMaintenanceViewModel () {
			var viewModel = Maintenance.DataContext as MaintenancePageViewModel;
			viewModel.ShowSidebar = ShowSidebar;
		}

		private void CreateCinemaHallViewModel () {
			var viewModel = CinemaHall.DataContext as CinemaHallViewModel;
			viewModel.ShowSidebar = ShowSidebar;
			viewModel.ChangePage = ChangePage;
		}

		private void CreateDownloadManagerViewModel () {
			var viewModel = DownloadManager.DataContext as DownloadManagerViewModel;
			viewModel.ShowSidebar = ShowSidebar;
			viewModel.ChangePage = ChangePage;
		}

		private void CreateYoutubeViewModel () {
			var viewModel = Youtube.DataContext as YoutubeViewModel;
			viewModel.ShowSidebar = ShowSidebar;
		}

		private void CreateAuthorizeViewModel ( HomeViewModel homeViewModel ) {
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

		private void CreateAboutViewModel () {
			var viewModel = About.DataContext as AboutViewModel;
			viewModel.ShowSidebar = ShowSidebar;
		}

		private void CreateDonateViewModel () {
			var viewModel = Donate.DataContext as DonateViewModel;
			viewModel.ShowSidebar = ShowSidebar;
		}

		private void CreateReleasesViewModel ( HomeViewModel homeViewModel ) {
			var releasesViewModel = Releases.DataContext as ReleasesViewModel;
			releasesViewModel.ShowSidebar = ShowSidebar;
			releasesViewModel.ChangePage = ChangePage;
			releasesViewModel.Signout = homeViewModel.Signout;

			homeViewModel.RefreshFavorites = releasesViewModel.SynchronizeFavorites;
		}

		private void ShowSidebar () {
			Sidebar.IsPaneOpen = true;
		}

		private void CurrentView_BackRequested ( object sender , BackRequestedEventArgs e ) {
			e.Handled = true;
			m_NavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
			ChangePage ( HomeViewModel.ReleasesPage , null );
		}

		private void RefreshStateBackButton () {
			var viewModel = DataContext as HomeViewModel;
			m_NavigationManager.AppViewBackButtonVisibility = viewModel.SelectedItem?.Page != HomeViewModel.ReleasesPage ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
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
			var pageItem = viewModel.Items.FirstOrDefault ( a => a.Page == page );
			viewModel.ChangeSelectedItem ( pageItem );

			m_PreviousPage = page;
			//hide sidebar after change page
			Sidebar.IsPaneOpen = false;

			RefreshStateBackButton ();
		}

		private async void Page_Loaded ( object sender , RoutedEventArgs e ) {
			var viewModel = DataContext as HomeViewModel;
			await viewModel.Initialize ();
		}

		private void Grid_Tapped ( object sender , TappedRoutedEventArgs e ) {
			var viewModel = DataContext as HomeViewModel;
			viewModel.SignoutCommand.Execute ( null );
		}

		private void RunHidePauseAnimation () {
			var hideStoryboard = Resources["HideMessage"] as Storyboard;
			hideStoryboard.Begin ();
		}

		private void RunShowPauseAnimation () {
			var storyboard = Resources["ShowMessage"] as Storyboard;
			storyboard.Begin ();
			if ( m_HideMessageTimer.IsEnabled ) m_HideMessageTimer.Stop ();
			m_HideMessageTimer.Start ();
		}

		private void StackPanel_Tapped ( object sender , TappedRoutedEventArgs e ) {
			ChangePage ( "About" , null );
		}

		/// <summary>
		/// Set api url.
		/// </summary>
		/// <param name="url">Url.</param>|
		public void SetApiPath ( string url ) {
			ChangeApi.SetApiAddress ( url );
			ChangeApi.Visibility = Visibility.Visible;
		}

		private async void OpenPrivacyPolicy ( object sender , TappedRoutedEventArgs e ) {
			await Launcher.LaunchUriAsync ( new Uri ( "https://github.com/anilibria/anilibria-win/blob/master/privacypolicy.md" ) );
		}

		private void PrivacyPolicySection_PointerEntered ( object sender , PointerRoutedEventArgs e ) {
			Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Hand , 1 );
		}

		private void PrivacyPolicySection_PointerExited ( object sender , PointerRoutedEventArgs e ) {
			Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Arrow , 1 );
		}
	}

}
