using System;
using System.Linq;
using Windows.UI.Xaml;

namespace UWP.NavigationService.Implementations.Resolvers
{
    public class DefaultViewResolver : IViewResolver
    {

        private object ConstructObject(Type type)
        {
            var constructors = type.GetConstructors();
            var defaultConstructor = constructors.FirstOrDefault(a => a.GetParameters().Count() == 0);

            if (constructors.Count() > 1 && defaultConstructor == null)
            {
                throw new ArgumentException($"Type {type.FullName} contains more then 1 constructor and don't contains default constructor (without parameters). You need or add default constructor or use only one constructor with parameters.");
            }

            if (defaultConstructor != null) return Activator.CreateInstance(type);

            var constructor = constructors.First();
            var parameters = constructor.GetParameters().Select(a => ConstructObject(a.ParameterType)).ToArray();

            return Activator.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Resolve type.
        /// </summary>
        /// <param name="type">View type.</param>
        public FrameworkElement Resolve(Type type) => ConstructObject(type) as FrameworkElement;

    }

}
