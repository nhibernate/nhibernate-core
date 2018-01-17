using System;
using System.Linq;
using System.Linq.Expressions;

// Simulates compiler-generated, non-namespaced anonymous type with one property
internal class AnonymousType1<TProp1>
{
	public TProp1 Prop1 { get; set; }
}

namespace NHibernate.DomainModel.NHSpecific
{
	public class AnonymousTypeQueryExpressionProviderFromNHibernateDomainModelAssembly
	{
		private readonly TypedQueryExpressionProvider<AnonymousType1<string>> _provider
			= new TypedQueryExpressionProvider<AnonymousType1<string>>();

		public System.Type GetAnonymousType()
		{
			return _provider.GetSuppliedType();
		}

		public Expression GetExpressionOfMethodCall()
		{
			return _provider.GetExpressionOfMethodCall();
		}
	}

	public class TypedQueryExpressionProvider<T> where T : new ()
	{
		public System.Type GetSuppliedType()
		{
			return typeof(T);
		}

		public Expression GetExpressionOfMethodCall()
		{
			Expression<Func<object>> exp = () =>
				Enumerable.Empty<object>().Select(o => (T)o).ToList();

			return exp;
		}
	}
}
