namespace NHibernate.Test.CacheTest;

public abstract class CacheEntity
{
	public virtual int Id { get; protected set; }
}

public abstract class NamedCacheEntity : CacheEntity
{
	public virtual string Name { get; set; }
}
