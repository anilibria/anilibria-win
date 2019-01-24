using System;
using System.Collections.Generic;
using UWP.NavigationService.Helpers;
using UWP.NavigationService.Implementations.NavigationService;
using UWP.NavigationService.Implementations.Resolvers;
using Windows.UI.Xaml;

namespace UWP.NavigationService
{
    public sealed class Navigator
    {

        private static Dictionary<string, Type> m_GlobalMapping = new Dictionary<string, Type>();

        private static Dictionary<Guid, INavigationService> m_Services = new Dictionary<Guid, INavigationService>();

        private static IViewFinder m_ViewFinder;

        private static IViewResolver m_ViewResolver = new DefaultViewResolver();

        private static string m_NavigationServicePropertyName = "NavigationService";

        private static string m_NavigateToMethodName = "NavigateTo";

        private static string m_NavigateFromMethodName = "NavigateFrom";

        private static IParameterNameResolver m_ParameterNameResolver = new DefaultParameterNameResolver();

        /// <summary>
        /// Set custom <see cref="IViewFinder"/> implementation.
        /// </summary>
        /// <param name="viewFinder">Custom implementation <see cref="IViewFinder"/></param>
        public static void SetViewFinder(IViewFinder viewFinder)
        {
            m_ViewFinder = viewFinder ?? throw new ArgumentNullException(nameof(viewFinder));
        }

        /// <summary>
        /// Set view resolver.
        /// </summary>
        /// <param name="viewResolver">View resolver.</param>
        public static void SetViewResolver (IViewResolver viewResolver)
        {
            m_ViewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        /// <summary>
        /// Set custom <see cref="IParameterNameResolver"/> implementation.
        /// </summary>
        /// <param name="viewFinder">Custom implementation <see cref="IParameterNameResolver"/></param>
        public static void SetParameterNameResolver(IParameterNameResolver parameterNameResolver)
        {
            m_ParameterNameResolver = parameterNameResolver ?? throw new ArgumentNullException(nameof(parameterNameResolver));
        }

        /// <summary>
        /// Register view.
        /// </summary>
        /// <param name="viewType">View type.</param>
        /// <param name="alias">Alias (if not specified will be equal full name ViewType).</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RegisterView(Type viewType, string alias = null)
        {
            if (viewType == null) throw new ArgumentNullException(nameof(viewType));

            if (string.IsNullOrEmpty(alias)) alias = viewType.FullName;

            m_GlobalMapping.Add(alias, viewType);
        }

        /// <summary>
        /// Get view by alias.
        /// </summary>
        /// <param name="alias">Alias.</param>
        public static Type GetView(string alias)
        {
            return m_GlobalMapping.ContainsKey(alias) ? m_GlobalMapping[alias] : null;
        }

        /// <summary>
        /// Get navigation service.
        /// </summary>
        /// <param name="control">Control.</param>
        /// <returns>Navigation service or null.</returns>
        public static INavigationService GetNavigationService(FrameworkElement control)
        {
            var id = GetNavigationContainerId(control);

            m_Services.TryGetValue(id, out var service);
            return service;
        }

        public static string NavigateToMethodName => m_NavigateToMethodName;

        public static string NavigateFromMethodName => m_NavigateFromMethodName;

        public static string NavigationServicePropertyName => m_NavigationServicePropertyName;

        public static IParameterNameResolver ParameterNameResolver => m_ParameterNameResolver;

        public static readonly DependencyProperty NavigationContainerProperty = DependencyProperty.RegisterAttached("NavigationContainer", typeof(string), typeof(Navigator), new PropertyMetadata(default(string), NavigationContainerChanged));
        private static void NavigationContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var id = GetNavigationContainerId(d);
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                SetNavigationContainerId(d, id);
                var element = d as FrameworkElement;
                element.Unloaded += Element_Unloaded;
                var service = new DefaultNavigationService
                {
                    Name = (string)e.NewValue,
                    Control = new WeakReference<FrameworkElement>(element)
                };
                service.SetViewResolver(m_ViewResolver);
                m_Services.Add(id, service);

                ReflectionHelper.SetToProperty(element, (string)e.NewValue, service);
                if (element.DataContext != null) ReflectionHelper.SetToProperty(element.DataContext, Navigator.NavigationServicePropertyName, service);
            }
        }
        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).Unloaded -= Element_Unloaded;
            var id = GetNavigationContainerId(sender as DependencyObject);
            m_Services.TryGetValue(id, out var service);
            if (service != null) m_Services.Remove(id);
        }
        public static void SetNavigationContainer(DependencyObject element, string value) => element.SetValue(NavigationContainerProperty, value);
        public static string GetNavigationContainer(DependencyObject element) => (string)element.GetValue(NavigationContainerProperty);

        public static readonly DependencyProperty MaxHistorySizeProperty = DependencyProperty.RegisterAttached("MaxHistorySize", typeof(int), typeof(Navigator), new PropertyMetadata(default(int), MaxHistorySizeChanged));
        private static void MaxHistorySizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var id = GetNavigationContainerId(d);
            m_Services.TryGetValue(id, out var service);
            if (service == null) return;

            service.MaxHistorySize = (int)e.NewValue;
        }
        public static void SetMaxHistorySize(DependencyObject element, int value) => element.SetValue(MaxHistorySizeProperty, value);
        public static int GetMaxHistorySize(DependencyObject element) => (int)element.GetValue(MaxHistorySizeProperty);

        public static readonly DependencyProperty HistoryItemProperty = DependencyProperty.RegisterAttached("HistoryItem", typeof(string), typeof(Navigator), new PropertyMetadata(default(string), HistoryItemChanged));
        private static void HistoryItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var id = GetNavigationContainerId(d);
            m_Services.TryGetValue(id, out var service);
            if (service == null) return;

            var viewName = (string)e.NewValue;

            if (m_GlobalMapping.TryGetValue(viewName, out var type))
            {
                service.Navigate(type);
                return;
            }

            //TODO: Add find by attribute HistoryItemAliasAttribute.

            var view = m_ViewFinder?.GetView(viewName);

            if (view != null) service.Navigate(view);
        }
        public static void SetHistoryItem(DependencyObject element, string value) => element.SetValue(HistoryItemProperty, value);
        public static string GetHistoryItem(DependencyObject element) => (string)element.GetValue(HistoryItemProperty);

        public static readonly DependencyProperty NavigationContainerIdProperty = DependencyProperty.RegisterAttached("NavigationContainerId", typeof(Guid), typeof(Navigator), new PropertyMetadata(Guid.Empty));
        public static void SetNavigationContainerId(DependencyObject element, Guid value) => element.SetValue(NavigationContainerIdProperty, value);
        public static Guid GetNavigationContainerId(DependencyObject element) => (Guid)element.GetValue(NavigationContainerIdProperty);

    }
}