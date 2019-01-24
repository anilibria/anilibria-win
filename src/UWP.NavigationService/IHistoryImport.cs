using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.NavigationService.Models;

namespace UWP.NavigationService
{
    public interface IHistoryImport
    {

        /// <summary>
        /// Import history.
        /// </summary>
        /// <returns></returns>
        Task<(IEnumerable<HistoryItem> items, int selected)> Import();

    }

}
