using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anilibria.Helpers;
using Anilibria.MVVM;
using Anilibria.Pages.DownloadManagerPage.PresentationClasses;
using Anilibria.Pages.PresentationClasses;
using Anilibria.Services;
using Anilibria.Services.Implementations;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Windows.Storage;

namespace Anilibria.Pages.DownloadManagerPage {

	/// <summary>
	/// Download manager view model.
	/// </summary>
	public class DownloadManagerViewModel : ViewModel, INavigation {

		private const string IsFullNotificationSettings = "DownloadManagerViewModelIsFullNotification";

		private string m_FilterByName;

		private DownloadSectionItem m_SelectedSection;

		private bool m_IsMultipleSelect;

		private IDownloadService m_DownloadService;

		private readonly IEntityCollection<ReleaseEntity> m_ReleaseCollection;

		private ObservableCollection<DownloadSectionItem> m_Sections = new ObservableCollection<DownloadSectionItem> (
			new List<DownloadSectionItem> {
				new DownloadSectionItem {
					Title = "Все релизы",
					Type = DownloadSectionType.All
				},
				new DownloadSectionItem {
					Title = "Скачанные",
					Type = DownloadSectionType.Downloaded
				},
				new DownloadSectionItem {
					Title = "Скачиваемые",
					Type = DownloadSectionType.Downloading
				},
				new DownloadSectionItem {
					Title = "Не скаченные",
					Type = DownloadSectionType.NotDownloaded
				},
			}
		);

		private IEnumerable<DownloadItemModel> m_Downloads = Enumerable.Empty<DownloadItemModel> ();

		private IEnumerable<ReleaseEntity> m_Releases = Enumerable.Empty<ReleaseEntity> ();

		private bool m_NoFilteredDownloads;

		private DownloadItemModel m_SelectedDownload;

		private bool m_IsFullNotification;

		public DownloadManagerViewModel ( IDownloadService downloadService , IDataContext dataContext ) {
			m_DownloadService = downloadService ?? throw new ArgumentNullException ( nameof ( downloadService ) );
			m_DownloadService.SetDownloadProgress ( ProgressHandler );
			m_DownloadService.SetDownloadFinished ( FinishHandler );
			m_ReleaseCollection = dataContext.GetCollection<ReleaseEntity> ();
			CreateCommands ();
			RestoreSettings ();

			m_SelectedSection = m_Sections.First ();
			ObserverEvents.SubscribeOnEvent ( "synchronizedReleases" , RefreshAfterSynchronize );
		}

		private void RestoreSettings () {
			var values = ApplicationData.Current.RoamingSettings.Values;
			if ( values.ContainsKey ( IsFullNotificationSettings ) ) IsFullNotification = (bool) values[IsFullNotificationSettings];
		}

		private void FinishHandler ( DownloadReleaseEntity downloadRelease , int videoId , long downloaded , VideoQuality videoQuality ) {
			var release = m_Downloads.FirstOrDefault ( a => a.ReleaseId == downloadRelease.ReleaseId );
			if ( release == null ) return;

			release.CurrentDownloadVideo = downloadRelease.Videos.FirstOrDefault ( a => a.IsProgress )?.Id ?? 0;
			release.DownloadProgress = downloadRelease.Videos.Count ( a => a.IsProgress );
			release.DownloadedHdVideos = downloadRelease.Videos.Count ( a => a.IsDownloaded && a.Quality == VideoQuality.HD );
			release.DownloadedSdVideos = downloadRelease.Videos.Count ( a => a.IsDownloaded && a.Quality == VideoQuality.SD );
			release.DownloadSpeed = "";
			release.NotDownloadedVideos = downloadRelease.Videos.Count ( a => !a.IsDownloaded && !a.IsProgress );
			release.Active = downloadRelease.Active;

			var displayQuality = GetDisplayQuality ( videoQuality );
			var video = release.Videos.Where ( a => a.Order == videoId && a.Quality == displayQuality ).FirstOrDefault ();
			if ( video == null ) return;

			video.IsProgress = false;
			video.DownloadedSize = FileHelper.GetFileSize ( downloaded );
			video.IsDownloaded = true;

			if ( IsFullNotification ) {
				ObserverEvents.FireEvent (
					"showMessage" ,
					new MessageModel {
						Header = $"Скачана {video.Name} {video.Quality}" ,
						Message = $"В релизе " + release.Title
					}
				);
			}
		}

