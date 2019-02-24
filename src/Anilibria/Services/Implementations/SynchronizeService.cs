using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;
using Anilibria.Storage;
using Anilibria.Storage.Entities;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Synchronize service.
	/// </summary>
	public class SynchronizeService : ISynchronizationService {

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IDataContext m_DataContext;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="anilibriaApiService">Anilibria api service.</param>
		/// <param name="dataContext">Data context.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public SynchronizeService ( IAnilibriaApiService anilibriaApiService , IDataContext dataContext ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_DataContext = dataContext ?? throw new ArgumentNullException ( nameof ( dataContext ) );
		}

		/// <summary>
		/// Synchronize favorites.
		/// </summary>
		public async Task SynchronizeFavorites () {
			if ( !m_AnilibriaApiService.IsAuthorized () ) return;

			try {
				var favorites = await m_AnilibriaApiService.GetUserFavorites ();
				var userModel = await m_AnilibriaApiService.GetUserData ();

				var userFavoritesCollection = m_DataContext.GetCollection<UserFavoriteEntity> ();
				var userFavorite = userFavoritesCollection.FirstOrDefault ( a => a.Id == userModel.Id );

				if ( userFavorite != null ) {
					userFavorite.Releases = favorites.ToList ();
					userFavoritesCollection.Update ( userFavorite );
				}
				else {
					userFavoritesCollection.Add (
						new UserFavoriteEntity {
							Id = userModel.Id ,
							Releases = favorites.ToList ()
						}
					);
				}
			}
			catch {
				//TODO: Added logging
			}
		}

		private ReleaseEntity MapToRelease ( Release release ) {
			return new ReleaseEntity {
				Id = release.Id ,
				Code = release.Code ,
				Description = release.Description ,
				Genres = release.Genres.ToArray () ,
				Moon = release.Moon ,
				Rating = release.Favorite?.Rating ?? 0 ,
				Blocked = release.BlockedInfo?.Blocked ?? false ,
				BlockedReason = release.BlockedInfo?.Reason ?? "" ,
				Names = release.Names.ToArray () ,
				Poster = release.Poster ,
				Status = release.Status ,
				Type = release.Type ,
				Title = release.Names?.FirstOrDefault () ?? "" ,
				Series = release.Series ,
				Year = release.Year ,
				Voices = release.Voices.ToArray () ,
				Timestamp = release.Last ,
				Playlist = release.Playlist?
					.Select (
						a => new PlaylistItemEntity {
							Id = a.Id ,
							Title = a.Title ,
							HD = a.HD ,
							SD = a.SD
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
			var blocked = release.BlockedInfo?.Blocked ?? false;
			var blockedReason = release.BlockedInfo?.Reason ?? "";

			if ( blocked && !releaseEntity.Blocked ) {
				//TODO: blocked changes!!!!
			}
			releaseEntity.Blocked = blocked;
			releaseEntity.BlockedReason = blockedReason;

			if ( releaseEntity.Description != release.Description ) releaseEntity.Description = release.Description;
			if ( releaseEntity.Type != release.Type ) releaseEntity.Type = release.Type;
			if ( releaseEntity.Status != release.Status ) releaseEntity.Status = release.Status;
			if ( releaseEntity.Series != release.Series?.TrimEnd () ) releaseEntity.Series = release.Series;
			releaseEntity.Rating = release.Favorite?.Rating ?? 0;
			releaseEntity.Title = release.Names?.FirstOrDefault () ?? "";
			releaseEntity.Names = release.Names.ToList ();
			releaseEntity.Voices = release.Voices.ToList ();
			releaseEntity.Timestamp = release.Last;

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
							Title = a.Title
						}
					)
				.ToList ();

			if ( releaseEntity.Torrents.Count () != release.Torrents.Count () ) {
				if ( !changesEntity.NewTorrents.ContainsKey ( release.Id ) ) changesEntity.NewTorrents.Add ( release.Id , releaseEntity.Torrents.Count () );
			}

			var torrentSeries = releaseEntity.Torrents.Select ( oldTorrent => (oldTorretnSerie: oldTorrent, newTorrentSerie: release.Torrents.FirstOrDefault ( newTorrent => newTorrent.Id == oldTorrent.Id )) ).ToList ();

			foreach ( var (oldTorretnSerie, newTorrentSerie) in torrentSeries ) {
				if ( oldTorretnSerie.Series != newTorrentSerie.Series.TrimEnd () ) {
					if ( !changesEntity.NewTorrentSeries.ContainsKey ( release.Id ) ) changesEntity.NewTorrentSeries.Add ( release.Id , new Dictionary<long , string> () );
					if ( !changesEntity.NewTorrentSeries[release.Id].ContainsKey ( oldTorretnSerie.Id ) ) changesEntity.NewTorrentSeries[release.Id].Add ( oldTorretnSerie.Id , oldTorretnSerie.Series );
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

		public async Task SynchronizeReleases () {
			try {
				var releases = await m_AnilibriaApiService.GetPage ( 1 , 2000 );

				var collection = m_DataContext.GetCollection<ReleaseEntity> ();
				var changesCollection = m_DataContext.GetCollection<ChangesEntity> ();
				var changes = GetChanges ( changesCollection );

				var cacheReleases = collection.Find ( a => true );

				var addReleases = new List<ReleaseEntity> ();
				var updatedReleases = new List<ReleaseEntity> ();

				var cacheReleasesDictionary = cacheReleases.ToDictionary ( a => a.Id );

				var index = 1;
				foreach ( var release in releases ) {
					cacheReleasesDictionary.TryGetValue ( release.Id , out var cacheRelease );

					if ( cacheRelease == null ) {
						addReleases.Add ( MapToRelease ( release ) );
					}
					else {
						UpdateCachedRelease ( release , cacheRelease , changes );
						updatedReleases.Add ( cacheRelease );
					}
					index++;
				}
				if ( addReleases.Any () ) collection.AddRange ( addReleases );
				if ( updatedReleases.Any () ) collection.Update ( updatedReleases );
				changesCollection.Update ( changes );
			}
			catch {
				//TODO: Added logging
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

		public Task SynchronizeYoutubes () {
			throw new NotImplementedException ();
		}

	}

}
