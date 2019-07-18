using Anilibria.Collections;
using Anilibria.MVVM;
using Anilibria.Pages.PresentationClasses;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services;
using Anilibria.Services.Implementations;
using Anilibria.Services.PresentationClasses;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Anilibria.ThemeChanger;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.Releases {

	/// <summary>
	/// Release view model.
	/// </summary>
	public class ReleasesViewModel : ViewModel, INavigation {

		private const string IsFavoriteNotificationsSettings = "IsFavoriteNotifications";

		private const string TorrentModeSettings = "TorrentMode";

		private const string OpenVideoSettings = "OpenVideo";

		private const string IsDarkThemeSettings = "IsDarkTheme";

		private Random m_Random = new Random ( Guid.NewGuid ().GetHashCode () );

		private bool m_IsMultipleSelect;

		private IEnumerable<ReleaseEntity> m_AllReleases;

		private IDictionary<int , IEnumerable<long>> m_SchedulesReleases = new Dictionary<int , IEnumerable<long>> ();

		private IncrementalLoadingCollection<ReleaseModel> m_Collection;

		private ObservableCollection<IGrouping<string , ReleaseModel>> m_GroupingCollection;

		private ObservableCollection<ReleaseModel> m_SelectedReleases;

		private ObservableCollection<ReleaseModel> m_SelectedGroupedReleases;

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

		private ObservableCollection<TorrentDownloadModeModel> m_TorrentDownloadModes = new ObservableCollection<TorrentDownloadModeModel> (
			new List<TorrentDownloadModeModel> {
				new TorrentDownloadModeModel {
					Mode = TorrentDownloadMode.OpenInTorrentClient,
					Title = "Открыть в торрент клиенте"
				},
				new TorrentDownloadModeModel {
					Mode = TorrentDownloadMode.SaveAsFile,
					Title = "Сохранить файл"
				},
				//new TorrentDownloadModeModel {
				//	Mode = TorrentDownloadMode.DownloadToDownloadManager,
				//	Name = "Использовать встроенный загрузчик"
				//},
			}
		);

		private ObservableCollection<OpenVideoModeModel> m_OpenVideoModes = new ObservableCollection<OpenVideoModeModel> (
			new List<OpenVideoModeModel> {
				new OpenVideoModeModel {
					Mode = OpenVideoMode.ImmediatlyOpenVideoPlayer,
					Title = "Сразу открыть видеоплеер"
				},
				new OpenVideoModeModel {
					Mode = OpenVideoMode.SelectOnlineVideo,
					Title = "Выбрать онлайн видео"
				}
			}
		);

		private ObservableCollection<SeenMarkItem> m_SeenMarkTypes = ReleasesItems.GetSeenMarkItems ();

		private ObservableCollection<FavoriteMarkItem> m_FavoriteMarkTypes = ReleasesItems.GetFavoriteMarkItems ();

		private IEnumerable<long> m_Favorites = Enumerable.Empty<long> ();

		private bool m_OpenedReleaseInFavorite;

		private bool m_IsAuthorized;

		private Uri m_CommentsUri;

		private bool m_IsShowComments;

		private bool m_EmptyReleases;

		private string m_FilterByGenres;

		private string m_FilterByYears;

		private string m_FilterBySeasons;

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

		private bool m_IsShowNotification = false;

		private ChangesEntity m_Changes;

		private UserModel m_UserModel;

		private bool m_ShowAnnounce;

		private bool m_isFavoriteNotifications;

		private TorrentDownloadModeModel m_SelectedTorrentDownloadMode;

		private bool m_GroupedGridVisible;

		private bool m_FilterIsFilled;

		private OpenVideoModeModel m_SelectedOpenVideoMode;

		private IEntityCollection<ReleaseVideoStateEntity> m_VideoStateCollection;

		private IDictionary<long , int> m_CountWachedVideos = new Dictionary<long , int> ();

		private SeenMarkItem m_SelectedSeenMarkType;

		private FavoriteMarkItem m_SelectedFavoriteMarkType;

		private IEntityCollection<ReleaseEntity> m_ReleasesCollection;

		private string m_FilterByDescription;

		private bool m_IsDarkTheme;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="anilibriaApiService">Anilibria Api Service.</param>
		public ReleasesViewModel ( IAnilibriaApiService anilibriaApiService , IDataContext dataContext , ISynchronizationService synchronizationService , IAnalyticsService analyticsService ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
			m_SynchronizeService = synchronizationService ?? throw new ArgumentNullException ( nameof ( synchronizationService ) );
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );

			m_ReleasesCollection = m_DataContext.GetCollection<ReleaseEntity> ();

			CreateCommands ();
			CreateSortingItems ();
			CreateSections ();
			RefreshSelectedReleases ();
			RestoreSettings ();
			ObserverEvents.SubscribeOnEvent ( "synchronizedReleases" , RefreshAfterSynchronize );

			m_AnalyticsService.TrackEvent ( "Releases" , "Opened" , "Simple start" );

			m_VideoStateCollection = m_DataContext.GetCollection<ReleaseVideoStateEntity> ();
			RefreshWatchedVideo ();
		}

		private void RefreshWatchedVideo () {
			var videoStates = m_VideoStateCollection.Find ( a => true );

			m_CountWachedVideos = new Dictionary<long , int> ();
			foreach ( var videoState in videoStates ) {
				m_CountWachedVideos.Add ( videoState.ReleaseId , videoState.VideoStates?.Count ( a => a.IsSeen ) ?? 0 );
			}
			if ( m_Collection == null ) return;

			foreach ( var release in m_Collection ) {
				if ( !m_CountWachedVideos.ContainsKey ( release.Id ) ) continue;

				release.IsSeen = m_CountWachedVideos[release.Id] == ( release.OnlineVideos?.Count () ?? -2 );
			}
		}

		private void RestoreSettings () {
			var values = ApplicationData.Current.RoamingSettings.Values;
			if ( values.ContainsKey ( IsFavoriteNotificationsSettings ) ) {
				m_isFavoriteNotifications = (bool) values[IsFavoriteNotificationsSettings];
			}
			if ( values.ContainsKey ( TorrentModeSettings ) ) {
				var torrentMode = (TorrentDownloadMode) ( (int) values[TorrentModeSettings] );
				m_SelectedTorrentDownloadMode = m_TorrentDownloadModes.FirstOrDefault ( a => a.Mode == torrentMode ) ?? m_TorrentDownloadModes.First ();
			}
			else {
				m_SelectedTorrentDownloadMode = m_TorrentDownloadModes.First ();
			}

			if ( values.ContainsKey ( OpenVideoSettings ) ) {
				var openVideoMode = (OpenVideoMode) ( (int) values[OpenVideoSettings] );
				m_SelectedOpenVideoMode = m_OpenVideoModes.FirstOrDefault ( a => a.Mode == openVideoMode ) ?? m_OpenVideoModes.First ();
			}
			else {
				m_SelectedOpenVideoMode = m_OpenVideoModes.First ();
			}

			IsDarkTheme = (bool) values[IsDarkThemeSettings];
		}

		private void CreateSections () {
			Sections = new ObservableCollection<SectionModel> {
				new SectionModel {
					Title = "Все релизы",
					Type = SectionType.All,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Избранное",
					Type = SectionType.Favorite,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Расписание",
					Type = SectionType.Schedule,
					SortingMode = SortingItemType.ScheduleDay,
					SortingDirection = SortingDirectionType.Ascending,
				},
				new SectionModel {
					Title = "История",
					Type = SectionType.HistoryViews,
					SortingMode = SortingItemType.HistoryView,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "История просмотров",
					Type = SectionType.HistoryWatch,
					SortingMode = SortingItemType.HistoryWatch,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Просмотренные",
					Type = SectionType.Seens,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Просматриваемые",
					Type = SectionType.PartiallySeen,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Не просмотренные",
					Type = SectionType.NotSeens,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Новые релизы",
					Type = SectionType.NewReleases,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Релизы с новыми сериями",
					Type = SectionType.NewOnlineSeries,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
				},
				new SectionModel {
					Title = "Релизы с обновленными торрентами",
					Type = SectionType.NewTorrentSeries,
					SortingMode = SortingItemType.DateLastUpdate,
					SortingDirection = SortingDirectionType.Descending,
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
						Name = "Дню в расписании",
						Type = SortingItemType.ScheduleDay,
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
					new SortingItemModel {
						Name = "История",
						Type = SortingItemType.HistoryView,
					},
					new SortingItemModel {
						Name = "История просмотра",
						Type = SortingItemType.HistoryWatch,
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
			m_SelectedFavoriteMarkType = m_FavoriteMarkTypes.First ();
			m_SelectedSeenMarkType = m_SeenMarkTypes.First ();
		}

		private async void RefreshAfterSynchronize ( object parameter ) {
			IsShowReleaseCard = false;
			RefreshReleases ();
			RefreshSelectedReleases ();
			await RefreshFavorites ();
			RefreshNotification ( needSendToasts: true );
		}


		private void NotificationToast ( bool isNewReleases , bool isNewSeries , bool isNewTorrents ) {
			try {
				ToastNotificationManager.CreateToastNotifier ().Show (
					new ToastNotification (
						GenerateToastContent ( isNewReleases , isNewSeries , isNewTorrents ).GetXml ()
					)
				);
			}
			catch {
				//WORKAROUND: Sometimes app crashes on this line, I sure that issue not in my code.
			}
		}

		public static ToastContent GenerateToastContent ( bool isNewReleases , bool isNewSeries , bool isNewTorrents ) {
			var entities = new List<string> ();
			if ( isNewReleases ) entities.Add ( "Новые релизы" );
			if ( isNewSeries ) entities.Add ( "Новые серии" );
			if ( isNewTorrents ) entities.Add ( "Новые торренты" );

			return new ToastContent () {
				Launch = "openfromnotification" ,
				Scenario = ToastScenario.Reminder ,

				Visual = new ToastVisual () {
					BindingGeneric = new ToastBindingGeneric () {
						Children =
						{
							new AdaptiveText()
							{
								Text = $"Есть обновления"
							},
							new AdaptiveText()
							{
								Text = string.Join(", ", entities)
							}
						}
					}
				}
			};
		}

		private void SendToastByChanges ( bool newReleases , bool newSeries , bool newTorrents ) {
			if ( newReleases || newSeries || newTorrents ) NotificationToast ( newReleases , newSeries , newTorrents );
		}

		private void CreateCommands () {
			ShowSidebarCommand = CreateCommand ( ToggleSidebar );
			HideReleaseCardCommand = CreateCommand ( HideReleaseCard );
			FilterCommand = CreateCommand ( Filter );
			OpenOnlineVideoCommand = CreateCommand ( OpenOnlineVideo );
			AddToFavoritesCommand = CreateCommand ( AddToFavorites , () => IsMultipleSelect && m_AnilibriaApiService.IsAuthorized () && GetSelectedReleases ().Count > 0 );
			RemoveFromFavoritesCommand = CreateCommand ( RemoveFromFavorites , () => IsMultipleSelect && m_AnilibriaApiService.IsAuthorized () && GetSelectedReleases ().Count > 0 );
			OpenTorrentCommand = CreateCommand<string> ( OpenTorrent );
			AddCardFavoriteCommand = CreateCommand ( AddCardFavorite );
			RemoveCardFavoriteCommand = CreateCommand ( RemoveCardFavorite );
			AddToLocalFavoritesCommand = CreateCommand ( AddToLocalFavorites , () => IsMultipleSelect && GetSelectedReleases ().Count > 0 );
			RemoveFromLocalFavoritesCommand = CreateCommand ( RemoveFromLocalFavorites , () => IsMultipleSelect && GetSelectedReleases ().Count > 0 );
			ShowCommentsCommand = CreateCommand ( ShowComments );
			CloseCommentsCommand = CreateCommand ( CloseComments );
			RefreshCommand = CreateCommand ( Refresh , () => !IsRefreshing );
			ResetNotificationCommand = CreateCommand ( ResetNotification );
			OpenCrossReleaseCommand = CreateCommand<string> ( OpenCrossRelease );
			ShowRandomReleaseCommand = CreateCommand ( ShowRandomRelease );
			ClearFiltersCommands = CreateCommand ( ClearFilters );
			AddStatusToFilterCommand = CreateCommand ( AddStatusToFilter );
			AddYearToFilterCommand = CreateCommand ( AddYearToFilter );
			AddGenreToFilterCommand = CreateCommand<string> ( AddGenreToFilter );
			AddVoicesToFilterCommand = CreateCommand<string> ( AddVoicesToFilter );
			RemoveSeensFavoritesCommand = CreateCommand ( RemoveSeensFavorites );
			AddSeenMarkCommand = CreateCommand ( AddSeenMark , () => IsMultipleSelect && GetSelectedReleases ().Count > 0 );
			RemoveSeenMarkCommand = CreateCommand ( RemoveSeenMark , () => IsMultipleSelect && GetSelectedReleases ().Count > 0 );
			RemoveAllSeensMarksCommand = CreateCommand ( RemoveAllSeensMarks );
			EnableFavoriteMarkFilterCommand = CreateCommand ( EnableFavoriteMarkFilter , () => !IsShowReleaseCard );
			EnableNotFavoriteMarkFilterCommand = CreateCommand ( NotEnableFavoriteMarkFilter , () => !IsShowReleaseCard );
			DisableFavoriteMarkFilterCommand = CreateCommand ( DisableFavoriteMarkFilter , () => !IsShowReleaseCard );
			EnableSeenMarkFilterCommand = CreateCommand ( EnableSeenMarkFilter , () => !IsShowReleaseCard );
			EnableNotSeenMarkFilterCommand = CreateCommand ( EnableNotSeenMarkFilter , () => !IsShowReleaseCard );
			EnableSeenNowMarkFilterCommand = CreateCommand ( EnableSeenNowMarkFilter , () => !IsShowReleaseCard );
			DisableSeenMarkFilterCommand = CreateCommand ( DisableSeenMarkFilter , () => !IsShowReleaseCard );
			EnableSeenMarkCardCommand = CreateCommand ( EnableSeenMarkCard , () => IsShowReleaseCard );
			DisableSeenMarkCardCommand = CreateCommand ( DisableSeenMarkCard , () => IsShowReleaseCard );
			AddDownloadHdCommand = CreateCommand ( AddDownloadHd , () => IsMultipleSelect && GetSelectedReleases ().Count > 0 );
			AddDownloadSdCommand = CreateCommand ( AddDownloadSd , () => IsMultipleSelect && GetSelectedReleases ().Count > 0 );
		}

		private void AddDownloadSd () => AddToDownloadReleases ( VideoQuality.SD );

		private void AddDownloadHd () => AddToDownloadReleases ( VideoQuality.HD );

		private async void AddToDownloadReleases ( VideoQuality videoQuality ) {
			var downloadManager = DownloadManager.Current ();

			foreach ( var selectedRelease in SelectedReleases ) {
				foreach ( var video in selectedRelease.OnlineVideos ) {
					downloadManager.AddDownloadFile ( selectedRelease.Id , video , videoQuality );
				}
			}

			RefreshSelectedReleases ();

			await DownloadManager.Current ().StartDownloadProcess ();
		}

		private void DisableSeenMarkCard () {
			RemoveSeenMark ( new long[] { OpenedRelease.Id } );

			FillSeenFields ( OpenedRelease );
		}

		private void EnableSeenMarkCard () {
			EnableSeenMark ( new long[] { OpenedRelease.Id } );

			FillSeenFields ( OpenedRelease );
		}

		private void DisableSeenMarkFilter () {
			if ( SelectedSeenMarkType.Type != SeenMarkType.NotUsed ) SelectedSeenMarkType = SeenMarkTypes.First ( a => a.Type == SeenMarkType.NotUsed );
		}

		private void EnableSeenNowMarkFilter () {
			if ( SelectedSeenMarkType.Type != SeenMarkType.SeenNow ) SelectedSeenMarkType = SeenMarkTypes.First ( a => a.Type == SeenMarkType.SeenNow );
		}

		private void EnableNotSeenMarkFilter () {
			if ( SelectedSeenMarkType.Type != SeenMarkType.NotSeen ) SelectedSeenMarkType = SeenMarkTypes.First ( a => a.Type == SeenMarkType.NotSeen );
		}

		private void EnableSeenMarkFilter () {
			if ( SelectedSeenMarkType.Type != SeenMarkType.Seen ) SelectedSeenMarkType = SeenMarkTypes.First ( a => a.Type == SeenMarkType.Seen );
		}

		private void DisableFavoriteMarkFilter () {
			if ( SelectedFavoriteMarkType.Type != FavoriteMarkType.NotUsed ) SelectedFavoriteMarkType = FavoriteMarkTypes.First ( a => a.Type == FavoriteMarkType.NotUsed );
		}

		private void NotEnableFavoriteMarkFilter () {
			if ( SelectedFavoriteMarkType.Type != FavoriteMarkType.NotFavorited ) SelectedFavoriteMarkType = FavoriteMarkTypes.First ( a => a.Type == FavoriteMarkType.NotFavorited );
		}

		private void EnableFavoriteMarkFilter () {
			if ( SelectedFavoriteMarkType.Type != FavoriteMarkType.Favorited ) SelectedFavoriteMarkType = FavoriteMarkTypes.First ( a => a.Type == FavoriteMarkType.Favorited );
		}

		private async void RemoveAllSeensMarks () {
			var result = await new ContentDialog {
				Title = "Удалить все отметки о просмотре" ,
				Content = "Вы уверены что хотите удалить абсолютно все отметки о просмотре во всем каталоге релизов?" ,
				PrimaryButtonText = "Удалить" ,
				CloseButtonText = "Отмена"
			}.ShowAsync ();
			if ( result != ContentDialogResult.Primary ) return;

			var states = m_VideoStateCollection
				.Find ( a => true )
				.ToList ();

			foreach ( var state in states ) {
				if ( state.VideoStates == null ) continue;

				foreach ( var videoState in state.VideoStates ) videoState.IsSeen = false;

				m_VideoStateCollection.Update ( state );
			}

			RefreshWatchedVideo ();
		}

		private void RemoveSeenMark () {
			RemoveSeenMark (
				GetSelectedReleases ()
					.Select ( a => a.Id )
					.ToArray ()
			);
		}

		private void RemoveSeenMark ( IEnumerable<long> selectedReleasesIds ) {
			var states = m_VideoStateCollection
				.Find ( a => true )
				.ToList ()
				.Where ( a => a.VideoStates != null && selectedReleasesIds.Contains ( a.ReleaseId ) )
				.ToList ();

			foreach ( var state in states ) {
				if ( state.VideoStates == null ) continue;

				foreach ( var videoState in state.VideoStates ) videoState.IsSeen = false;

				m_VideoStateCollection.Update ( state );
			}

			RefreshWatchedVideo ();
			RefreshSelectedReleases ();
		}

		private ICollection<VideoStateEntity> GetSeensReleasePlaylistStates ( long releaseId ) {
			var release = m_AllReleases.FirstOrDefault ( a => a.Id == releaseId );
			if ( release == null ) return Enumerable.Empty<VideoStateEntity> ().ToList ();

			return release.Playlist
				.Select (
					a => new VideoStateEntity {
						Id = a.Id ,
						IsSeen = true ,
						LastPosition = 0
					}
				)
				.ToList ();
		}

		private ICollection<VideoStateEntity> GetSeensReleasePlaylistStates ( long releaseId , IEnumerable<VideoStateEntity> videoStates ) {
			var release = m_AllReleases.FirstOrDefault ( a => a.Id == releaseId );
			if ( release == null ) return Enumerable.Empty<VideoStateEntity> ().ToList ();

			var result = new List<VideoStateEntity> ();

			foreach ( var item in release.Playlist ) {
				var existsItem = videoStates.FirstOrDefault ( a => a.Id == item.Id );
				if ( existsItem != null ) {
					existsItem.IsSeen = true;
					result.Add ( existsItem );
					continue;
				}

				result.Add (
					new VideoStateEntity {
						Id = item.Id ,
						IsSeen = true ,
						LastPosition = 0
					}
				);
			}

			return result;
		}

		private void AddSeenMark () {
			EnableSeenMark (
				GetSelectedReleases ()
					.Select ( a => a.Id )
					.ToArray ()
			);
		}

		private void EnableSeenMark ( IEnumerable<long> selectedReleasesIds ) {
			var states = m_VideoStateCollection
				.Find ( a => true )
				.ToList ()
				.Where ( a => a.VideoStates != null && selectedReleasesIds.Contains ( a.ReleaseId ) )
				.ToList ();

			foreach ( var selectedReleaseId in selectedReleasesIds ) {
				var state = states.FirstOrDefault ( a => a.ReleaseId == selectedReleaseId );
				if ( state == null ) {
					m_VideoStateCollection.Add (
						new ReleaseVideoStateEntity {
							ReleaseId = selectedReleaseId ,
							VideoStates = GetSeensReleasePlaylistStates ( selectedReleaseId )
						}
					);
					continue;
				}
				state.VideoStates = GetSeensReleasePlaylistStates ( selectedReleaseId , state.VideoStates );
				m_VideoStateCollection.Update ( state );
			}

			RefreshWatchedVideo ();
			RefreshSelectedReleases ();
		}

		private async void RemoveSeensFavorites () {
			if ( !m_Favorites.Any () ) {
				await new ContentDialog {
					Title = "Удаление просмотренных релизов из избранного" ,
					Content = "У Вас нет релизов в избранном!" ,
					CloseButtonText = "Понятно"
				}.ShowAsync ();
				return;
			}

			var result = await new ContentDialog {
				Title = "Удаление просмотренных релизов из избранного" ,
				Content = "Вы уверены что хотите удалить просмотренные релизы из избранного? \nБудут удалены все релизы в которых количество просмотренных серий равно общему количеству вне зависимости от статуса релиза,\n но учтите что релизы могут продолжаться и новые серии могут добавляться! Все равно удалить просмотренные релизы из избранного?" ,
				PrimaryButtonText = "Удалить" ,
				CloseButtonText = "Отмена"
			}.ShowAsync ();
			if ( result != ContentDialogResult.Primary ) return;

			var states = m_VideoStateCollection
				.Find ( a => true )
				.Where ( a => a.VideoStates != null )
				.ToList ();
			var statesDictionary = states.ToDictionary ( a => a.ReleaseId );

			var needRemoveFavorites = new List<long> ();

			foreach ( var favoriteId in m_Favorites ) {
				if ( !statesDictionary.ContainsKey ( favoriteId ) ) continue;

				var release = m_AllReleases.FirstOrDefault ( a => a.Id == favoriteId );
				if ( release.Playlist == null || !release.Playlist.Any () ) continue;

				var statesSeenCount = statesDictionary[favoriteId].VideoStates.Count ( a => a.IsSeen );
				if ( release.Playlist.Count () == statesSeenCount ) needRemoveFavorites.Add ( favoriteId );
			}

			if ( !needRemoveFavorites.Any () ) {
				ObserverEvents.FireEvent (
					"showMessage" ,
					new MessageModel {
						Header = "Удаление просмотренного избранного" ,
						Message = $"У Вас нет просмотренных релизов."
					}
				);
				return;
			}

			var tasks = needRemoveFavorites.Select ( a => m_AnilibriaApiService.RemoveUserFavorites ( a ) );
			await Task.WhenAll ( tasks );

			await RefreshFavorites ();
			RefreshCardFavorite ();

			ObserverEvents.FireEvent (
				"showMessage" ,
				new MessageModel {
					Header = "Удаление просмотренного избранного" ,
					Message = $"Удаление успешно завершено, удалено {needRemoveFavorites.Count} релизов из избранного."
				}
			);
		}

		private void AddVoicesToFilter ( string voice ) {
			FilterByVoicers = voice;

			Filter ();
			HideReleaseCard ();
		}

		private void AddGenreToFilter ( string genre ) {
			FilterByGenres = genre;

			Filter ();
			HideReleaseCard ();
		}

		private void AddYearToFilter () {
			FilterByYears = OpenedRelease.Year;

			Filter ();
			HideReleaseCard ();
		}

		private void AddStatusToFilter () {
			FilterByStatus = OpenedRelease.Status;

			Filter ();
			HideReleaseCard ();
		}

		private void RefreshFilterState () {
			var allEmpties = string.IsNullOrEmpty ( m_FilterByGenres ) &&
			string.IsNullOrEmpty ( m_FilterByStatus ) &&
			string.IsNullOrEmpty ( m_FilterByType ) &&
			string.IsNullOrEmpty ( m_FilterByVoicers ) &&
			string.IsNullOrEmpty ( m_FilterByYears ) &&
			string.IsNullOrEmpty ( m_FilterBySeasons ) &&
			string.IsNullOrEmpty ( m_FilterByDescription ) &&
			m_SelectedSeenMarkType.Type == SeenMarkType.NotUsed &&
			m_SelectedFavoriteMarkType.Type == FavoriteMarkType.NotUsed;

			FilterIsFilled = !allEmpties;
		}

		private void ClearFilters () {
			m_FilterByGenres = "";
			RaisePropertyChanged ( () => FilterByGenres );
			m_FilterByStatus = "";
			RaisePropertyChanged ( () => FilterByStatus );
			m_FilterByType = "";
			RaisePropertyChanged ( () => FilterByType );
			m_FilterByVoicers = "";
			RaisePropertyChanged ( () => FilterByVoicers );
			m_FilterByYears = "";
			RaisePropertyChanged ( () => FilterByYears );
			m_SelectedFavoriteMarkType = FavoriteMarkTypes.FirstOrDefault ( a => a.Type == FavoriteMarkType.NotUsed );
			RaisePropertyChanged ( () => SelectedFavoriteMarkType );
			m_SelectedSeenMarkType = SeenMarkTypes.FirstOrDefault ( a => a.Type == SeenMarkType.NotUsed );
			RaisePropertyChanged ( () => SelectedSeenMarkType );
			m_FilterByDescription = "";
			RaisePropertyChanged ( () => FilterByDescription );
			m_FilterBySeasons = "";
			RaisePropertyChanged ( () => FilterBySeasons );

			Filter ();
			RefreshFilterState ();
		}

		private int SeensVideos ( long releaseId ) {
			var videoState = m_VideoStateCollection.FirstOrDefault ( a => a.ReleaseId == releaseId );

			if ( videoState == null ) return 0;

			return videoState.VideoStates?.Where ( a => a.IsSeen ).Count () ?? 0;
		}

		private async void ShowRandomRelease () {
			if ( m_AllReleases == null || m_AllReleases.Count () == 0 ) return;

			var randomIndex = m_Random.Next ( m_AllReleases.Count () - 1 );

			var release = m_AllReleases.ElementAtOrDefault ( randomIndex );
			if ( release == null ) return;

			var openedRelease = MapToReleaseModel ( release );
			FillSeenFields ( openedRelease );
			OpenedRelease = openedRelease;

			IsShowReleaseCard = true;
			await SaveReleaseViewTimestamp ( OpenedRelease.Id );
		}

		private async void OpenCrossRelease ( string releaseUrl ) {
			var releaseCode = releaseUrl.Replace ( "https://www.anilibria.tv/release/" , "" ).Replace ( "http://www.anilibria.tv/release/" , "" ).Replace ( ".html" , "" );
			if ( releaseCode.IndexOf ( "?" ) > -1 ) releaseCode = releaseCode.Substring ( 0 , releaseCode.IndexOf ( "?" ) );
			var release = m_AllReleases.FirstOrDefault ( a => a.Code == releaseCode );
			if ( release != null ) {
				OpenedRelease = MapToReleaseModel ( release );
				await SaveReleaseViewTimestamp ( OpenedRelease.Id );
			}
		}

		private void ResetNotification () {
			if ( m_Changes == null ) return;

			var collection = m_DataContext.GetCollection<ChangesEntity> ();

			m_Changes.NewOnlineSeries?.Clear ();
			m_Changes.NewReleases = Enumerable.Empty<long> ();
			m_Changes.NewTorrents?.Clear ();
			m_Changes.NewTorrentSeries?.Clear ();

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
			SetCommentsUrl ( new Uri ( $"https://vk.com/widget_comments.php?app=5315207&width=100%&_ver=1&limit=8&norealtime=0&url=https://www.anilibria.tv/release/{OpenedRelease.Code}.html" ) );
			IsShowComments = true;
		}

		private int GetNewSeries ( long releaseId , int oldCount , IEnumerable<ReleaseEntity> releaseEntities ) {
			var release = releaseEntities.FirstOrDefault ( a => a.Id == releaseId );
			if ( release == null ) return 0;

			var currentCount = release.Playlist?.Count () ?? 0;
			if ( currentCount == 0 ) return 0;

			return currentCount - oldCount;
		}

		private int GetCountOnlineSeries ( IEnumerable<ReleaseEntity> onlineSeriesReleases , IDictionary<long , int> newSeries ) {
			if ( newSeries == null || !newSeries.Any () ) return 0;

			return newSeries.Where ( a => IsFavoriteNotifications ? m_Favorites.Contains ( a.Key ) : true ).Select ( a => GetNewSeries ( a.Key , a.Value , onlineSeriesReleases ) ).Sum ();
		}

		private int GetCountTorrentSeries ( IDictionary<long , IDictionary<long , string>> newTorrents ) {
			if ( newTorrents == null || !newTorrents.Any () ) return 0;

			return newTorrents.Where ( a => IsFavoriteNotifications ? m_Favorites.Contains ( a.Key ) : true ).Count ();
		}

		private void RefreshNotification ( bool needSendToasts = false ) {
			var collection = m_DataContext.GetCollection<ChangesEntity> ();
			m_Changes = collection.FirstOrDefault ();
			if ( m_Changes == null ) return;

			var onlineSeriesReleases = Enumerable.Empty<ReleaseEntity> ();
			if ( m_Changes.NewOnlineSeries.Any () ) {
				var ids = m_Changes.NewOnlineSeries.Select ( a => a.Key ).ToArray ();
				onlineSeriesReleases = m_AllReleases.Where ( a => ids.Contains ( a.Id ) );
			}

			NewReleasesCount = m_Changes.NewReleases.Count ();
			NewOnlineSeriesCount = GetCountOnlineSeries ( onlineSeriesReleases , m_Changes.NewOnlineSeries );
			NewTorrentSeriesCount = GetCountTorrentSeries ( m_Changes.NewTorrentSeries );
			IsNewReleases = NewReleasesCount > 0;
			IsNewOnlineSeries = NewOnlineSeriesCount > 0;
			IsNewTorrentSeries = NewTorrentSeriesCount > 0;
			IsShowNotification = NewReleasesCount > 0 || NewOnlineSeriesCount > 0 || NewTorrentSeriesCount > 0;

			if ( !needSendToasts ) return;

			var historyChanges = m_DataContext.GetCollection<HistoryChangeEntity> ().FirstOrDefault ();
			if ( historyChanges == null ) return;

			var historyOnlineSeriesReleases = Enumerable.Empty<ReleaseEntity> ();
			if ( historyChanges.NewOnlineSeries != null && historyChanges.NewOnlineSeries.Any () && historyChanges.ReleaseOnlineSeries != null ) {
				historyOnlineSeriesReleases = historyChanges.ReleaseOnlineSeries
					.Select (
						a => new ReleaseEntity {
							Id = a.Key ,
							Playlist = Enumerable.Repeat ( new PlaylistItemEntity () , a.Value )
						}
					)
					.ToArray ();
			}

			SendToastByChanges (
				NewReleasesCount > historyChanges.NewReleases.Count () ,
				NewOnlineSeriesCount > GetCountOnlineSeries ( historyOnlineSeriesReleases , historyChanges.NewOnlineSeries ) ,
				NewTorrentSeriesCount > GetCountTorrentSeries ( historyChanges.NewTorrentSeries )
			);
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

			foreach ( var id in GetSelectedReleases ().Select ( a => a.Id ) ) favorites.Releases.Remove ( id );

			favorites.Releases = favorites.Releases.Distinct ().ToList ();
			collection.Update ( favorites );

			await RefreshFavorites ();

			RefreshSelectedReleases ();
		}

		private async void AddToLocalFavorites () {
			var collection = m_DataContext.GetCollection<LocalFavoriteEntity> ();
			var favorites = GetLocalFavorites ( collection );

			foreach ( var id in GetSelectedReleases ().Select ( a => a.Id ) ) favorites.Releases.Add ( id );

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
			StorageFile file = null;
			try {
				file = await m_AnilibriaApiService.DownloadTorrent ( torrent );
			}
			catch {
				ObserverEvents.FireEvent (
					"showMessage" ,
					new MessageModel {
						Header = "Сохранение торрента" ,
						Message = "Не удалось скачать торрент файл"
					}
				);
				return;
			}
			var mode = SelectedTorrentDownloadMode?.Mode ?? TorrentDownloadMode.OpenInTorrentClient;

			switch ( mode ) {
				case TorrentDownloadMode.OpenInTorrentClient:
					await Launcher.LaunchFileAsync ( file );
					break;
				case TorrentDownloadMode.SaveAsFile:
					var savePicker = new FileSavePicker {
						SuggestedStartLocation = PickerLocationId.Downloads ,
						SuggestedFileName = Path.GetFileName ( torrent )
					};
					savePicker.FileTypeChoices.Add ( "Torrent file" , new List<string> () { ".torrent" } );
					var savedFileLocation = await savePicker.PickSaveFileAsync ();
					if ( savedFileLocation != null ) {
						CachedFileManager.DeferUpdates ( savedFileLocation );
						try {
							using ( var sourceFile = await file.OpenStreamForReadAsync () )
							using ( var targetFile = await savedFileLocation.OpenStreamForWriteAsync () ) {
								await sourceFile.CopyToAsync ( targetFile );
							}
							var status = await CachedFileManager.CompleteUpdatesAsync ( savedFileLocation );
							if ( status == FileUpdateStatus.Complete ) {
								ObserverEvents.FireEvent (
									"showMessage" ,
									new MessageModel {
										Header = "Сохранение торрента" ,
										Message = "Сохранение успешно выполнено"
									}
								);
							}
							else {
								ObserverEvents.FireEvent (
									"showMessage" ,
									new MessageModel {
										Header = "Сохранение торрента" ,
										Message = "Не удалось сохранить торрент файл"
									}
								);
							}
						}
						catch {
							ObserverEvents.FireEvent (
								"showMessage" ,
								new MessageModel {
									Header = "Сохранение торрента" ,
									Message = "Ошибка при сохранении торрент файл"
								}
							);
						}
					}
					break;
				case TorrentDownloadMode.DownloadToDownloadManager:
				default: throw new NotSupportedException ( $"Download Mode {SelectedTorrentDownloadMode.Mode} not supported." );
			}

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
					userModel.ImageUrl = m_AnilibriaApiService.GetUrl ( userModel.Avatar );
				}
				UserModel = userModel;
			}

			var collection = m_DataContext.GetCollection<LocalFavoriteEntity> ();
			var localFavorites = GetLocalFavorites ( collection );

			m_Favorites = favorites.Concat ( localFavorites.Releases );
			if ( GroupedGridVisible ) {
				foreach ( var release in m_GroupingCollection.SelectMany ( a => a ) ) release.AddToFavorite = m_Favorites.Contains ( release.Id );
			}
			else {
				foreach ( var release in m_Collection ) release.AddToFavorite = m_Favorites.Contains ( release.Id );
			}

			IsAuthorized = m_AnilibriaApiService.IsAuthorized ();
			RefreshNotification ();
		}

		public async Task SynchronizeFavorites () {
			await RefreshFavorites ();
		}

		private async void RemoveFromFavorites () {
			var ids = GetSelectedReleases ().Select ( a => a.Id ).ToList ();

			var tasks = ids.Select ( a => m_AnilibriaApiService.RemoveUserFavorites ( a ) );

			await Task.WhenAll ( tasks );

			await RefreshFavorites ();

			RefreshSelectedReleases ();
		}

		private async void AddToFavorites () {
			var ids = GetSelectedReleases ().Select ( a => a.Id ).ToList ();

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
			if ( SelectedReleases.Count == 1 || SelectedGroupedReleases.Count == 1 ) RefreshSelectedReleases ();
		}

		private void ToggleSidebar () {
			ShowSidebar?.Invoke ();
		}

		private ObservableCollection<ReleaseModel> GetSelectedReleases () => GroupedGridVisible ? SelectedGroupedReleases : SelectedReleases;

		private void RefreshSelectedReleases () {
			RaiseCommands ();

			SelectedReleases = new ObservableCollection<ReleaseModel> ();
			SelectedReleases.CollectionChanged += SelectedReleasesChanged;

			SelectedGroupedReleases = new ObservableCollection<ReleaseModel> ();
			SelectedGroupedReleases.CollectionChanged += SelectedGroupedReleasesChanged;
		}

		private async void SelectedReleasesChanged ( object sender , NotifyCollectionChangedEventArgs e ) {
			RaiseCommands ();

			if ( !IsMultipleSelect && SelectedReleases.Count == 1 ) {
				var openedRelease = SelectedReleases.First ();
				FillSeenFields ( openedRelease );

				OpenedRelease = SelectedReleases.First ();
				IsShowReleaseCard = true;
				ClearReleaseNotification ( OpenedRelease.Id );
				RefreshSelectedReleases ();
				await SaveReleaseViewTimestamp ( OpenedRelease.Id );
			}
		}

		private void FillSeenFields ( ReleaseModel openedRelease ) {
			openedRelease.CountSeenVideoOnline = SeensVideos ( openedRelease.Id );
			openedRelease.DisplaySeenVideoOnline = openedRelease.CountSeenVideoOnline > 0 ? $"({openedRelease.CountSeenVideoOnline})" : "";
		}

		private async void SelectedGroupedReleasesChanged ( object sender , NotifyCollectionChangedEventArgs e ) {
			RaiseCommands ();

			if ( !IsMultipleSelect && SelectedGroupedReleases.Count == 1 ) {
				var openedRelease = SelectedGroupedReleases.First ();
				FillSeenFields ( openedRelease );

				OpenedRelease = openedRelease;
				IsShowReleaseCard = true;
				ClearReleaseNotification ( OpenedRelease.Id );
				RefreshSelectedReleases ();
				await SaveReleaseViewTimestamp ( OpenedRelease.Id );
			}
		}

		private async Task SaveReleaseViewTimestamp ( long releaseId ) {
			var release = m_AllReleases.FirstOrDefault ( a => a.Id == releaseId );
			if ( release == null ) return;

			release.LastViewTimestamp = (long) ( DateTime.UtcNow.Subtract ( new DateTime ( 1970 , 1 , 1 ) ) ).TotalSeconds;
			m_ReleasesCollection.Update ( release );

			var lastThreeViewReleases = m_AllReleases
				.Where ( a => a.LastViewTimestamp > 0 )
				.OrderByDescending ( a => a.LastViewTimestamp )
				.Take ( 3 )
				.ToList ();
			if ( !lastThreeViewReleases.Any () ) return;

			var jumpService = new JumpListService ();
			var dictionary = new Dictionary<long , string> ();
			foreach ( var lastRelease in lastThreeViewReleases ) dictionary.Add ( lastRelease.Id , lastRelease.Title );
			await jumpService.ChangeHistoryItems ( dictionary );

		}

		private IEnumerable<ReleaseEntity> GetReleasesByCurrentMode () {
			return m_ReleasesCollection
				.Find ( a => true )
				.ToList ()
				.Where ( a => !a.Blocked ) // All blocking releases not showed on releases page!!!
				.ToList ();
		}

		private ObservableCollection<IGrouping<string , ReleaseModel>> GetGroupedReleases () {
			var releases = FilteringReleases ( m_AllReleases );
			releases = FilteringBySection ( releases );
			releases = OrderReleases ( releases );

			return new ObservableCollection<IGrouping<string , ReleaseModel>> ( releases.Select ( MapToReleaseModel ).GroupBy ( a => a.ScheduledOnDay ) );
		}

		/// <summary>
		/// Refresh releases.
		/// </summary>
		private void RefreshReleases () {
			m_AllReleases = GetReleasesByCurrentMode ();
			m_SchedulesReleases = GetScheduleReleases ();
			EmptyReleases = m_AllReleases.Count () == 0;

			if ( GroupedGridVisible ) {
				GroupingCollection = GetGroupedReleases ();
				HideReleaseCard (); //WORKAROUND: other hand selcted items will be first item, I don't know why.
			}
			else {
				m_Collection = new IncrementalLoadingCollection<ReleaseModel> {
					PageSize = 20 ,
					GetPageFunction = GetItemsPageAsync
				};
				RaisePropertyChanged ( () => Collection );
			}
		}

		private IDictionary<int , IEnumerable<long>> GetScheduleReleases () {
			var scheduleCollection = m_DataContext.GetCollection<ScheduleEntity> ();
			var entity = scheduleCollection.FirstOrDefault ();
			if ( entity == null ) return new Dictionary<int , IEnumerable<long>> ();

			return entity.Days ?? new Dictionary<int , IEnumerable<long>> ();
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
				case SortingItemType.ScheduleDay:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => GetScheduleDayIndexOnRelease ( a ) ) : releases.OrderByDescending ( a => GetScheduleDayIndexOnRelease ( a ) );
				case SortingItemType.HistoryView:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.LastViewTimestamp ) : releases.OrderByDescending ( a => a.LastViewTimestamp );
				case SortingItemType.HistoryWatch:
					return m_SelectedSortingDirection.Type == SortingDirectionType.Ascending ? releases.OrderBy ( a => a.LastWatchTimestamp ) : releases.OrderByDescending ( a => a.LastWatchTimestamp );
				default: throw new NotSupportedException ( $"Sorting sorting item {m_SelectedSortingItem}." );
			}
		}

		private bool ContainsInArrayCaseSensitive ( string filter , IEnumerable<string> values ) {
			return values?
				.Where ( a => a != null )
				.Select (
					a =>
						a.ToLowerInvariant ()
							.Replace ( "ё" , "е" ) )
							.Any ( a => a.Contains ( filter.ToLowerInvariant ().Replace ( "ё" , "е" ) )
				) ?? false;
		}

		private IEnumerable<ReleaseEntity> FilteringReleases ( IEnumerable<ReleaseEntity> releases ) {
			if ( releases == null ) return Enumerable.Empty<ReleaseEntity> ();

			if ( !string.IsNullOrEmpty ( FilterByName ) ) releases = releases.Where ( a => ContainsInArrayCaseSensitive ( FilterByName , a.Names ) );
			if ( !string.IsNullOrEmpty ( FilterByType ) ) releases = releases.Where ( a => a.Type?.ToLowerInvariant ().Contains ( FilterByType.ToLowerInvariant () ) ?? false );
			if ( !string.IsNullOrEmpty ( FilterByDescription ) ) releases = releases.Where ( a => a.Description?.ToLowerInvariant ().Contains ( FilterByDescription.ToLowerInvariant () ) ?? false );
			if ( !string.IsNullOrEmpty ( FilterByStatus ) ) {
				var statuses = FilterByStatus.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => statuses?.Any ( b => ContainsInArrayCaseSensitive ( b , new string[] { a.Status } ) ) ?? false );
			}
			if ( !string.IsNullOrEmpty ( FilterByGenres ) ) {
				var genres = FilterByGenres.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => a.Genres?.Any ( genre => genres?.Any ( b => ContainsInArrayCaseSensitive ( b , new string[] { genre } ) ) ?? false ) ?? false );
			}
			if ( !string.IsNullOrEmpty ( FilterBySeasons ) ) {
				var seasons = FilterBySeasons.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => seasons.Any ( b => ContainsInArrayCaseSensitive ( b , new string[] { a.Season } ) ) );
			}
			if ( !string.IsNullOrEmpty ( FilterByYears ) ) {
				var years = FilterByYears.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => a.Year != null && years.Contains ( a.Year ) );
			}
			if ( !string.IsNullOrEmpty ( FilterByVoicers ) ) {
				var voicers = FilterByVoicers.Split ( ',' ).Select ( a => a.Trim () ).Where ( a => !string.IsNullOrEmpty ( a ) ).ToList ();
				releases = releases.Where ( a => a.Voices?.Any ( voice => voicers?.Any ( b => ContainsInArrayCaseSensitive ( b , new string[] { voice } ) ) ?? false ) ?? false );
			}
			switch ( SelectedFavoriteMarkType.Type ) {
				case FavoriteMarkType.Favorited:
					releases = releases.Where ( a => m_Favorites.Contains ( a.Id ) );
					break;
				case FavoriteMarkType.NotFavorited:
					releases = releases.Where ( a => !m_Favorites.Contains ( a.Id ) );
					break;
				case FavoriteMarkType.NotUsed: break;
			}

			switch ( SelectedSeenMarkType.Type ) {
				case SeenMarkType.Seen:
					releases = releases.Where ( a => m_CountWachedVideos.ContainsKey ( a.Id ) && m_CountWachedVideos[a.Id] == ( a.Playlist?.Count () ?? 0 ) );
					break;
				case SeenMarkType.SeenNow:
					releases = releases.Where ( a => m_CountWachedVideos.ContainsKey ( a.Id ) && m_CountWachedVideos[a.Id] > 0 && m_CountWachedVideos[a.Id] < ( a.Playlist?.Count () ?? 0 ) );
					break;
				case SeenMarkType.NotSeen:
					releases = releases.Where ( a => !m_CountWachedVideos.ContainsKey ( a.Id ) || m_CountWachedVideos[a.Id] == 0 );
					break;
				case SeenMarkType.NotUsed: break;
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
				case SectionType.Schedule:
					return releases.Where ( a => m_SchedulesReleases?.SelectMany ( b => b.Value )?.Contains ( a.Id ) ?? true );
				case SectionType.NewReleases:
					var newReleases = m_Changes?.NewReleases ?? Enumerable.Empty<long> ();
					return releases.Where ( a => newReleases.Contains ( a.Id ) );
				case SectionType.NewOnlineSeries:
					var newSeries = m_Changes?.NewOnlineSeries?.Keys.Where ( a => IsFavoriteNotifications ? m_Favorites?.Contains ( a ) ?? true : true ) ?? Enumerable.Empty<long> ();
					return releases.Where ( a => newSeries.Contains ( a.Id ) );
				case SectionType.NewTorrentSeries:
					var newTorrents = m_Changes?.NewTorrentSeries?.Keys.Where ( a => IsFavoriteNotifications ? m_Favorites?.Contains ( a ) ?? true : true ) ?? Enumerable.Empty<long> ();
					return releases.Where ( a => newTorrents.Contains ( a.Id ) );
				case SectionType.HistoryViews:
					return releases.Where ( a => a.LastViewTimestamp > 0 );
				case SectionType.HistoryWatch:
					return releases.Where ( a => a.LastWatchTimestamp > 0 );
				case SectionType.Seens:
					return releases.Where ( a => m_CountWachedVideos.ContainsKey ( a.Id ) && m_CountWachedVideos[a.Id] == ( a.Playlist?.Count () ?? 0 ) );
				case SectionType.PartiallySeen:
					return releases.Where ( a => m_CountWachedVideos.ContainsKey ( a.Id ) && m_CountWachedVideos[a.Id] > 0 && m_CountWachedVideos[a.Id] < ( a.Playlist?.Count () ?? 0 ) );
				case SectionType.NotSeens:
					return releases.Where ( a => !m_CountWachedVideos.ContainsKey ( a.Id ) || m_CountWachedVideos[a.Id] == 0 );
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
				.Select ( MapToReleaseModel );

			return Task.FromResult ( result );
		}

		private readonly Dictionary<int , string> m_DayNames = new Dictionary<int , string> {
			[1] = "Понедельник" ,
			[2] = "Вторник" ,
			[3] = "Среда" ,
			[4] = "Четверг" ,
			[5] = "Пятница" ,
			[6] = "Суббота" ,
			[7] = "Воскресенье"
		};

		private string GetScheduleDayOnRelease ( ReleaseEntity releaseEntity ) {
			if ( m_SchedulesReleases == null ) return "";

			int day = m_SchedulesReleases
				.Where ( a => a.Value.Any ( releaseId => releaseId == releaseEntity.Id ) )
				.Select ( a => a.Key )
				.FirstOrDefault ();
			if ( day == 0 ) return "";

			m_DayNames.TryGetValue ( day , out var dayTitle );
			return dayTitle;
		}

		private int GetScheduleDayIndexOnRelease ( ReleaseEntity releaseEntity ) {
			if ( m_SchedulesReleases == null ) return 10;

			var day = m_SchedulesReleases
				.Where ( a => a.Value.Any ( releaseId => releaseId == releaseEntity.Id ) )
				.Select ( a => a.Key )
				.FirstOrDefault ();
			return day == 0 ? 10 : day;
		}

		private ReleaseModel MapToReleaseModel ( ReleaseEntity a ) {
			var releaseModel = new ReleaseModel {
				Id = a.Id ,
				AddToFavorite = m_Favorites?.Contains ( a.Id ) ?? false ,
				Code = a.Code ,
				Announce = a.Announce ,
				Description = a.Description ,
				Genres = a.Genres != null ? string.Join ( ", " , a.Genres ) : "" ,
				Title = a.Names != null ? a.Names.FirstOrDefault () : "" ,
				Names = a.Names ,
				Poster = m_AnilibriaApiService.GetUrl ( a.Poster ) ,
				Rating = a.Rating ,
				Series = a.Series ,
				Status = a.Status ,
				Type = a.Type ,
				ScheduledOnDay = GetScheduleDayOnRelease ( a ) ,
				Voices = a.Voices != null ? string.Join ( ", " , a.Voices ) : "" ,
				Year = a.Year ,
				Season = a.Season ,
				CountVideoOnline = a.Playlist?.Count () ?? 0 ,
				IsSeen = ( m_CountWachedVideos?.FirstOrDefault ( watchVideo => watchVideo.Key == a.Id ).Value ?? -1 ) == ( a.Playlist?.Count () ?? -2 ) ,
				Torrents = a?.Torrents?.Select (
					torrent =>
						new TorrentModel {
							Completed = torrent.Completed ,
							Quality = $"[{torrent.Quality}]" ,
							Series = torrent.Series ,
							Size = GetFileSize ( torrent.Size ) ,
							Url = torrent.Url
						}
					)?.ToList () ?? Enumerable.Empty<TorrentModel> () ,
				TorrentsCount = a?.Torrents?.Count () ?? 0 ,
				OnlineVideos = a.Playlist?
					.Select (
					videoOnline =>
						new OnlineVideoModel {
							Order = videoOnline.Id ,
							Title = videoOnline.Title ,
							HDQuality = videoOnline.HD ,
							SDQuality = videoOnline.SD ,
							FullHDQuality = videoOnline.FullHD ,
							DownloadableHD = videoOnline.DownloadableHD,
							DownloadableSD = videoOnline.DownloadableSD
						}
					)?.ToList () ?? Enumerable.Empty<OnlineVideoModel> ()
			};
			releaseModel.IsExistsScheduledOnDay = !string.IsNullOrEmpty ( releaseModel.ScheduledOnDay );

			return releaseModel;
		}

		private void ClearReleaseNotification ( long releaseId ) {
			if ( m_Changes == null ) return;

			if ( m_Changes.NewReleases != null && m_Changes.NewReleases.Any () && m_Changes.NewReleases.Any ( a => a == releaseId ) ) {
				m_Changes.NewReleases = m_Changes.NewReleases.Where ( a => a != releaseId ).ToList ();
			}

			if ( m_Changes.NewOnlineSeries != null && m_Changes.NewOnlineSeries.Any () && m_Changes.NewOnlineSeries.Any ( a => a.Key == releaseId ) ) {
				m_Changes.NewOnlineSeries.Remove ( releaseId );
			}

			if ( m_Changes.NewTorrentSeries != null && m_Changes.NewTorrentSeries.Any () && m_Changes.NewTorrentSeries.Any ( a => a.Key == releaseId ) ) {
				m_Changes.NewTorrentSeries.Remove ( releaseId );
			}

			var collection = m_DataContext.GetCollection<ChangesEntity> ();
			collection.Update ( m_Changes );
			RefreshNotification ();
		}

		/// <summary>
		/// Initialize view model.
		/// </summary>
		public void Initialize () {
			RefreshReleases ();
			RefreshNotification ();
		}

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public async void NavigateTo ( object parameter ) {
			m_AnalyticsService.TrackEvent ( "Releases" , "NavigatedTo" , "Simple" );
			RefreshWatchedVideo ();

			var cardLink = parameter as ReleaseCardLinkModel;
			if ( cardLink == null ) return;
			if ( m_AllReleases == null ) return;

			var openedRelease = m_AllReleases.FirstOrDefault ( a => a.Id == cardLink.ReleaseId );
			if ( openedRelease == null ) return;

			OpenedRelease = MapToReleaseModel ( openedRelease );
			IsShowReleaseCard = true;
			FilterByName = OpenedRelease.Title;
			await SaveReleaseViewTimestamp ( OpenedRelease.Id );
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
		/// Grouping collection.
		/// </summary>
		public ObservableCollection<IGrouping<string , ReleaseModel>> GroupingCollection
		{
			get => m_GroupingCollection;
			set => Set ( ref m_GroupingCollection , value );
		}

		/// <summary>
		/// Is multiple select.
		/// </summary>
		public bool IsMultipleSelect
		{
			get => m_IsMultipleSelect;
			set
			{
				if ( !Set ( ref m_IsMultipleSelect , value ) ) return;

				IsShowReleaseCard = false;
			}
		}

		/// <summary>
		/// Show announce.
		/// </summary>
		public bool ShowAnnounce
		{
			get => m_ShowAnnounce;
			set => Set ( ref m_ShowAnnounce , value );
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
				ShowAnnounce = value != null ? !string.IsNullOrEmpty ( value.Announce ) : false;
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
			set
			{
				if ( !Set ( ref m_FilterByGenres , value ) ) return;

				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Filter by years.
		/// </summary>
		public string FilterByYears
		{
			get => m_FilterByYears;
			set
			{
				if ( !Set ( ref m_FilterByYears , value ) ) return;

				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Filter by seasons.
		/// </summary>
		public string FilterBySeasons
		{
			get => m_FilterBySeasons;
			set
			{
				if ( !Set ( ref m_FilterBySeasons , value ) ) return;

				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Filter by voices.
		/// </summary>
		public string FilterByVoicers
		{
			get => m_FilterByVoicers;
			set
			{
				if ( !Set ( ref m_FilterByVoicers , value ) ) return;

				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Filter by type.
		/// </summary>
		public string FilterByType
		{
			get => m_FilterByType;
			set
			{
				if ( !Set ( ref m_FilterByType , value ) ) return;

				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Filter by status.
		/// </summary>
		public string FilterByStatus
		{
			get => m_FilterByStatus;
			set
			{
				if ( !Set ( ref m_FilterByStatus , value ) ) return;

				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Filter by description.
		/// </summary>
		public string FilterByDescription
		{
			get => m_FilterByDescription;
			set
			{
				if ( !Set ( ref m_FilterByDescription , value ) ) return;

				RefreshFilterState ();
			}
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
				var oldSection = m_SelectedSection;
				if ( !Set ( ref m_SelectedSection , value ) ) return;

				if ( value == null ) return;

				if ( oldSection != null ) {
					oldSection.SortingMode = m_SelectedSortingItem.Type;
					oldSection.SortingDirection = m_SelectedSortingDirection.Type;
				}

				if ( m_SelectedSortingItem.Type != value.SortingMode ) {
					m_SelectedSortingItem = m_SortingItems.First ( a => a.Type == value.SortingMode );
					RaisePropertyChanged ( () => SelectedSortingItem );
				}
				if ( m_SelectedSortingDirection.Type != value.SortingDirection ) {
					m_SelectedSortingDirection = m_SortingDirections.First ( a => a.Type == value.SortingDirection );
					RaisePropertyChanged ( () => SelectedSortingDirection );
				}

				GroupedGridVisible = value.Type == SectionType.Schedule;

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
		/// Selected grouped releases.
		/// </summary>
		public ObservableCollection<ReleaseModel> SelectedGroupedReleases
		{
			get => m_SelectedGroupedReleases;
			set => Set ( ref m_SelectedGroupedReleases , value );
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
		/// User model.
		/// </summary>
		public UserModel UserModel
		{
			get => m_UserModel;
			set => Set ( ref m_UserModel , value );
		}

		/// <summary>
		/// Is favorite notifications.
		/// </summary>
		public bool IsFavoriteNotifications
		{
			get => m_isFavoriteNotifications;
			set
			{
				if ( !Set ( ref m_isFavoriteNotifications , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[IsFavoriteNotificationsSettings] = value;

				RefreshNotification ();
			}
		}

		/// <summary>
		/// Is favorite notifications.
		/// </summary>
		public TorrentDownloadModeModel SelectedTorrentDownloadMode
		{
			get => m_SelectedTorrentDownloadMode;
			set
			{
				if ( !Set ( ref m_SelectedTorrentDownloadMode , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[TorrentModeSettings] = (int) value.Mode;
			}
		}

		/// <summary>
		/// Is favorite notifications.
		/// </summary>
		public ObservableCollection<TorrentDownloadModeModel> TorrentDownloadModes
		{
			get => m_TorrentDownloadModes;
			set => Set ( ref m_TorrentDownloadModes , value );
		}

		/// <summary>
		/// Grouped grid visible.
		/// </summary>
		public bool GroupedGridVisible
		{
			get => m_GroupedGridVisible;
			set => Set ( ref m_GroupedGridVisible , value );
		}

		/// <summary>
		/// Filter is filled.
		/// </summary>
		public bool FilterIsFilled
		{
			get => m_FilterIsFilled;
			set => Set ( ref m_FilterIsFilled , value );
		}

		/// <summary>
		/// Open video modes.
		/// </summary>
		public ObservableCollection<OpenVideoModeModel> OpenVideoModes
		{
			get => m_OpenVideoModes;
			set => Set ( ref m_OpenVideoModes , value );
		}

		/// <summary>
		/// Selected open video mode.
		/// </summary>
		public OpenVideoModeModel SelectedOpenVideoMode
		{
			get => m_SelectedOpenVideoMode;
			set
			{
				if ( !Set ( ref m_SelectedOpenVideoMode , value ) ) return;

				ApplicationData.Current.RoamingSettings.Values[OpenVideoSettings] = (int) value.Mode;
			}
		}

		/// <summary>
		/// Seen mark types.
		/// </summary>
		public ObservableCollection<SeenMarkItem> SeenMarkTypes
		{
			get => m_SeenMarkTypes;
			set => Set ( ref m_SeenMarkTypes , value );
		}

		/// <summary>
		/// Selected seen mark type.
		/// </summary>
		public SeenMarkItem SelectedSeenMarkType
		{
			get => m_SelectedSeenMarkType;
			set
			{
				if ( !Set ( ref m_SelectedSeenMarkType , value ) ) return;

				Filter ();
				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Favorite mark types.
		/// </summary>
		public ObservableCollection<FavoriteMarkItem> FavoriteMarkTypes
		{
			get => m_FavoriteMarkTypes;
			set => Set ( ref m_FavoriteMarkTypes , value );
		}

		/// <summary>
		/// Selected favorite mark type.
		/// </summary>
		public FavoriteMarkItem SelectedFavoriteMarkType
		{
			get => m_SelectedFavoriteMarkType;
			set
			{
				if ( !Set ( ref m_SelectedFavoriteMarkType , value ) ) return;

				Filter ();
				RefreshFilterState ();
			}
		}

		/// <summary>
		/// Is dark theme.
		/// </summary>
		public bool IsDarkTheme
		{
			get => m_IsDarkTheme;
			set
			{
				if ( !Set ( ref m_IsDarkTheme , value ) ) return;

				ControlsThemeChanger.ChangeTheme ( value ? ControlsThemeChanger.DarkTheme : ControlsThemeChanger.DefaultTheme );

				ApplicationData.Current.RoamingSettings.Values[IsDarkThemeSettings] = value;
			}
		}

		/// <summary>
		/// Set comments url in web view.
		/// </summary>
		public Action<Uri> SetCommentsUrl
		{
			get;
			set;
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
		/// Signout.
		/// </summary>
		public Action Signout
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

		/// <summary>
		/// Open cross release by hyperlink in text command.
		/// </summary>
		public ICommand OpenCrossReleaseCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Show random release command.
		/// </summary>
		public ICommand ShowRandomReleaseCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Clear filters command.
		/// </summary>
		public ICommand ClearFiltersCommands
		{
			get;
			set;
		}

		/// <summary>
		/// Add status to filters.
		/// </summary>
		public ICommand AddStatusToFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add year to filters.
		/// </summary>
		public ICommand AddYearToFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add genre to filters.
		/// </summary>
		public ICommand AddGenreToFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add voices to filter.
		/// </summary>
		public ICommand AddVoicesToFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Remove seen favorites command.
		/// </summary>
		public ICommand RemoveSeensFavoritesCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add seen mark command.
		/// </summary>
		public ICommand AddSeenMarkCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Remove seen mark command.
		/// </summary>
		public ICommand RemoveSeenMarkCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Remove seen mark command.
		/// </summary>
		public ICommand RemoveAllSeensMarksCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Enable favorite mark filter command.
		/// </summary>
		public ICommand EnableFavoriteMarkFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Enable not favorite mark filter command.
		/// </summary>
		public ICommand EnableNotFavoriteMarkFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Disable favorite mark filter command.
		/// </summary>
		public ICommand DisableFavoriteMarkFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Enable seen mark filter command.
		/// </summary>
		public ICommand EnableSeenMarkFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Enable not seen mark filter command.
		/// </summary>
		public ICommand EnableNotSeenMarkFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Enable seen now mark filter command.
		/// </summary>
		public ICommand EnableSeenNowMarkFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Disable seen mark filter command.
		/// </summary>
		public ICommand DisableSeenMarkFilterCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Enable seen mark command.
		/// </summary>
		public ICommand EnableSeenMarkCardCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Disable seen mark card command.
		/// </summary>
		public ICommand DisableSeenMarkCardCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add download HD command.
		/// </summary>
		public ICommand AddDownloadHdCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Add download SD command.
		/// </summary>
		public ICommand AddDownloadSdCommand
		{
			get;
			set;
		}
	}

}