using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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

		private bool m_IsMultipleSelect;

		private CinemaHallReleaseModel m_OpenedRelease;

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
			Releases = new ObservableCollection<CinemaHallReleaseModel> ();
		}

		private void RefreshSelectedReleases () {
			SelectedReleases = new ObservableCollection<CinemaHallReleaseModel> ();
			SelectedReleases.CollectionChanged += SelectedReleasesChanged;
		}

		private void SelectedReleasesChanged ( object sender , NotifyCollectionChangedEventArgs e ) {
			RaiseCommands ();

			if ( !IsMultipleSelect && SelectedReleases.Count == 1 ) {
				OpenedRelease = SelectedReleases.First ();

				RefreshSelectedReleases ();
			}
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
		/// Is multiple select.
		/// </summary>
		public bool IsMultipleSelect
		{
			get => m_IsMultipleSelect;
			set => Set ( ref m_IsMultipleSelect , value );
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
		/// Opened release.
		/// </summary>
		public CinemaHallReleaseModel OpenedRelease
		{
			get => m_OpenedRelease;
			set => Set ( ref m_OpenedRelease , value );
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
