using System;
using System.Collections.Generic;

namespace UWP.NavigationService.Models
{

    /// <summary>
    /// History Item.
    /// </summary>
    public class HistoryItem
    {

        /// <summary>
        /// Type at user control.
        /// </summary>
        public Type Type
        {
            get;
            set;
        }

        /// <summary>
        /// Parameters.
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get;
            set;
        }

    }

}