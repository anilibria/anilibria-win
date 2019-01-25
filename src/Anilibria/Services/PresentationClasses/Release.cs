using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Release.
	/// </summary>
	public class Release {

		/// <summary>
		/// Identifier.
		/// </summary>
		public string RId
		{
			get;
			set;
		}

		/// <summary>
		/// Name.
		/// </summary>
		public IEnumerable<string> Name
		{
			get;
			set;
		}

		/// <summary>
		/// Rating (probably number).
		/// </summary>
		public string Rating
		{
			get;
			set;
		}

		/// <summary>
		/// ?????.
		/// </summary>
		public string Last
		{
			get;
			set;
		}

		/// <summary>
		/// ??????.
		/// </summary>
		public Uri Moon
		{
			get;
			set;
		}

		/// <summary>
		/// Status (1 В работе 2 Завершен).
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
		/// Genres between commas.
		/// </summary>
		public string Genre
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
		/// Code.
		/// </summary>
		public string Code
		{
			get;
			set;
		}



	}
}
