namespace NHibernate.DomainModel.Northwind.Entities
{
    public abstract class Entity<T>
    {
	    public virtual long Id { get; set; } = -1;
    }
}