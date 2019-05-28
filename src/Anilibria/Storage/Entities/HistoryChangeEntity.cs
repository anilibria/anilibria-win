using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// History changes entity.
	/// </summary>
	public class HistoryChangeEntity {

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
		/// Release online series.
		/// </summary>
		public IDictionary<long , int> ReleaseOnlineSeries
		{
			get;
			set;
		}

		/// <summary>
		/// New torrens.
		/// </summary>
		public IDictionary<long , int> NewTorrents
		{
			get;
			set;
		}

		/// <summary>
		/// New torren series.
		/// </summary>
		public IDictionary<long , IDictionary<long , string>> NewTorrentSeries
		{
			get;
			set;
		}

	}

}
