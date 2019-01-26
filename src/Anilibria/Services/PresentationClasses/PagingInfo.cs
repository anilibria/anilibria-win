namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Paging info.
	/// </summary>
	public class PagingInfo {

		/// <summary>
		/// Page.
		/// </summary>
		public int Page
		{
			get;
			set;
		}

		/// <summary>
		/// Number of records on page.
		/// </summary>
		public int PerPage
		{
			get;
			set;
		}

		/// <summary>
		/// All pages.
		/// </summary>
		public int AllPages
		{
			get;
			set;
		}

		/// <summary>
		/// All items.
		/// </summary>
		public long AllItems
		{
			get;
			set;
		}

	}

}