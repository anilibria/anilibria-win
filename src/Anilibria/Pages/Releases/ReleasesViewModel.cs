using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anilibria.Collections;
using Anilibria.MVVM;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services;
using Anilibria.Services.Implementations;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Windows.System;

namespace Anilibria.Pages.Releases {

	/// <summary>
	/// Release view model.
	/// </summary>
	public class ReleasesViewModel : ViewModel, INavigation {

		private bool m_IsMultipleSelect;

		private IEnumerable<ReleaseEntity> m_AllReleases;

		private IncrementalLoadingCollection<ReleaseModel> m_Collection;

		private ObservableCollection<ReleaseModel> m_SelectedReleases;

		private ObservableCollection<SortingItemModel> m_SortingItems;

		private SortingItemModel m_SelectedSortingItem;

		private ObservableCollection<SortingDirectionModel> m_SortingDirections;

		private SortingDirectionModel m_SelectedSortingDirection;

		private ObservableCollection<SectionModel> m_Sections;

		private SectionModel m_SelectedSection;

		private ReleaseModel m_OpenedRelease;

		private bool m_IsShowReleaseCard;

		private string m_FilterByName;

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IDataContext m_DataContext;

		private readonly ISynchronizationService m_SynchronizeService;

		private readonly IAnalyticsService m_AnalyticsService;

		private readonly string[] m_FileSizes = { "B" , "KB" , "MB" , "GB" , "TB" };

		private IEnumerable<long> m_Favorites = Enumerable.Empty<long> ();

		private bool m_OpenedReleaseInFavorite;

		private bool m_IsAuthorized;

		private Uri m_CommentsUri;

		private bool m_IsShowComments;

		private bool m_EmptyReleases;

		private string m_FilterByGenres;

		private string m_FilterByYears;

		private string m_FilterByVoicers;

		private string m_FilterByType;

		private string m_FilterByStatus;

		private bool m_IsRefreshing;

		private bool m_IsNewReleases;

		private bool m_IsNewOnlineSeries;

		private bool m_IsNewTorrentSeries;

		private int m_NewReleasesCount;

		private int m_NewOnlineSeriesCount;

		private int m_NewTorrentSeriesCount;

		private bool m_IsShowNotification;

		private ChangesEntity m_Changes;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="anilibriaApiService">Anilibria Api Service.</param>
		public ReleasesViewModel ( IAnilibriaApiService anilibriaApiService , IDataContext dataContext , ISynchronizationService synchronizationService , IAnalyticsService analyticsService ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
			m_SynchronizeService = synchronizationService ?? throw new ArgumentNullException ( nameof ( synchronizationService ) );
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );

			CreateCommands ();
			CreateSortingItems ();
			CreateSections ();
			RefreshSelectedReleases ();
			ObserverEvents.SubscribeOnEvent ( "synchronizedReleases" , RefreshAfterSynchronize );

			m_AnalyticsService.TrackEvent ( "Releases" , "Opened" , "Simple start" );
		}

		private void CreateSections () {
			Sections = new ObservableCollection<SectionModel> {
				new SectionModel {
					Title = "Все релизы",
					Type = SectionType.All
				},
				new SectionModel {
					Title = "Избранное",
					Type = SectionType.Favorite
				},
				new SectionModel {
					Title = "Новые релизы",
					Type = SectionType.NewReleases
				},
				new SectionModel {
					Title = "Релизы с новыми сериями",
					Type = SectionType.NewOnlineSeries
				},
				new SectionModel {
					Title = "Релизы с обновленными торрентами",
					Type = SectionType.NewTorrentSeries
				},
			};
			m_SelectedSection = Sections.First ();
			RaisePropertyChanged ( () => SelectedSection );
		}

