using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Local favorite entity.
	/// </summary>
	public class LocalFavoriteEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Releases.
		/// </summary>
		public ICollection<long> Releases
		{
			get;
			set;
		}

	}

}
