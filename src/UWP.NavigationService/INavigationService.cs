using System;

namespace UWP.NavigationService
{

    /// <summary>
    /// Navigation service.
    /// </summary>
    public interface INavigationService
    {

        /// <summary>
        /// Name.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Maximum history size.
        /// </summary>
        int MaxHistorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Is stack.
        /// </summary>
        bool IsStack
        {
            get;
            set;
        }

        /// <summary>
        /// Navigate to view.
        /// </summary>
        /// <param name="view">View name.</param>
        /// <param name="parameters">Parameters as single object.</param>
        void Navigate(Type view, object parameters = null);

        /// <summary>
        /// Navigate to back.
        /// </summary>
        void GoBack();

    }

}