		private void ProgressHandler ( long releaseId , int videoId , int progress , long speed , VideoQuality quality , long downloaded ) {
			var release = m_Downloads.FirstOrDefault ( a => a.ReleaseId == releaseId );
			if ( release == null ) return;

			var downloadRelease = m_DownloadService.GetDownloadRelease ( releaseId );

			release.CurrentDownloadVideo = downloadRelease.Videos
				.Where ( a => a.Id == videoId )
				.FirstOrDefault ( a => a.IsProgress )?.Id ?? 0;
			release.DownloadProgress = progress;
			release.DownloadedHdVideos = downloadRelease.Videos.Count ( a => a.IsDownloaded && a.Quality == VideoQuality.HD );
			release.DownloadedSdVideos = downloadRelease.Videos.Count ( a => a.IsDownloaded && a.Quality == VideoQuality.SD );
			release.DownloadSpeed = FileHelper.GetFileSize ( speed ) + "/с";
			release.NotDownloadedVideos = downloadRelease.Videos.Count ( a => !a.IsDownloaded );

			var displayQuality = GetDisplayQuality ( quality );
			var video = release.Videos.Where ( a => a.Order == videoId && a.Quality == displayQuality ).FirstOrDefault ();
			if ( video == null ) return;

			video.IsProgress = true;
			video.DownloadedSize = FileHelper.GetFileSize ( downloaded );
		}

		private string GetDisplayQuality ( VideoQuality videoQuality ) {
			switch ( videoQuality ) {
				case VideoQuality.SD:
					return "SD";
				case VideoQuality.HD:
					return "HD";
				case VideoQuality.FullHD:
					return "FullHD";
				default: throw new NotSupportedException ( $"Quality {videoQuality} not supported." );
			}
		}

		private VideoQuality GetEnumQuality ( string videoQuality ) {
			switch ( videoQuality ) {
				case "SD":
					return VideoQuality.SD;
				case "HD":
					return VideoQuality.HD;
				case "FullHD":
					return VideoQuality.FullHD;
				default: throw new NotSupportedException ( $"Quality {videoQuality} not supported." );
			}
		}

		private DownloadItemModel MapToModel ( DownloadReleaseEntity downloadRelease ) {
			var release = m_Releases.FirstOrDefault ( a => a.Id == downloadRelease.ReleaseId );

			return new DownloadItemModel {
				ReleaseId = downloadRelease.ReleaseId ,
				Order = downloadRelease.Order ,
				Active = downloadRelease.Active ,
				Title = release?.Title ,
				Poster = ApiService.Current ().GetUrl ( release?.Poster ) ,
				DownloadedHdVideos = downloadRelease.Videos.Count ( a => a.IsDownloaded ) ,
				DownloadingVideos = 0 ,
				DownloadSpeed = "" ,
				NotDownloadedVideos = downloadRelease.Videos.Count ( a => !a.IsDownloaded ) ,
				Videos = new ObservableCollection<DownloadVideoItemModel> (
					downloadRelease.Videos
						.OrderBy ( a => a.Id )
						.Select (
							a => new DownloadVideoItemModel {
								Identifier = $"{a.Id}{a.Quality}" ,
								Order = a.Id ,
								Name = $"Серия {a.Id}" ,
								DownloadedSize = FileHelper.GetFileSize ( Convert.ToInt64 ( a.DownloadedSize ) ) ,
								IsDownloaded = a.IsDownloaded ,
								Quality = GetDisplayQuality ( a.Quality ) ,
								IsProgress = a.IsProgress
							}
						)
						.ToList ()
				)
			};
		}

		private void RefreshAfterSynchronize ( object parameter ) {
			m_Releases = m_ReleaseCollection
				.All ()
				.ToList ();
		}

		/// <summary>
		/// Initialize.
		/// </summary>
		/// <returns></returns>
		public async Task Initialize () => await m_DownloadService.StartDownloadProcess ();

		private void CreateCommands () {
			ShowSidebarCommand = CreateCommand ( OpenSidebar );
			DeleteFilesCommand = CreateCommand<DownloadItemModel> ( DeleteFiles );
			DeleteVideoCommand = CreateCommand<string> ( DeleteVideo );
			FilterCommand = CreateCommand ( Filter );
		}

