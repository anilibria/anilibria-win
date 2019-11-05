using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.CinemaHall.PresentationClasses;
using Anilibria.Pages.OnlinePlayer.PresentationClasses;
using Anilibria.Services;
using Anilibria.Storage;
using Anilibria.Storage.Entities;

namespace Anilibria.Pages.CinemaHall {

	/// <summary>
	/// View model for page of Cinema Hall.
	/// </summary>
	public class CinemaHallViewModel : ViewModel, INavigation {

		private ObservableCollection<CinemaHallReleaseModel> m_Releases;

		private ObservableCollection<CinemaHallReleaseModel> m_SelectedReleases;

		private bool m_IsMultipleSelect;

		private CinemaHallReleaseModel m_OpenedRelease;

		private bool m_IsEmptyList;

		private readonly IDataContext m_DataContext;

		private readonly IAnalyticsService m_AnalyticsService;

		private CinemaHallReleaseEntity m_ReleasesEntity;

		public CinemaHallViewModel ( IDataContext dataContext , IAnalyticsService analyticsService ) {
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );

			CreateCommand ();
		}

		private void CreateCommand () {
			WatchCommand = CreateCommand ( Watch );
			ReorderCommand = CreateCommand ( Reorder );
			ShowSidebarCommand = CreateCommand ( OpenSidebar );
		}

		private void OpenSidebar () {
			ShowSidebar ();
		}

		private void Reorder () {

		}

		private void Watch () {
			ChangePage (
				"Player" ,
				new ReleaseLinkModel {
					//TODO: change to cinemahall model
					//ReleaseId = downloadRelease.ReleaseId
				}
			);

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

			var collection = m_DataContext.GetCollection<CinemaHallReleaseEntity> ();
			m_ReleasesEntity = collection.FirstOrDefault ();

			if ( m_ReleasesEntity == null ) {
				m_ReleasesEntity = new CinemaHallReleaseEntity {
					NewReleases = new List<long> ()
				};
				collection.Add ( m_ReleasesEntity );
			}

			IsEmptyList = !m_ReleasesEntity.NewReleases.Any ();

			m_AnalyticsService.TrackEvent ( "CinemaHallpage" , "NavigatedTo" , "Simple" );
		}

		/// <summary>
		/// Show sidebar.
		/// </summary>
		public Action ShowSidebar
		{
			get;
			set;
		}

		/// <summary>
		/// Change page.
		/// </summary>
		public Action<string , object> ChangePage
		{
			get;
			set;
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
		/// Is empty list.
		/// </summary>
		public bool IsEmptyList
		{
			get => m_IsEmptyList;
			set => Set ( ref m_IsEmptyList , value );
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

		/// <summary>
		/// Redorder command.
		/// </summary>
		public ICommand ReorderCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Show sidebar command.
		/// </summary>
		public ICommand ShowSidebarCommand
		{
			get;
			set;
		}

	}

}
