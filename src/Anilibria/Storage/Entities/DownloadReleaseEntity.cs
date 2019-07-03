using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Download release entity.
	/// </summary>
	public class DownloadReleaseEntity {

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
		/// Videos.
		/// </summary>
		public IEnumerable<DownloadReleaseVideoEntity> Videos
		{
			get;
			set;
		}

	}

}
