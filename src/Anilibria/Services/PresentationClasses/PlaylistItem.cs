using System;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Play list item.
	/// </summary>
	public class PlaylistItem {

		/// <summary>
		/// Number video.
		/// </summary>
		public int Id
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
		/// Video in SD quality.
		/// </summary>
		public Uri SD
		{
			get;
			set;
		}

		/// <summary>
		/// Video in HD quality.
		/// </summary>
		public Uri HD
		{
			get;
			set;
		}

		/// <summary>
		/// SD source for download as single video file.
		/// </summary>
		public Uri SrcSd
		{
			get;
			set;
		}

		/// <summary>
		/// HD source for download as single video file.
		/// </summary>
		public Uri SrcHd
		{
			get;
			set;
		}

		/// <summary>
		/// Video in Full HD quality (optional).
		/// </summary>
		public Uri FullHd
		{
			get;
			set;
		}

	}

}