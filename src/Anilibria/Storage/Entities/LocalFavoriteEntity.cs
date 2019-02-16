using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Local favorite entity.
	/// </summary>
	public class LocalFavoriteEntity {

		/// <summary>
		/// Releases.
		/// </summary>
		public IEnumerable<long> Releases
		{
			get;
			set;
		}

	}

}
