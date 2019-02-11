using System;

namespace Anilibria.Services.Exceptions {

	/// <summary>
	/// Authorize deleted exception.
	/// </summary>
	public class AuthorizeDeletedException : ApplicationException {

		/// <summary>
		/// Initializes a new instance of the System.ApplicationException class.
		/// </summary>
		public AuthorizeDeletedException () : base ( "Authorization deleted from server." ) {
		}

		/// <summary>
		/// Initializes a new instance of the System.ApplicationException class with a specified error message.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		public AuthorizeDeletedException ( string message ) : base ( message ) {
		}

		/// <summary>
		/// Initializes a new instance of the System.ApplicationException class with a specified
		/// error message and a reference to the inner exception that is the cause of this
		/// exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception.
		/// If the innerException error message and a reference to the inner exception that is the cause of this
		/// block that handles the inner exception.
		/// </param>
		public AuthorizeDeletedException ( string message , Exception innerException ) : base ( message ) {
		}

	}

}
