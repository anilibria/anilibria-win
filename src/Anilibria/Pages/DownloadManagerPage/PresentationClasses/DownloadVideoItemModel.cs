using Anilibria.Storage.Entities;

namespace Anilibria.Pages.DownloadManagerPage.PresentationClasses {
	
	/// <summary>
	/// Download video item model.
	/// </summary>
	public class DownloadVideoItemModel {

		/// <summary>
		/// Identifier.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Downloaded size.
		/// </summary>
		public string DownloadedSize
		{
			get;
			set;
		}

		/// <summary>
		/// Is downloaded.
		/// </summary>
		public bool IsDownloaded
		{
			get;
			set;
		}

		/// <summary>
		/// Quality.
		/// </summary>
		public string Quality
		{
			get;
			set;
		}

	}

}