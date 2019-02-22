using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Changes for online video, torrent files and releases.
	/// </summary>
	public class ChangesEntity {

		/// <summary>
		/// identifier.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// New releases.
		/// </summary>
		public IEnumerable<long> NewReleases
		{
			get;
			set;
		}

		/// <summary>
		/// New online series.
		/// </summary>
		public IDictionary<long , int> NewOnlineSeries
		{
			get;
			set;
		}

		/// <summary>
		/// New torrent series.
		/// </summary>
		public IDictionary<long , int> NewTorrentSeries
		{
			get;
			set;
		}

	}

}
