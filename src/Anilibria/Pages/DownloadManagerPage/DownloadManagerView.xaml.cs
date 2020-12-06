using Anilibria.Services.Implementations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.DownloadManagerPage {

	/// <summary>
	/// Download manager view.
	/// </summary>
	public sealed partial class DownloadManagerView : UserControl {

		private readonly DownloadManagerViewModel m_ViewModel;

		public DownloadManagerView () {
			InitializeComponent ();
			
			m_ViewModel = new DownloadManagerViewModel ( DownloadManager.Current (), StorageService.Current () , new AnalyticsService(), ReleaseSingletonService.Current() );
			DataContext = m_ViewModel;
		}

		private async void UserControl_Loaded ( object sender , RoutedEventArgs e ) => await m_ViewModel.Initialize ();

	}

}
