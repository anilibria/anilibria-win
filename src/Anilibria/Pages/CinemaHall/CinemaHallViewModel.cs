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
using Newtonsoft.Json;
using Windows.Storage;

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

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IReleasesService m_ReleasesService;

		private CinemaHallReleaseEntity m_ReleasesEntity;

		private CinemaHallReleaseModel m_ReorderItem;

		public CinemaHallViewModel ( IAnilibriaApiService anilibriaApiService , IDataContext dataContext , IAnalyticsService analyticsService , IReleasesService releasesService ) {
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_ReleasesService = releasesService ?? throw new ArgumentNullException ( nameof ( releasesService ) );

			RefreshSelectedReleases ();

			CreateCommand ();
		}

		private void CreateCommand () {
			WatchCommand = CreateCommand ( Watch , () => m_ReleasesEntity != null && m_ReleasesEntity.Releases.Any () );
			ShowSidebarCommand = CreateCommand ( OpenSidebar );
			RemoveReleasesCommand = CreateCommand ( RemoveReleases , () => m_SelectedReleases.Any () );
			ClearAllReleasesCommand = CreateCommand ( ClearAllReleases , () => m_ReleasesEntity.Releases.Any () );
		}

		private void ClearAllReleases () {
			m_ReleasesEntity.Releases = Enumerable.Empty<long> ();

			var collection = m_DataContext.GetCollection<CinemaHallReleaseEntity> ();
			collection.Update ( m_ReleasesEntity );

			RefreshSelectedReleases ();
			RefreshReleases ( Enumerable.Empty<CinemaHallReleaseModel> ().ToList () );

			RaiseCommands ();

			IsEmptyList = true;
		}

		private void RemoveReleases () {
			var releases = m_SelectedReleases.Select ( a => a.ReleaseId ).ToList ();
			m_ReleasesEntity.Releases = m_ReleasesEntity.Releases
				.Where ( a => !releases.Contains ( a ) )
				.ToList ();

			RefreshSelectedReleases ();

			var collection = m_DataContext.GetCollection<CinemaHallReleaseEntity> ();
			collection.Update ( m_ReleasesEntity );

			var deletedReleases = Releases
				.Where ( a => releases.Contains ( a.ReleaseId ) )
				.ToList ();
			foreach ( var deletedRelease in deletedReleases ) Releases.Remove ( deletedRelease );

			IsEmptyList = !m_ReleasesEntity.Releases.Any ();
		}

		private void OpenSidebar () {
			ShowSidebar ();
		}

		private void Watch () {
			ChangePage (
				"Player" ,
				new CinemaHallLinkModel {
					Releases = m_ReleasesEntity.Releases
						.Select ( a => a )
						.ToList ()
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

		private void SelectedReleasesChanged ( object sender , NotifyCollectionChangedEventArgs e ) => RaiseCommands ();

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
					Releases = new List<long> ()
				};
				collection.Add ( m_ReleasesEntity );
			}

			IsEmptyList = !m_ReleasesEntity.Releases.Any ();

			var releases = m_ReleasesService.GetReleases ();

			var releasesDictionary = releases.ToDictionary ( a => a.Id );

			var cinemaHallReleases = new List<CinemaHallReleaseModel> ();
			var iterator = -1;
			foreach ( var releaseId in m_ReleasesEntity.Releases ) {
				var releaseModel = releasesDictionary[releaseId];
				cinemaHallReleases.Add (
					new CinemaHallReleaseModel {
						Title = releaseModel.Title ,
						Description = releaseModel.Description ,
						Order = iterator ,
						Poster = m_AnilibriaApiService.GetUrl ( releaseModel.Poster ) ,
						ReleaseId = releaseModel.Id
					}
				);
			}

			RefreshReleases ( cinemaHallReleases );

			RaiseCommands ();

			m_AnalyticsService.TrackEvent ( "CinemaHallpage" , "NavigatedTo" , "Simple" );
		}

		private void RefreshReleases ( List<CinemaHallReleaseModel> cinemaHallReleases ) {
			Releases = new ObservableCollection<CinemaHallReleaseModel> ( cinemaHallReleases );
			Releases.CollectionChanged += Releases_CollectionChanged;
		}

		private void Releases_CollectionChanged ( object sender , NotifyCollectionChangedEventArgs e ) {
			switch ( e.Action ) {
				case NotifyCollectionChangedAction.Remove:
					m_ReorderItem = (CinemaHallReleaseModel) e.OldItems[0];
					break;
				case NotifyCollectionChangedAction.Add:
					if ( m_ReorderItem == null ) return;

					var releases = m_ReleasesEntity.Releases.ToList ();

					releases.Remove ( m_ReorderItem.ReleaseId );
					releases.Insert ( e.NewStartingIndex , m_ReorderItem.ReleaseId );

					m_ReorderItem = null;

					m_ReleasesEntity.Releases = releases;

					var collection = m_DataContext.GetCollection<CinemaHallReleaseEntity> ();
					collection.Update ( m_ReleasesEntity );

					break;
			}
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
		/// Show sidebar command.
		/// </summary>
		public ICommand ShowSidebarCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Remove releases command.
		/// </summary>
		public ICommand RemoveReleasesCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Clear all releases command.
		/// </summary>
		public ICommand ClearAllReleasesCommand
		{
			get;
			set;
		}

	}

}
