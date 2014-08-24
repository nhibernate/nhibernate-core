namespace NHibernate.Test.DynamicProxyTests.GenericMethodsTests
{
	public class MyClass
	{
		public virtual object BasicGenericMethod<T>()
		{
			if (typeof(T) == typeof(int))
				return 5;

			if (typeof(T) == typeof(string))
				return "blha";

			return default(T);
		}

		public virtual object MethodWithGenericBaseClassConstraint<T, TY>() where T : MyGenericClass<TY>
		{
			return typeof(T);
		}

		public virtual object MethodWithInterfaceConstraint<T>(T arg) where T : IMyInterface
		{
			return typeof(T);
		}

		public virtual TRequestedType As<TRequestedType>() where TRequestedType : MyClass
		{
			return this as TRequestedType;
		}

		public virtual TRequestedType MethodWithConstructorConstraint<TRequestedType>() where TRequestedType : new()
		{
			return new TRequestedType();
		}

		public virtual TRequestedType MethodWithReferenceTypeAndInterfaceConstraint<TRequestedType>() where TRequestedType : class, IMyInterface
		{
			return this as TRequestedType;
		}
	}
}