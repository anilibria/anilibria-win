using System.Collections.ObjectModel;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.CinemaHall.PresentationClasses;

namespace Anilibria.Pages.CinemaHall {

	/// <summary>
	/// View model for page of Cinema Hall.
	/// </summary>
	public class CinemaHallViewModel : ViewModel, INavigation {

		private ObservableCollection<CinemaHallReleaseModel> m_Releases;

		private ObservableCollection<CinemaHallReleaseModel> m_SelectedReleases;

		public CinemaHallViewModel () {
			CreateCommand ();
		}

		private void CreateCommand () {
			WatchCommand = CreateCommand ( Watch );
		}

		private void Watch () {
			//start watch video
		}

		/// <summary>
		/// Refresh releases.
		/// </summary>
		private void RefreshReleases () {

		}

		/// <summary>
		/// Navigate from.
		/// </summary>
		public void NavigateFrom () {
			
		}

		/// <summary>
		/// Navigate to.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void NavigateTo ( object parameter ) {
			
		}

		/// <summary>
		/// Releases.
		/// </summary>
		public ObservableCollection<CinemaHallReleaseModel> Releases
		{
			get => m_Releases;
			set => Set ( ref m_Releases , value );
		}

		/// <summary>
		/// Releases.
		/// </summary>
		public ObservableCollection<CinemaHallReleaseModel> SelectedReleases
		{
			get => m_SelectedReleases;
			set => Set ( ref m_SelectedReleases , value );
		}

		/// <summary>
		/// Watch command.
		/// </summary>
		public ICommand WatchCommand
		{
			get;
			set;
		}

	}

}
