using System;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// User model.
	/// </summary>
	public class UserModel {

		/// <summary>
		/// Identifier.
		/// </summary>
		public long Id
		{
			get;
			set;
		}

		/// <summary>
		/// Login.
		/// </summary>
		public string Login
		{
			get;
			set;
		}

		/// <summary>
		/// Path to user avatar image.
		/// </summary>
		public string Avatar
		{
			get;
			set;
		}

		/// <summary>
		/// Image url.
		/// </summary>
		public Uri ImageUrl
		{
			get;
			set;
		}

	}

}
