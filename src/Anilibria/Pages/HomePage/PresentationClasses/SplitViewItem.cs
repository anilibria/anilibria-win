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

		/// <summary>
		/// Is release page.
		/// </summary>
		public bool IsReleasePage
		{
			get;
			set;
		}

		/// <summary>
		/// Is online player.
		/// </summary>
		public bool IsOnlinePlayer
		{
			get;
			set;
		}

		/// <summary>
		/// Is youtube page.
		/// </summary>
		public bool IsYoutubePage
		{
			get;
			set;
		}

		/// <summary>
		/// Is donate page.
		/// </summary>
		public bool IsDonatePage
		{
			get;
			set;
		}

		/// <summary>
		/// Is singin page.
		/// </summary>
		public bool IsSigninPage
		{
			get;
			set;
		}

		/// <summary>
		/// Download manager page.
		/// </summary>
		public bool IsDownloadManagerPage
		{
			get;
			set;
		}

		/// <summary>
		/// Is cinema hall page.
		/// </summary>
		public bool IsCinemaHallPage
		{
			get;
			set;
		}

		public bool IsMaintenancePage {
			get;
			set;
		}

	}

}