		private void Filter () {
			RefreshDownloadItems ();
		}

		private async void DeleteVideo ( string identifier ) {
			var id = identifier.Replace ( "D" , "" ).Replace ( "H" , "" ).Replace ( "S" , "" );
			var quality = identifier.Replace ( id , "" );
			await m_DownloadService.RemoveDownloadFile ( SelectedDownload.ReleaseId , Convert.ToInt32 ( id ) , GetEnumQuality ( quality ) );

			RefreshDownloadItems ();
		}

		private async void DeleteFiles ( DownloadItemModel item ) {
			await m_DownloadService.RemoveDownloadRelease ( item.ReleaseId );

			RefreshDownloadItems ();
		}

		private void OpenSidebar () {
			ShowSidebar?.Invoke ();
		}

		public void NavigateFrom () {
		}

		private IEnumerable<DownloadItemModel> FilterDownloads ( IEnumerable<DownloadItemModel> downloadItems ) {
			if ( !string.IsNullOrEmpty ( FilterByName ) ) downloadItems = downloadItems.Where ( a => a.Title.Contains ( FilterByName ) ).ToList ();

			return downloadItems;
		}

		public void RefreshDownloadItems () {
			if ( !m_Releases.Any () ) RefreshAfterSynchronize ( null );

			var selectedReleaseId = SelectedDownload?.ReleaseId;

			DownloadItemsMode type = DownloadItemsMode.All;
			switch ( m_SelectedSection.Type ) {
				case DownloadSectionType.All:
					type = DownloadItemsMode.All;
					break;
				case DownloadSectionType.Downloading:
					type = DownloadItemsMode.Downloading;
					break;
				case DownloadSectionType.Downloaded:
					type = DownloadItemsMode.Downloaded;
					break;
				case DownloadSectionType.NotDownloaded:
					type = DownloadItemsMode.NotDownloaded;
					break;
				default: throw new NotSupportedException ( $"Type {type} not supported" );
			}
			var downloads = m_DownloadService.GetDownloads ( type )
				.Select ( MapToModel )
				.ToList ();
			Downloads = FilterDownloads ( downloads );
			NoFilteredDownloads = !Downloads.Any ();

			if ( selectedReleaseId.HasValue ) SelectedDownload = Downloads.FirstOrDefault ( a => a.ReleaseId == selectedReleaseId.Value );
		}

		public void NavigateTo ( object parameter ) => RefreshDownloadItems ();

		/// <summary>
		/// Filter by name.
		/// </summary>
		public string FilterByName
		{
			get => m_FilterByName;
			set => Set ( ref m_FilterByName , value );
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
		/// Not filtered downloads count.
		/// </summary>
		public bool NoFilteredDownloads
		{
			get => m_NoFilteredDownloads;
			set => Set ( ref m_NoFilteredDownloads , value );
		}

		/// <summary>
		/// Downloads.
		/// </summary>
		public IEnumerable<DownloadItemModel> Downloads
		{
			get => m_Downloads;
			set => Set ( ref m_Downloads , value );
		}

		/// <summary>
		/// Selected download.
		/// </summary>
		public DownloadItemModel SelectedDownload
		{
			get => m_SelectedDownload;
			set => Set ( ref m_SelectedDownload , value );
		}

		/// <summary>
		/// Filter by name.
		/// </summary>
		public DownloadSectionItem SelectedSection
		{
			get => m_SelectedSection;
			set
			{
				if ( !Set ( ref m_SelectedSection , value ) ) return;

				RefreshDownloadItems ();
			}
		}

		/// <summary>
		/// Sections;
		/// </summary>
		public ObservableCollection<DownloadSectionItem> Sections
		{
			get => m_Sections;
			set => Set ( ref m_Sections , value );
		}

		/// <summary>
		/// Is full notification.
		/// </summary>
		public bool IsFullNotification
		{
			get => m_IsFullNotification;
			set
			{
				if ( !Set ( ref m_IsFullNotification , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[IsFullNotificationSettings] = value;
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
		/// Show sidebar command.
		/// </summary>
		public ICommand ShowSidebarCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Delete files command.
		/// </summary>
		public ICommand DeleteFilesCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Delete video command.
		/// </summary>
		public ICommand DeleteVideoCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Filter command.
		/// </summary>
		public ICommand FilterCommand
		{
			get;
			set;
		}

	}

}
