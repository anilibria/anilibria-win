using System.Linq;

namespace UWP.NavigationService.Implementations.Resolvers
{
    public class DefaultParameterNameResolver : IParameterNameResolver
    {

        /// <summary>
        /// Resolve name of parameter.
        /// </summary>
        /// <param name="name">parameter name.</param>
        /// <returns>Resolved name.</returns>
        public string Resolve(string name)
        {
            var firstCharacter = name[0].ToString().ToLowerInvariant();
            var withoutFirstCharacter = new string(name.Skip(1).ToArray());
            return firstCharacter + withoutFirstCharacter;
        }

    }
}
