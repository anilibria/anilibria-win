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
		/// Release identifier.
		/// </summary>
		public long ReleaseId
		{
			get;
			set;
		}

		/// <summary>
		/// Seria.
		/// </summary>
		public int Seria
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

		/// <summary>
		/// Progress.
		/// </summary>
		public double Progress
		{
			get;
			set;
		}

		/// <summary>
		/// Position.
		/// </summary>
		public long Position
		{
			get;
			set;
		}

		/// <summary>
		/// Size.
		/// </summary>
		public long Size
		{
			get;
			set;
		}

		/// <summary>
		/// File name.
		/// </summary>
		public string FileName
		{
			get;
			set;
		}

		/// <summary>
		/// Download path.
		/// </summary>
		public Uri DownloadPath
		{
			get;
			set;
		}

		/// <summary>
		/// Quality.
		/// </summary>
		public DownloadVideoQuality Quality
		{
			get;
			set;
		}

	}

}
