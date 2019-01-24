using System;
using Windows.UI.Xaml;

namespace UWP.NavigationService
{

    /// <summary>
    /// View resolver.
    /// </summary>
    public interface IViewResolver
    {

        /// <summary>
        /// Resolve type and it dependencies.
        /// </summary>
        /// <param name="type">Type that need receive.</param>
        FrameworkElement Resolve(Type type);

    }

}
