using System.Collections.Generic;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;

namespace Anilibria.Services {

	/// <summary>
	/// Anilibria api service.
	/// </summary>
	public interface IAnilibriaApiService {

		/// <summary>
		/// Get catalog.
		/// </summary>
		/// <returns>Release's collection.</returns>
		Task<IEnumerable<Release>> GetCatalog ();

	}

}
