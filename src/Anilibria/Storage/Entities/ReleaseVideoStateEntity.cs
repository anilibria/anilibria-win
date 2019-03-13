using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Entity for keep state in player for releases.
	/// </summary>
	public class ReleaseVideoStateEntity {

		/// <summary>
		/// Release identifier.
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
		/// State for online video in release.
		/// </summary>
		public ICollection<VideoStateEntity> VideoStates
		{
			get;
			set;
		}

	}

}
