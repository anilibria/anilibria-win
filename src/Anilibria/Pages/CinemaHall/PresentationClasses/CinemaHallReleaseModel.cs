using System;
using Anilibria.MVVM;

namespace Anilibria.Pages.CinemaHall.PresentationClasses {

	/// <summary>
	/// Cinema hall release model.
	/// </summary>
	public class CinemaHallReleaseModel : ViewModel {

		private int m_Order;

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
		public Uri Poster
		{
			get;
			set;
		}

		/// <summary>
		/// Order.
		/// </summary>
		public int Order
		{
			get => m_Order;
			set => Set ( ref m_Order , value );
		}

		/// <summary>
		/// Description.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

	}

}
