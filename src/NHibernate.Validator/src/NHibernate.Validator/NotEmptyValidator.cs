namespace NHibernate.Validator
{
    using System;
    using System.Collections;
    using Mapping;

    public class NotEmptyValidator : IValidator<NotEmptyAttribute>, IPropertyConstraint
    {
        public bool IsValid(object value) 
        {
            if (value == null) return false;
            
            if(value is ICollection)
                return ((ICollection)value).Count > 0;

            if (value is string)
                return ((string)value).Length > 0;

            throw new ArgumentException("the object to validate must be a string or a collection", "value");
        }
        
        public void apply(Property property)
        {
            
        }
        
        public void Initialize(Attribute parameters)
        {
            
        }

        public void Initialize(NotEmptyAttribute parameters) 
        {
            Initialize((Attribute)parameters);
        }
    }
}