		private void CreateSortingItems () {
			m_SortingItems = new ObservableCollection<SortingItemModel> (
				new List<SortingItemModel> {
					new SortingItemModel {
						Name = "Дате последнего обновления",
						Type = SortingItemType.DateLastUpdate,
					},
					new SortingItemModel {
						Name = "Имени",
						Type = SortingItemType.Name,
					},
					new SortingItemModel {
						Name = "Году",
						Type = SortingItemType.Year,
					},
					new SortingItemModel {
						Name = "Рейтингу",
						Type = SortingItemType.Rating,
					},
					new SortingItemModel {
						Name = "Статусу",
						Type = SortingItemType.Status,
					},
					new SortingItemModel {
						Name = "Оригинальному имени",
						Type = SortingItemType.OriginalName,
					},
				}
			);

			m_SortingDirections = new ObservableCollection<SortingDirectionModel> (
				new List<SortingDirectionModel> {
					new SortingDirectionModel {
						Name = "Восходящем",
						Type = SortingDirectionType.Ascending
					},
					new SortingDirectionModel {
						Name = "Нисходящем",
						Type = SortingDirectionType.Descending
					}
				}
			);

			m_SelectedSortingItem = m_SortingItems.First ();
			m_SelectedSortingDirection = m_SortingDirections.Last ();
		}

		private void RefreshAfterSynchronize ( object parameter ) {
			IsShowReleaseCard = false;
			RefreshReleases ();
			RefreshSelectedReleases ();
			RefreshNotification ();
		}

		private void CreateCommands () {
			ShowSidebarCommand = CreateCommand ( ToggleSidebar );
			HideReleaseCardCommand = CreateCommand ( HideReleaseCard );
			FilterCommand = CreateCommand ( Filter );
			OpenOnlineVideoCommand = CreateCommand ( OpenOnlineVideo );
			AddToFavoritesCommand = CreateCommand ( AddToFavorites , () => IsMultipleSelect && m_AnilibriaApiService.IsAuthorized () && SelectedReleases.Count > 0 );
			RemoveFromFavoritesCommand = CreateCommand ( RemoveFromFavorites , () => IsMultipleSelect && m_AnilibriaApiService.IsAuthorized () && SelectedReleases.Count > 0 );
			OpenTorrentCommand = CreateCommand<string> ( OpenTorrent );
			AddCardFavoriteCommand = CreateCommand ( AddCardFavorite );
			RemoveCardFavoriteCommand = CreateCommand ( RemoveCardFavorite );
			AddToLocalFavoritesCommand = CreateCommand ( AddToLocalFavorites , () => IsMultipleSelect && SelectedReleases.Count > 0 );
			RemoveFromLocalFavoritesCommand = CreateCommand ( RemoveFromLocalFavorites , () => IsMultipleSelect && SelectedReleases.Count > 0 );
			ShowCommentsCommand = CreateCommand ( ShowComments );
			CloseCommentsCommand = CreateCommand ( CloseComments );
			RefreshCommand = CreateCommand ( Refresh , () => !IsRefreshing );
			ResetNotificationCommand = CreateCommand ( ResetNotification );
		}

		private void ResetNotification () {
			if ( m_Changes == null ) return;

			var collection = m_DataContext.GetCollection<ChangesEntity> ();

			m_Changes.NewOnlineSeries.Clear ();
			m_Changes.NewReleases = Enumerable.Empty<long> ();
			m_Changes.NewTorrents.Clear ();
			m_Changes.NewTorrentSeries.Clear ();

			collection.Update ( m_Changes );

			RefreshNotification ();
		}

		private async void Refresh () {
			IsRefreshing = true;
			RaiseCanExecuteChanged ( RefreshCommand );

			await m_SynchronizeService.SynchronizeReleases ();

			IsRefreshing = false;
			RaiseCanExecuteChanged ( RefreshCommand );
		}

		private void CloseComments () {
			IsShowComments = false;
		}

		private void ShowComments () {
			var uri = new Uri ( $"https://vk.com/widget_comments.php?app=5315207&width=100%&_ver=1&limit=8&norealtime=0&url=https://www.anilibria.tv/release/{OpenedRelease.Code}.html" );
			CommentsUri = uri;
			IsShowComments = true;
		}

		private int GetNewSeries ( long releaseId , int oldCount , IEnumerable<ReleaseEntity> releaseEntities ) {
			var release = releaseEntities.FirstOrDefault ( a => a.Id == releaseId );
			if ( release == null ) return 0;

			var currentCount = release.Playlist?.Count () ?? 0;
			if ( currentCount == 0 ) return 0;

			return currentCount - oldCount;
		}

