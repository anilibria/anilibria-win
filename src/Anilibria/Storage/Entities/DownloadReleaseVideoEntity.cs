namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Download release video entity.
	/// </summary>
	public class DownloadReleaseVideoEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Downloaded size.
		/// </summary>
		public long DownloadedSize
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
		/// Downloaded page.
		/// </summary>
		public string DownloadedPath
		{
			get;
			set;
		}

		/// <summary>
		/// Download url.
		/// </summary>
		public string DownloadUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Quality.
		/// </summary>
		public VideoQuality Quality
		{
			get;
			set;
		}

	}

}
