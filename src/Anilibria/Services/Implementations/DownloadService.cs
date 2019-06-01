using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Download service.
	/// </summary>
	public class DownloadService : IDownloadService {

		/// <summary>
		/// Add download file.
		/// </summary>
		/// <param name="downloadItem">Download item.</param>
		public void AddDownloadFile ( DownloadItem downloadItem ) {
			
		}

		/// <summary>
		/// Get panding downloads.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DownloadItem> GetPendingDownloads () {
			return null;
		}

		/// <summary>
		/// Remove dpwnload file.
		/// </summary>
		/// <param name="id">Download item identifier.</param>
		public async Task RemoveDownloadFile ( long id ) {
			try {
				await DownloadManager.RemoveDownloadItem ( id );
			} catch {
				//TODO: send error message for user
			}
		}

	}

}
