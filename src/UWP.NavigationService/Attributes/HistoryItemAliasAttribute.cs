using System;

namespace UWP.NavigationService.Attributes
{
    /// <summary>
    /// Attribute for specify alias for Page or UserControl.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HistoryItemAliasAttribute : Attribute
    {

        /// <summary>
        /// Alias for search and for saving to external storage.
        /// </summary>
        public string Alias { get; set; }

    }
}
