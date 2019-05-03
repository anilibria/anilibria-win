using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;
using Windows.Storage;

namespace Anilibria.Services {

	/// <summary>
	/// Anilibria api service.
	/// </summary>
	public interface IAnilibriaApiService {

		/// <summary>
		/// Get page from releases.
		/// </summary>
		/// <param name="page">Page number.</param>
		/// <param name="pageSize">Page size.</param>
		/// <param name="name">Filter by name if value presented.</param>
		/// <returns>Release's collection.</returns>
		Task<IEnumerable<Release>> GetPage ( int page , int pageSize , string name = default ( string ) );

		/// <summary>
		/// Get touched releases.
		/// </summary>
		/// <returns>Release's collection.</returns>
		Task<IEnumerable<TouchReleaseModel>> GetTouchedReleases ();

		/// <summary>
		/// Get schedule.
		/// </summary>
		/// <returns>Schedule data.</returns>
		Task<IDictionary<int , IEnumerable<long>>> GetSchedule ();

		/// <summary>
		/// Get youtube videos.
		/// </summary>
		/// <param name="page">Page number.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Youtube videos.</returns>
		Task<IEnumerable<YoutubeModel>> GetYoutubeVideosPage ( int page , int pageSize );

		/// <summary>
		/// Get user favorites.
		/// </summary>
		/// <returns>Favorites releases collection.</returns>
		Task<IEnumerable<long>> GetUserFavorites ();

		/// <summary>
		/// Authentification.
		/// </summary>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		/// <param name="fa2code">2fa code.</param>
		/// <returns>Authorization result.</returns>
		Task<(bool, string)> Authentification ( string email , string password, string fa2code);

		/// <summary>
		/// Logout.
		/// </summary>
		/// <returns></returns>
		Task Logout ();

		/// <summary>
		/// Get user data.
		/// </summary>
		/// <returns>Information about user profile.</returns>
		Task<UserModel> GetUserData ();

		/// <summary>
		/// Add to user favorites.
		/// </summary>
		Task AddUserFavorites ( long id );

		/// <summary>
		/// Remove from user favorites.
		/// </summary>
		Task RemoveUserFavorites ( long id );

		/// <summary>
		/// Get mark that user is authorized or not.
		/// </summary>
		bool IsAuthorized ();

		/// <summary>
		/// Get url.
		/// </summary>
		/// <param name="relativeUrl">Relative url.</param>
		/// <returns>Full url.</returns>
		Uri GetUrl ( string relativeUrl );

		/// <summary>
		/// User model.
		/// </summary>
		UserModel GetUserModel ();

		/// <summary>
		/// Download torrent.
		/// </summary>
		/// <param name="torrentUri">Torrent uri.</param>
		/// <returns>Torrent path.</returns>
		Task<StorageFile> DownloadTorrent ( string torrentUri );

		/// <summary>
		/// Clear user session.
		/// </summary>
		void ClearSession ();

	}

}
