namespace NHibernate.Validator
{
    using Mapping;

    /// <summary>
    /// Interface implemented by the validator
    /// when a constraint may be represented in a
    /// hibernate metadata property
    /// </summary>
    public interface IPropertyConstraint
    {
        void apply(Property property);
    }
}