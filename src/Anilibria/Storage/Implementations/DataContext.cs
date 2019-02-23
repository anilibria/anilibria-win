using System;
using System.Diagnostics;
using System.IO;
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
			Debug.WriteLine ( "Local path: " + m_LocalDatabasePath.GetDatabasePath () );
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
		/// Download file.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		/// <returns>Stream.</returns>
		public Stream DownloadFile ( string alias , long id ) {
			using ( var stream = m_LiteDatabase.FileStorage.OpenRead ( $"{alias}{id}" ) ) {
				var outputStream = new MemoryStream ();

				stream.CopyTo ( outputStream );
				outputStream.Position = 0;

				return outputStream;
			}
		}

		/// <summary>
		/// Is file exists.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		public bool IsFileExists ( string alias , long id ) {
			return m_LiteDatabase.FileStorage.FindById ( $"{alias}{id}" ) != null;
		}

		/// <summary>
		/// Upload file.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="stream">Stream.</param>
		public void UploadFile ( string alias , long id , Stream stream ) {
			m_LiteDatabase.FileStorage.Upload ( $"{alias}{id}" , alias + Guid.NewGuid ().ToString () , stream );
		}

		/// <summary>
		/// Delete file.
		/// </summary>
		/// <param name="alias">Alias.</param>
		/// <param name="id">Identifier.</param>
		public void DeleteFile ( string alias , long id ) {
			m_LiteDatabase.FileStorage.Delete ( $"{alias}{id}" );
		}

		/// <summary>
		/// Dispose managed resources.
		/// </summary>
		public void Dispose () {
			m_LiteDatabase?.Dispose ();
			m_LiteDatabase = null;
		}


	}


}
