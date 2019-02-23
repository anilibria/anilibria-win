using System.IO;

namespace Anilibria.Storage {

	/// <summary>
	/// Data context.
	/// </summary>
	public interface IDataContext {

		/// <summary>
		/// Get collection.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		IEntityCollection<T> GetCollection<T> ();

		/// <summary>
		/// Upload file.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="stream">Stream.</param>
		void UploadFile ( string alias , long id , Stream stream );

		/// <summary>
		/// Download file.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		/// <returns>Stream.</returns>
		Stream DownloadFile ( string alias , long id );

		/// <summary>
		/// Is file exists.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		bool IsFileExists ( string alias , long id );

		/// <summary>
		/// Is file exists.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		void DeleteFile ( string alias , long id );


	}

}
