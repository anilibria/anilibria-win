using System.Collections.Generic;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Paging list.
	/// </summary>
	public class PagingList<T> {

		/// <summary>
		/// Items.
		/// </summary>
		public IEnumerable<T> Items
		{
			get;
			set;
		}

		/// <summary>
		/// Pagination.
		/// </summary>
		public PagingInfo Pagination
		{
			get;
			set;
		}

	}

}
