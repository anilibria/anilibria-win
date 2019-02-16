using System;
using System.Diagnostics;
using Anilibria.Services;
using Anilibria.Services.Implementations;
using Anilibria.Storage.Entities;
using LiteDB;

namespace Anilibria.Storage.Implementations {

	/// <summary>
	/// Data context.
	/// </summary>
	public class DataContext : IDataContext {

		private readonly ILocalDatabasePath m_LocalDatabasePath;

		private LiteDatabase m_LiteDatabase;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="localDatabasePath">Local database path.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public DataContext ( ILocalDatabasePath localDatabasePath ) {
			m_LocalDatabasePath = localDatabasePath ?? throw new ArgumentNullException ( nameof ( localDatabasePath ) );

			CreateDatabase ();
		}

		/// <summary>
		/// Create database.
		/// </summary>
		private void CreateDatabase () {
#if DEBUG
			m_LiteDatabase = new LiteDatabase ( m_LocalDatabasePath.GetDatabasePath () , log: new Logger ( Logger.QUERY , LogMessage ) );
#else
			m_LiteDatabase = new LiteDatabase ( m_LocalDatabasePath.GetDatabasePath () );
#endif
		}

#if DEBUG
		/// <summary>
		/// Log message.
		/// </summary>
		/// <param name="message">Message.</param>
		private void LogMessage ( string message ) => Debug.WriteLine ( message );

#endif

		/// <summary>
		/// Initialize database.
		/// </summary>
		public void Initialize () {
			m_LiteDatabase.GetCollection<ReleaseEntity> ().EnsureIndex ( a => a.Title );
			m_LiteDatabase.GetCollection<YoutubeEntity> ().EnsureIndex ( a => a.Title );
		}

		/// <summary>
		/// Get collection.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		public IEntityCollection<T> GetCollection<T> () => new LiteDbEntityCollection<T> ( m_LiteDatabase.GetCollection<T> () );

		/// <summary>
		/// Dispose managed resources.
		/// </summary>
		public void Dispose () {
			m_LiteDatabase?.Dispose ();
			m_LiteDatabase = null;
		}


	}


}
