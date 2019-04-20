using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Schedule entity.
	/// </summary>
	public class ScheduleEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Days.
		/// </summary>
		public IDictionary<int, IEnumerable<long>> Days
		{
			get;
			set;
		}

	}

}
