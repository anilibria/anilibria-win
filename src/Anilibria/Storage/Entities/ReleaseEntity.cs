using System;
using System.Collections.Generic;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Release entity.
	/// </summary>
	public class ReleaseEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public long Id
		{
			get;
			set;
		}

		/// <summary>
		/// Code.
		/// </summary>
		public string Code
		{
			get;
			set;
		}

		/// <summary>
		/// Announce.
		/// </summary>
		public string Announce
		{
			get;
			set;
		}

		/// <summary>
		/// Main name.
		/// </summary>
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// Names.
		/// </summary>
		public IEnumerable<string> Names
		{
			get;
			set;
		}

		/// <summary>
		/// Series.
		/// </summary>
		public string Series
		{
			get;
			set;
		}

		/// <summary>
		/// Poster for list.
		/// </summary>
		public string Poster
		{
			get;
			set;
		}

		/// <summary>
		/// Rating.
		/// </summary>
		public long Rating
		{
			get;
			set;
		}

		/// <summary>
		/// URL for external web player.
		/// </summary>
		public Uri Moon
		{
			get;
			set;
		}

		/// <summary>
		/// Status.
		/// </summary>
		public string Status
		{
			get;
			set;
		}

		/// <summary>
		/// Type (eg ТВ (∞ эп.), 25 мин.).
		/// </summary>
		public string Type
		{
			get;
			set;
		}

		/// <summary>
		/// Season.
		/// </summary>
		public string Season
		{
			get;
			set;
		}

		/// <summary>
		/// Genres.
		/// </summary>
		public IEnumerable<string> Genres
		{
			get;
			set;
		}

		/// <summary>
		/// Voices.
		/// </summary>
		public IEnumerable<string> Voices
		{
			get;
			set;
		}

		/// <summary>
		/// Year.
		/// </summary>
		public string Year
		{
			get;
			set;
		}

		/// <summary>
		/// Timestamp.
		/// </summary>
		public string Timestamp
		{
			get;
			set;
		}

		/// <summary>
		/// Description.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Last view timestamp.
		/// </summary>
		public long LastViewTimestamp
		{
			get;
			set;
		}

		/// <summary>
		/// Last watch timestamp.
		/// </summary>
		public long LastWatchTimestamp
		{
			get;
			set;
		}

		/// <summary>
		/// Playlist.
		/// </summary>
		public IEnumerable<PlaylistItemEntity> Playlist
		{
			get;
			set;
		}

		/// <summary>
		/// Torrents.
		/// </summary>
		public IEnumerable<TorrentItemEntity> Torrents
		{
			get;
			set;
		}

	}

}