		private void RefreshNotification () {
			var collection = m_DataContext.GetCollection<ChangesEntity> ();
			m_Changes = collection.FirstOrDefault ();
			if ( m_Changes == null ) return;


			var onlineSeriesReleases = Enumerable.Empty<ReleaseEntity> ();
			if ( m_Changes.NewOnlineSeries.Any () ) {
				var ids = m_Changes.NewOnlineSeries.Select ( a => a.Key ).ToArray ();
				onlineSeriesReleases = m_AllReleases.Where ( a => ids.Contains ( a.Id ) );
			}

			NewReleasesCount = m_Changes.NewReleases.Count ();
			NewOnlineSeriesCount = m_Changes.NewOnlineSeries.Any () ? m_Changes.NewOnlineSeries.Select ( a => GetNewSeries ( a.Key , a.Value , onlineSeriesReleases ) ).Sum () : 0;
			NewTorrentSeriesCount = m_Changes.NewTorrentSeries.Count ();
			IsNewReleases = NewReleasesCount > 0;
			IsNewOnlineSeries = NewOnlineSeriesCount > 0;
			IsNewTorrentSeries = NewTorrentSeriesCount > 0;
			IsShowNotification = NewReleasesCount > 0 || NewOnlineSeriesCount > 0 || NewTorrentSeriesCount > 0;
		}

		private LocalFavoriteEntity GetLocalFavorites ( IEntityCollection<LocalFavoriteEntity> collection ) {
			var favorites = collection.FirstOrDefault ();

			if ( favorites == null ) {
				favorites = new LocalFavoriteEntity {
					Releases = new List<long> ()
				};
				collection.Add ( favorites );
			}

			return favorites;
		}

		private async void RemoveFromLocalFavorites () {
			var collection = m_DataContext.GetCollection<LocalFavoriteEntity> ();
			var favorites = GetLocalFavorites ( collection );

			foreach ( var id in SelectedReleases.Select ( a => a.Id ) ) favorites.Releases.Remove ( id );

			favorites.Releases = favorites.Releases.Distinct ().ToList ();
			collection.Update ( favorites );

			await RefreshFavorites ();

			RefreshSelectedReleases ();
		}

		private async void AddToLocalFavorites () {
			var collection = m_DataContext.GetCollection<LocalFavoriteEntity> ();
			var favorites = GetLocalFavorites ( collection );

			foreach ( var id in SelectedReleases.Select ( a => a.Id ) ) favorites.Releases.Add ( id );

			favorites.Releases = favorites.Releases.Distinct ().ToList ();
			collection.Update ( favorites );

			await RefreshFavorites ();

			RefreshSelectedReleases ();
		}

		private void RefreshCardFavorite () => OpenedReleaseInFavorite = m_Favorites.Any ( a => a == OpenedRelease.Id );

		private async void RemoveCardFavorite () {
			await m_AnilibriaApiService.RemoveUserFavorites ( OpenedRelease.Id );

			await RefreshFavorites ();
			RefreshCardFavorite ();
		}

		private async void AddCardFavorite () {
			await m_AnilibriaApiService.AddUserFavorites ( OpenedRelease.Id );

			await RefreshFavorites ();
			RefreshCardFavorite ();
		}

		public async void OpenTorrent ( string torrent ) {
			var file = await m_AnilibriaApiService.DownloadTorrent ( torrent );
			await Launcher.LaunchFileAsync ( file );
		}

		private async Task RefreshFavorites () {
			var favorites = new List<long> ();
			if ( m_AnilibriaApiService.IsAuthorized () ) {
				await m_SynchronizeService.SynchronizeFavorites ();

				var userFavoritesCollection = m_DataContext.GetCollection<UserFavoriteEntity> ();
				var userModel = m_AnilibriaApiService.GetUserModel ();
				if ( userModel != null ) {
					var userFavorite = userFavoritesCollection.FirstOrDefault ( a => a.Id == userModel.Id );
					if ( userFavorite != null ) favorites.AddRange ( userFavorite.Releases );
				}
			}

			var collection = m_DataContext.GetCollection<LocalFavoriteEntity> ();
			var localFavorites = GetLocalFavorites ( collection );

			m_Favorites = favorites.Concat ( localFavorites.Releases );
			foreach ( var release in m_Collection ) release.AddToFavorite = m_Favorites.Contains ( release.Id );

			IsAuthorized = m_AnilibriaApiService.IsAuthorized ();
		}

