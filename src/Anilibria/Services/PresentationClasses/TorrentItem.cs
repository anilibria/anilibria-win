namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Torrent item.
	/// </summary>
	public class TorrentItem {

		/// <summary>
		/// Identifier.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Hash.
		/// </summary>
		public string Hash
		{
			get;
			set;
		}

		/// <summary>
		/// Number of leechers.
		/// </summary>
		public int Leechers
		{
			get;
			set;
		}

		/// <summary>
		/// Number of seeders.
		/// </summary>
		public int Seeders
		{
			get;
			set;
		}

		/// <summary>
		/// Number of leechers that completed torrent.
		/// </summary>
		public int Completed
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

		/// <summary>
		/// Number of series from all series.
		/// </summary>
		public string Series
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
		/// Torrent File URL.
		/// </summary>
		public string Url
		{
			get;
			set;
		}

	}

}