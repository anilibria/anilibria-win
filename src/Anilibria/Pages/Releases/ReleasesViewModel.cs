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

		private ReleaseModel m_OpenedRelease;

		private bool m_IsShowReleaseCard;

		private string m_FilterByName;

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IDataContext m_DataContext;

		private readonly ISynchronizationService m_SynchronizeService;

		private readonly string[] m_FileSizes = { "B" , "KB" , "MB" , "GB" , "TB" };

		private IEnumerable<long> m_Favorites = Enumerable.Empty<long> ();

		private bool m_OpenedReleaseInFavorite;

		private bool m_IsAuthorized;

		private Uri m_CommentsUri;

		private bool m_IsShowComments;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="anilibriaApiService">Anilibria Api Service.</param>
		public ReleasesViewModel ( IAnilibriaApiService anilibriaApiService , IDataContext dataContext , ISynchronizationService synchronizationService ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
			m_SynchronizeService = synchronizationService ?? throw new ArgumentNullException ( nameof ( synchronizationService ) );

			CreateCommands ();
			RefreshSelectedReleases ();
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
		}

		private void CloseComments () {
			IsShowComments = false;
		}

		private void ShowComments () {
			var uri = new Uri ( $"https://vk.com/widget_comments.php?app=5315207&width=100%&_ver=1&limit=8&norealtime=0&url=https://www.anilibria.tv/release/{OpenedRelease.Code}.html" );
			CommentsUri = uri;
			IsShowComments = true;
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
			ChangePage ( "Player" , SelectedReleases.ToList () );
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

		/// <summary>
		/// Get items page.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Items on current page.</returns>
		private Task<IEnumerable<ReleaseModel>> GetItemsPageAsync ( int page , int pageSize ) {
			var releases = m_AllReleases;
			if ( !string.IsNullOrEmpty ( FilterByName ) ) releases = releases.Where ( a => a.Names.Any ( name => name.Contains ( FilterByName ) ) );

			releases = releases.OrderByDescending ( a => a.Timestamp );

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
		/// Selected releases.
		/// </summary>
		public ObservableCollection<ReleaseModel> SelectedReleases
		{
			get => m_SelectedReleases;
			set => Set ( ref m_SelectedReleases , value );
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

	}

}