		public async Task SynchronizeFavorites () => await RefreshFavorites ();

		private async void RemoveFromFavorites () {
			var ids = SelectedReleases.Select ( a => a.Id ).ToList ();

			var tasks = ids.Select ( a => m_AnilibriaApiService.RemoveUserFavorites ( a ) );

			await Task.WhenAll ( tasks );

			await RefreshFavorites ();

			RefreshSelectedReleases ();
		}

		private async void AddToFavorites () {
			var ids = SelectedReleases.Select ( a => a.Id ).ToList ();

			var tasks = ids.Select ( a => m_AnilibriaApiService.AddUserFavorites ( a ) );

			await Task.WhenAll ( tasks );

			await RefreshFavorites ();

			RefreshSelectedReleases ();
		}

		private void OpenOnlineVideo () {
			IsShowReleaseCard = false;
			ChangePage ( "Player" , new List<ReleaseModel> { OpenedRelease } );
		}

		private void Filter () => RefreshReleases ();

		private void HideReleaseCard () {
			IsShowReleaseCard = false;
			if ( SelectedReleases.Count == 1 ) RefreshSelectedReleases ();
		}

		private void ToggleSidebar () {
			ShowSidebar?.Invoke ();
		}

		private void RefreshSelectedReleases () {
			RaiseCommands ();

			SelectedReleases = new ObservableCollection<ReleaseModel> ();
			SelectedReleases.CollectionChanged += SelectedReleasesChanged;
		}

		private void SelectedReleasesChanged ( object sender , NotifyCollectionChangedEventArgs e ) {
			RaiseCommands ();

			if ( !IsMultipleSelect && SelectedReleases.Count == 1 ) {
				OpenedRelease = SelectedReleases.First ();
				IsShowReleaseCard = true;
				RefreshSelectedReleases ();
			}
		}

		private IEnumerable<ReleaseEntity> GetReleasesByCurrentMode () {
			var context = m_DataContext.GetCollection<ReleaseEntity> ();

			return context
				.Find ( a => true )
				.ToList ();
		}

		/// <summary>
		/// Refresh releases.
		/// </summary>
		private void RefreshReleases () {
			m_AllReleases = GetReleasesByCurrentMode ();
			EmptyReleases = m_AllReleases.Count () == 0;

			m_Collection = new IncrementalLoadingCollection<ReleaseModel> {
				PageSize = 20 ,
				GetPageFunction = GetItemsPageAsync
			};
			RaisePropertyChanged ( () => Collection );
		}

		/// <summary>
		/// Get file size.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <returns>Readable size.</returns>
		private string GetFileSize ( long size ) {
			var readableSize = size;
			int order = 0;
			while ( readableSize >= 1024 && order < m_FileSizes.Length - 1 ) {
				order++;
				readableSize = readableSize / 1024;
			}
			return readableSize + " " + m_FileSizes[order];
		}

