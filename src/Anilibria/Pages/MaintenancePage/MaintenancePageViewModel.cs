using Anilibria.MVVM;
using Anilibria.Services;
using System;
using System.Windows.Input;

namespace Anilibria.Pages.MaintenancePage
{
	public class MaintenancePageViewModel : ViewModel, INavigation
	{
		private readonly IAnalyticsService m_AnalyticsService;

		public MaintenancePageViewModel ( IAnalyticsService analyticsService ) {
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );

			CreateCommands ();
		}

		private void CreateCommands () {
			ClearPosterCacheCommand = CreateCommand ( ClearPosterCache );
			ShowSidebarCommand = CreateCommand ( OpenSidebar );
		}

		private void OpenSidebar () => ShowSidebar?.Invoke ();

		private void ClearPosterCache () {

		}

		public void NavigateFrom () {
		}

		public void NavigateTo ( object parameter ) {
			m_AnalyticsService.TrackEvent ( "MaintenancePage" , "NavigatedTo" , "Simple" );
		}

		public Action ShowSidebar { get; set; }

		public ICommand ClearPosterCacheCommand { get; set; }

		public ICommand ShowSidebarCommand { get; set; }

	}

}
