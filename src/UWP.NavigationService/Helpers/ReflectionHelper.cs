using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UWP.NavigationService.Helpers
{
    public static class ReflectionHelper
    {

        private static object GetDefault(Type type)
        {
            return !type.IsByRef ? Activator.CreateInstance(type) : null;
        }

        private static IEnumerable<object> PrepareParameters(IDictionary<string, object> parameters, IEnumerable<ParameterInfo> methodParameters)
        {
            var parametersValues = new List<object>(methodParameters.Count());
            foreach (var methodParameter in methodParameters)
            {
                var value = parameters.TryGetValue(methodParameter.Name, out var parameter);

                if (!value)
                {
                    parametersValues.Add(GetDefault(methodParameter.ParameterType));
                }
                else
                {
                    parametersValues.Add(parameter);
                }
            }

            return parametersValues;
        }

        /// <summary>
        /// Invoke method with parameters.
        /// </summary>
        /// <param name="instance">Instance of object.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Return method parameter back.</returns>
        public static object InvokeMethod(object instance, string methodName, IDictionary<string, object> parameters)
        {
            var method = instance.GetType().GetMethod(methodName);
            if (method == null) return null;

            var methodParameters = method.GetParameters();
            var parametersValues = PrepareParameters(parameters ?? new Dictionary<string, object>(), methodParameters);

            try
            {
                return method.Invoke(instance, parametersValues.ToArray());
            }
            catch
            {
                throw new ArgumentException(
                    $"Method {methodName} expect follow arguments {string.Join(", ", methodParameters.Select(a => $"{a.Name} ({a.ParameterType.FullName})"))} " +
                    $"but passed parameters {string.Join(", ", parameters.Select(a => $"{a.Key} ({a.Value.GetType().FullName})"))}"
                );
            }

        }

        /// <summary>
        /// Set value to property.
        /// </summary>
        /// <param name="instance">Instance of object.</param>
        /// <param name="name">Property name.</param>
        /// <param name="value">Value.</param>
        public static void SetToProperty(object instance, string name, object value)
        {
            var property = instance.GetType().GetProperty(name);
            if (property == null || property.SetMethod == null) return;

            property.SetMethod.Invoke(instance, new object[] { value });
        }

        /// <summary>
        /// Map parameters dictionary from object properties.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <returns>Mapped parameters as dictionary.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDictionary<string, object> MapParametersFromObjectProperties(object instance, IParameterNameResolver parameterNameResolver)
        {
            if (parameterNameResolver == null) throw new ArgumentNullException(nameof(parameterNameResolver));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var properties = instance.GetType().GetProperties();
            var parameters = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                parameters.Add(parameterNameResolver.Resolve(property.Name), property.GetGetMethod().Invoke(instance, null));
            }

            return parameters;
        }

    }

}