		private IOrderedEnumerable<ReleaseEntity> OrderReleases ( IEnumerable<ReleaseEntity> releases ) {
			switch ( m_SelectedSortingItem.Type ) {
				case SortingItemType.DateLastUpdate:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.Timestamp ) : releases.OrderByDescending ( a => a.Timestamp );
				case SortingItemType.Name:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.Names.First () ) : releases.OrderByDescending ( a => a.Names.First () );
				case SortingItemType.OriginalName:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.Names.Last () ) : releases.OrderByDescending ( a => a.Names.Last () );
				case SortingItemType.Status:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.Status ) : releases.OrderByDescending ( a => a.Status );
				case SortingItemType.Year:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.Year ) : releases.OrderByDescending ( a => a.Year );
				case SortingItemType.Rating:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.Rating ) : releases.OrderByDescending ( a => a.Rating );
				default: throw new NotSupportedException ( $"Sorting sorting item {m_SelectedSortingItem}." );
			}
		}

		private IEnumerable<ReleaseEntity> FilteringReleases ( IEnumerable<ReleaseEntity> releases ) {
			if ( !string.IsNullOrEmpty ( FilterByName ) ) releases = releases.Where ( a => a.Names.Any ( name => name.Contains ( FilterByName ) ) );
			if ( !string.IsNullOrEmpty ( FilterByType ) ) releases = releases.Where ( a => a.Type.Contains ( FilterByType ) );
			if ( !string.IsNullOrEmpty ( FilterByStatus ) ) {
				var statuses = FilterByStatus.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => statuses.Contains ( a.Status ) );
			}
			if ( !string.IsNullOrEmpty ( FilterByGenres ) ) {
				var genres = FilterByGenres.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => a.Genres.Any ( genre => genres.Contains ( genre ) ) );
			}
			if ( !string.IsNullOrEmpty ( FilterByYears ) ) {
				var years = FilterByYears.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => a.Year != null && years.Contains ( a.Year ) );
			}
			if ( !string.IsNullOrEmpty ( FilterByVoicers ) ) {
				var voicers = FilterByVoicers.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => a.Voices.Any ( voice => voicers.Contains ( voice ) ) );
			}

			return releases;
		}

		private IEnumerable<ReleaseEntity> FilteringBySection ( IEnumerable<ReleaseEntity> releases ) {
			var sectionType = SelectedSection.Type;

			switch ( sectionType ) {
				case SectionType.All:
					return releases;
				case SectionType.Favorite:
					return releases.Where ( a => m_Favorites.Contains ( a.Id ) );
				case SectionType.NewReleases:
					var newReleases = m_Changes.NewReleases ?? Enumerable.Empty<long> ();
					return releases.Where ( a => newReleases.Contains ( a.Id ) );
				case SectionType.NewOnlineSeries:
					var newSeries = m_Changes.NewOnlineSeries?.Keys ?? Enumerable.Empty<long> ();
					return releases.Where ( a => newSeries.Contains ( a.Id ) );
				case SectionType.NewTorrentSeries:
					var newTorrents = m_Changes.NewOnlineSeries?.Keys ?? Enumerable.Empty<long> ();
					return releases.Where ( a => newTorrents.Contains ( a.Id ) );
				default: throw new NotSupportedException ( "Section type not supported." );
			}
		}

		/// <summary>
		/// Get items page.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Items on current page.</returns>
		private Task<IEnumerable<ReleaseModel>> GetItemsPageAsync ( int page , int pageSize ) {
			var releases = FilteringReleases ( m_AllReleases );

			releases = FilteringBySection ( releases );

			releases = OrderReleases ( releases );

			var result = releases
				.Skip ( ( page - 1 ) * pageSize )
				.Take ( pageSize )
				.Select (
				a => new ReleaseModel {
					Id = a.Id ,
					AddToFavorite = m_Favorites?.Contains ( a.Id ) ?? false ,
					Code = a.Code ,
					Description = a.Description ,
					Genres = string.Join ( ", " , a.Genres ) ,
					Title = a.Names.FirstOrDefault () ,
					Names = a.Names ,
					Poster = m_AnilibriaApiService.GetUrl ( a.Poster ) ,
					Rating = a.Rating ,
					Series = a.Series ,
					Status = a.Status ,
					Type = a.Type ,
					Voices = string.Join ( ", " , a.Voices ) ,
					Year = a.Year ,
					CountVideoOnline = a.Playlist?.Count () ?? 0 ,
					Torrents = a?.Torrents?.Select (
						torrent => new TorrentModel {
							Completed = torrent.Completed ,
							Quality = $"[{torrent.Quality}]" ,
							Series = torrent.Series ,
							Size = GetFileSize ( torrent.Size ) ,
							Url = torrent.Url
						}
					)?.ToList () ?? Enumerable.Empty<TorrentModel> () ,
					OnlineVideos = a.Playlist?.Select (
						videoOnline => new OnlineVideoModel {
							Order = videoOnline.Id ,
							Title = videoOnline.Title ,
							HDQuality = videoOnline.HD ,
							SDQuality = videoOnline.SD
						}
					)?.ToList () ?? Enumerable.Empty<OnlineVideoModel> ()
				}
			);

			return Task.FromResult ( result );
		}

		/// <summary>
		/// Initialize view model.
		/// </summary>
		public void Initialize () {
			RefreshReleases ();
		}

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void NavigateTo ( object parameter ) {

		}

		/// <summary>
		/// End navigate to page.
		/// </summary>
		public void NavigateFrom () {

		}

		/// <summary>
		/// Is refreshing.
		/// </summary>
		public bool IsRefreshing
		{
			get => m_IsRefreshing;
			set => Set ( ref m_IsRefreshing , value );
		}

		/// <summary>
		/// Sorting items.
		/// </summary>
		public ObservableCollection<SortingItemModel> SortingItems
		{
			get => m_SortingItems;
			set => Set ( ref m_SortingItems , value );
		}

		/// <summary>
		/// Selected sorting item.
		/// </summary>
		public SortingItemModel SelectedSortingItem
		{
			get => m_SelectedSortingItem;
			set
			{
				if ( !Set ( ref m_SelectedSortingItem , value ) ) return;

				RefreshSelectedReleases ();
				RefreshReleases ();
			}
		}

		/// <summary>
		/// Sorting directions
		/// </summary>
		public ObservableCollection<SortingDirectionModel> SortingDirections
		{
			get => m_SortingDirections;
			set => Set ( ref m_SortingDirections , value );
		}

		/// <summary>
		/// Selected sorting direction.
		/// </summary>
		public SortingDirectionModel SelectedSortingDirection
		{
			get => m_SelectedSortingDirection;
			set
			{
				if ( !Set ( ref m_SelectedSortingDirection , value ) ) return;

				RefreshSelectedReleases ();
				RefreshReleases ();
			}
		}

		/// <summary>
		/// Collection.
		/// </summary>
		public IncrementalLoadingCollection<ReleaseModel> Collection
		{
			get => m_Collection;
			set => Set ( ref m_Collection , value );
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
		/// Release for show in Release Card.
		/// </summary>
		public ReleaseModel OpenedRelease
		{
			get => m_OpenedRelease;
			set
			{
				if ( !Set ( ref m_OpenedRelease , value ) ) return;

				RefreshCardFavorite ();
			}
		}

		/// <summary>
		/// Comment uri.
		/// </summary>
		public Uri CommentsUri
		{
			get => m_CommentsUri;
			set => Set ( ref m_CommentsUri , value );
		}

		/// <summary>
		/// Is show comments.
		/// </summary>
		public bool IsShowComments
		{
			get => m_IsShowComments;
			set => Set ( ref m_IsShowComments , value );
		}

		/// <summary>
		/// Is authorized.
		/// </summary>
		public bool IsAuthorized
		{
			get => m_IsAuthorized;
			set => Set ( ref m_IsAuthorized , value );
		}

		/// <summary>
		/// Opened release in favorite.
		/// </summary>
		public bool OpenedReleaseInFavorite
		{
			get => m_OpenedReleaseInFavorite;
			set => Set ( ref m_OpenedReleaseInFavorite , value );
		}

		/// <summary>
		/// Release for show in Release Card.
		/// </summary>
		public bool IsShowReleaseCard
		{
			get => m_IsShowReleaseCard;
			set
			{
				if ( !Set ( ref m_IsShowReleaseCard , value ) ) return;

				if ( !value ) IsShowComments = false;
			}
		}

		/// <summary>
		/// Filter by name.
		/// </summary>
		public string FilterByName
		{
			get => m_FilterByName;
			set => Set ( ref m_FilterByName , value );
		}

		/// <summary>
		/// Filter by genres.
		/// </summary>
		public string FilterByGenres
		{
			get => m_FilterByGenres;
			set => Set ( ref m_FilterByGenres , value );
		}

		/// <summary>
		/// Filter by years.
		/// </summary>
		public string FilterByYears
		{
			get => m_FilterByYears;
			set => Set ( ref m_FilterByYears , value );
		}

		/// <summary>
		/// Filter by voices.
		/// </summary>
		public string FilterByVoicers
		{
			get => m_FilterByVoicers;
			set => Set ( ref m_FilterByVoicers , value );
		}

		/// <summary>
		/// Filter by type.
		/// </summary>
		public string FilterByType
		{
			get => m_FilterByType;
			set => Set ( ref m_FilterByType , value );
		}

		/// <summary>
		/// Filter by status.
		/// </summary>
		public string FilterByStatus
		{
			get => m_FilterByStatus;
			set => Set ( ref m_FilterByStatus , value );
		}

		/// <summary>
		/// Is show notification.
		/// </summary>
		public bool IsShowNotification
		{
			get => m_IsShowNotification;
			set => Set ( ref m_IsShowNotification , value );
		}

		/// <summary>
		/// Sections.
		/// </summary>
		public ObservableCollection<SectionModel> Sections
		{
			get => m_Sections;
			set => Set ( ref m_Sections , value );
		}

		/// <summary>
		/// Selected section.
		/// </summary>
		public SectionModel SelectedSection
		{
			get => m_SelectedSection;
			set
			{
				if ( !Set ( ref m_SelectedSection , value ) ) return;

				RefreshReleases ();
				RefreshSelectedReleases ();
			}
		}

		/// <summary>
		/// New releases exists.
		/// </summary>
		public bool IsNewReleases
		{
			get => m_IsNewReleases;
			set => Set ( ref m_IsNewReleases , value );
		}

		/// <summary>
		/// New online series exists.
		/// </summary>
		public bool IsNewOnlineSeries
		{
			get => m_IsNewOnlineSeries;
			set => Set ( ref m_IsNewOnlineSeries , value );
		}

		/// <summary>
		/// New torrent series exists.
		/// </summary>
		public bool IsNewTorrentSeries
		{
			get => m_IsNewTorrentSeries;
			set => Set ( ref m_IsNewTorrentSeries , value );
		}

		/// <summary>
		/// New releases exists.
		/// </summary>
		public int NewReleasesCount
		{
			get => m_NewReleasesCount;
			set => Set ( ref m_NewReleasesCount , value );
		}

		/// <summary>
		/// New online series exists.
		/// </summary>
		public int NewOnlineSeriesCount
		{
			get => m_NewOnlineSeriesCount;
			set => Set ( ref m_NewOnlineSeriesCount , value );
		}

		/// <summary>
		/// New torrent series exists.
		/// </summary>
		public int NewTorrentSeriesCount
		{
			get => m_NewTorrentSeriesCount;
			set => Set ( ref m_NewTorrentSeriesCount , value );
		}


		/// <summary>
		/// Selected releases.
		/// </summary>
		public ObservableCollection<ReleaseModel> SelectedReleases
		{
			get => m_SelectedReleases;
			set => Set ( ref m_SelectedReleases , value );
		}

		/// <summary>
		/// Empty releases.
		/// </summary>
		public bool EmptyReleases
		{
			get => m_EmptyReleases;
			set => Set ( ref m_EmptyReleases , value );
		}

		/// <summary>
		/// Change page handler.
		/// </summary>
		public Action<string , object> ChangePage
		{
			get;
			set;
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
		/// Show sidebar command.
		/// </summary>
		public ICommand ShowSidebarCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Hide release card command.
		/// </summary>
		public ICommand HideReleaseCardCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add favorite from release card.
		/// </summary>
		public ICommand RemoveCardFavoriteCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Remove favorite from release card.
		/// </summary>
		public ICommand AddCardFavoriteCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Filter releases list.
		/// </summary>
		public ICommand FilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Open online video command.
		/// </summary>
		public ICommand OpenOnlineVideoCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add to favorites command.
		/// </summary>
		public ICommand AddToFavoritesCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Remove from favorites command.
		/// </summary>
		public ICommand RemoveFromFavoritesCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add to favorites command.
		/// </summary>
		public ICommand AddToLocalFavoritesCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Remove from favorites command.
		/// </summary>
		public ICommand RemoveFromLocalFavoritesCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Open torrent.
		/// </summary>
		public ICommand OpenTorrentCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Show comments.
		/// </summary>
		public ICommand ShowCommentsCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Close comments commands.
		/// </summary>
		public ICommand CloseCommentsCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Refresh command.
		/// </summary>
		public ICommand RefreshCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Reset notification command.
		/// </summary>
		public ICommand ResetNotificationCommand
		{
			get;
			set;
		}

	}

}
