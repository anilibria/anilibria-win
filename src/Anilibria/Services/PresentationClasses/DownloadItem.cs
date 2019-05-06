using System;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Download item.
	/// </summary>
	public class DownloadItem {

		/// <summary>
		/// Identifier.
		/// </summary>
		public long Id
		{
			get;
			set;
		}

		/// <summary>
		/// Release name.
		/// </summary>
		public string ReleaseName
		{
			get;
			set;
		}

		/// <summary>
		/// Серия.
		/// </summary>
		public string Seria
		{
			get;
			set;
		}

		/// <summary>
		/// Uri.
		/// </summary>
		public Uri Uri
		{
			get;
			set;
		}

	}

}
