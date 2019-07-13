using System;
using Anilibria.MVVM;

namespace Anilibria.Pages.DownloadManagerPage.PresentationClasses {

	/// <summary>
	/// Download item model.
	/// </summary>
	public class DownloadItemModel : ViewModel {
		
		private int m_DownloadedVideos;
		
		private int m_DownloadingVideos;
		
		private int m_NotDownloadedVideos;

		/// <summary>
		/// Release identifier.
		/// </summary>
		public long ReleaseId
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

		/// <summary>
		/// Active.
		/// </summary>
		public bool Active
		{
			get;
			set;
		}

		/// <summary>
		/// Poster.
		/// </summary>
		public Uri Poster
		{
			get;
			set;
		}

		/// <summary>
		/// Title.
		/// </summary>
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// Downloaded videos.
		/// </summary>
		public int DownloadedVideos
		{
			get => m_DownloadedVideos;
			set => Set ( ref m_DownloadedVideos , value );
		}

		/// <summary>
		/// Not downloaded videos.
		/// </summary>
		public int NotDownloadedVideos
		{
			get => m_NotDownloadedVideos;
			set => Set ( ref m_NotDownloadedVideos , value );
		}

		/// <summary>
		/// Downloading videos.
		/// </summary>
		public int DownloadingVideos
		{
			get => m_DownloadingVideos;
			set => Set ( ref m_DownloadingVideos , value );
		}

	}

}
