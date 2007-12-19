namespace NHibernate.Validator
{
    using Mapping;

    /// <summary>
    /// Interface implemented by the validator
    /// when a constraint may be represented in the
    /// hibernate metadata
    /// </summary>
    public interface IPersistentClassConstraint
    {
        /// <summary>
        ///  Apply the constraint in the hibernate metadata
        /// </summary>
        /// <param name="persistentClass">PersistentClass</param>
        void Apply(PersistentClass persistentClass);
    }
}