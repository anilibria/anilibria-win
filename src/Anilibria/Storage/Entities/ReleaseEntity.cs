using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Release entity.
	/// </summary>
	public class ReleaseEntity {

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

	}

}
