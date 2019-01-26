using System.Collections.Generic;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Paging list.
	/// </summary>
	public class PagingList {

		/// <summary>
		/// Items.
		/// </summary>
		public IEnumerable<Release> Items
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
