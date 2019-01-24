using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UWP.NavigationService.Implementations.Resolvers
{
    /// <summary>
	/// Default implementation <see cref="IViewFinder"/>.
	/// </summary>
	public class AssembliesViewFinder : IViewFinder
    {

        IReadOnlyCollection<Assembly> m_Assemblies;

        readonly bool m_FullName;

        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="assemblies">List of assemblies where will be find views.</param>
        /// <param name="fullName">Find will be make by full name class or only name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AssembliesViewFinder(IEnumerable<Assembly> assemblies, bool fullName = true)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            m_Assemblies = assemblies.ToList();
            m_FullName = fullName;
        }

        /// <summary>
        /// Get <see cref="Type"/> by name.
        /// </summary>
        /// <param name="typeName">Type name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Type GetView(string typeName)
        {
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            foreach (var assembly in m_Assemblies)
            {
                var type = m_FullName ? assembly.GetType(typeName) : assembly.GetTypes().FirstOrDefault(a => a.Name == typeName);
                return type;
            }
            return null;
        }

    }
}
