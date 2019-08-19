using Anilibria.MVVM;

namespace Anilibria.Pages.DownloadManagerPage.PresentationClasses {

	/// <summary>
	/// Download video item model.
	/// </summary>
	public class DownloadVideoItemModel : ViewModel {

		private bool m_IsDownloaded;

		private string m_downloadedSize;

		private bool m_isProgress;

		/// <summary>
		/// Identifier.
		/// </summary>
		public string Identifier
		{
			get;
			set;
		}

		/// <summary>
		/// Name.
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
			get => m_downloadedSize;
			set => Set ( ref m_downloadedSize , value );
		}

		/// <summary>
		/// Is downloaded.
		/// </summary>
		public bool IsDownloaded
		{
			get => m_IsDownloaded;
			set => Set ( ref m_IsDownloaded , value );
		}

		/// <summary>
		/// Is progress.
		/// </summary>
		public bool IsProgress
		{
			get => m_isProgress;
			set => Set ( ref m_isProgress , value );
		}

		/// <summary>
		/// Quality.
		/// </summary>
		public string Quality
		{
			get;
			set;
		}

		/// <summary>
		/// Order.
		/// </summary>
		public int Order
		{
			get;
			set;
		}

	}

}