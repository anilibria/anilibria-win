using System;
using System.Collections.ObjectModel;
using Anilibria.MVVM;

namespace Anilibria.Pages.DownloadManagerPage.PresentationClasses {

	/// <summary>
	/// Download item model.
	/// </summary>
	public class DownloadItemModel : ViewModel {

		private int m_DownloadedHdVideos;

		private int m_DownloadingVideos;

		private int m_NotDownloadedVideos;

		private int m_CurrentDownloadVideo;

		private int m_DownloadProgress;

		private string m_DownloadSpeed;

		private int m_DownloadedSdVideos;

		private bool m_Active;

		private int m_Order;

		private ObservableCollection<DownloadVideoItemModel> m_Videos;

		private string m_AllDownloadedSize;

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
			get => m_Order;
			set => Set ( ref m_Order , value );
		}

		/// <summary>
		/// Active.
		/// </summary>
		public bool Active
		{
			get => m_Active;
			set => Set ( ref m_Active , value );
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
		/// Downloaded Hd videos.
		/// </summary>
		public int DownloadedHdVideos
		{
			get => m_DownloadedHdVideos;
			set => Set ( ref m_DownloadedHdVideos , value );
		}

		/// <summary>
		/// All download size.
		/// </summary>
		public string AllDownloadedSize
		{
			get => m_AllDownloadedSize;
			set => Set ( ref m_AllDownloadedSize , value );
		}

		/// <summary>
		/// Downloaded Sd videos.
		/// </summary>
		public int DownloadedSdVideos
		{
			get => m_DownloadedSdVideos;
			set => Set ( ref m_DownloadedSdVideos , value );
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

		/// <summary>
		/// Download pogress.
		/// </summary>
		public int DownloadProgress
		{
			get => m_DownloadProgress;
			set => Set ( ref m_DownloadProgress , value );
		}

		/// <summary>
		/// Curretn download video.
		/// </summary>
		public int CurrentDownloadVideo
		{
			get => m_CurrentDownloadVideo;
			set => Set ( ref m_CurrentDownloadVideo , value );
		}

		/// <summary>
		/// Download speed.
		/// </summary>
		public string DownloadSpeed
		{
			get => m_DownloadSpeed;
			set => Set ( ref m_DownloadSpeed , value );
		}

		/// <summary>
		/// Downloading/downloaded videos.
		/// </summary>
		public ObservableCollection<DownloadVideoItemModel> Videos
		{
			get => m_Videos;
			set => Set ( ref m_Videos , value );
		}

	}

}
