namespace NHibernate.Test.DynamicProxyTests.GenericMethodsTests
{
	public class MyGenericClass<TId> : IMyGenericInterface<TId>
	{
		public virtual TRequestedType As<TRequestedType>() where TRequestedType : MyGenericClass<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsInterface<TRequestedType>() where TRequestedType : class, IMyGenericInterface<TId>
		{
			return this as TRequestedType;
		}
	}
}