using System;
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

		private void UpdateCachedRelease ( Release release , ReleaseEntity releaseEntity ) {
			var blocked = release.BlockedInfo?.Blocked ?? false;
			var blockedReason = release.BlockedInfo?.Reason ?? "";

			if ( blocked && !releaseEntity.Blocked) {
				//TODO: blocked changes!!!!
			}
			releaseEntity.Blocked = blocked;
			releaseEntity.BlockedReason = blockedReason;

			releaseEntity.Description = release.Description;
			releaseEntity.Rating = release.Favorite?.Rating ?? 0;
			releaseEntity.Type = release.Type;
			releaseEntity.Title = release.Names?.FirstOrDefault () ?? "";
			releaseEntity.Names = release.Names.ToList ();
			releaseEntity.Status = release.Status;
			releaseEntity.Voices = release.Voices.ToList ();
			releaseEntity.Series = release.Series;

			if (releaseEntity.Playlist.Count() != release.Playlist.Count()) {
				//TODO: Playlist changed!!!!
			}
			//TODO: need extra check for torrents on field Series!!!!
			if ( releaseEntity.Torrents.Count () != release.Torrents.Count () ) {
				//TODO: Torrents changed!!!!
			}
		}

		public async Task SynchronizeReleases () {
			try {
				var releases = await m_AnilibriaApiService.GetPage ( 1 , 9000 );

				var collection = m_DataContext.GetCollection<ReleaseEntity> ();

				var cacheReleases = collection.Find ( a => true );

				foreach ( var release in releases ) {
					var cacheRelease = cacheReleases.FirstOrDefault ( a => a.Id == release.Id );

					if ( cacheRelease == null ) {
						collection.Add ( MapToRelease ( release ) );
					}
					else {
						UpdateCachedRelease ( release , cacheRelease );
					}
				}
			}
			catch {
				//TODO: Added logging
			}

		}

		public Task SynchronizeYoutubes () {
			throw new NotImplementedException ();
		}

	}

}
