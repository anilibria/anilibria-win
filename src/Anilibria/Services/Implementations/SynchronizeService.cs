using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anilibria.Pages.PresentationClasses;
using Anilibria.Services.PresentationClasses;
using Anilibria.Storage;
using Anilibria.Storage.Entities;
using Newtonsoft.Json;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Synchronize service.
	/// </summary>
	public class SynchronizeService : ISynchronizationService {

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IDataContext m_DataContext;

		private readonly IReleasesService m_ReleasesService;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="anilibriaApiService">Anilibria api service.</param>
		/// <param name="dataContext">Data context.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public SynchronizeService ( IAnilibriaApiService anilibriaApiService , IDataContext dataContext , IReleasesService releasesService ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
			m_ReleasesService = releasesService ?? throw new ArgumentNullException ( nameof ( releasesService ) );
		}

		/// <summary>
		/// Synchronize favorites.
		/// </summary>
		public async Task SynchronizeFavorites () {
			if ( !m_AnilibriaApiService.IsAuthorized () ) return;

			try {
				var favorites = await m_AnilibriaApiService.GetUserFavorites ();
				var userModel = m_AnilibriaApiService.GetUserModel ();
				if ( userModel == null ) return;

				var userFavoritesCollection = m_DataContext.GetCollection<UserFavoriteEntity> ();
				var userFavorite = userFavoritesCollection.FirstOrDefault ( a => a.Id == userModel.Id );

				if ( userFavorite != null ) {
					userFavorite.Releases = favorites.ToList ();
					userFavoritesCollection.Update ( userFavorite );
				} else {
					userFavoritesCollection.Add (
						new UserFavoriteEntity {
							Id = userModel.Id ,
							Releases = favorites.ToList ()
						}
					);
				}
			} catch {
				ObserverEvents.FireEvent (
					"showMessage" ,
					new MessageModel {
						Header = "Синхронизация избранного" ,
						Message = "Не удалось выполнить синхронизацию избранного"
					}
				);
			}
		}

		private ReleaseEntity MapToRelease ( Release release ) {
			return new ReleaseEntity {
				Id = release.Id ,
				Code = release.Code ,
				Announce = release.Announce ,
				Description = release.Description ,
				Genres = release.Genres.ToArray () ,
				Rating = release.Favorite?.Rating ?? 0 ,
				Names = release.Names.ToArray () ,
				Poster = release.Poster ,
				Status = release.Status ,
				Type = release.Type ,
				Title = release.Names?.FirstOrDefault () ?? "" ,
				Series = release.Series ,
				Year = release.Year ,
				Season = release.Season ,
				Voices = release.Voices.ToArray () ,
				Timestamp = release.Last ,
				Playlist = release.Playlist?
					.Select (
						a => new PlaylistItemEntity {
							Id = a.Id ,
							Title = a.Title ,
							HD = a.HD ,
							SD = a.SD ,
							FullHD = a.FullHd ,
							DownloadableHD = a.SrcHd ,
							DownloadableSD = a.SrcSd
						}
					)
					.ToArray () ?? Enumerable.Empty<PlaylistItemEntity> () ,
				Torrents = release.Torrents?
					.Select (
						a => new TorrentItemEntity {
							Id = a.Id ,
							Hash = a.Hash ,
							Completed = a.Completed ,
							Url = a.Url ,
							Leechers = a.Leechers ,
							Quality = a.Quality ,
							Seeders = a.Seeders ,
							Series = a.Series ,
							Size = a.Size
						}
					)
					.ToList () ?? Enumerable.Empty<TorrentItemEntity> ()
			};
		}

		private void UpdateCachedRelease ( Release release , ReleaseEntity releaseEntity , ChangesEntity changesEntity ) {
			if ( releaseEntity.Description != release.Description ) releaseEntity.Description = release.Description;
			if ( releaseEntity.Type != release.Type ) releaseEntity.Type = release.Type;
			if ( releaseEntity.Status != release.Status ) releaseEntity.Status = release.Status;
			if ( releaseEntity.Series != release.Series?.TrimEnd () ) releaseEntity.Series = release.Series;
			if ( releaseEntity.Announce != release.Announce ) releaseEntity.Announce = release.Announce;
			if ( releaseEntity.Season != release.Season ) releaseEntity.Season = release.Season;
			releaseEntity.Rating = release.Favorite?.Rating ?? 0;
			releaseEntity.Title = release.Names?.FirstOrDefault () ?? "";
			releaseEntity.Names = release.Names.ToList ();
			releaseEntity.Voices = release.Voices.ToList ();
			releaseEntity.Timestamp = release.Last;
			if ( releaseEntity.Poster != release.Poster ) {
				releaseEntity.Poster = release.Poster;
				//invalidate poster's cache
				if ( m_DataContext.IsFileExists ( "Poster" , release.Id ) ) m_DataContext.DeleteFile ( "Poster" , release.Id );
			}

			if ( releaseEntity.Playlist.Count () != release.Playlist.Count () ) {
				if ( !changesEntity.NewOnlineSeries.ContainsKey ( release.Id ) ) changesEntity.NewOnlineSeries.Add ( release.Id , releaseEntity.Playlist.Count () );
			}

			releaseEntity.Playlist = release.Playlist
				.Select (
					a =>
						new PlaylistItemEntity {
							Id = a.Id ,
							HD = a.HD ,
							SD = a.SD ,
							FullHD = a.FullHd ,
							Title = a.Title ,
							DownloadableHD = a.SrcHd ,
							DownloadableSD = a.SrcSd
						}
					)
				.ToList ();

			if ( releaseEntity.Torrents.Count () != release.Torrents.Count () ) {
				if ( !changesEntity.NewTorrents.ContainsKey ( release.Id ) ) changesEntity.NewTorrents.Add ( release.Id , releaseEntity.Torrents.Count () );
			}

			for ( var i = 0 ; i < releaseEntity.Torrents.Count () ; i++ ) {
				var oldTorrent = releaseEntity.Torrents.ElementAt ( i );
				var newTorrent = release.Torrents.ElementAtOrDefault ( i );
				if ( newTorrent == null ) break;

				if ( oldTorrent.Size != newTorrent.Size ) {
					if ( !changesEntity.NewTorrentSeries.ContainsKey ( release.Id ) ) changesEntity.NewTorrentSeries.Add ( release.Id , new Dictionary<long , string> () );
					if ( !changesEntity.NewTorrentSeries[release.Id].ContainsKey ( oldTorrent.Id ) ) changesEntity.NewTorrentSeries[release.Id].Add ( oldTorrent.Id , oldTorrent.Size.ToString () ?? "" );
				}
			}

			releaseEntity.Torrents = release.Torrents
				.Select (
					a => new TorrentItemEntity {
						Id = a.Id ,
						Completed = a.Completed ,
						Hash = a.Hash ,
						Leechers = a.Leechers ,
						Quality = a.Quality ,
						Seeders = a.Seeders ,
						Series = a.Series ,
						Size = a.Size ,
						Url = a.Url
					}
				)
				.ToList ();
		}

		public async Task SynchronizeReleasesFromContent ( string content ) {
			await SynchronizeReleasesFromSource (
				() => Task.FromResult ( JsonConvert.DeserializeObject<IEnumerable<Release>> ( content ) )
			);
		}

		public async Task SynchronizeReleases () {
			await SynchronizeReleasesFromSource (
				async () => {
					var releases = await m_AnilibriaApiService.GetPage ( 1 , 2000 );
					var schedules = await m_AnilibriaApiService.GetSchedule ();

					SaveSchedule ( schedules );

					return releases;
				}
			);
		}

		private async Task SynchronizeReleasesFromSource ( Func<Task<IEnumerable<Release>>> loadContentHandler ) {
			try {
				var firstRun = !m_ReleasesService.GetReleases ().Any ();

				var releases = await loadContentHandler ();

				var cacheReleases = m_ReleasesService.GetReleases ().ToList ();

				var changesCollection = m_DataContext.GetCollection<ChangesEntity> ();
				var changes = GetChanges ( changesCollection );

				if ( changes != null ) SaveHistoryChanges ( changes , cacheReleases );

				var cacheReleasesDictionary = cacheReleases.ToDictionary ( a => a.Id );

				foreach ( var release in releases ) {
					cacheReleasesDictionary.TryGetValue ( release.Id , out var cacheRelease );

					if ( cacheRelease == null ) {
						cacheReleases.Add ( MapToRelease ( release ) );
						if ( cacheReleases.Count () > 0 ) {
							if ( changes.NewReleases == null ) {
								changes.NewReleases = new List<long> { release.Id };
							} else {
								var newReleases = changes.NewReleases.ToList ();
								newReleases.Add ( release.Id );
								changes.NewReleases = newReleases;
							}
						}
					} else {
						UpdateCachedRelease ( release , cacheRelease , changes );
					}
				}

				m_ReleasesService.SetReleases ( cacheReleases );
				await m_ReleasesService.SaveReleases ();

				if ( !firstRun ) changesCollection.Update ( changes );

				ObserverEvents.FireEvent ( "synchronizedReleases" , null );
				ObserverEvents.FireEvent (
					"showMessage" ,
					new MessageModel {
						Header = "Синхронизация релизов" ,
						Message = "Синхронизация релизов успешно выполнена"
					}
				);
			} catch {
				ObserverEvents.FireEvent (
					"showMessage" ,
					new MessageModel {
						Header = "Синхронизация релизов" ,
						Message = "Не удалось выполнить синхронизацию релизов"
					}
				);
			}
		}

		private void SaveSchedule ( IDictionary<int , IEnumerable<long>> schedules ) {
			var collection = m_DataContext.GetCollection<ScheduleEntity> ();
			var entity = collection.FirstOrDefault ();
			if ( entity == null ) {
				collection.Add (
					new ScheduleEntity {
						Days = schedules
					}
				);
			} else {
				entity.Days = schedules;
				collection.Update ( entity );
			}
		}

		private ChangesEntity GetChanges ( IEntityCollection<ChangesEntity> changesCollection ) {
			var changes = changesCollection.FirstOrDefault ();
			if ( changes == null ) {
				changes = new ChangesEntity {
					NewOnlineSeries = new Dictionary<long , int> () ,
					NewReleases = new List<long> () ,
					NewTorrents = new Dictionary<long , int> () ,
					NewTorrentSeries = new Dictionary<long , IDictionary<long , string>> ()
				};
				changesCollection.Add ( changes );
			}

			return changes;
		}

		private void SaveHistoryChanges ( ChangesEntity changes , IEnumerable<ReleaseEntity> releaseCollection ) {
			var allReleases = releaseCollection.ToList ();
			var collection = m_DataContext.GetCollection<HistoryChangeEntity> ();
			var historyChanges = collection.FirstOrDefault ();
			var releasesIds = changes.NewOnlineSeries.Select ( a => a.Key );
			var seriesReleases = allReleases.Where ( a => releasesIds.Contains ( a.Id ) );
			var releaseOnlineSeries = new Dictionary<long , int> ();
			foreach ( var seriesRelease in seriesReleases ) releaseOnlineSeries.Add ( seriesRelease.Id , seriesRelease.Playlist?.Count () ?? 0 );
			if ( historyChanges == null ) {
				historyChanges = new HistoryChangeEntity {
					NewOnlineSeries = changes.NewOnlineSeries ?? new Dictionary<long , int> () ,
					NewReleases = changes.NewReleases ?? new List<long> () ,
					NewTorrents = changes.NewTorrents ?? new Dictionary<long , int> () ,
					NewTorrentSeries = changes.NewTorrentSeries ?? new Dictionary<long , IDictionary<long , string>> () ,
					ReleaseOnlineSeries = releaseOnlineSeries
				};
				collection.Add ( historyChanges );
			} else {
				historyChanges.NewOnlineSeries = changes.NewOnlineSeries ?? new Dictionary<long , int> ();
				historyChanges.NewReleases = changes.NewReleases ?? new List<long> ();
				historyChanges.NewTorrents = changes.NewTorrents ?? new Dictionary<long , int> ();
				historyChanges.NewTorrentSeries = changes.NewTorrentSeries ?? new Dictionary<long , IDictionary<long , string>> ();
				historyChanges.ReleaseOnlineSeries = releaseOnlineSeries;
				collection.Update ( historyChanges );
			}
		}

		public Task SynchronizeYoutubes () {
			throw new NotImplementedException ();
		}

	}

}
