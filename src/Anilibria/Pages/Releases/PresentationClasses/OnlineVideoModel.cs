using System;

namespace Anilibria.Pages.Releases.PresentationClasses {

	/// <summary>
	/// Online video model.
	/// </summary>
	public class OnlineVideoModel {

		/// <summary>
		/// Order.
		/// </summary>
		public int Order
		{
			get;
			set;
		}

		/// <summary>
		/// Video title.
		/// </summary>
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// Link to video in HD quality.
		/// </summary>
		public Uri HDQuality
		{
			get;
			set;
		}

		/// <summary>
		/// Link to video in SD quality.
		/// </summary>
		public Uri SDQuality
		{
			get;
			set;
		}

		/// <summary>
		/// Link to video in FullHD quality.
		/// </summary>
		public Uri FullHDQuality
		{
			get;
			set;
		}

	}

}