namespace NHibernate.DomainModel.Northwind.Entities
{
    public abstract class Entity<T>
    {
        private long _id = -1;

        public virtual long Id { get { return _id; } set { _id = value; }}
    }
}