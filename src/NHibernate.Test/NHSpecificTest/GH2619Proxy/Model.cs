namespace NHibernate.Test.NHSpecificTest.GH2619Proxy
{
	public abstract class ClassWithGenericNonVirtualMethod : IWithGenericMethod
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public T CopyTo<T>() where T : class, IWithGenericMethod
		{
			return null;
		}
	}

	public interface IWithGenericMethod
	{
		T CopyTo<T>() where T : class, IWithGenericMethod;
	}
}
