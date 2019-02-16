using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// User favorite entity.
	/// </summary>
	public class UserFavoriteEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public long Id
		{
			get;
			set;
		}

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
