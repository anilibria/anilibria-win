using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;

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
		/// Authentification.
		/// </summary>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		/// <returns>Authorization result.</returns>
		Task<bool> Authentification ( string email , string password );

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
		/// Get mark that user is authorized or not.
		/// </summary>
		bool IsAuthorized ();

		/// <summary>
		/// Get url.
		/// </summary>
		/// <param name="relativeUrl">Relative url.</param>
		/// <returns>Full url.</returns>
		Uri GetUrl ( string relativeUrl );

	}

}
