namespace NHibernate.Validator
{
    using System;
    using System.Collections;
    using Mapping;

    public class NotEmptyValidator : Validator<NotEmptyAttribute>, IPropertyConstraint
    {
        public override bool IsValid(object value) 
        {
            if (value == null) return false;
            
            if(value is ICollection)
                return ((ICollection)value).Count > 0;

            if (value is string)
                return ((string)value).Length > 0;

            throw new ArgumentException("the object to validate must be a string or a collection", "value");
        }
        
        public void Apply(Property property)
        {
			//single table should not be forced to null
			if (property is SingleTableSubclass) return;

			if (!property.IsComposite)
				foreach (Column column in property.ColumnIterator)
					column.IsNullable = false;
        }

		public override void Initialize(NotEmptyAttribute parameters)
        {
            
        }

    }
}