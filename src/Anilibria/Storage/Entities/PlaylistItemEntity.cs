using System;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Playlist item entity.
	/// </summary>
	public class PlaylistItemEntity {

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
		/// Full in HD quality.
		/// </summary>
		public Uri FullHD
		{
			get;
			set;
		}

		/// <summary>
		/// Downloadable HD.
		/// </summary>
		public Uri DownloadableHD
		{
			get;
			set;
		}

		/// <summary>
		/// Downloadable SD.
		/// </summary>
		public Uri DownloadableSD
		{
			get;
			set;
		}

	}

}