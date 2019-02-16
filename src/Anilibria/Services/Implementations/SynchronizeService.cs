using System;
using System.Linq;
using System.Threading.Tasks;
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

		public Task SynchronizeReleases () {
			throw new NotImplementedException ();
		}

		public Task SynchronizeYoutubes () {
			throw new NotImplementedException ();
		}

	}

}
