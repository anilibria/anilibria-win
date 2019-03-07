using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Entity for keep player state for restore it after player first opened.
	/// </summary>
	public class PlayerRestoreEntity {

		/// <summary>
		/// Identifier.
		/// </summary>
		public long Id
		{
			get;
			set;
		}

		/// <summary>
		/// Release identifier.
		/// </summary>
		public long ReleaseId
		{
			get;
			set;
		}

		/// <summary>
		/// Video identifier.
		/// </summary>
		public int VideoId
		{
			get;
			set;
		}

		/// <summary>
		/// Video position.
		/// </summary>
		public double VideoPosition
		{
			get;
			set;
		}

	}

}
