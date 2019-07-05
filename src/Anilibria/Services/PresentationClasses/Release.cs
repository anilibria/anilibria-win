using System;
using System.Collections.Generic;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Release.
	/// </summary>
	public class Release {

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
		/// Favorite info.
		/// </summary>
		public Favorite Favorite
		{
			get;
			set;
		}

		/// <summary>
		/// Timestamp last updated.
		/// </summary>
		public string Last
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
		/// Season.
		/// </summary>
		public string Season
		{
			get;
			set;
		}

		/// <summary>
		/// Day.
		/// </summary>
		public string Day
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
		/// Blocked info.
		/// </summary>
		public BlockedInfo BlockedInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Playlist.
		/// </summary>
		public IEnumerable<PlaylistItem> Playlist
		{
			get;
			set;
		}

		/// <summary>
		/// Torrents.
		/// </summary>
		public IEnumerable<TorrentItem> Torrents
		{
			get;
			set;
		}

	}

}
