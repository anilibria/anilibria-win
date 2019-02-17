using System;
using System.Collections.Generic;
using Anilibria.MVVM;

namespace Anilibria.Pages.Releases.PresentationClasses {

	/// <summary>
	/// Release model.
	/// </summary>
	public class ReleaseModel : ViewModel {

		private bool m_AddToFavorite;

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
		public Uri Poster
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
		/// Title.
		/// </summary>
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// Add to favorite.
		/// </summary>
		public bool AddToFavorite
		{
			get => m_AddToFavorite;
			set => Set ( ref m_AddToFavorite , value );
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
		public string Genres
		{
			get;
			set;
		}

		/// <summary>
		/// Voices.
		/// </summary>
		public string Voices
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
		/// Description.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Torrents.
		/// </summary>
		public IEnumerable<TorrentModel> Torrents
		{
			get;
			set;
		}

		/// <summary>
		/// Count video online.
		/// </summary>
		public int CountVideoOnline
		{
			get;
			set;
		}

		/// <summary>
		/// Online videos.
		/// </summary>
		public IEnumerable<OnlineVideoModel> OnlineVideos
		{
			get;
			set;
		}

	}

}
