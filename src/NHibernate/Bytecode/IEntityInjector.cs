namespace NHibernate.Bytecode
{
    /// <summary>
    /// Plugin for using dependency injection with NHibernate
    /// </summary>
    public interface IEntityInjector
    {
        /// <summary>
        /// Provides entity constructor parameters to NHibernate
        /// </summary>
        /// <param name="type">type of entity NHibernate is constructing</param>
        /// <returns>object array of the constructor parameters - return null or empty array to instruct NHibernate to use the default constructor</returns>
        object[] GetConstructorParameters(System.Type type);
    }
}