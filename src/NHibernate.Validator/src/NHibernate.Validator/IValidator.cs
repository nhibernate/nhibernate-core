namespace NHibernate.Validator
{
    using System;

    public interface IValidator
    {
        bool IsValid(object value);
        void Initialize(Attribute parameters);
    }
}