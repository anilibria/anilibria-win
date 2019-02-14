using System;

namespace Anilibria.Pages.Youtube.PresentationClasses {

	/// <summary>
	/// Youtube vide model.
	/// </summary>
	public class YoutubeVideoModel {

		/// <summary>
		/// Identifier.
		/// </summary>
		public long Id
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
		/// Image.
		/// </summary>
		public Uri Image
		{
			get;
			set;
		}

		/// <summary>
		/// Video url.
		/// </summary>
		public Uri VideoUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Views.
		/// </summary>
		public long Views
		{
			get;
			set;
		}

		/// <summary>
		/// Comments.
		/// </summary>
		public long Comments
		{
			get;
			set;
		}

		/// <summary>
		/// Timestamp.
		/// </summary>
		public long Timestamp
		{
			get;
			set;
		}

	}

}
