namespace NHibernate.Validator
{
	using Mapping;

    public class NotEmptyNotNullPropertyConstraint : IPropertyConstraint
    {
        public void Apply(Property property)
        {
			//single table should not be forced to null
            if(property is SingleTableSubclass) return;
			
			if (!property.IsComposite) 
				foreach(Column column in property.ColumnIterator)
					column.IsNullable = false;
        }
    }
}