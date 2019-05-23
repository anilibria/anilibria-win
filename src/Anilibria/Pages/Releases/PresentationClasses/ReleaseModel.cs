using System;
using System.Collections.Generic;
using Anilibria.MVVM;

namespace Anilibria.Pages.Releases.PresentationClasses {

	/// <summary>
	/// Release model.
	/// </summary>
	public class ReleaseModel : ViewModel {

		private bool m_AddToFavorite;

		private OnlineVideoModel m_PrefferedOpenedVideo;
		
		private int m_CountSeenVideoOnline;
		
		private string m_DisplaySeenVideoOnline;

		private bool m_IsSeen;

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
		/// Scheduled on day.
		/// </summary>
		public string ScheduledOnDay
		{
			get;
			set;
		}

		/// <summary>
		/// Is exists scheduled on day.
		/// </summary>
		public bool IsExistsScheduledOnDay
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
		/// Count seen video online.
		/// </summary>
		public int CountSeenVideoOnline
		{
			get => m_CountSeenVideoOnline;
			set => Set ( ref m_CountSeenVideoOnline , value );
		}

		/// <summary>
		/// Display seen video online.
		/// </summary>
		public string DisplaySeenVideoOnline
		{
			get => m_DisplaySeenVideoOnline;
			set => Set ( ref m_DisplaySeenVideoOnline , value );
		}

		/// <summary>
		/// Is seen release.
		/// </summary>
		public bool IsSeen
		{
			get => m_IsSeen;
			set => Set ( ref m_IsSeen , value );
		}

		/// <summary>
		/// Online videos.
		/// </summary>
		public IEnumerable<OnlineVideoModel> OnlineVideos
		{
			get;
			set;
		}

		/// <summary>
		/// Preffered opened video.
		/// </summary>
		public OnlineVideoModel PrefferedOpenedVideo
		{
			get => m_PrefferedOpenedVideo;
			set => Set ( ref m_PrefferedOpenedVideo , value );
		}

		/// <summary>
		/// Torrents count.
		/// </summary>
		public int TorrentsCount
		{
			get;
			set;
		}

	}

}
