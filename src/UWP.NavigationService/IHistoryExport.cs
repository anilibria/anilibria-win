using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.NavigationService.Models;

namespace UWP.NavigationService
{

    /// <summary>
    /// Interface for export history to external storage (file, database etc)
    /// </summary>
    public interface IHistoryExport
    {

        /// <summary>
		/// Export history.
		/// </summary>
		/// <param name="items">Item collection.</param>
		/// <param name="selected">Selected item.</param>
		Task Export(IEnumerable<HistoryItem> items, int selected);

    }

}
