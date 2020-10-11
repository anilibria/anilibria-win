using Anilibria.Storage.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Releases service.
	/// </summary>
	public class ReleasesService : IReleasesService {

		private const string m_FileName = "releases.cache";

		private static readonly SemaphoreSlim m_Semaphore = new SemaphoreSlim ( 1 , 1 );

		private List<ReleaseEntity> m_Releases = Enumerable.Empty<ReleaseEntity> ().ToList ();

		public async Task LoadReleases () {
			var releasesFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync ( m_FileName );

			if ( releasesFile == null ) {
				await ApplicationData.Current.LocalFolder.CreateFileAsync ( m_FileName , CreationCollisionOption.ReplaceExisting );
				releasesFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync ( m_FileName );
			}

			var relasesJson = await FileIO.ReadTextAsync ( (IStorageFile) releasesFile );
			m_Releases = JsonConvert.DeserializeObject<List<ReleaseEntity>> ( relasesJson ) ?? Enumerable.Empty<ReleaseEntity> ().ToList ();
		}

		public async Task SaveReleases () {
			var releasesFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync ( m_FileName );
			if ( releasesFile == null ) return;

			await m_Semaphore.WaitAsync ();
			try {
				await FileIO.WriteTextAsync ( (IStorageFile) releasesFile , JsonConvert.SerializeObject ( m_Releases ) );
			} finally {
				if ( m_Semaphore.CurrentCount == 0 ) m_Semaphore.Release ();
			}
		}

		/// <summary>
		/// Get release by identifier.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public ReleaseEntity GetReleaseById ( long id ) => m_Releases.FirstOrDefault ( a => a.Id == id );

		/// <summary>
		/// Get release by identifier.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public IEnumerable<ReleaseEntity> GetReleases () => m_Releases.ToList ();

		public void SetReleases ( IEnumerable<ReleaseEntity> releases ) => m_Releases = releases.ToList ();

	}

}
