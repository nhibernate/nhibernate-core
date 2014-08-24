namespace NHibernate.Test.DynamicProxyTests.GenericMethodsTests
{
	public class MyGenericClassBase<TId, T2>
	{
		public virtual TRequestedType As4<TRequestedType>() where TRequestedType : MyGenericClass<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsBase4<TRequestedType>() where TRequestedType : MyGenericClassBase<TId, T2>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsInterface4<TRequestedType>() where TRequestedType : class, IMyGenericInterface<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsInterfaceBase4<TRequestedType>() where TRequestedType : class, IMyGenericInterfaceBase<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType As5<TRequestedType>() where TRequestedType : MyGenericClass<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsBase5<TRequestedType>() where TRequestedType : MyGenericClassBase<TId, T2>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsInterface5<TRequestedType>() where TRequestedType : class, IMyGenericInterface<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsInterfaceBase5<TRequestedType>() where TRequestedType : class, IMyGenericInterfaceBase<TId>
		{
			return this as TRequestedType;
		}
	}
}