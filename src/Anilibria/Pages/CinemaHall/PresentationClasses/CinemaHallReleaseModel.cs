using System;

namespace Anilibria.Pages.CinemaHall.PresentationClasses {

	/// <summary>
	/// Cinema hall release model.
	/// </summary>
	public class CinemaHallReleaseModel {

		/// <summary>
		/// Release identifier.
		/// </summary>
		public long ReleaseId
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
		/// Poster.
		/// </summary>
		public Uri Porter
		{
			get;
			set;
		}

	}

}
