namespace UWP.NavigationService
{
    public interface IParameterNameResolver
    {

        /// <summary>
        /// Resolve name of parameter.
        /// </summary>
        /// <param name="name">parameter name.</param>
        /// <returns>Resolved name.</returns>
        string Resolve(string name);

    }

}
