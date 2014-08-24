namespace NHibernate.Mapping
{
	public interface IPersistentClassVisitor
	{
		object Accept(PersistentClass clazz);
	}

	public interface IPersistentClassVisitor<T> where T : PersistentClass
	{
		object Accept(T clazz);
	}
}