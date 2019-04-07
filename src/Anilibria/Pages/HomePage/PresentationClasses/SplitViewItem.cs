using System;

namespace Anilibria.Pages.HomePage.PresentationClasses {

	/// <summary>
	/// Split view item.
	/// </summary>
	public class SplitViewItem {

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Page.
		/// </summary>
		public string Page
		{
			get;
			set;
		}

		/// <summary>
		/// Icon.
		/// </summary>
		public string Icon
		{
			get;
			set;
		}

		/// <summary>
		/// Icon in uri respresent.
		/// </summary>
		public Uri IconUri
		{
			get;
			set;
		}

		/// <summary>
		/// Is visible.
		/// </summary>
		public Func<bool> IsVisible
		{
			get;
			set;
		}

	}

}
