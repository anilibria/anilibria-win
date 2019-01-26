using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Anilibria.Collections {

	/// <summary>
	/// Incremental loading collection.
	/// </summary>
	public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading where T : class {

		private int m_CurrentPage = 0;

		/// <summary>
		/// Has more items.
		/// </summary>
		public bool HasMoreItems { get; private set; } = true;

		/// <summary>
		/// Page size.
		/// </summary>
		public int PageSize { get; set; } = 20;

		/// <summary>
		/// Get page function.
		/// </summary>
		public Func<int , int , Task<IEnumerable<T>>> GetPageFunction
		{
			get;
			set;
		}

		/// <summary>
		/// Load more items asynchtonized.
		/// </summary>
		/// <param name="count">Count.</param>
		/// <returns>Load more items result.</returns>
		public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync ( uint count ) {
			return AsyncInfo.Run ( ( c ) => LoadItemsAsync ( c , count ) );
		}

		/// <summary>
		/// Load items asynchronized.
		/// </summary>
		/// <param name="c">Cancellation token.</param>
		/// <param name="count">Count.</param>
		/// <returns>Load more items result.</returns>
		private async Task<LoadMoreItemsResult> LoadItemsAsync ( CancellationToken c , uint count ) {
			if ( GetPageFunction == null ) {
				return new LoadMoreItemsResult {
					Count = 0
				};
			}

			var items = await GetPageFunction ( ++m_CurrentPage , PageSize );
			var countItems = items.Count ();
			if ( countItems < PageSize ) HasMoreItems = false;

			foreach ( var item in items ) {
				Add ( item );
			}

			return new LoadMoreItemsResult {
				Count = (uint) countItems
			};
		}

		/// <summary>
		/// Reset.
		/// </summary>
		public void Reset () {
			Clear ();
			m_CurrentPage = 0;
			HasMoreItems = true;
		}

	}

}
