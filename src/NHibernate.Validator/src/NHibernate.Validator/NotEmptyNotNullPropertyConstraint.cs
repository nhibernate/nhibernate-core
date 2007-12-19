namespace NHibernate.Validator
{
    using Mapping;

    public class NotEmptyNotNullPropertyConstraint : IPropertyConstraint
    {
        public void apply(Property property)
        {
            //Todo: Column type has not the property Nullable
            //if (!(property is SingleTableSubclass))
            //{
            //    if (!property.IsComposite)
            //    {
            //        IEnumerator<Column> ie = (IEnumerator<Column>) property.ColumnIterator.GetEnumerator();

            //        while(ie.MoveNext())
            //        {
            //            ie.Current.Nullable = false;
            //        }
            //    }
            //}
        }
    }
}