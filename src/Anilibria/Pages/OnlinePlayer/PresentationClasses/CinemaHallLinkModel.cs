using System.Collections.Generic;

namespace Anilibria.Pages.OnlinePlayer.PresentationClasses {

	/// <summary>
	/// Cinema hall link model.
	/// </summary>
	public class CinemaHallLinkModel {

		/// <summary>
		/// Releases.
		/// </summary>
		public IEnumerable<long> Releases
		{
			get;
			set;
		}

	}

}
