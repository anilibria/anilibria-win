using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;
using Windows.Storage;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Download manager.
	/// </summary>
	public static class DownloadManager {

		private static List<DownloadItem> m_DownloadedItems = new List<DownloadItem> ();

		public static void Initialize () {
			//TODO: Read downloading item from local database
		}

		/// <summary>
		/// Remove download item.
		/// </summary>
		/// <param name="downloadId">Download identifier.</param>
		public static async Task RemoveDownloadItem ( long downloadId ) {
			var items = m_DownloadedItems.Where ( a => a.ReleaseId == downloadId ).ToList ();
			foreach ( var item in items ) {
				m_DownloadedItems.Remove ( item );
				var downloadItemFile = await ApplicationData.Current.TemporaryFolder.GetFileAsync ( item.FileName );
				await downloadItemFile.DeleteAsync ( StorageDeleteOption.PermanentDelete );
			}
		}

	}

}
