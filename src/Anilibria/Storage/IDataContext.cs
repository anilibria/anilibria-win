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

	}

}
