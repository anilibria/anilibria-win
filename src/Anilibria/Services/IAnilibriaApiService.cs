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
		/// <returns>Release's collection.</returns>
		Task<IEnumerable<Release>> GetPage ( int page , int pageSize );

		/// <summary>
		/// Get url.
		/// </summary>
		/// <param name="relativeUrl">Relative url.</param>
		/// <returns>Full url.</returns>
		Uri GetUrl ( string relativeUrl );

	}

}
