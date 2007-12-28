namespace NHibernate.Validator
{
    using Mapping;

	public class NotNullValidator : Validator<NotNullAttribute>, IPropertyConstraint
    {
        public override bool IsValid(object value)
        {
            return value != null;
        }

		public override void Initialize(NotNullAttribute parameters)
        {
        }

		public void Apply(Property property)
		{
			//single table should not be forced to null
			if (property is SingleTableSubclass) return;

			if (!property.IsComposite)
				foreach (Column column in property.ColumnIterator)
					column.IsNullable = false;
		}
    }
}