using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Cinema hall release.
	/// </summary>
	public class CinemaHallReleaseEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// New releases.
		/// </summary>
		public IEnumerable<long> Releases
		{
			get;
			set;
		}

	}

}
