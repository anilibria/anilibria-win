using Anilibria.MVVM;
using Anilibria.Services;
using Anilibria.Services.Implementations;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Anilibria.Pages.MaintenancePage
{
	public class MaintenancePageViewModel : ViewModel, INavigation
	{
		private readonly IAnalyticsService m_AnalyticsService;

		private readonly object m_ClearPosterLock = new object();

		private const string PosterGroupName = "Poster";

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
			var storageClient = StorageService.Current ();
			var releasesService = ReleaseSingletonService.Current ();

			Task.Run (
				() => {
					lock ( m_ClearPosterLock ) {
						foreach ( var release in releasesService.GetReleases () ) {
							if ( !storageClient.IsFileExists ( PosterGroupName , release.Id ) ) continue;

							storageClient.DeleteFile ( PosterGroupName , release.Id );
						}
					}
				}
			);
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
