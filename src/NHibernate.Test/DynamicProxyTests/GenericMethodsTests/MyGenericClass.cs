namespace NHibernate.Test.DynamicProxyTests.GenericMethodsTests
{
	public class MyGenericClass<TId> : MyGenericClassBase<TId, int>, IMyGenericInterface<TId>, IMyGenericInterface<MyGenericClass<TId>>
	{
		public virtual TRequestedType As<TRequestedType>() where TRequestedType : MyGenericClass<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsBase<TRequestedType>() where TRequestedType : MyGenericClassBase<TId, int>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsInterface<TRequestedType>() where TRequestedType : class, IMyGenericInterface<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType AsInterfaceBase<TRequestedType>() where TRequestedType : class, IMyGenericInterfaceBase<TId>
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType As2<TRequestedType>() where TRequestedType : MyGenericClass<object>, new()
		{
			return new TRequestedType();
		}
		
		public virtual TRequestedType AsBase2<TRequestedType>() where TRequestedType : MyGenericClassBase<object, int>, new()
		{
			return new TRequestedType();
		}

		public virtual TRequestedType AsInterface2<TRequestedType>() where TRequestedType : class, IMyGenericInterface<object>, new()
		{
			return new TRequestedType();
		}
		
		public virtual TRequestedType AsInterfaceBase2<TRequestedType>() where TRequestedType : class, IMyGenericInterfaceBase<object>, new()
		{
			return new TRequestedType();
		}

		public virtual TRequestedType As3<TRequestedType,T>() where TRequestedType : MyGenericClass<T>, new()
		{
			return new TRequestedType();
		}
		
		public virtual TRequestedType AsBase3<TRequestedType,T>() where TRequestedType : MyGenericClassBase<T, object>, new()
		{
			return new TRequestedType();
		}

		public virtual TRequestedType AsInterface3<TRequestedType, T>() where TRequestedType : class, IMyGenericInterface<T>, new()
		{
			return new TRequestedType();
		}

		public virtual TRequestedType AsInterfaceBase3<TRequestedType, T>() where TRequestedType : class, IMyGenericInterfaceBase<T>, new()
		{
			return new TRequestedType();
		}

		public override TRequestedType As5<TRequestedType>()
		{
			return base.As5<TRequestedType>();
		}

		public override TRequestedType AsBase5<TRequestedType>()
		{
			return base.AsBase5<TRequestedType>();
		}

		public override TRequestedType AsInterface5<TRequestedType>()
		{
			return base.AsInterface5<TRequestedType>();
		}

		public override TRequestedType AsInterfaceBase5<TRequestedType>()
		{
			return base.AsInterfaceBase5<TRequestedType>();
		}
	}
}
